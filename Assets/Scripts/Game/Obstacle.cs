using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public enum Shapes
    {
        Circle,
        Hexagon,
        Square,
        Star,
        Triangle
    }

    public Shapes shape;

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
    
    public void SetRandomColor()
    {
        transform.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV(0, 1.00f, 0, 1.00f, 0.90f, 1.00f);
    }

    public void SetRandomScale(float min, float max)
    {
        float rand = Random.Range(min, max);
        SetObjectScale(new Vector3(rand, rand, 1));
    }

    public void SetObjectScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void SetRandomRotation()
    {
        //float xAngle = Random.Range(0, 10);
        //float yAngle = Random.Range(0, 10);
        float zAngle = Random.Range(0, 359);
        transform.Rotate(new Vector3(0, 0, zAngle));
    }

}
