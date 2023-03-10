using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool selected;
    
    // Start is called before the first frame update
    void Awake()
    {
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(selected)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }
}
