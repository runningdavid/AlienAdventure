using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScroller : MonoBehaviour {

    public GameObject[] collectablePrefabArray;
    public string[] collectableIntroArray;

    [Header("Scroller Config")]
    [SerializeField]
    private float minCharacterScale = 1f;
    [SerializeField]
    private float maxCharacterScale = 2f;
    [SerializeField]
    private float characterSpace = 5f;
    [SerializeField]
    private float moveForwardAmount = 2f;
    [SerializeField]
    private float swipeThresholdX = 5f;
    [SerializeField]
    private float swipeThresholdY = 30f;
    [SerializeField]
    private float rotateSpeed = 30f;
    [SerializeField]
    private float snappingTime = 0.3f;
    [SerializeField]
    private float resetRotateSpeed = 180f;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float scrollSpeedFactor = 0.25f;
    [SerializeField]
    private Color lockedColor = Color.black;

    //[Header("Object References")]
    //[SerializeField]
    //private Text totalCoinsTxt;
    //[SerializeField]
    //private Text characterPriceTxt;
    [SerializeField]
    private Text characterIntroText;
    //[SerializeField]
    //private Image priceImg;
    //[SerializeField]
    //private Button selectButon;
    //[SerializeField]
    //private Button unlockButton;

    private List<GameObject> listCharacter = new List<GameObject>();
    private GameObject currentCharacter;
    private int currentCharacterIndex;
    private GameObject lastCurrentCharacter;
    private IEnumerator rotateCoroutine;
    //private Vector3 centerPoint = Vector3.zero;
    private Vector3 centerPoint = new Vector3(0, 1.50f, 0);
    private Vector3 originalScale = Vector3.one;
    private Vector3 originalRotation = Vector3.zero;
    private Vector3 startPos;
    private Vector3 endPos;
    private float startTime;
    private float endTime;
    private bool isCurrentCharacterRotating = false;
    private bool hasMoved = false;

    // Use this for initialization
    void Start()
    {
        currentCharacterIndex = 0;

        for (int i = 0; i < collectablePrefabArray.Length; i++)
        {
            int deltaIndex = i - currentCharacterIndex;

            GameObject character = Instantiate(collectablePrefabArray[i], centerPoint, Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z));
            //CharacterInfo charData = character.GetComponent<CharacterInfo>();
            //charData.characterSequenceNumber = i;
            listCharacter.Add(character);
            character.transform.localScale = originalScale;
            character.transform.position = centerPoint + new Vector3(deltaIndex * characterSpace, 0, 0);

            //Use this code for 3d character
            //Renderer charRender = character.GetComponent<Renderer>();
            //if (charData.IsUnlocked)
            //    charRender.material.color = Color.white;
            //else
            //    charRender.material.color = lockColor;




            /////////Use this code for 2d character
            SpriteRenderer charRender = character.GetComponentInChildren<SpriteRenderer>();
            // TODO: write code to read/store unlock progress
            if (true)
                charRender.color = Color.white;
            else
                charRender.color = lockedColor;

            // set as child of this object
            character.transform.parent = transform;
        }


        currentCharacter = listCharacter[currentCharacterIndex];
        currentCharacter.transform.localScale = maxCharacterScale * originalScale;
        currentCharacter.transform.position += moveForwardAmount * Vector3.forward;
        lastCurrentCharacter = null;

        characterIntroText.text = collectableIntroArray[currentCharacterIndex];
        //Use this code for 3d character
        //StartRotateCurrentCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        #region Scrolling field

        if (Input.GetMouseButtonDown(0)) //Touch the screen
        {
            startPos = Input.mousePosition;
            startTime = Time.time;
            hasMoved = false;
        }
        else if (Input.GetMouseButton(0)) //Holding and scrolling
        {
            endPos = Input.mousePosition;
            endTime = Time.time;

            float deltaX = Mathf.Abs(startPos.x - endPos.x);
            float deltaY = Mathf.Abs(startPos.y - endPos.y);

            if (deltaX >= swipeThresholdX && deltaY <= swipeThresholdY)
            {
                hasMoved = true;

                if (isCurrentCharacterRotating)
                    StopRotateCurrentCharacter(true);

                float speed = deltaX / (endTime - startTime);
                Vector3 dir = (startPos.x - endPos.x < 0) ? Vector3.right : Vector3.left;
                Vector3 moveVector = dir * (speed / 10) * scrollSpeedFactor * Time.deltaTime;

                // Move and scale the children
                for (int i = 0; i < listCharacter.Count; i++)
                {
                    MoveAndScaleCharacter(listCharacter[i].transform, moveVector);
                }

                // Update for next step
                startPos = endPos;
                startTime = endTime;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (hasMoved)
            {
                // Remember the last currentCharacter
                lastCurrentCharacter = currentCharacter;

                // Update current character to the one nearest to center point
                currentCharacter = GetCharacterNearestToCenter();
                currentCharacterIndex = listCharacter.IndexOf(currentCharacter);

                characterIntroText.text = collectableIntroArray[currentCharacterIndex];

                // Snap
                float snapDistance = centerPoint.x - currentCharacter.transform.position.x;
                StartCoroutine(SnappingAndRotating(snapDistance));
            }
        }

        #endregion

        // Update UI
        //totalCoinsTxt.text = CoinManager.Instance.Coins.ToString();
        //CharacterInfo charData = currentCharacter.GetComponent<CharacterInfo>();

        //if (!charData.isFree)
        //{
        //    characterPriceTxt.gameObject.SetActive(true);
        //    characterPriceTxt.text = charData.characterPrice.ToString();
        //    priceImg.gameObject.SetActive(true);
        //}
        //else
        //{
        //    characterPriceTxt.gameObject.SetActive(false);
        //    priceImg.gameObject.SetActive(false);
        //}

        //if (currentCharacter != lastCurrentCharacter)
        //{
        //    if (charData.IsUnlocked)
        //    {
        //        unlockButton.gameObject.SetActive(false);
        //        selectButon.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        selectButon.gameObject.SetActive(false);
        //        if (CoinManager.Instance.Coins >= charData.characterPrice)
        //        {
        //            unlockButton.gameObject.SetActive(true);
        //            unlockButton.interactable = true;
        //        }
        //        else
        //        {
        //            unlockButton.gameObject.SetActive(false);
        //            unlockButton.interactable = false;
        //        }
        //    }
        //}
    }


    GameObject GetCharacterNearestToCenter()
    {
        float min = -1;
        GameObject nearestObj = null;

        for (int i = 0; i < listCharacter.Count; i++)
        {
            float d = Mathf.Abs(listCharacter[i].transform.position.x - centerPoint.x);
            if (d < min || min < 0)
            {
                min = d;
                nearestObj = listCharacter[i];
            }
        }

        return nearestObj;
    }


    void MoveAndScaleCharacter(Transform co, Vector3 moveVector)
    {
        // Move
        co.position += moveVector;

        // Scale and move forward according to distance from current position to center position
        float distance = Mathf.Abs(co.position.x - centerPoint.x);
        if (distance < (characterSpace / 2))
        {
            float factor = 1 - distance / (characterSpace / 2);
            float scaleFactor = Mathf.Lerp(minCharacterScale, maxCharacterScale, factor);
            co.localScale = scaleFactor * originalScale;

            float fd = Mathf.Lerp(0, moveForwardAmount, factor);
            Vector3 pos = co.position;
            pos.z = centerPoint.z + fd;
            co.position = pos;
        }
        else
        {
            co.localScale = minCharacterScale * originalScale;
            Vector3 pos = co.position;
            pos.z = centerPoint.z;
            co.position = pos;
        }
    }
    IEnumerator SnappingAndRotating(float snapDistance)
    {
        float snapDistanceAbs = Mathf.Abs(snapDistance);
        float snapSpeed = snapDistanceAbs / snappingTime;
        float sign = snapDistance / snapDistanceAbs;
        float movedDistance = 0;

        //SoundManager.Instance.PlaySound(SoundManager.Instance.tick);

        while (Mathf.Abs(movedDistance) < snapDistanceAbs)
        {
            float d = sign * snapSpeed * Time.deltaTime;
            float remainedDistance = Mathf.Abs(snapDistanceAbs - Mathf.Abs(movedDistance));
            d = Mathf.Clamp(d, -remainedDistance, remainedDistance);

            Vector3 moveVector = new Vector3(d, 0, 0);
            for (int i = 0; i < listCharacter.Count; i++)
            {
                MoveAndScaleCharacter(listCharacter[i].transform, moveVector);
            }

            movedDistance += d;
            yield return null;
        }


        //Use this code for 3d character
        //if (currentCharacter != lastCurrentCharacter || !isCurrentCharacterRotating)
        //{
        //    // Stop rotating the last current character
        //    StopRotateCurrentCharacter();

        //    // Now rotate the new current character
        //    StartRotateCurrentCharacter();
        //}
    }

    void StopRotateCurrentCharacter(bool resetRotation = false)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        isCurrentCharacterRotating = false;

        if (resetRotation)
            StartCoroutine(CRResetCharacterRotation(currentCharacter.transform));
    }


    void StartRotateCurrentCharacter()
    {
        StopRotateCurrentCharacter(false);
        rotateCoroutine = CRRotateCharacter(currentCharacter.transform);
        StartCoroutine(rotateCoroutine);
        isCurrentCharacterRotating = true;
    }
    IEnumerator CRResetCharacterRotation(Transform charTf)
    {
        Vector3 startRotation = charTf.rotation.eulerAngles;
        Vector3 endRotation = originalRotation;
        float timePast = 0;
        float rotateAngle = Mathf.Abs(endRotation.y - startRotation.y);
        float rotateTime = rotateAngle / resetRotateSpeed;

        while (timePast < rotateTime)
        {
            timePast += Time.deltaTime;
            Vector3 rotation = Vector3.Lerp(startRotation, endRotation, timePast / rotateTime);
            charTf.rotation = Quaternion.Euler(rotation);
            yield return null;
        }
    }

    IEnumerator CRRotateCharacter(Transform charTf)
    {
        while (true)
        {
            charTf.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
            yield return null;
        }
    }



    ////////////////////////////////////////// Publish functions

    public void UnlockButton()
    {
        //bool unlockSucceeded = currentCharacter.GetComponent<CharacterInfo>().Unlock();
        //if (unlockSucceeded)
        //{

        //    //Use this code for 3d character
        //    //currentCharacter.GetComponent<Renderer>().material.color = Color.white;

        //    //Use this code for 2d character
        //    currentCharacter.GetComponent<SpriteRenderer>().color = Color.white;

        //    unlockButton.gameObject.SetActive(false);
        //    selectButon.gameObject.SetActive(true);

        //    //SoundManager.Instance.PlaySound(SoundManager.Instance.unlock);
        //}
    }

    public void SelectButton()
    {
        //CharacterManager.Instance.SelectedCharacterIndex = currentCharacter.GetComponent<CharacterInfo>().characterSequenceNumber;
        //SoundManager.Instance.PlaySound(SoundManager.Instance.button);
        Back();
    }

    public void Back()
    {
        //SceneManager.LoadScene("Gameplay");
        //SoundManager.Instance.PlaySound(SoundManager.Instance.button);
    }
}
