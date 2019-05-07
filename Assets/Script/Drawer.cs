
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;
using static PupilLabs.GazeData;

enum player_mode
{
    draw, ui
}

public class Drawer : MonoBehaviour
{
    public bool testing;

    public Camera cam;
    [Header("PupilLabs")]
    public PupilLabs.SubscriptionsController PupilConnection;
    public PupilLabs.CalibrationController calibration_ctrl;


    [Header("Prefabs")]
    public GameObject draw_sprite;

    [Header("Necessary Reference")]
    public GameObject Menu;
    public Transform controller_trans;
    public List<GameObject> answer_paper;

    [Header("Settings")]
    public LayerMask drawingboard_layermask;
    public float trace_distance = 100;
    public float draw_max_gap = 10;

    GameObject drawn_sprites_parent;
    List<Texture> image_list = new List<Texture>();
    player_mode current_mode = player_mode.ui;

    //Initialize some containers
    PupilLabs.GazeListener gazeListener = null;

    Vector3 plGiwVector_xyz;
    Vector3 plEIH0_xyz;
    Vector3 plEIH1_xyz;

    Vector3 plEyeCenter0_xyz;
    Vector3 plEyeCenter1_xyz;

    Vector3 last_drawPoint;

    float plConfidence;
    float plTimeStamp;

    string mode;
    bool isFirstDraw = true;
    // Start is called before the first frame update
    void Start()
    {
        gazeListener = new PupilLabs.GazeListener(PupilConnection);
        gazeListener.OnReceive3dGaze += ReceiveGaze;
        //initialize parent for sprites
        drawn_sprites_parent = new GameObject("SpriteParent");
        var dicInfo = new DirectoryInfo(Application.dataPath + "\\Image\\random_image");
        var fileInfos = dicInfo.GetFiles("*.jpg");
        foreach(FileInfo f in fileInfos)
        {
            Debug.Log(f.ToString());
            byte[] fileData;
            if (File.Exists(f.ToString()))
            {
                fileData = File.ReadAllBytes(f.ToString());
                Texture2D tex = new Texture2D(2, 2);
        
                tex.LoadImage(fileData);
                image_list.Add(tex);
            }
        }
        foreach (Texture2D t in image_list)
        {
            Debug.Log(t.name);
        }
    }

    void ReceiveGaze(PupilLabs.GazeData gazeData)
    {

        plConfidence = float.NaN;
        plTimeStamp = float.NaN;
        plGiwVector_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

        plEIH0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
        plEIH1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

        plEyeCenter0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
        plEyeCenter1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

        mode = "none";

        switch (gazeData.Mode)
        {

            case PupilLabs.GazeData.GazeDataMode.Binocular:

                plConfidence = gazeData.Confidence;
                plTimeStamp = gazeData.Timestamp;
                plGiwVector_xyz = gazeData.GazePoint3d;

                plEIH0_xyz = gazeData.GazeNormal0;
                plEIH1_xyz = gazeData.GazeNormal1;

                plEyeCenter0_xyz = gazeData.EyeCenter0;
                plEyeCenter1_xyz = gazeData.EyeCenter1;

                mode = "binocular";
                break;
            case PupilLabs.GazeData.GazeDataMode.Monocular_0:

                plConfidence = gazeData.Confidence;
                plTimeStamp = gazeData.Timestamp;
                plGiwVector_xyz = gazeData.GazePoint3d;

                plEIH0_xyz = gazeData.GazeNormal0;
                plEyeCenter0_xyz = gazeData.EyeCenter0;

                plEIH1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                plEyeCenter1_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                mode = "monocular_0";
                break;
            case PupilLabs.GazeData.GazeDataMode.Monocular_1:

                plConfidence = gazeData.Confidence;
                plTimeStamp = gazeData.Timestamp;
                plGiwVector_xyz = gazeData.GazePoint3d;

                plEIH0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);
                plEyeCenter0_xyz = new Vector3(float.NaN, float.NaN, float.NaN);

                plEIH1_xyz = gazeData.GazeNormal0;
                plEyeCenter1_xyz = gazeData.EyeCenter0;

                mode = "monocular_1";
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        Ray penRay;
        Ray eyeRay;
        if (testing)
        {
            penRay = cam.ScreenPointToRay(Input.mousePosition);
            eyeRay = new Ray(cam.transform.position, cam.transform.forward);
        }
        else
        {
            penRay = new Ray(controller_trans.position, controller_trans.TransformDirection(Vector3.forward));
            eyeRay = new Ray(cam.transform.position, Vector3.Normalize(cam.transform.rotation * plGiwVector_xyz));
            Debug.Log(eyeRay.direction);
            Debug.DrawLine(cam.transform.position, cam.transform.position + Vector3.Normalize(cam.transform.rotation * plGiwVector_xyz) * 10);
        }
        switch (current_mode)
        {
            case player_mode.draw:
                //draw if press fire
                if (isFiringDown())
                {
                    draw(penRay);
                }
                else
                {
                    isFirstDraw = true;
                }
                eye_check(eyeRay);
                break;
            case player_mode.ui:
                if (isFiring())
                {
                    raycastUI(penRay);
                }
                break;
        }
    }
    bool isFiringDown()
    {
        return Input.GetButton("Fire1") || SteamVR_Actions._default.GrabPinch.GetState(SteamVR_Input_Sources.Any);
    }
    bool isFiring()
    {
        return Input.GetButtonDown("Fire1") || SteamVR_Actions._default.GrabPinch.GetLastStateDown(SteamVR_Input_Sources.Any);
    }

    void eye_check(Ray eyeRay)
    {
        RaycastHit hit;
        if (Physics.Raycast(eyeRay, out hit, trace_distance, drawingboard_layermask))
        {
            Debug.Log("valid");
        }
        else
        {
            cheat_event();
        }
    }

    //event happen when cheating
    void cheat_event()
    {
        Debug.Log("Cheating");
    }

    //raycast to ui to select in menu
    void raycastUI(Ray ray)
    {
        RaycastHit hit; 
        if (Physics.Raycast(ray, out hit, trace_distance, LayerMask.GetMask("UI")))
        {
            Click_UI ui = hit.collider.gameObject.GetComponent<Click_UI>();
            if (ui != null)
            {
                handleUI(ui.get_message());
            }
        }
    }

    void handleUI(string input)
    {
        switch (input)
        {
            case "begin":
                Menu.SetActive(false);
                StartCoroutine(wait_n_start(2));
                break;
            case "calibrate":
                calibration_ctrl.ToggleCalibration();
                break;
        }
    }

    IEnumerator wait_n_start(float t)
    {
        yield return new WaitForSeconds(t);
        current_mode = player_mode.draw;
        set_rand_image();
    }

    void set_rand_image()
    {
        int rand_index = Random.Range(0, image_list.Count);
        foreach (GameObject gO in answer_paper)
        {
            gO.GetComponent<MeshRenderer>().material.mainTexture = image_list[rand_index];
        }
    }
    //cast ray and create draw sprite
    void draw(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, trace_distance, drawingboard_layermask))
        {
            if (isFirstDraw)
            {
                last_drawPoint = hit.transform.position;
                GameObject.Instantiate(draw_sprite, hit.point, Quaternion.identity, drawn_sprites_parent.transform);
                isFirstDraw = false;
            }
            else {
                Vector3 startPoint = last_drawPoint;
                Vector3 endPoint = hit.point;
                int steps = (int) ((endPoint - startPoint).magnitude/draw_max_gap);
                Vector3 step_vec = (endPoint - startPoint) / steps;
                Vector3 currentLoc = startPoint;
                for (int i = 0; i<steps; i++)
                {
                    GameObject.Instantiate(draw_sprite, startPoint + step_vec*i, Quaternion.identity, drawn_sprites_parent.transform);
                }
                GameObject.Instantiate(draw_sprite, endPoint, Quaternion.identity, drawn_sprites_parent.transform);
                last_drawPoint = endPoint;
                
            }
        }
    }
}
