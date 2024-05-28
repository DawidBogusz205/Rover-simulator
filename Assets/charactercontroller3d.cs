using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class charactercontroller3d : MonoBehaviour
{
    public WheelCollider fl_w_collider, fr_w_collider, bl_w_collider, br_w_collider;
    public Transform fl_steer_model, fr_steer_model, bl_steer_model, br_steer_model, fl_wheel_model, fr_wheel_model, bl_wheel_model, br_wheel_model;
    public float max_steering_angle;
    public float motor_force;
    public float brake_force;

    public GameObject[] rover_parts;
    public GameObject arm_lr_rot, arm_zginacz1, arm_zginacz2, arm_zginacz3, arm_szczypce_obrot, arm_szczypce_lewo, arm_szczypce_prawo;

    private GameObject Rover;

    private float h_input = 0;
    private float v_input = 0;
    private float steering_angle;
    private float rotation_speed;
    private float brake_force_value;
    private bool arm_steer_mode = false;

    struct PositionAndRotation
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    Dictionary<Transform, PositionAndRotation> initialPositions = new();

    private void Start()
    {
        Rover = gameObject;

        GameObject.Find("Torque Mod").transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = motor_force.ToString();
        GameObject.Find("Torque Mod").GetComponent<Slider>().value = motor_force;

        GameObject.Find("Brake Torque Mod").transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = brake_force.ToString();
        GameObject.Find("Brake Torque Mod").GetComponent<Slider>().value = brake_force;

        foreach (GameObject part in rover_parts)
        {
            PositionAndRotation pandr;
            pandr.position = part.transform.position;
            pandr.rotation = part.transform.rotation;
            initialPositions[part.transform] = pandr;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Menu_xbox")) ResetPos();

        if (Input.GetButtonDown("X_xbox")) arm_steer_mode = !arm_steer_mode;

        if (arm_steer_mode) GameObject.Find("Mode").GetComponent<TMPro.TextMeshProUGUI>().text = "Mode: Arm";
        else GameObject.Find("Mode").GetComponent<TMPro.TextMeshProUGUI>().text = "Mode: Wheels";
    }

    private void FixedUpdate()
    {
        if (!arm_steer_mode)
        {
            GetInput();
            Steer();
            Acceleration();
            ModelPose();
        }
        else arm_steer();
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.A)) rotation_speed = -5f;
        else if (Input.GetKey(KeyCode.D)) rotation_speed = 5f;
        else if (Input.GetAxis("Left_Stick_Horizontal") != 0) rotation_speed = Input.GetAxis("Left_Stick_Horizontal") *5;
        else rotation_speed = 0f;

        if (h_input > 1) h_input = 1;
        else if(h_input < -1) h_input = -1;

        if (h_input <=1 && h_input >= -1) h_input += rotation_speed/300;

        v_input = Input.GetAxis("Vertical");
        if (Input.GetAxis("LT_RT_xbox") != 0) v_input = -Input.GetAxis("LT_RT_xbox");

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetAxis("LT_RT_xbox") != 0) brake_force = 0f;
        else brake_force = brake_force_value;
    }

    private void ModelPose()
    {
        fl_steer_model.localRotation = Quaternion.Euler(0, -steering_angle, 0);
        fr_steer_model.localRotation = Quaternion.Euler(0, steering_angle, 0);
        bl_steer_model.localRotation = Quaternion.Euler(0, -steering_angle, 0);
        br_steer_model.localRotation = Quaternion.Euler(0, steering_angle, 0);

        fl_wheel_model.Rotate(0, 0, -(motor_force * v_input));
        fr_wheel_model.Rotate(0, 0, -(motor_force * v_input));
        bl_wheel_model.Rotate(0, 0, motor_force * v_input);
        br_wheel_model.Rotate(0, 0, motor_force * v_input); 
    }

    private void Steer()
    {
        steering_angle = max_steering_angle * h_input;

        fl_w_collider.steerAngle = steering_angle;
        fr_w_collider.steerAngle = steering_angle;
        bl_w_collider.steerAngle = -steering_angle;
        br_w_collider.steerAngle = -steering_angle;
    }

    private void Acceleration()
    {
        bl_w_collider.motorTorque = motor_force * v_input;
        br_w_collider.motorTorque = motor_force * v_input;
        fl_w_collider.motorTorque = motor_force * v_input;
        fr_w_collider.motorTorque = motor_force * v_input;

        bl_w_collider.brakeTorque = brake_force;
        br_w_collider.brakeTorque = brake_force;
        fl_w_collider.brakeTorque = brake_force;
        fr_w_collider.brakeTorque = brake_force;
    }

    private void arm_steer()
    {
        if (Input.GetKey(KeyCode.Q)) arm_lr_rot.transform.Rotate(new Vector3(0, -1, 0));
        else if(Input.GetKey(KeyCode.E)) arm_lr_rot.transform.Rotate(new Vector3(0, 1, 0));
        if (Input.GetAxis("Left_Stick_Horizontal") != 0) arm_lr_rot.transform.Rotate(new Vector3(0, Input.GetAxis("Left_Stick_Horizontal"), 0));

        if (Input.GetKey(KeyCode.R) && arm_zginacz1.transform.localRotation.z < 0.77f ) arm_zginacz1.transform.Rotate(new Vector3(0, 0, 1));
        else if (Input.GetKey(KeyCode.F) && arm_zginacz1.transform.localRotation.z > -0.77f) arm_zginacz1.transform.Rotate(new Vector3(0, 0, -1));
        if (Input.GetAxis("Left_Stick_Vertical") > 0 && arm_zginacz1.transform.localRotation.z < 0.77f) arm_zginacz1.transform.Rotate(new Vector3(0, 0, Input.GetAxis("Left_Stick_Vertical")));
        else if (Input.GetAxis("Left_Stick_Vertical") < 0 && arm_zginacz1.transform.localRotation.z > -0.77f) arm_zginacz1.transform.Rotate(new Vector3(0, 0, Input.GetAxis("Left_Stick_Vertical")));

        if (Input.GetKey(KeyCode.T) && arm_zginacz2.transform.localRotation.z < 1f) arm_zginacz2.transform.Rotate(new Vector3(0, 0, 1));
        else if (Input.GetKey(KeyCode.G) && arm_zginacz2.transform.localRotation.z > -0.42f) arm_zginacz2.transform.Rotate(new Vector3(0, 0, -1));
        if (Input.GetAxis("Right_Stick_Vertical") > 0 && arm_zginacz2.transform.localRotation.z < 1f) arm_zginacz2.transform.Rotate(new Vector3(0, 0, Input.GetAxis("Right_Stick_Vertical")));
        else if (Input.GetAxis("Right_Stick_Vertical") < 0 && arm_zginacz2.transform.localRotation.z > -0.42f) arm_zginacz2.transform.Rotate(new Vector3(0, 0, Input.GetAxis("Right_Stick_Vertical")));

        if ((Input.GetKey(KeyCode.I) || Input.GetButton("Y_xbox")) && arm_zginacz3.transform.localRotation.z < 0.70f) arm_zginacz3.transform.Rotate(new Vector3(0, 0, 1));
        else if ((Input.GetKey(KeyCode.K) || Input.GetButton("A_xbox")) && arm_zginacz3.transform.localRotation.z > -0.70f) arm_zginacz3.transform.Rotate(new Vector3(0, 0, -1));

        if (Input.GetKey(KeyCode.J)) arm_szczypce_obrot.transform.Rotate(new Vector3(1, 0, 0));
        else if (Input.GetKey(KeyCode.L)) arm_szczypce_obrot.transform.Rotate(new Vector3(-1, 0, 0));
        if (Input.GetAxis("Right_Stick_Horizontal") != 0) arm_szczypce_obrot.transform.Rotate(new Vector3(Input.GetAxis("Right_Stick_Horizontal"), 0, 0));

        if ((Input.GetKey(KeyCode.U) || Input.GetAxis("LT_RT_xbox") < 0) && arm_szczypce_lewo.transform.localRotation.y < 0.078f && arm_szczypce_prawo.transform.localRotation.y > -0.078f)
        {
            arm_szczypce_lewo.transform.Rotate(0, 0.4f, 0);
            arm_szczypce_prawo.transform.Rotate(0, -0.4f, 0);
        }
        else if ((Input.GetKey(KeyCode.O) || Input.GetAxis("LT_RT_xbox") > 0) && arm_szczypce_lewo.transform.localRotation.y > -0.34f && arm_szczypce_prawo.transform.localRotation.y < 0.34f)
        {
            arm_szczypce_lewo.transform.Rotate(0, -0.4f, 0);
            arm_szczypce_prawo.transform.Rotate(0, 0.4f, 0);
        }
    }

    public void ResetPos()
    {
        foreach (var part_pandr in initialPositions)
        {
            Transform t = part_pandr.Key;
            t.position = part_pandr.Value.position;
            t.rotation = part_pandr.Value.rotation;

            var rb = t.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        arm_lr_rot.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void MotorTorqueMod(float value)
    {
        motor_force = value;

        GameObject.Find("Torque Mod").transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = value.ToString();
    }

    public void BrakeTorqueMod(float value)
    {
        brake_force_value = value;

        GameObject.Find("Brake Torque Mod").transform.Find("Value").GetComponent<TMPro.TextMeshProUGUI>().text = value.ToString();
    }
}