using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public Camera[] CamArr;
    public GameObject Lidar_lr, lidar_koszyk;

    private int Cameraidx = 0;
    private Transform DisplayCamName;
    private string CamName;

    private Vector3 firstpoint;
    private Vector3 secondpoint;
    private float xAngle = 0f; //angle for axes x for rotation
    private float yAngle = 0f;
    private float xAngTemp; //temp variable for angle
    private float yAngTemp;
    private bool fframe = false;

    private void Awake()
    {
        DisplayCamName = gameObject.transform.GetChild(2).GetChild(0);
    }

    void Update()
    {
        CamName = "Name Error";

        if (Input.GetButtonDown("RB_xbox")) CameraSwitchRight();
        if (Input.GetButtonDown("LB_xbox")) CameraSwitchleft();

        if (CamArr[Cameraidx] != null)
        {
            CamName = CamArr[Cameraidx].name;

            if (CamName == "First person camera")
            {
                swipe();

                if (Input.GetAxis("Dpad_xbox_vertical") != 0) yAngle = Input.GetAxis("Dpad_xbox_vertical")/2;
                if (Input.GetAxis("Dpad_xbox_horizontal") != 0) xAngle = Input.GetAxis("Dpad_xbox_horizontal")/2;

                //down
                if (lidar_koszyk.transform.localRotation.z <= 0.35f)
                {
                    if (Input.GetKey(KeyCode.DownArrow)) lidar_koszyk.transform.Rotate(0, 0, 1);
                    if (yAngle > 0) lidar_koszyk.transform.Rotate(0, 0, yAngle);
                }

                //up
                if (lidar_koszyk.transform.localRotation.z >= -0.20f)
                {
                    if (Input.GetKey(KeyCode.UpArrow)) lidar_koszyk.transform.Rotate(0, 0, -1);
                    if (yAngle < 0) lidar_koszyk.transform.Rotate(0, 0, yAngle);
                }

                //right
                if (Lidar_lr.transform.localRotation.y <= 0.40f)
                {
                    if (Input.GetKey(KeyCode.RightArrow)) Lidar_lr.transform.Rotate(0, 1, 0);
                    if (xAngle > 0) Lidar_lr.transform.Rotate(0, xAngle, 0);
                }

                //left
                if (Lidar_lr.transform.localRotation.y >= -0.35f)
                {
                    if (Input.GetKey(KeyCode.LeftArrow)) Lidar_lr.transform.Rotate(0, -1, 0);
                    if (xAngle < 0) Lidar_lr.transform.Rotate(0, xAngle, 0);
                }
            }
        }

        DisplayCamName.GetComponent<TMPro.TextMeshProUGUI>().text = CamName;
    }

    private void OnoffCam()
    {
        if (Cameraidx < 0)Cameraidx = CamArr.Length - 1;
        if (Cameraidx >= CamArr.Length) Cameraidx = 0;

        foreach (var cam in CamArr)
            cam.gameObject.SetActive(false);

        CamArr[Cameraidx].gameObject.SetActive(true);
    }

    public void CameraSwitchRight()
    {
        Cameraidx++;
        OnoffCam();
    }

    public void CameraSwitchleft()
    {
        Cameraidx--;
        OnoffCam();
    }

    private void swipe()
    {
        /*
        if (xAngle > 0.0001f || xAngle < -0.0001f)
        {
            if (xAngle > 0) xAngle -= 0.0001f;
            if (xAngle < 0) xAngle += 0.0001f;
        }
        else
        if (yAngle != 0)
        {
            if (yAngle > 0) yAngle--;
            if (yAngle < 0) yAngle++;
        }

        Debug.Log(yAngle + " " + xAngle);
        */
        xAngle = 0f;
        yAngle = 0f;

        if (Input.GetMouseButton(0))
        {

            if (!fframe)
            {
                firstpoint = Input.mousePosition;
                xAngTemp = xAngle;
                yAngTemp = yAngle;

                fframe = true;
            }

            secondpoint = Input.mousePosition;

            xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 15 / Screen.width;
            yAngle = yAngTemp - (secondpoint.y - firstpoint.y) * 15 / Screen.height;
        }
        else fframe = false;
    }
}