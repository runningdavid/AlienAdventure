using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(new Vector3(Time.deltaTime * rotationSpeed.x, Time.deltaTime * rotationSpeed.y, Time.deltaTime * rotationSpeed.z));
    }
}
