// Example controller for the quadcopter which uses an Xbox 360 controller

using UnityEngine;

[RequireComponent(typeof(Quadcopter))]
public class KeyboardController : MonoBehaviour
{
    //------------------------- PUBLIC -------------------------//

    // Target quadcopter this script controls
    public Quadcopter drone;


    //------------------------- PRIVATE -------------------------//


    private void Update()
    {
        // Reads axis inputs from the keyboard
        float power = Input.GetAxisRaw("Power");  // Spacebar
        float pitch = Input.GetAxisRaw("Pitch");   // W and S
        float yaw = Input.GetAxisRaw("Yawn");    // Left and Right Arrow
        float roll = Input.GetAxisRaw("Roll");    // A and D

        Debug.Log(power + " " + pitch + " " + yaw + " " + roll);

        // Flys the quadcopter using the inputs
        drone.Drive(power, pitch, yaw, roll);


        // Perform quick flips
        if (Input.GetKeyDown(KeyCode.DownArrow)) //Down Arrow
        {
            drone.FlipPitch(-1);
        }

        if (Input.GetKeyDown(KeyCode.E)) //E
        {
            drone.FlipRoll(1);
        }

        if (Input.GetKeyDown(KeyCode.Q)) //Q
        {
            drone.FlipRoll(-1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) //Up Arrow
        {
            drone.FlipPitch(1);
        }

        if (Input.GetKeyDown(KeyCode.T))  //T
        {
            drone.gyroStabilization = !drone.gyroStabilization;
        }
    }

#if UNITY_EDITOR

    // Editor QoL
    private void Reset()
    {
        // Auto detect quadcopter
        if (drone == null)
        {
            drone = GetComponent<Quadcopter>();
        }
    }

#endif
}



// Written by Garrett Eddy: 10/9/2017
