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

    public float woodCrackTime;
    private float woodCounter;
    private int cubesTouching;
    
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

        iceMeltTime = 5f;
        iceCounter = 0;
        woodCrackTime = 10f;
        woodCounter = 0;
        cubesTouching = 0;

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
            //indexText.text = woodCounter.ToString(); debugging
        }

        // Functionality for the melting of the ice cube
        if(materialIndex == 2)
        {
            iceCounter += Time.deltaTime;
            rend.material.color = Color.Lerp(rend.material.color, Color.black, Time.deltaTime / (iceMeltTime - iceCounter));

            if(iceCounter > iceMeltTime)
            {
                AudioManager.aManager.Play("splash");
                DestroyThisGameobject();
            }
        }

        if (materialIndex != 2)
        {
            iceCounter = 0;
            rend.material.color = Color.white;
        }

        // Functionality for the cracking of the wooden cube
        if (materialIndex == 1 && cubesTouching > 1)
        {
            woodCounter += Time.deltaTime;

            if (woodCounter > woodCrackTime)
            {
                AudioManager.aManager.Play("woodDestroy");
                DestroyThisGameobject();
            }
        }

        if (materialIndex != 1)
        {
            woodCounter = 0;
        }
    }

    public void ChangeMaterial()
    {
        if(!isChangingMaterial)
        {
            isChangingMaterial = true;

            // Checking the next availability index
            int nextIndex = (materialIndex + 1) % spawnableManager.GetComponent<SpawnableManager>().cubeMaterials.Count;

            // Changing the material for the cube
            materialIndex = nextIndex;
            rend.material = spawnableManager.GetComponent<SpawnableManager>().cubeMaterials[materialIndex];

            // Reset the flag after a short delay
            StartCoroutine(ResetFlag());
        }
    }

    public void DestroyThisGameobject()
    {
        if (materialIndex >= 0 && materialIndex < spawnableManager.GetComponent<SpawnableManager>().cubeMaterials.Count)
        {
            // Destroy the gameObject
            Destroy(gameObject);
        }
    }

    private IEnumerator ResetFlag()
    {
        yield return new WaitForSeconds(0.2f);
        isChangingMaterial = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Spawnable")
        {
            cubesTouching++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Spawnable")
        {
            cubesTouching--;
        }
    }
}
