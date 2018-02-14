using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionButton : MonoBehaviour {

    private void Awake()
    {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 bottomRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distanceToCamera));
        Vector3 collectionButtonPos = bottomRightPos + new Vector3(-1.00f, 0.75f, 0);
        transform.position = collectionButtonPos;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
