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
        // Check if there's a touch event
        if(Input.touchCount == 0)
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
                        hit.collider.gameObject.GetComponent<Block>().ChangeMaterial();
                    }
                    // If not, we instantiate the prefab and assing it as selectedObject
                    else
                    {
                        Vector3 spawnPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + 0.1f, hits[0].pose.position.z);
                        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
                    }
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Stationary && spawnedObject == null)
            {
                destroyCounter += Time.deltaTime;
                
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
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
                        Vector3 newPosition = new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + 0.5f, hits[0].pose.position.z);
                        spawnedObject.transform.position = newPosition;
                        debugText.text = "Hits pose position y: " + hits[0].pose.position.y + "\n"
                            + "Hit y:" + hit.transform.position.y;
                    }
                    // If not, we move the spawnedObject freely
                    else
                    {
                        spawnedObject.transform.position = hits[0].pose.position;
                    }
                }
            }
            // If the touch ends, we set the spawnedObject to null so we're no longer able to drag the prefab around
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObject.tag = "Spawnable";
                spawnedObject.AddComponent<Rigidbody>();
                spawnedObject.GetComponent<BoxCollider>().isTrigger = false;

                if (heightStalker != null)
                {
                    heightStalker.GetComponent<HeightCheck>().objects.Add(spawnedObject);
                }

                spawnedObject = null;
                destroyCounter = 0;
            }
        }
    }
}
