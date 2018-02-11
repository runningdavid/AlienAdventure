using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour {

    public static int bestScore = 0;

    public Text bestScoreText;

	// Use this for initialization
	void Start () {
        if (File.Exists(Application.persistentDataPath + "/gameData.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.gd", FileMode.Open);
            bestScore = (int)bf.Deserialize(file);
            file.Close();
        }

        bestScoreText.text = string.Format("Best: {0} ft", bestScore.ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LogBestScore()
    {
        if (ScoreManager.score > bestScore)
        {
            BinaryFormatter bf = new BinaryFormatter();
            // overwrites existing file
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.gd", FileMode.Create);
            bf.Serialize(file, ScoreManager.score);
            bestScore = ScoreManager.score;
            file.Close();
        }
    }
}
