using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static int score;

    public static int scorePerSecond = 1;

    private Text scoreText;

    // Use this for initialization
    private void Start()
    {
        score = 0;
        scoreText = GetComponent<Text>();
        scoreText.text = "0";
    }

    // Update is called once per frame
    private void Update()
    {
        score += Mathf.CeilToInt(scorePerSecond * Time.deltaTime);
        scoreText.text = score.ToString();
    }
}
