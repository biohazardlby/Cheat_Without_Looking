using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answerboard : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isMoving = false;
    Vector3 target_loc = new Vector3(1.372f, 0, -1.073f);
    public Transform parent;
    float transitionSpeed = 2f;
    void Start()
    {
        target_loc = parent.TransformPoint(target_loc);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, target_loc, Time.deltaTime * transitionSpeed);
        }
    }
}
