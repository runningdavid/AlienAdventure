using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {
    
    [Tooltip("An array of obstacle prefabs")]
    public GameObject[] obstaclePrefabArray;

    [Tooltip("Rare space collectable obstacles")]
    public GameObject[] collectablePrefabArray;

    [Tooltip("An array of power ups")]
    public GameObject[] powerUpPrefabArray;

    public static ObstacleManager instance = null;

    private bool[] hasCollectableAppeared;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        hasCollectableAppeared = new bool[collectablePrefabArray.Length];
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public GameObject GetRandomObstacle(GameObject sliderObject)
    {
        if (ScoreManager.score > 500000 && !hasCollectableAppeared[2])
        {
            GameObject galaxy = Instantiate(collectablePrefabArray[2], sliderObject.transform.position, Quaternion.identity);
            galaxy.transform.parent = sliderObject.transform;
            hasCollectableAppeared[2] = true;
            //DataManager.SaveUnlockedCollectable(2);
            return galaxy;
        }

        if (ScoreManager.score > 200000 && !hasCollectableAppeared[6])
        {
            GameObject sun = Instantiate(collectablePrefabArray[6], sliderObject.transform.position, Quaternion.identity);
            sun.transform.parent = sliderObject.transform;
            hasCollectableAppeared[6] = true;
            //DataManager.SaveUnlockedCollectable(6);
            return sun;
        }

        if (ScoreManager.score > 100000 && !hasCollectableAppeared[3])
        {
            GameObject moon = Instantiate(collectablePrefabArray[3], sliderObject.transform.position, Quaternion.identity);
            moon.transform.parent = sliderObject.transform;
            hasCollectableAppeared[3] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return moon;
        }

        int index = Random.Range(0, obstaclePrefabArray.Length);
        GameObject prefab = obstaclePrefabArray[index];
        if (prefab == null)
        {
            Debug.LogError("Failed to get obstacle prefab");
        }
        GameObject obstacle = Instantiate(prefab, sliderObject.transform.position, Quaternion.identity);
        obstacle.transform.parent = sliderObject.transform;
        return obstacle;
    }
}
