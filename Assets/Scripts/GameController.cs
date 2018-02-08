using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Tooltip("Prefab array for obstacles")]
    public GameObject[] obstaclePrefabArray;

    [Tooltip("Number of container being used by the game (must be greater than max containers moving at the same time)")]
    public int containerCount = 2;

    [Tooltip("Number of containers allowed to move at the same time")]
    public int maxContainersInProgress = 1;

    public GameObject obstacleContainerPrefab;

    private int prefabCount;
    private Queue<GameObject> containerQueue;
    private List<GameObject> InProgressContainerList;

    private Vector3 screenBottomLeftPos;
    private Vector3 screenUpperRightPos;

    // Use this for initialization
    private void Start()
    {
        // initialize data structures
        prefabCount = obstaclePrefabArray.Length;
        containerQueue = new Queue<GameObject>();
        InProgressContainerList = new List<GameObject>();

        // calculate screen size in world point units
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        screenBottomLeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        screenUpperRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));

        // initialize containers' container
        GameObject containerCollection = GameObject.Find("ContainerCollection");
        if (containerCollection == null)
        {
            containerCollection = new GameObject("ContainerCollection");
        }
        containerCollection.transform.position = new Vector3(0, 0, 0);

        // initialize container
        for (int i = 0; i < containerCount; i++)
        {
            // containerObject will contain a grid
            GameObject containerObject = Instantiate(obstacleContainerPrefab, containerCollection.transform);
            ObstacleContainer container = containerObject.GetComponent<ObstacleContainer>();
            container.originalPosition = new Vector3(0, screenUpperRightPos.y + container.height / 2);
            container.endingPosition = new Vector3(0, screenBottomLeftPos.y - container.height / 2);
            container.Reset();
            containerQueue.Enqueue(containerObject);
        }

    }

    // Update is called once per frame
    private void Update()
    {
        // initialize next container
        GameObject firstContainerObject = containerQueue.Peek();
        ObstacleContainer firstContainer = firstContainerObject.GetComponent<ObstacleContainer>();
        if (!firstContainer.IsReady)
        {
            GenerateLayout(firstContainerObject);
            firstContainer.IsReady = true;
        }


        // update in progress containers
        if (InProgressContainerList.Count < maxContainersInProgress)
        {
            if (InProgressContainerList.Count == 0 || HasContainerCompletelyLeft(InProgressContainerList.Last()))
            {
                GameObject availableContainer = containerQueue.Dequeue();
                availableContainer.GetComponent<ObstacleContainer>().StartMoving();
                InProgressContainerList.Add(availableContainer);
            }            
        }

        // recycle used containers
        for (int i = InProgressContainerList.Count - 1; i >= 0; i--)
        {
            GameObject containerObject = InProgressContainerList[i];
            ObstacleContainer container = containerObject.GetComponent<ObstacleContainer>();
            if (container.IsTripEnded)
            {
                container.StopMoving();
                container.Reset();
                InProgressContainerList.RemoveAt(i);
                containerQueue.Enqueue(containerObject);
            }
        }

    }

    private void GenerateLayout(GameObject containerObject)
    {
        ObstacleContainer container = containerObject.GetComponent<ObstacleContainer>();
        List<GameObject> obstacleList = new List<GameObject>();
        for (int i = 0; i < 6; i++)
        {
            GameObject obstacleObject = GetRandomObstacle();
            SetRandomColor(obstacleObject);
            SetRandomScale(obstacleObject, 1.00f, 3.00f);
            //container.AddObstacleUsingRelativePosition(obstacleObject, new Vector3(i, 0, 0));
            obstacleList.Add(obstacleObject);
        }

        container.AddObjectsToRandomGridPositions(obstacleList);

        // TODO: test handling big objects, they should not be intentionally be placed outside of container,
        // container should throw an error in that case
        GameObject bigObstacleObject = GetRandomObstacle();
        bigObstacleObject.transform.localScale = new Vector3(9, 9, 0);
        container.AddObjectUsingRelativePosition(bigObstacleObject, new Vector3(-5.5f, 0, 0));
        
        container.GenerateFeasiblePath(1, 2, InProgressContainerList.Count > 0 ? InProgressContainerList.Last() : null);
       
    }

    // TODO: must ensure there's a way through
    // TODO: must generate multiple rows and columns
    // TODO: Bigger objects might rotate at a slower speed
    private void GenerateRandomLayout(GameObject containerObject, int diffLevel)
    {
        
    }

    private GameObject GetRandomObstacle()
    {
        int index = Random.Range(0, prefabCount);
        GameObject prefab = obstaclePrefabArray[index];
        if (prefab == null)
        {
            Debug.LogError("Spawn obstacle failed");
        }
        GameObject obstacle = Instantiate(prefab);
        return obstacle;
    }

    private bool HasContainerCompletelyLeft(GameObject containerObject)
    {
        ObstacleContainer container = containerObject.GetComponent<ObstacleContainer>();
        return containerObject.transform.position.y + container.height / 2 <= screenUpperRightPos.y;
    }

    private void SetRandomColor(GameObject obstacleObject)
    {
        obstacleObject.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV();
    }

    private void SetRandomScale(GameObject obstacleObject, float min, float max)
    {
        float rand = Random.Range(min, max);
        SetScale(obstacleObject, new Vector3(rand, rand, 1));
    }

    private void SetScale(GameObject obstacleObject, Vector3 scale)
    {
        obstacleObject.transform.localScale = scale;
    }

    private void SetParent(GameObject child, GameObject parent)
    {
        child.transform.parent = parent.transform;
    }
}
