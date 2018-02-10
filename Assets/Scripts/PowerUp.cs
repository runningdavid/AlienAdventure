using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public enum Effects
    {
        Invincible,
        Recover,
        Sprint,
        ScoreMultiplier,
        Gift
    }

    public Effects effect;

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            switch (effect)
            {
                case Effects.Invincible:
                    break;
                case Effects.Recover:
                    break;
                case Effects.Sprint:
                    break;
                case Effects.ScoreMultiplier:
                    break;
                case Effects.Gift:
                    break;
                default:
                    break;
            }
            Destroy(gameObject);
        }
    }
}
