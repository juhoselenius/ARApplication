using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Timeline;
using TMPro;

/*
 * Made with the help of Unity Tutorial: Placing and Manipulating Objects in AR
 * (https://learn.unity.com/tutorial/placing-and-manipulating-objects-in-ar)
 */

public class SpawnableManager : MonoBehaviour
{
    public int currentLevel;
    
    public ARRaycastManager arRaycastManager;
    public GameObject spawnablePrefab;
    public GameObject heightStalker;

    public GameObject gameUI;
    public GameObject readyWindow;
    public GameObject levelClearedWindow;

    public List<Material> cubeMaterials = new List<Material>();
    //public List<int> materialAmount = new List<int>();

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Camera arCam;
    private GameObject spawnedObject;
    private float destroyCounter;
    private float completedCounter;
    private float timeCounter;

    private int time;
    private int totalScore;
    private int levelScore;
    private int levelTimeScore;
    private int cubePenalty;
    private bool scoreCounted;
    public TextMeshProUGUI timeValueText;
    public TextMeshProUGUI scoreValueText;
    public TextMeshProUGUI levelValueText;

    public TextMeshProUGUI timeScoreValueText;
    public TextMeshProUGUI cubePenaltyValueText;
    public TextMeshProUGUI levelScoreValueText;

    private bool disabledControls;

    // Start is called before the first frame update
    void Awake()
    {
        disabledControls = true;
        spawnedObject = null;
        destroyCounter = 0;
        completedCounter = 0;
        timeCounter = 0;
        currentLevel = 0;
        cubePenalty = 0;
        time = 0;
        totalScore = 0;
        levelScore = 0;
        levelTimeScore = 0;
        scoreCounted = false;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(completedCounter < 3 && !disabledControls)
        {
            timeCounter += Time.deltaTime;
            time = Mathf.RoundToInt(timeCounter);
            timeValueText.text = time.ToString();
        }
        
        if(heightStalker.GetComponent<HeightCheck>().cubesHeight == 3 + currentLevel)
         {
            completedCounter += Time.deltaTime;

            if (!AudioManager.aManager.IsClipPlaying("tick-tock") && !disabledControls)
            {
                AudioManager.aManager.Play("tick-tock");
            }

            if(completedCounter > 3)
            {
                disabledControls = true;
                
                if(!scoreCounted)
                {
                    CountLevelScore();
                    scoreCounted = true;
                }

                levelClearedWindow.SetActive(true);
                gameUI.SetActive(false);
            }
         }
         else
        {
            if (AudioManager.aManager.IsClipPlaying("tick-tock"))
            {
                AudioManager.aManager.Stop("tick-tock");
            }
            completedCounter = 0;
        }
    }
    void FixedUpdate()
    {
        // Check if there's a touch event
        if (Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        // Check if the ray hits a plane
        if (arRaycastManager.Raycast(Input.GetTouch(0).position, hits) && !disabledControls)
        {
            // If touching the screen and no object is selected
            if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if(hit.collider.gameObject.tag == "Spawnable")
                    {
                        hit.collider.gameObject.GetComponent<Block>().ChangeMaterial();

                        /*
                        // Check if have available materials left
                        if (materialAmount[0] > 0 || materialAmount[1] > 0 || materialAmount[2] > 0)
                        {
                            hit.collider.gameObject.GetComponent<Block>().ChangeMaterial();
                        }
                        */
                    }
                    // If not, we instantiate the prefab and assing it as selectedObject
                    else
                    {
                        Vector3 spawnPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + 0.1f, hits[0].pose.position.z);
                        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
                        spawnedObject.GetComponent<Block>().materialIndex = Random.Range(0, cubeMaterials.Count);
                        spawnedObject.GetComponent<Renderer>().material = cubeMaterials[spawnedObject.GetComponent<Block>().materialIndex];

                        /*if (materialAmount[0] > 0 || materialAmount[1] > 0 || materialAmount[2] > 0)
                        {
                            Vector3 spawnPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + 0.1f, hits[0].pose.position.z);
                            spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
                            if(materialAmount[0] > 0)
                            {
                                spawnedObject.GetComponent<Renderer>().material = cubeMaterials[0];
                                spawnedObject.GetComponent<Block>().materialIndex = 0;
                                materialAmount[0]--;
                            }
                            else if (materialAmount[1] > 0)
                            {
                                spawnedObject.GetComponent<Renderer>().material = cubeMaterials[1];
                                spawnedObject.GetComponent<Block>().materialIndex = 1;
                                materialAmount[1]--;
                            }
                            else if (materialAmount[2] > 0)
                            {
                                spawnedObject.GetComponent<Renderer>().material = cubeMaterials[2];
                                spawnedObject.GetComponent<Block>().materialIndex = 2;
                                materialAmount[2]--;
                            }
                        }*/
                    }
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Stationary && spawnedObject == null)
            {
                // If we are touching the screen, but not moving the finger, destroyCounter advances
                destroyCounter += Time.deltaTime;
                
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        // Destroying the object if finger stays on it over 0.5 seconds
                        if(destroyCounter > 0.5f)
                        {
                            hit.collider.gameObject.GetComponent<Block>().DestroyThisGameobject();
                            destroyCounter = 0;
                        }
                    }
                }
            }
            // If change the touch location as we are touching, we move also the position of the spawnedObject
            else if(Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        Vector3 newPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + heightStalker.GetComponent<HeightCheck>().structureHeight + 0.2f, hits[0].pose.position.z);
                        spawnedObject.transform.position = newPosition;
                    }
                    // If not, we move the spawnedObject freely
                    else
                    {
                        spawnedObject.transform.position = hits[0].pose.position;
                    }
                }
            }
            // If the touch ends, we set the spawnedObject to null so we're no longer able to drag the prefab around
            if(Input.GetTouch(0).phase == TouchPhase.Ended && spawnedObject != null)
            {
                spawnedObject.tag = "Spawnable";
                spawnedObject.AddComponent<Rigidbody>();
                spawnedObject.GetComponent<BoxCollider>().isTrigger = false;

                spawnedObject = null;
                destroyCounter = 0;
            }
        }
    }

    public void CountLevelScore()
    {
        AudioManager.aManager.Stop("tick-tock");
        AudioManager.aManager.Play("ding");

        GameObject[] destroyObjects = GameObject.FindGameObjectsWithTag("Spawnable");

        if (destroyObjects.Length > 0)
        {
            foreach (GameObject obj in destroyObjects)
            {
                switch (obj.GetComponent<Block>().materialIndex)
                {
                    case 0:
                        cubePenalty += 10;
                        break;
                    case 1:
                        cubePenalty += 5;
                        break;
                    case 2:
                        break;
                    default:
                        break;
                }
            }
        }

        levelTimeScore += Mathf.RoundToInt(time);
        levelScore += cubePenalty;
        levelScore += levelTimeScore;
        totalScore += levelScore;

        timeScoreValueText.text = levelTimeScore.ToString();
        cubePenaltyValueText.text = cubePenalty.ToString();
        levelScoreValueText.text = levelScore.ToString();
        scoreValueText.text = totalScore.ToString();
    }

    public void LevelFinished()
    {
        if(readyWindow.activeInHierarchy)
        {
            readyWindow.SetActive(false);
        }
        if (levelClearedWindow.activeInHierarchy)
        {
            levelClearedWindow.SetActive(false);
        }

        GameObject[] destroyObjects = GameObject.FindGameObjectsWithTag("Spawnable");

        if(destroyObjects.Length > 0)
        {
            foreach(GameObject obj in destroyObjects)
            {
                obj.GetComponent<Block>().DestroyThisGameobject();
            }
        }

        spawnedObject = null;
        destroyCounter = 0;
        completedCounter = 0;
        timeCounter = 0;
        cubePenalty = 0;
        levelScore = 0;
        levelTimeScore = 0;

        currentLevel++;
        levelValueText.text = currentLevel.ToString();

        disabledControls = false;
        scoreCounted = false;

        gameUI.SetActive(true);

        if (AudioManager.aManager.IsClipPlaying("ding"))
        {
            AudioManager.aManager.Stop("ding");
        }
    }
}
