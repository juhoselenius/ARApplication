using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public List<Material> cubeMaterials = new List<Material>();

    private Renderer rend;
    private int materialIndex;
    
    // Start is called before the first frame update
    void Awake()
    {
        rend = gameObject.GetComponent<Renderer>();
        materialIndex = 0;
        rend.material = cubeMaterials[materialIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y < -10f)
        {
            DestroyThisGameobject();
        }
    }

    public void ChangeMaterial()
    {
        if(materialIndex + 1 < cubeMaterials.Count)
        {
            materialIndex++;
        }
        else
        {
            materialIndex = 0;
        }
        rend.material = cubeMaterials[materialIndex];
    }

    public void DestroyThisGameobject()
    {
        Destroy(gameObject);
    }
}
