﻿using UnityEngine;
using System.Collections;

public class Controller_RayTracer : MonoBehaviour
{
    public Transform controller_trans;
    LineRenderer line_render;

    [Header("Debugging")]
    public bool useFakeController = false;
    public GameObject fake_ctrl;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 8;
        if (line_render != null)
        {
            line_render.SetPosition(0, controller_trans.position);
            line_render.SetPosition(1, controller_trans.position + (controller_trans.forward * 10));
        }

        RaycastHit hit;
        if (useFakeController)
        {
            Debug.DrawLine(fake_ctrl.transform.position, fake_ctrl.transform.position + fake_ctrl.transform.forward * 100);
            if (Physics.Raycast(fake_ctrl.transform.position, fake_ctrl.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {

                GameObject obj = hit.collider.gameObject;
            }
        }
        else
        {
            if (Physics.Raycast(controller_trans.position, controller_trans.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
            }
        }
    }
}
