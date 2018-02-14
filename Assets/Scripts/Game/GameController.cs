using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    
    [Tooltip("Number of sliders being used by the game (must > numMovingSliders)")]
    public int totalSliders = 2;

    [Tooltip("Number of sliders allowed to move at the same time")]
    public int maxMovingSliders = 1;

    [Tooltip("Prefab for sliders that will contain obstalces")]
    public GameObject sliderPrefab;

    [Tooltip("Current gamespeed we are moving at")]
    public float gameSpeed = 3.00f;

    [Tooltip("Max game speed allowed")]
    public float maxGameSpeed = 10.00f;

    [Tooltip("Current obstacle spawn probability")]
    public float obstacleSpawnProbability = 0.03f;

    [Tooltip("Max obstacle spawn probability")]
    public float maxObstacleSpawnProbability = 0.06f;

    [Tooltip("Current obstacle rotation probability")]
    public float obstacleRotationProbability = 0.30f;

    [Tooltip("Max obstacle rotation probability")]
    public float maxObstacleRotationProbability = 0.70f;

    [Tooltip("Time we will wait before increase game speed to next level")]
    public float timeIntervalBetweenLevel = 20.00f;

    private bool gameRunning = false;
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
        if (!gameRunning)
        {
            return;
        }

        // adjust level based on time in game
        if (Time.realtimeSinceStartup - lastUpdatedTime > timeIntervalBetweenLevel && gameSpeed < maxGameSpeed)
        {
            lastUpdatedTime = Time.realtimeSinceStartup;
            gameSpeed++;
            obstacleSpawnProbability += 0.02f;
        }

        // initialize next container
        GameObject firstSliderObject = sliderQueue.Peek();
        if (!sliderReadyStates[firstSliderObject])
        {
            Slider firstSlider = firstSliderObject.GetComponent<Slider>();
            firstSlider.obstacleSpawnProbability = obstacleSpawnProbability;
            firstSlider.GenerateObstacles();
            sliderReadyStates[firstSliderObject] = true;

            if (gameSpeed == maxGameSpeed)
            {
                GameObject.Find("MeteorSpawner").GetComponent<MeteorSpawner>().SpawnRandom();
            }

        }
        
        // update in progress containers
        if (movingSlidersList.Count < maxMovingSliders)
        {
            if (movingSlidersList.Count == 0 || HasSliderTopEnteredScreen(movingSlidersList.Last()))
            {
                GameObject nextSliderObject = sliderQueue.Dequeue();
                Slider nextSlider = nextSliderObject.GetComponent<Slider>();
                if (nextSlider.HasCollectable)
                {
                    nextSlider.StartMoving(3.00f);
                    movingSlidersList.Add(nextSliderObject);
                    StartCoroutine(Wait(3.00f));
                }
                else
                {
                    nextSlider.StartMoving(gameSpeed);
                    movingSlidersList.Add(nextSliderObject);
                }
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

    public void StartGame()
    {
        GameObject.Find("Menu").GetComponent<Animator>().SetTrigger("startMoving");
        GameObject.Find("Player").GetComponent<Animator>().SetTrigger("gameStarted");
        GameObject.FindObjectOfType<Player>().EnableControl();
        GameObject.Find("Player").GetComponentInChildren<ParticleSystem>().Play();
        GameObject.FindObjectOfType<ScoreManager>().StartCounting();
        GameObject.FindObjectOfType<CameraController>().StartColorTransition();
        gameRunning = true;
    }

    public void EndGame()
    {
        GameObject.FindObjectOfType<ScoreManager>().StopCounting();
        GameObject.Find("Player").GetComponentInChildren<ParticleSystem>().Stop();
        GameObject.Find("LoseScore").GetComponent<Text>().text = string.Format("You reached\n{0} mi", ScoreManager.score.ToString());
        GameObject.Find("LoseMenu").GetComponent<Animator>().SetTrigger("gameLost");
        GameObject.FindObjectOfType<BestScore>().LogBestScore();
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

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
