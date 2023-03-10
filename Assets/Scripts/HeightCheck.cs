using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeightCheck : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    public float structureHeight;
    public float counter;
    public TextMeshProUGUI structureHeightText;
    
    void Awake()
    {
        structureHeight = 0;
        counter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;
        
        if(counter > 2)
        {
            CheckHeight();
            counter = 0;
        }
    }

    void CheckHeight()
    {
        structureHeight = 0;

        foreach (GameObject gameObject in objects)
        {
            if (gameObject.transform.position.y > structureHeight)
            {
                structureHeight = gameObject.transform.position.y;
            }
        }

        structureHeightText.text = structureHeight.ToString();
    }
}
