﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slider : MonoBehaviour {

    [Tooltip("An array of obstacle prefabs")]
    public GameObject[] obstaclePrefabArray;

    [Tooltip("The direction slider will be moving towards")]
    public Vector3 translate = Vector3.down;

    [Tooltip("Moving speed of current container")]
    public float speed = 5.00f;
    
    [Tooltip("Slider width")]
    public float width = 9.00f;

    [Tooltip("Slider height")]
    public float height = 16.00f;

    public Vector3 TopRightWorldPos
    {
        get
        {
            Vector3 currentPos = transform.position;
            return new Vector3(currentPos.x + width / 2, currentPos.y + height / 2, 0);
        }
    }

    public Vector3 BottomLeftWorldPos
    {
        get
        {
            Vector3 currentPos = transform.position;
            return new Vector3(currentPos.x - width / 2, currentPos.y - height / 2, 0);
        }
    }

    public Vector3 TopRightLocalPos
    {
        get
        {
            return new Vector3(width / 2, height / 2, 0);
        }
    }

    public Vector3 BottomLeftLocalPos
    {
        get
        {
            return new Vector3(-width / 2, -height / 2, 0);
        }
    }

    // TODO: might have to use a grid system to handle object overlapping
    private Grid sliderGrid;
    private HashSet<int> occupiedCells;
    private bool isMoving = false;

    private Vector3Int TopRightCellPos
    {
        get
        {
            return sliderGrid.LocalToCell(new Vector3(TopRightLocalPos.x, TopRightLocalPos.y, 0));
        }
    }

    private Vector3Int BottomLeftCellPos
    {
        get
        {
            return sliderGrid.LocalToCell(new Vector3(BottomLeftLocalPos.x, BottomLeftLocalPos.y, 0));
        }
    }

    // Use this for initialization
    private void Start()
    {
        // initialize grid
        sliderGrid = GetComponent<Grid>();

        // initialize grid mask
        occupiedCells = new HashSet<int>();
    }

    // Update is called once per frame
    private void Update()
    {   
        // TODO: adaptively adjust speed based on player skill
        if (isMoving)
        {
            transform.Translate(translate * speed * Time.deltaTime);
        }
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
        
        // Destroy all obstacles
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // reset masks
        occupiedCells = new HashSet<int>();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    /// <summary>
    /// Add obstacle to a position in world units
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    public void AddObjectToWorldPosition(GameObject obj, Vector3? position = null)
    {
        if (position == null)
        {
            position = transform.position;
        }

        obj.transform.parent = transform;
        obj.transform.position = position.Value;
    }

    /// <summary>
    /// Add obstalce to the slider using a position in local units (relative to center of slider)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    public void AddObjectToLocalPosition(GameObject obj, Vector3 position)
    {      
        // TODO: handle rotated object position so it won't exceed screen
        obj.transform.parent = transform;
        obj.transform.localPosition = position;
    }
    
    /// <summary>
    /// Add object to slider using a cell position in grid
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cellPosition"></param>
    public void AddObjectToCell(GameObject obj, Vector3Int cellPosition)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = sliderGrid.CellToLocal(cellPosition);
    }

    /// <summary>
    /// Major method to generate obstacle layouts
    /// </summary>
    /// <param name="objList"></param>
    public void GenerateObstacles()
    {
        float generationProbability = 0.03f;
        float rotationProbability = 0.5f;

        for (int i = BottomLeftCellPos.x; i <= TopRightCellPos.x; i++)
        {
            for (int j = BottomLeftCellPos.y; j <= TopRightCellPos.y; j++)
            {
                if (Random.Range(0.00f, 1.00f) <= generationProbability)
                {
                    GameObject obstacleObject = GetRandomObstacle();
                    SetRandomColor(obstacleObject);
                    SetRandomScale(obstacleObject, 1.00f, 6.00f);
                    SetRandomRotation(obstacleObject);

                    Vector3Int cellPos = new Vector3Int(i, j, 0);
                    obstacleObject.transform.localPosition = sliderGrid.CellToLocal(cellPos);

                    if (IsObjectInBounds(obstacleObject, "horizontal") && !IsObjectPositionOccupied(obstacleObject))
                    {
                        // object is already there, no need to add again
                        MarkOccupied(obstacleObject);
                        
                        // TODO: experiment with obstacle spinning, but when spinning it might exceed horizontal position again
                        // need to use bounds to calculate maximum possible object width
                        if (Random.Range(0.00f, 1.00f) <= rotationProbability)
                        {
                            float xSpeed = Random.Range(-10.00f, 10.00f);
                            float ySpeed = 0;
                            float zSpeed = Random.Range(-50.00f, 50.00f);
                            obstacleObject.GetComponent<Obstacle>().StartSpinning(new Vector3(xSpeed, ySpeed, zSpeed));
                        }
                    }
                    else
                    {
                        Destroy(obstacleObject);
                    }

                }
            }
        }
    }

    #region Obstacle Generation

    private GameObject GetRandomObstacle()
    {
        int index = Random.Range(0, obstaclePrefabArray.Length);
        GameObject prefab = obstaclePrefabArray[index];
        if (prefab == null)
        {
            Debug.LogError("Failed to get obstacle prefab");
        }
        GameObject obstacle = Instantiate(prefab, transform);
        return obstacle;
    }

    private void SetRandomColor(GameObject obstacleObject)
    {
        obstacleObject.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV();
    }

    private void SetRandomScale(GameObject obstacleObject, float min, float max)
    {
        float rand = Random.Range(min, max);
        SetObjectScale(obstacleObject, new Vector3(rand, rand, 1));
    }

    private void SetObjectScale(GameObject obstacleObject, Vector3 scale)
    {
        obstacleObject.transform.localScale = scale;
    }

    private void SetRandomRotation(GameObject obstacleObject)
    {
        float xAngle = Random.Range(0, 30);
        float yAngle = Random.Range(0, 30);
        float zAngle = Random.Range(0, 359);
        obstacleObject.transform.Rotate(new Vector3(xAngle, yAngle, zAngle));
    }

    #endregion

    /// <summary>
    /// Checks if object is within boundaries
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="direction">
    /// Can be "vertical", "horizontal", or "all".
    /// "vertical" - checks vertical bounds only
    /// "horizontal" - checks horizontal bounds only
    /// "all" - checks all bounds
    /// </param>
    /// <returns>True if object is within specified bounds, false otherwise</returns>
    private bool IsObjectInBounds(GameObject obj, string direction = "all")
    {
        // use bounds to handle rotated objects
        Bounds bounds = obj.GetComponentInChildren<SpriteRenderer>().bounds;
        Vector3 minPos = sliderGrid.WorldToLocal(bounds.min);
        Vector3 maxPos = sliderGrid.WorldToLocal(bounds.max);

        bool isInVerticalBounds = minPos.x >= BottomLeftLocalPos.x && maxPos.x <= TopRightLocalPos.x;
        bool isInHorizontalBounds = minPos.y >= BottomLeftLocalPos.y && maxPos.y <= TopRightLocalPos.y;
        
        switch (direction)
        {
            case "vertical":
                return isInVerticalBounds;
            case "horizontal":
                return isInHorizontalBounds;
            case "all":
                return isInVerticalBounds && isInHorizontalBounds;
            default:
                throw new System.Exception("Invalid parameter received: direction");
        }
    }
    
    /// <summary>
    /// Determines whether an object's location is already occupied
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool IsObjectPositionOccupied(GameObject obj)
    {
        Bounds bounds = obj.GetComponentInChildren<SpriteRenderer>().bounds;
        Vector3Int minPos = sliderGrid.WorldToCell(bounds.min);
        Vector3Int maxPos = sliderGrid.WorldToCell(bounds.max);

        for (int i = minPos.x; i <= maxPos.x; i++)
        {
            for (int j = minPos.y; j <= maxPos.y; j++)
            {
                Vector3Int cell = new Vector3Int(i, j, 0);
                if (occupiedCells.Contains(GetHash(cell)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Mark object's occupied cell in hashset
    /// </summary>
    /// <param name="obj"></param>
    private void MarkOccupied(GameObject obj)
    {
        Bounds bounds = obj.GetComponentInChildren<SpriteRenderer>().bounds;
        Vector3Int minPos = sliderGrid.WorldToCell(bounds.min);
        Vector3Int maxPos = sliderGrid.WorldToCell(bounds.max);

        for (int i = minPos.x; i <= maxPos.x; i++)
        {
            for (int j = minPos.y; j <= maxPos.y; j++)
            {
                Vector3Int cell = new Vector3Int(i, j, 0);
                occupiedCells.Add(GetHash(cell));
            }
        }

    }

    /// <summary>
    /// This hashing method is not perfect but good enough for our small grid
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private static int GetHash(Vector3Int vec)
    {
        return vec.x * 1000 + vec.z + vec.y * 1000000;
    }
}
