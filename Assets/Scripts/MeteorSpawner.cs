using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour {

    public GameObject[] meteorPrefabArray;

    public float minScale;

    public float maxScale;

    public float minGravityScale;

    public float maxGravityScale;

    public float probability = 0.03f;

    private float xMin;

    private float xMax;

    // Use this for initialization
    private void Start()
    {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 bottomLeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        Vector3 upperRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));

        xMin = bottomLeftPos.x;
        xMax = upperRightPos.x;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Random.Range(0, 1.00f) <= probability)
        {
            // generate meteor obstacle object
            GameObject meteorObject = GetRandomObstacle();
            float xPos = Random.Range(xMin, xMax);
            meteorObject.transform.position = new Vector3(xPos, transform.position.y, 0);

            // set scale and color
            Obstacle obstacle = meteorObject.GetComponent<Obstacle>();
            obstacle.SetRandomScale(minScale, maxScale);
            obstacle.SetRandomColor();

            // set rigid body 2d configurations
            Rigidbody2D rigidBody2D = meteorObject.GetComponent<Rigidbody2D>();
            rigidBody2D.bodyType = RigidbodyType2D.Dynamic;
            rigidBody2D.gravityScale = Random.Range(minGravityScale, maxGravityScale);
        }
    }

    private GameObject GetRandomObstacle()
    {
        int index = Random.Range(0, meteorPrefabArray.Length);
        GameObject prefab = meteorPrefabArray[index];
        if (prefab == null)
        {
            Debug.LogError("Failed to get obstacle prefab");
        }
        GameObject obstacle = Instantiate(prefab, transform.position, Quaternion.identity);
        obstacle.transform.parent = transform;
        return obstacle;
    }
}
