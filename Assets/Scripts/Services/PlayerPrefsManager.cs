using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour {

    // Use this for initialization
    const string MUSIC_MUTED = "music_muted";

    public static void SetMutedState(int state)
    {
        if (state == 0 || state == 1)
        {
            PlayerPrefs.SetInt(MUSIC_MUTED, state);
        }
        else
        {
            Debug.LogError("Music muted state takes only two values 0 or 1. 0 is unmuted, 1 is muted");
        }
    }

    public static bool IsMusicMuted()
    {
        if (PlayerPrefs.HasKey(MUSIC_MUTED))
        {
            return PlayerPrefs.GetInt(MUSIC_MUTED) == 1;
        }

        return false;
    }

}
