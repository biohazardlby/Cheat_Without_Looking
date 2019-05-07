using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawingboard : MonoBehaviour
{
    public bool launching = false;
    Vector3 translateVec = new Vector3(0, 6f, 1f);
    Vector3 rot = new Vector3(40, 0, 0);
    float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (launching)
        {
            transform.Translate(translateVec * Time.deltaTime);
            transform.Rotate(rot *Time.deltaTime);
            time += Time.deltaTime;
            if (time >= 3)
            {
                launching = false;
            }
        }
    }
}
