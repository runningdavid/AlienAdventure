using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static int score;

    public int scorePerSecond = 500;

    private bool isCounting = false;

    private Text scoreText;

    // Use this for initialization
    private void Start()
    {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 upperRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));
        Vector3 scoreDisplayPos = upperRightPos - new Vector3(0.30f, 0.75f, 0);
        transform.position = scoreDisplayPos;

        score = 0;
        scoreText = GetComponent<Text>();
        scoreText.text = "0 mi";
    }

    // Update is called once per frame
    private void Update()
    {
        if (isCounting)
        {
            score += Mathf.CeilToInt(scorePerSecond * Time.deltaTime);
            scoreText.text = score.ToString() + " mi";
        }
        
    }

    public void StartCounting()
    {
        isCounting = true;
    }

    public void StopCounting()
    {
        isCounting = false;
    }
}
