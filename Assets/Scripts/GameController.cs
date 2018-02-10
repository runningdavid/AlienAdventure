using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {
    
    [Tooltip("Number of sliders being used by the game (must > numMovingSliders)")]
    public int totalSliders = 2;

    [Tooltip("Number of sliders allowed to move at the same time")]
    public int maxMovingSliders = 1;

    [Tooltip("Prefab for sliders that will contain obstalces")]
    public GameObject sliderPrefab;

    [Tooltip("Initial game speed we will be moving at")]
    public float gameSpeed = 3.00f;

    [Tooltip("Max game speed allowed")]
    public float maxGameSpeed = 10.00f;

    private Queue<GameObject> sliderQueue;
    private List<GameObject> movingSlidersList;
    private Dictionary<GameObject, bool> sliderReadyStates;
    private Vector3 screenBottomLeftWorldPos;
    private Vector3 screenTopRightWorldPos;
    private float lastUpdatedTime = 0;

    // Use this for initialization
    private void Start()
    {
        // initialize data structures
        sliderQueue = new Queue<GameObject>();
        movingSlidersList = new List<GameObject>();
        sliderReadyStates = new Dictionary<GameObject, bool>();

        // calculate screen size in world point units
        float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
        screenBottomLeftWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distanceToCamera));
        screenTopRightWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distanceToCamera));

        // initialize the slider holder
        GameObject sliderHolder = GameObject.Find("SliderHolder");
        if (sliderHolder == null)
        {
            sliderHolder = new GameObject("SliderHolder");
        }

        // initialize slider object using its prefab
        // by default a slider comes with a grid to ease position of obstacles
        for (int i = 0; i < totalSliders; i++)
        {
            GameObject sliderObject = Instantiate(sliderPrefab, sliderHolder.transform);
            Slider slider = sliderObject.GetComponent<Slider>();
            PutSliderAtBeginPos(sliderObject);
            slider.Reset();
            sliderQueue.Enqueue(sliderObject);
            sliderReadyStates.Add(sliderObject, false);
        }

    }

    // Update is called once per frame
    private void Update()
    {
        // adjust level based on time in game
        if (Time.realtimeSinceStartup - lastUpdatedTime > 20.00f && gameSpeed < maxGameSpeed)
        {
            lastUpdatedTime = Time.realtimeSinceStartup;
            gameSpeed++;
        }

        // initialize next container
        GameObject firstSliderObject = sliderQueue.Peek();
        if (!sliderReadyStates[firstSliderObject])
        {
            Slider firstSlider = firstSliderObject.GetComponent<Slider>();
            firstSlider.GenerateObstacles();
            sliderReadyStates[firstSliderObject] = true;
        }
        
        // update in progress containers
        if (movingSlidersList.Count < maxMovingSliders)
        {
            if (movingSlidersList.Count == 0 || HasSliderTopEnteredScreen(movingSlidersList.Last()))
            {
                GameObject nextSliderObject = sliderQueue.Dequeue();
                Slider nextSlider = nextSliderObject.GetComponent<Slider>();
                nextSlider.StartMoving(gameSpeed);
                movingSlidersList.Add(nextSliderObject);
            }            
        }

        // recycle used containers
        for (int i = movingSlidersList.Count - 1; i >= 0; i--)
        {
            GameObject sliderObject = movingSlidersList[i];
            Slider slider = sliderObject.GetComponent<Slider>();
            if (HasSliderReachedEnd(sliderObject))
            {
                slider.StopMoving();
                slider.Reset();
                PutSliderAtBeginPos(sliderObject);
                movingSlidersList.RemoveAt(i);
                sliderQueue.Enqueue(sliderObject);
                sliderReadyStates[sliderObject] = false;
            }
        }

    }

    private void PutSliderAtBeginPos(GameObject sliderObject)
    {
        Slider slider = sliderObject.GetComponent<Slider>();
        sliderObject.transform.position = new Vector3(0, screenTopRightWorldPos.y + slider.height / 2, 0);
    }

    private bool HasSliderReachedEnd(GameObject sliderObject)
    {
        Slider slider = sliderObject.GetComponent<Slider>();
        return sliderObject.transform.position.y <= screenBottomLeftWorldPos.y - slider.height / 2;
    }

    private bool HasSliderTopEnteredScreen(GameObject sliderObject)
    {
        Slider slider = sliderObject.GetComponent<Slider>();
        return sliderObject.transform.position.y + slider.height / 2 <= screenTopRightWorldPos.y;
    }


}
