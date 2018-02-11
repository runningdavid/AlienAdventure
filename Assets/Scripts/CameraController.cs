using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Vector3 cameraTranslate = Vector3.up;
    public float cameraSpeed = 0.00f;

    public bool isFollowEnabled = false;
    public GameObject playerObject;
    public Vector3 offset = new Vector3(0, 0, -10);
    public Color endColor;
    public float timeToTransitToEndColor;

    private Color beginColor;
    private bool isTransition = false;

    // Use this for initialization
    private void Start()
    {
        ColorUtility.TryParseHtmlString("#D0FFFB8D", out beginColor);

        if (isFollowEnabled && playerObject == null)
        {
            Debug.LogError("Camera is set to follow mode yet no gameObject is specified to follow");
        }
    }

    // Update is called once per frame
    private void Update()
    {   
        if (isTransition && timeToTransitToEndColor > Time.deltaTime)
        {
            Color transitionColor = Color.Lerp(beginColor, endColor, Time.deltaTime / timeToTransitToEndColor);
            Camera.main.backgroundColor = transitionColor;
            beginColor = transitionColor;
            timeToTransitToEndColor -= Time.deltaTime;
        }

        if (isFollowEnabled)
        {
            transform.position = playerObject.transform.position + offset;
        }
        else
        {
            transform.Translate(cameraTranslate * cameraSpeed * Time.deltaTime);
        }
        
    }

    public void StartColorTransition()
    {
        isTransition = true;
    }
}
