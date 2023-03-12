using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Unity.VisualScripting;

/*
 * Made with the help of Unity Tutorial: Placing and Manipulating Objects in AR
 * (https://learn.unity.com/tutorial/placing-and-manipulating-objects-in-ar)
 */

public class SpawnableManager : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public GameObject spawnablePrefab;
    public GameObject heightStalker;

    public List<Material> cubeMaterials = new List<Material>();
    public List<int> materialAmount = new List<int>();

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Camera arCam;
    private GameObject spawnedObject;
    private float destroyCounter;

    public Text debugText;

    // Start is called before the first frame update
    void Awake()
    {
        spawnedObject = null;
        destroyCounter = 0;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        
        debugText = FindObjectOfType<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        debugText.text = "MaterialsAmount 0: " + materialAmount[0].ToString() + "\n"
                            + "MaterialsAmount 1: " + materialAmount[1].ToString() + "\n"
                            + "MaterialsAmount 2: " + materialAmount[2].ToString() + "\n";

        // Check if there's a touch event
        if (Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        //debugText.text = "Hit pose position y: " + hits[0].pose.position.y;

        // Check if the ray hits a plane
        if (arRaycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            // If touching the screen and no object is selected
            if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if(hit.collider.gameObject.tag == "Spawnable")
                    {
                        // Check if have available materials left
                        if (materialAmount[0] > 0 || materialAmount[1] > 0 || materialAmount[2] > 0)
                        {
                            hit.collider.gameObject.GetComponent<Block>().ChangeMaterial();
                        }
                    }
                    // If not, we instantiate the prefab and assing it as selectedObject
                    else
                    {
                        if (materialAmount[0] > 0 || materialAmount[1] > 0 || materialAmount[2] > 0)
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
                        }
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
                        if(destroyCounter > 0.8f)
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
                        Vector3 newPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + heightStalker.GetComponent<HeightCheck>().structureHeight + 0.15f, hits[0].pose.position.z);
                        spawnedObject.transform.position = newPosition;
                        //debugText.text = "Hits pose position y: " + hits[0].pose.position.y + "\n"
                           // + "Hit y:" + hit.transform.position.y;
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
}
