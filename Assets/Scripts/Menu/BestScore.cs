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
        int bestScore = DataManager.GetBestScore();
        bestScoreText.text = string.Format("Best: {0} mi", bestScore.ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LogBestScore()
    {
        if (ScoreManager.score > bestScore)
        {
            DataManager.SaveBestScore(ScoreManager.score);
            bestScore = ScoreManager.score;
        }
    }
}
