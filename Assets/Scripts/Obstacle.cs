using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    private bool shouldRotate = false;

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (shouldRotate)
        {
            transform.Rotate(new Vector3(Time.deltaTime * rotationSpeed.x, Time.deltaTime * rotationSpeed.y, Time.deltaTime * rotationSpeed.z));
        }
        
    }

    public void StartSpinning()
    {
        shouldRotate = true;
    }

    public void StartSpinning(Vector3 speed)
    {
        rotationSpeed = speed;
        StartSpinning();
    }

    public void StopSpinning()
    {
        shouldRotate = false;
    }
}
