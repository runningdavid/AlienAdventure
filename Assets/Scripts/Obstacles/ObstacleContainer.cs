using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleContainer : MonoBehaviour {

    [Tooltip("Direction container is moving towards")]
    public Vector3 translate = Vector3.down;

    [Tooltip("Moving speed of current container")]
    public float speed = 5.00f;

    [Tooltip("Container width in world units")]
    public float width = 9.00f;

    [Tooltip("Container height in world units")]
    public float height = 16.00f;

    [Tooltip("Original position for container")]
    public Vector3 originalPosition;

    [Tooltip("Ending position for the container. Container may be recycled upon reaching this position")]
    public Vector3 endingPosition;

    // associated with level difficulty
    public float leastVerticalGap = 0;

    public float leastHorizontalGap = 0;
    
    [Tooltip("INTERNAL ONLY: should we visualize the generated path")]
    public bool shouldVisualizePath = false;

    [Tooltip("INTERNAL ONLY: prefab used to visualize the generated path")]
    public GameObject pathCellPrefab;

    public bool IsTripEnded
    {
        get
        {
            return transform.position.y < endingPosition.y;
        }
    }

    public bool IsReady { get; set; }
    
    public float localXMin;
    public float localXMax;
    public float localYMin;
    public float localYMax;

    public float worldXMin;
    public float worldXMax;
    public float worldYMin;
    public float worldYMax;

    // TODO: might have to use a grid system to handle object overlapping
    private Grid containerGrid;
    private List<Vector3> feasiblePathLocalPositionList;
    private HashSet<int> occupiedCellPositions;
    private GameObject player;
    private bool isMoving = false;

    // Use this for initialization
    private void Start()
    {
        localXMin = -width / 2;
        localXMax = width / 2;
        localYMin = -height / 2;
        localYMax = height / 2;

        worldXMin = transform.position.x + localXMin;
        worldXMax = transform.position.x + localXMax;
        worldYMin = transform.position.y + localYMin;
        worldYMax = transform.position.y + localYMax;

        containerGrid = GetComponent<Grid>();
        feasiblePathLocalPositionList = new List<Vector3>();
        occupiedCellPositions = new HashSet<int>();
        player = GameObject.Find("Player");
        
    }

    // Update is called once per frame
    private void Update()
    {   
        // TODO: adaptively adjust speed based on player skill
        if (isMoving)
        {
            transform.Translate(translate * speed * Time.deltaTime);
        }
        
        // We do not want the container to slide down forever
        // If it slides out of screen we will reposition it at top
        // TODO: need to get world units in a better way
        // TODO: need to use two containers so objects don't disappear halfway
        //if (transform.position.y < -15.0f)
        //{
        //    Debug.Log("Obstacle container reposition");
        //    transform.position = new Vector3(transform.position.x, 8.0f, transform.position.z);
        //}

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void Reset()
    {
        StopMoving();

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        IsReady = false;
        transform.position = originalPosition;
        feasiblePathLocalPositionList = new List<Vector3>();
        occupiedCellPositions = new HashSet<int>();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    /// <summary>
    /// Add obstacle using absolute positioning
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    public void AddObject(GameObject obj, Vector3? position = null)
    {
        if (position == null)
        {
            position = transform.position;
        }

        obj.transform.parent = transform;
        obj.transform.position = position.Value;
    }

    /// <summary>
    /// Add obstalce to the container and position it relative to the container's pivot (center of container)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    public void AddObjectUsingRelativePosition(GameObject obj, Vector3 position)
    {        
        if (position.x < transform.position.x - width / 2 || position.x > transform.position.x + width / 2 
            || position.y < transform.position.y - height / 2 || position.y > transform.position.y + height / 2)
        {
            Debug.LogWarning("Object pivot exceeds container boundary");
        }

        obj.transform.parent = transform;
        obj.transform.localPosition = position;
    }
    
    /// <summary>
    /// Add obstacle to the container using a vector of width/height ratio
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="posInRatio"></param>
    public void AddObjectUsingNormalizedPosition(GameObject obj, Vector3 normPos)
    {

    }
    
    /// <summary>
    /// Add object to container grid using cell position
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cellPosition"></param>
    public void AddObjectToGridUsingCellPosition(GameObject obj, Vector3Int cellPosition)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = containerGrid.CellToLocal(cellPosition);
    }

    /// <summary>
    /// Add obstacles to the container, ensures that there will always be a path through using restricting gap variables
    /// Obstacles that cannot fit inside the container will be discarded
    /// </summary>
    /// <param name="obstacles"></param>
    public void AddObjectsToRandomPositions(List<GameObject> objList)
    {
        int discardCount = 0;
        foreach (GameObject obstacle in objList)
        {
            float xPos = Random.Range(localXMin, localXMax);
            float yPos = Random.Range(localYMin, localYMax);
            Vector3 pos = new Vector3(xPos, yPos, 0);
            if (IsObjectWithinHorizontalBoundaries(obstacle, pos))
            {
                AddObjectUsingRelativePosition(obstacle, pos);
            }
            else
            {
                Destroy(obstacle);
                discardCount++;
            }
            
        }
        Debug.LogWarning(discardCount + " object(s) discarded");
    }

    public void AddObjectsToRandomGridPositions(List<GameObject> objList)
    {
        int discardCount = 0;
        foreach (GameObject obstacle in objList)
        {
            float xPos = Random.Range(localXMin, localXMax);
            float yPos = Random.Range(localYMin, localYMax);
            Vector3 pos = new Vector3(xPos, yPos, 0);
            // convert to grid units and get that cell position in local units before checking!
            Vector3Int cellPosition = containerGrid.LocalToCell(pos);
            Vector3 localPosition = containerGrid.CellToLocal(cellPosition);
            if (IsObjectWithinHorizontalBoundaries(obstacle, localPosition) && !WillObjectOverlapWithOccupiedPositions(obstacle, localPosition))
            {
                //occupiedCellPositions.Add(HashVector3Int(cellPosition));
                AddObjectUsingRelativePosition(obstacle, localPosition);
            }
            else
            {
                Destroy(obstacle);
                discardCount++;
            }

        }
        Debug.LogWarning(discardCount + " object(s) discarded");
    }

    /// <summary>
    /// Generate random path from bottom to top which cannot have obstacles overlapped with
    /// This ensures there's always a way out for each container (no impossible plays)
    /// 
    /// Caller of this method should pass in a local initial position and ensure that path is continuous
    /// </summary>
    /// <param name="localInitialPosition"></param>
    /// <param name="pathWidth"></param>
    /// <param name="variation">The more variation the difficult it gets</param>
    public void GenerateFeasiblePath(int pathWidth, int variation, GameObject lastContainerObject)
    {
        float initXCellPos = containerGrid.WorldToLocal(new Vector3(player.transform.position.x, 0, 0)).x;
        Vector3Int beginCell = containerGrid.LocalToCell(new Vector3(initXCellPos, localYMin, 0));
        if (lastContainerObject != null)
        {
            ObstacleContainer lastContainer = lastContainerObject.GetComponent<ObstacleContainer>();
            beginCell = containerGrid.LocalToCell(new Vector3(lastContainer.feasiblePathLocalPositionList.Last().x, localYMin, 0));
        }

        int beginCellYPos = beginCell.y;
        int endCellYPos = containerGrid.LocalToCell(new Vector3(0, localYMax, 0)).y;
        int cellMinXPos = containerGrid.LocalToCell(new Vector3(transform.position.x - width / 2, 0, 0)).x;
        int cellMaxXPos = containerGrid.LocalToCell(new Vector3(transform.position.x + width / 2, 0, 0)).x;

        int previousCellXPos = beginCell.x;
        for (int currentCellYPos = beginCellYPos; currentCellYPos <= endCellYPos; currentCellYPos++)
        {
            int currentXMin = previousCellXPos - variation < cellMinXPos ? previousCellXPos : previousCellXPos - variation;
            int currentXMax = previousCellXPos + variation + pathWidth > cellMaxXPos ? previousCellXPos - pathWidth : previousCellXPos + variation;
            int currentCellXPos = Random.Range(currentXMin, currentXMax + 1);
            Vector3Int currentCell = new Vector3Int(currentCellXPos, currentCellYPos, 0);
            feasiblePathLocalPositionList.Add(containerGrid.CellToLocal(currentCell));

            // TODO: the algorithm may not be exactly correct but can be used
            occupiedCellPositions.Add(HashVector3Int(currentCell));
            for (int i = 1; i < pathWidth; i++)
            {
                Vector3Int pathCell = new Vector3Int(currentCell.x + i, currentCell.y, currentCell.z);
                occupiedCellPositions.Add(HashVector3Int(pathCell));
                feasiblePathLocalPositionList.Add(containerGrid.CellToLocal(pathCell));
            }

            previousCellXPos = currentCell.x;
        }

        if (shouldVisualizePath)
        {
            foreach (Vector3 position in feasiblePathLocalPositionList)
            {
                GameObject pathIndicator = Instantiate(pathCellPrefab, transform.position, Quaternion.identity);
                AddObjectUsingRelativePosition(pathIndicator, position);
            }
        }
    }

    private bool IsObjectWithinBoundary(GameObject obj, Vector3 position)
    {
        Vector3 scale = obj.transform.localScale;
        float width = scale.x;
        float height = scale.y;
        float xPos = position.x;
        float yPos = position.y;

        return xPos - (width / 2) >= localXMin && xPos + (width / 2) <= localXMax
            && yPos - (height / 2) >= localYMin && yPos + (height / 2) <= localYMax;
    }

    /// <summary>
    /// Ensures that object will not appear in camera before container starts to move
    /// or be destroyed before leaving screen completely because it exceeds container boundaries
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsObjectWithinHorizontalBoundaries(GameObject obj, Vector3 position)
    {
        Vector3 scale = obj.transform.localScale;
        float width = scale.x;
        float height = scale.y;
        float xPos = position.x;
        float yPos = position.y;

        return yPos - (height / 2) >= localYMin && yPos + (height / 2) <= localYMax;
    }

    /// <summary>
    /// Determines whether object will overlap with occupied positions
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool WillObjectOverlapWithOccupiedPositions(GameObject obj, Vector3 position)
    {
        Vector3 scale = obj.transform.localScale;
        float width = scale.x;
        float height = scale.y;
        float xPos = position.x;
        float yPos = position.y;

        int xMinCell = containerGrid.LocalToCell(new Vector3(xPos - width / 2, 0, 0)).x;
        int xMaxCell = containerGrid.LocalToCell(new Vector3(xPos + width / 2, 0, 0)).x;
        int yMinCell = containerGrid.LocalToCell(new Vector3(0, yPos - height / 2, 0)).y;
        int yMaxCell = containerGrid.LocalToCell(new Vector3(0, yPos + height / 2, 0)).y;

        for (int i = xMinCell; i <= xMaxCell; i++)
        {
            for (int j = yMinCell; j <= yMaxCell; j++)
            {
                if (occupiedCellPositions.Contains(HashVector3Int(new Vector3Int(i, j, 0))))
                {
                    return true;
                }
                occupiedCellPositions.Add(HashVector3Int(new Vector3Int(i, j, 0)));
            }
        }

        return false;
    }

    public static int HashVector3Int(Vector3Int vec)
    {
        return vec.x * 1000 + vec.z + vec.y * 1000000;
    }
}
