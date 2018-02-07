using System.Collections;
using System.Collections.Generic;
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

    public bool IsTripEnded
    {
        get
        {
            return transform.position.y < endingPosition.y;
        }
    }

    public bool IsReady { get; set; }

    private bool isMoving = false;

    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;
    
    // Use this for initialization
    private void Start()
    {
        xMin = -width / 2;
        xMax = width / 2;
        yMin = -height / 2;
        yMax = height / 2;
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
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    /// <summary>
    /// Add obstacle using absolute positioning
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="position"></param>
    public void AddObstacle(GameObject obstacle, Vector3? position = null)
    {
        if (position == null)
        {
            position = transform.position;
        }

        obstacle.transform.parent = transform;
        obstacle.transform.position = position.Value;
    }

    /// <summary>
    /// Add obstalce to the container and position it relative to the container's pivot (center of container)
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="position"></param>
    public void AddObstacleUsingRelativePosition(GameObject obstacle, Vector3 position)
    {        
        if (position.x < transform.position.x - width / 2 || position.x > transform.position.x + width / 2 
            || position.y < transform.position.y - height / 2 || position.y > transform.position.y + height / 2)
        {
            Debug.LogWarning("Object pivot exceeds container boundary");
        }

        obstacle.transform.parent = transform;
        obstacle.transform.localPosition = position;
    }

    /// <summary>
    /// Add obstacle to the container using a vector of width/height ratio
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="posInRatio"></param>
    public void AddObstacleUsingRatioVector(GameObject obstacle, Vector3 posInRatio)
    {

    }
    
    /// <summary>
    /// Add obstacles to the container, ensures that there will always be a path through using restricting gap variables
    /// Obstacles that cannot fit inside the container will be discarded
    /// </summary>
    /// <param name="obstacles"></param>
    public void AddObstalcesToRandomPositions(List<GameObject> obstacleList)
    {
        int discardCount = 0;
        foreach (GameObject obstacle in obstacleList)
        {
            float xPos = Random.Range(xMin, xMax);
            float yPos = Random.Range(yMin, yMax);
            Vector3 pos = new Vector3(xPos, yPos, 0);
            if (IsObstacleWithinHorizontalBoundaries(obstacle, pos))
            {
                AddObstacleUsingRelativePosition(obstacle, pos);
            }
            else
            {
                Destroy(obstacle);
                discardCount++;
            }
            
        }
        Debug.LogWarning(discardCount + " object(s) discarded");
    }

    private bool IsObstacleWithinBoundary(GameObject obstacle, Vector3 position)
    {
        Vector3 scale = obstacle.transform.localScale;
        float width = scale.x;
        float height = scale.y;
        float xPos = position.x;
        float yPos = position.y;

        return xPos - (width / 2) >= xMin && xPos + (width / 2) <= xMax
            && yPos - (height / 2) >= yMin && yPos + (height / 2) <= yMax;
    }

    /// <summary>
    /// Ensures that object will not appear in camera before container starts to move
    /// or be destroyed before leaving screen completely because it exceeds container boundaries
    /// </summary>
    /// <param name="obstacle"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsObstacleWithinHorizontalBoundaries(GameObject obstacle, Vector3 position)
    {
        Vector3 scale = obstacle.transform.localScale;
        float width = scale.x;
        float height = scale.y;
        float xPos = position.x;
        float yPos = position.y;

        return yPos - (height / 2) >= yMin && yPos + (height / 2) <= yMax;
    }
}
