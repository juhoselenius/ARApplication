using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeightCheck : MonoBehaviour
{
    public GameObject[] objects;
    public float structureHeight;
    public int cubesHeight;
    public float high;
    public float low;
    public float counter;
    public TextMeshProUGUI structureHeightText;
    public SpawnableManager spawnableManager;

    // Update is called once per frame
    void Update()
    {
        CheckHeight();
    }

    void CheckHeight()
    {
        objects = GameObject.FindGameObjectsWithTag("Spawnable");
        
        structureHeight = 0;

        high = -100;
        low = 100;

        if (objects.Length == 0)
        {
            structureHeightText.text = (3 + spawnableManager.currentLevel).ToString();
            return;
        }
        
        foreach (GameObject obj in objects)
        {
            if(obj.transform.position.y < low)
            {
                low = obj.transform.position.y;
            }
            
            if (obj.transform.position.y > high)
            {
                high = obj.transform.position.y;
            }
        }

        structureHeight = high - low;
        cubesHeight = Mathf.RoundToInt(structureHeight / 0.1f) + 1;

        if(3 + spawnableManager.currentLevel - cubesHeight > 0)
        {
            structureHeightText.text = ((3 + spawnableManager.currentLevel) - cubesHeight).ToString();
        }
        else
        {
            structureHeightText.text = "0";
        }
    }
}
