using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleContainer : MonoBehaviour {

    [Tooltip("Direction container is moving towards")]
    public Vector3 translate = Vector3.down;

    [Tooltip("Moving speed of current container")]
    public float speed = 3.00f;

    [Tooltip("Container width in world units")]
    public float width = 9.00f;

    [Tooltip("Container height in world units")]
    public float height = 16.00f;

    [Tooltip("Original position for container")]
    public Vector3 originalPosition;

    [Tooltip("Ending position for the container. Container may be recycled upon reaching this position")]
    public Vector3 endingPosition;

    private bool isMoving = false;

    public bool IsTripEnded
    {
        get
        {
            // TODO: calculate value based on current container height
            return transform.position.y < endingPosition.y;
        }
    }

    public bool IsReady { get; set; }

    // Use this for initialization
    private void Start()
    {
        
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
    
    // TODO: need to restrict obstacle to not exceed current container
    public void AddObstacle(GameObject obstacle, Vector3? position = null)
    {
        if (position == null)
        {
            position = transform.position;
        }

        obstacle.transform.parent = transform;
        obstacle.transform.position = position.Value;
    }

    public void AddObstacleUsingRelativePosition(GameObject obstacle, Vector3 position)
    {
        obstacle.transform.parent = transform;
        obstacle.transform.localPosition = position;
    }
    
}
