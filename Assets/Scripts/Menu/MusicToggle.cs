using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour {

    public Sprite musicOnSprite;

    public Sprite musicOffSprite;

    // Using awake so Player.cs can detect MusicToggle position in start
    private void Awake()
    {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 bottomLeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        Vector3 musicTogglePos = bottomLeftPos + new Vector3(1.00f, 0.75f, 0);
        transform.position = musicTogglePos;

        if (PlayerPrefsManager.IsMusicMuted())
        {
            GetComponent<Image>().sprite = musicOffSprite;
        }
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HandleClick()
    {
        if (PlayerPrefsManager.IsMusicMuted())
        {
            // turn music on
            gameObject.GetComponent<Image>().sprite = musicOnSprite;
            GameObject.FindObjectOfType<MusicPlayer>().PlayMusic();
        }
        else
        {
            // turn music off
            gameObject.GetComponent<Image>().sprite = musicOffSprite;
            GameObject.FindObjectOfType<MusicPlayer>().StopMusic();
        }
    }
}
