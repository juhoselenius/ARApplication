using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/*
 * Made with the help of Unity Tutorial: Placing and Manipulating Objects in AR
 * (https://learn.unity.com/tutorial/placing-and-manipulating-objects-in-ar)
 */

public class SpawnableManager : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public GameObject spawnablePrefab;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Camera arCam;
    private GameObject spawnedObject;

    
    // Start is called before the first frame update
    void Awake()
    {
        spawnedObject = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there's a touch event
        if(Input.touchCount == 0)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        // Check if the ray hits a plane
        if(arRaycastManager.Raycast(Input.GetTouch(0).position, hits))
        {
            // Check to see which phase our touch event is at
            if(Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    // Check if we are touching a previously spawned object
                    if(hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                        if(!spawnedObject.GetComponent<Block>().selected)
                        {
                            spawnedObject.GetComponent<Block>().selected = true;
                        }
                        else
                        {
                            spawnedObject.GetComponent<Block>().selected = false;
                        }
                    }
                    // If not, we instantiate the prefab and assing it as spawnedObject
                    else
                    {
                        spawnedObject = Instantiate(spawnablePrefab, hits[0].pose.position, Quaternion.identity);
                        spawnedObject.GetComponent<Block>().selected = true;
                    }
                }
            }
            // If change the touch location as we are touching, we move also the position of the spawnedObject
            else if(Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = hits[0].pose.position;
            }
            // If the touch ends, we set the spawnedObject to null so we're no longer able to drag the prefab around
            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObject = null;
            }
        }
    }
}
