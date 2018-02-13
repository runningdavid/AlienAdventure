using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    static MusicPlayer instance = null;

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

            if (!PlayerPrefsManager.IsMusicMuted())
            {
                instance.GetComponent<AudioSource>().Play();
            }
        }
    }
    
    public void PlayMusic()
    {
        PlayerPrefsManager.SetMutedState(0);
        instance.GetComponent<AudioSource>().Play();
    }

    public void StopMusic()
    {
        PlayerPrefsManager.SetMutedState(1);
        instance.GetComponent<AudioSource>().Stop();
    }
   
}
