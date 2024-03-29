﻿using System.Collections;
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
        // galaxy
        if (ScoreManager.score > 500000 && !hasCollectableAppeared[4])
        {
            GameObject galaxy = Instantiate(collectablePrefabArray[4], sliderObject.transform.position, Quaternion.identity);
            galaxy.transform.parent = sliderObject.transform;
            hasCollectableAppeared[4] = true;
            //DataManager.SaveUnlockedCollectable(2);
            return galaxy;
        }

        // sun
        if (ScoreManager.score > 200000 && !hasCollectableAppeared[3])
        {
            GameObject sun = Instantiate(collectablePrefabArray[3], sliderObject.transform.position, Quaternion.identity);
            sun.transform.parent = sliderObject.transform;
            hasCollectableAppeared[3] = true;
            //DataManager.SaveUnlockedCollectable(6);
            return sun;
        }

        // moon
        if (ScoreManager.score > 100000 && !hasCollectableAppeared[2])
        {
            GameObject moon = Instantiate(collectablePrefabArray[2], sliderObject.transform.position, Quaternion.identity);
            moon.transform.parent = sliderObject.transform;
            hasCollectableAppeared[2] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return moon;
        }

        // satellite
        if (ScoreManager.score > 50000 && !hasCollectableAppeared[0])
        {
            GameObject satellite = Instantiate(collectablePrefabArray[0], sliderObject.transform.position, Quaternion.identity);
            satellite.transform.parent = sliderObject.transform;
            hasCollectableAppeared[0] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return satellite;
        }

        // tesla (audi)
        if (ScoreManager.score > 150000 && Random.Range(0, 10.00f) < 0.01f && !hasCollectableAppeared[1])
        {
            GameObject car = Instantiate(collectablePrefabArray[1], sliderObject.transform.position, Quaternion.identity);
            car.transform.parent = sliderObject.transform;
            hasCollectableAppeared[1] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return car;
        }

        // dark planet
        if (Random.Range(0, 1000.00f) < 0.01f && !hasCollectableAppeared[6])
        {
            GameObject darkPlanet = Instantiate(collectablePrefabArray[6], sliderObject.transform.position, Quaternion.identity);
            darkPlanet.transform.parent = sliderObject.transform;
            hasCollectableAppeared[6] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return darkPlanet;
        }

        // planet DJ
        if (Random.Range(0, 100.00f) < 0.01f && !hasCollectableAppeared[5])
        {
            GameObject planetDJ = Instantiate(collectablePrefabArray[5], sliderObject.transform.position, Quaternion.identity);
            planetDJ.transform.parent = sliderObject.transform;
            hasCollectableAppeared[5] = true;
            //DataManager.SaveUnlockedCollectable(3);
            return planetDJ;
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
