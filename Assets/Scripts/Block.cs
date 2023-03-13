using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    public GameObject spawnableManager;
    public int materialIndex;

    public float iceMeltTime;
    private float iceCounter;
    
    private Renderer rend;

    public TextMeshPro indexText;

    // TouchPhase.Began keeps sometimes firing twice per touch so this flag prevents double triggering
    private bool isChangingMaterial;


    // Start is called before the first frame update
    void Awake()
    {
        rend = gameObject.GetComponent<Renderer>();
        materialIndex = 0;
        isChangingMaterial = false;

        iceMeltTime = 10f;

        spawnableManager = GameObject.FindGameObjectWithTag("SpawnableManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.localScale.x <= 0)
        {
            DestroyThisGameobject();
        }
        
        if(gameObject.transform.position.y < -10f)
        {
            DestroyThisGameobject();
        }

        if(indexText != null)
        {
            indexText.text = materialIndex.ToString();
        }

        // Functionality for the melting of the ice cube
        if(materialIndex == 2)
        {
            iceCounter += Time.deltaTime;
            rend.material.color = Color.Lerp(rend.material.color, Color.black, Time.deltaTime / (iceMeltTime - iceCounter));

            if(iceCounter > iceMeltTime)
            {
                DestroyThisGameobject();
            }
        }

        if (materialIndex != 2)
        {
            iceCounter = 0;
            rend.material.color = Color.white;
        }
    }

    public void ChangeMaterial()
    {
        if(!isChangingMaterial)
        {
            isChangingMaterial = true;

            // Checking the next availability index
            int nextIndex = (materialIndex + 1) % spawnableManager.GetComponent<SpawnableManager>().cubeMaterials.Count;
            
            /*while (spawnableManager.GetComponent<SpawnableManager>().materialAmount[nextIndex] == 0)
            {
                nextIndex = (nextIndex + 1) % spawnableManager.GetComponent<SpawnableManager>().cubeMaterials.Count;
                if (nextIndex == materialIndex) // All materials are used up
                {
                    return; // Exit the function
                }
            }

            // Increasing the amount of available objects for previous material
            spawnableManager.GetComponent<SpawnableManager>().materialAmount[materialIndex]++;*/

            // Changing the material for the cube
            materialIndex = nextIndex;
            rend.material = spawnableManager.GetComponent<SpawnableManager>().cubeMaterials[materialIndex];

            // Decreasing the amount of available objects for new material
            ////spawnableManager.GetComponent<SpawnableManager>().materialAmount[materialIndex]--;

            // Reset the flag after a short delay
            StartCoroutine(ResetFlag());
        }
    }

    public void DestroyThisGameobject()
    {
        if (materialIndex >= 0 && materialIndex < spawnableManager.GetComponent<SpawnableManager>().cubeMaterials.Count)
        {
            // Increase the amount of available objects for destroyed material
            ////spawnableManager.GetComponent<SpawnableManager>().materialAmount[materialIndex]++;

            // Destroy the gameObject
            Destroy(gameObject);
        }
    }

    private IEnumerator ResetFlag()
    {
        yield return new WaitForSeconds(0.2f);
        isChangingMaterial = false;
    }
}
