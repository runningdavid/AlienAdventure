using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Tooltip("Prefab array for obstacles")]
    public GameObject[] obstaclePrefabArray;

    [Tooltip("Number of container being used by the game")]
    public int containerCount = 2;

    [Tooltip("Number of containers allowed to move at the same time")]
    public int numContainersInProgress = 1;

    private int prefabCount;
    private Queue<GameObject> containerQueue;
    private List<GameObject> InProgressContainerList;

    // Use this for initialization
    private void Start()
    {
        // initialize data structures
        prefabCount = obstaclePrefabArray.Length;
        containerQueue = new Queue<GameObject>();
        InProgressContainerList = new List<GameObject>();

        // calculate screen size in world point units
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 bottomLeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        Vector3 upperRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));

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
            GameObject containerObject = new GameObject("_container_" + i);
            ObstacleContainer container = containerObject.AddComponent<ObstacleContainer>();
            container.originalPosition = new Vector3(0, upperRightPos.y + container.height / 2);
            container.endingPosition = new Vector3(0, bottomLeftPos.y - container.height / 2);
            SetParent(containerObject, containerCollection);
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
            GenerateFixedLayout(firstContainerObject);
            firstContainer.IsReady = true;
        }

        // update in progress containers
        if (InProgressContainerList.Count < numContainersInProgress)
        {
            GameObject availableContainer = containerQueue.Peek();
            availableContainer.GetComponent<ObstacleContainer>().StartMoving();
            InProgressContainerList.Add(availableContainer);
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

    private void GenerateFixedLayout(GameObject containerObject)
    {
        ObstacleContainer container = containerObject.GetComponent<ObstacleContainer>();
        for (int i = 0; i < 5; i++)
        {
            GameObject obstacleObject = GetRandomObstacle();
            SetParent(obstacleObject, containerObject);
            SetRandomColor(obstacleObject);
            container.AddObstacleUsingRelativePosition(obstacleObject, new Vector3(i, 0, 0));
        }

        // TODO: test handling big objects, they should not be intentionally be placed outside of container,
        // container should throw an error in that case
        GameObject bigObstacleObject = GetRandomObstacle();
        bigObstacleObject.transform.localScale = new Vector3(9, 9, 0);
        container.AddObstacleUsingRelativePosition(bigObstacleObject, new Vector3(-5.5f, 0, 0));
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

    private void SetRandomColor(GameObject obstacleObject)
    {
        obstacleObject.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV();
    }

    private void SetParent(GameObject child, GameObject parent)
    {
        child.transform.parent = parent.transform;
    }
}
