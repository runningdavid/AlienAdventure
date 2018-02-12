using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Vector3 playerTranslate = Vector3.up;

    public float playerSpeed = 0;

    [Tooltip("Padding of player object relative to the vertical boundary")]
    public float xPadding = 0.5f;

    [Tooltip("Padding of player object relative to the horizontal boundary")]
    public float yPadding = 0.5f;

    [Tooltip("For touch screen, we want to make some offset so that fingers don't block character")]
    public Vector3 playerMouseOffset = new Vector3(0, 0.75f, 0);

    public LevelManager levelManager;

    public bool IsInvinsible = false;

    private bool isMoving = false;
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    // Use this for initialization
    private void Start()
    {
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        Vector3 bottomLeftPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        Vector3 upperRightPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));
        xMin = bottomLeftPos.x + xPadding;
        xMax = upperRightPos.x - xPadding;
        yMin = bottomLeftPos.y + yPadding;
        yMax = upperRightPos.y - yPadding;

        transform.position = new Vector3(0, -2, 10);
    }

    // Update is called once per frame
    private void Update()
    {
        //transform.Translate(playerTranslate * playerSpeed * Time.deltaTime);
        // TODO: may need to add object rotation
        // TODO: player control method is subject to change
        if (isMoving)
        {
            MoveWithMouse();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            levelManager.QuitRequest();
        }
        
    }

    public void EnableControl()
    {
        isMoving = true;
    }

    // TODO: may need to set a playerDistanceToCamera variable
    private void MoveWithMouse()
    {
        Vector3 mousePosInWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosInWorldPoint.x = Mathf.Clamp(mousePosInWorldPoint.x, xMin, xMax);
        mousePosInWorldPoint.y = Mathf.Clamp(mousePosInWorldPoint.y, yMin, yMax);
        transform.position = new Vector3(mousePosInWorldPoint.x, mousePosInWorldPoint.y, 10) + playerMouseOffset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You've collided with an obstacle" + " " + collision.name);

        // TODO: handle lose condition
        if (!IsInvinsible && collision.gameObject.GetComponent<PowerUp>() == null)
        {
            isMoving = false;

            Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            rigidBody.AddForce(new Vector2(0.00f, -20.00f));
            rigidBody.AddTorque(-300.00f);

            GameObject.FindObjectOfType<GameController>().EndGame();
            //levelManager.LoadLevel("Lose");
        }
        
    }
}
