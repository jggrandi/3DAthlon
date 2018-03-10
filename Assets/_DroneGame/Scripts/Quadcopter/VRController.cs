// Controls the quadcopter with VR Controllers

using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Quadcopter))]
public class VRController : MonoBehaviour {
    //------------------------- PUBLIC -------------------------//

    // Target quadcopter this script controls
    public Quadcopter drone;
    public float targetY = 5;
    public float targetPitch = 0;
    public float targetYaw = 0;
    public float targetRoll = 0;
    public float scalingY = 0.001f;
    private float[] PID1 = { 1f, 0f, 1f };
    private float[] PID2 = { 10f, 3f, 1 };
    private float[] res1 = { 0f, 0f }; // integral, lasterror
    private float[] res2 = { 0f, 0f };
    private float[] res3 = { 0f, 0f };
    private float[] res4 = { 0f, 0f };
    private float[] calibration = { 0.0f, 0.0f }; // Inferior heigth medium heigth upper heigth, Angle ofset.
    private int gameOn = 0;
    private bool Start = false;
    float handDistance;
    public Hand hand1, hand2;
    public GameObject player;
    public Camera cam;
    //------------------------- PRIVATE -------------------------//

    //-------------------------------------------------
    // Calculates the PID for the drone stabilization
    //-------------------------------------------------
    private float PIDcontroller(float[] PID, float[] Residuals, float Target, float Measurement) {
        float error = Target - Measurement;
        Residuals[0] = Residuals[0] + error * Time.fixedDeltaTime;
        float derivative = (error - Residuals[1]) / Time.fixedDeltaTime;
        float output = PID[0] * error + PID[1] * Residuals[0] + PID[2] * derivative;
        Residuals[1] = error;
        return output;
    }

    //-------------------------------------------------
    // Map angles from 0..360 to -180..180
    //-------------------------------------------------
    private float ClampAngle(float angle) {
        if (angle > 180) angle -= 360.0f;
        return angle;
    }

    //-------------------------------------------------
    // Map values from a range to another
    //-------------------------------------------------
    private float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    //-------------------------------------------------
    // Verify if the hands are close to each other
    //-------------------------------------------------
    private bool IsHandsClose(Hand h1, Hand h2) {
        var dist = Vector3.Distance(hand1.transform.position, hand2.transform.position);
        if (dist <= 0.15f) return true;
        return false;
    }

    //-------------------------------------------------
    // Return the most distant hand from the body 
    //-------------------------------------------------
    private Hand FartherstHand(Hand h1, Hand h2, GameObject player) {
        var distCamHand1 = Vector3.Distance(hand1.transform.position, player.transform.position);
        var distCamHand2 = Vector3.Distance(hand2.transform.position, player.transform.position);

        if (distCamHand1 > distCamHand2) return h1;
        else return h2;
    }

    //-------------------------------------------------
    // Smooth the hand movements to avoid indesirable drone movements
    //-------------------------------------------------
    private float SmoothMovement(float value, float floatingPointNumber, int intensity) {
        return Mathf.Pow(floatingPointNumber, 2.0f * (1.0f - intensity)) * Mathf.Pow(value, 2.0f * (float)intensity - 1.0f);
    }

    private void FixedUpdate() {
        var handfart = FartherstHand(hand1, hand2, player); // Bug: Needs to be corrected 

        var droneFoward = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        var handFoward = new Vector3(handfart.transform.forward.x, 0.0f, handfart.transform.forward.z);

        if (Start) {

            // Yaw
            var cross = Vector3.Cross(droneFoward, handFoward);
            targetYaw = Vector3.Angle(droneFoward, handFoward);
            if (cross.y > 0.0f) { targetYaw = -targetYaw; }
            targetYaw = SmoothMovement(targetYaw, 80.0f, 2);

            // Height
            var c = Remap(handfart.transform.localPosition.y, calibration[0], calibration[1], -2.0f, 2.0f);
            c = SmoothMovement(c, 2.0f, 2);
            c = (c < 0) ? (c - 9.8f * Mathf.Pow(Time.fixedDeltaTime, 2)) : (c);
            //var c = ClampAngle(handfart.transform.rotation.eulerAngles.x) - calibration; //Correction
            targetY += c * scalingY;
            //Debug.Log(c + " " + c * scalingY);
            targetY = Mathf.Clamp(targetY, -70.0f, 70.0f);

            // Forward Backward
            if (IsHandsClose(hand1, hand2)) { //when the hands are close to each other, backward
                var distCamHand1 = Vector3.Distance(hand1.transform.position, cam.transform.position);
                distCamHand1 = Remap(distCamHand1, 0.2f, 0.6f, -0.4f, 0.0f);
                targetPitch = Mathf.Clamp(distCamHand1, -0.4f, 0.0f);
                //targetPitch = SmoothMovement(targetPitch, -0.4f, 3);
            } else { // forward
                handDistance = Vector3.Distance(hand1.transform.position, hand2.transform.position);
                handDistance = Remap(handDistance, 0.1f, 0.5f, 0.0f, 0.4f);
                targetPitch = Mathf.Clamp(handDistance, 0.0f, 0.4f);
                targetPitch = SmoothMovement(targetPitch, 0.4f, 3);
                var handAngle = -ClampAngle(handfart.transform.rotation.eulerAngles.z);
                handAngle = Remap(handAngle, -80.0f, 80.0f, -0.4f, 0.4f);
                targetRoll = Mathf.Clamp(handAngle, -0.4f, 0.4f);
                targetRoll = SmoothMovement(targetRoll, 0.4f, 3);
            }
        }
        // trigger to calibrate and start the flight
        if (hand1.GetStandardInteractionButtonUp()) {
            if (gameOn < 2) {
                calibration[gameOn] = hand1.transform.localPosition.y;
                gameOn += 1;
            } else {
                Start = true;
            }
        }

        // PIDs
        //targetPitch = 0;
        //targetYaw = 0;
        //targetRoll = 0;

        // Calculates the PIDS based on the hands position and orientation
        float position = PIDcontroller(PID1, res1, targetY, transform.position.y);
        float pitch = PIDcontroller(PID2, res2, targetPitch, ClampAngle(transform.rotation.eulerAngles.x) / 180.0f);
        //  Measurement Always 0 because this rotation is relative.
        float yaw = PIDcontroller(PID2, res3, (targetYaw / 180.0f) * 0.1f, 0.0f);
        // Absolute.
        //float yaw = PIDcontroller(PID2, res3, (targetYaw / 180.0f) * 0.1f, (-(ClampAngle(transform.rotation.eulerAngles.y))/180.0f)));
        float roll = PIDcontroller(PID2, res4, targetRoll, -(ClampAngle(transform.rotation.eulerAngles.z)) / 180.0f);

        // Apply the calculated values to drive the drone
        drone.Drive(position, pitch, yaw, roll);
    }
}

