using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour {

    public static Dictionary<string, object> dataDict = null;

    const string BEST_SCORE = "bestScore";
    const string UNLOCKED_COLLECTABLES = "unlockedCollectables";

    private void Awake()
    {
        ReadData();

        if (dataDict == null)
        {
            dataDict = new Dictionary<string, object>
            {
                { BEST_SCORE, 0 },
                { UNLOCKED_COLLECTABLES, new bool[7] }
            };
            SaveData();
        }

    }

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public static int GetBestScore()
    {
        return (int)dataDict[BEST_SCORE];
    }

    public static void SaveBestScore(int score)
    {
        dataDict[BEST_SCORE] = score;
        SaveData();
    }

    public static bool IsCollectableUnlocked(int index)
    {
        bool[] unlocked = (bool[])dataDict[UNLOCKED_COLLECTABLES];
        return unlocked[index];
    }

    public static void SaveUnlockedCollectable(int index)
    {
        bool[] unlocked = (bool[])dataDict[UNLOCKED_COLLECTABLES];
        unlocked[index] = true;
        SaveData();
    }

    public static void ReadData()
    {
        if (File.Exists(Application.persistentDataPath + "/gameData.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.gd", FileMode.Open);
            dataDict = (Dictionary<string, object>)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        // overwrites existing file
        FileStream file = File.Open(Application.persistentDataPath + "/gameData.gd", FileMode.Create);
        bf.Serialize(file, dataDict);
        file.Close();
    }
}
