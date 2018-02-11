using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseScore : MonoBehaviour {

    public Text scoreText;

	// Use this for initialization
	void Start () {
        scoreText.text = string.Format("You reached\n{0} ft", ScoreManager.score.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
