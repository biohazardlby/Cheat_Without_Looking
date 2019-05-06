
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum mode
{
    draw, ui
}

public class Drawer : MonoBehaviour
{
    public Camera cam;
    public GameObject fakeDraw;
    public LayerMask layermask;
    float trace_distance = 100;
    public bool testing;
    mode current_mode;

    public PupilLabs.CalibrationController calibration_ctrl;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //change for controller
        Ray penRay = cam.ScreenPointToRay(Input.mousePosition);
        switch (current_mode)
        {
            case mode.draw:
                if (Input.GetButton("Fire1"))
                {
                    draw(penRay);
                }
                break;
        }

        RaycastHit hit;
        if (Physics.Raycast(penRay, out hit, trace_distance, LayerMask.GetMask("UI")))
        {
            Debug.Log(hit.collider.gameObject);

            Click_UI ui = hit.collider.gameObject.GetComponent<Click_UI>();
            if (ui != null)
            {
                Debug.Log(ui.get_message());
            }
        }


        if (testing)
        {
            Ray camFrontRay = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(camFrontRay, out hit, trace_distance, layermask))
            {
                Debug.Log("valid");
            }
            else
            {
                cheat_event();
            }
        }
    }
    void cheat_event()
    {
        Debug.Log("Cheating");
    }

    //cast ray and create draw sprite
    void draw(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, trace_distance, layermask))
        {
            Transform objectHit = hit.transform;
            GameObject.Instantiate(fakeDraw, hit.point, Quaternion.identity);
        }
    }
}
