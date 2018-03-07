// Example controller for the quadcopter which uses an Xbox 360 controller

using UnityEngine;

[RequireComponent(typeof(Quadcopter))]
public class ControllerX360 : MonoBehaviour
{
	//------------------------- PUBLIC -------------------------//

	// Target quadcopter this script controls
	public Quadcopter drone;


	//------------------------- PRIVATE -------------------------//


	private void Update()
	{
		// Reads axis inputs from the controller
		float power = Input.GetAxisRaw("LeftY");	// Left Stick Y
		float pitch = Input.GetAxisRaw("RightY");   // Right Stick Y
		float yaw = Input.GetAxisRaw("Trigger");    // Analog triggers
		float roll = Input.GetAxisRaw("RightX");    // RIght Stick X

        //Debug.Log(power + " " + pitch + " " + yaw + " " + roll);

		// Flys the quadcopter using the inputs
		drone.Drive(power, pitch, yaw, roll);

		// Perform quick flips using the A,B,X,Y buttons
		if (Input.GetKeyDown(KeyCode.Joystick1Button0)) //A
		{
			drone.FlipPitch(-1);
		}

		if (Input.GetKeyDown(KeyCode.Joystick1Button1)) //B
		{
			drone.FlipRoll(1);
		}

		if (Input.GetKeyDown(KeyCode.Joystick1Button2)) //X
		{
			drone.FlipRoll(-1);
		}

		if (Input.GetKeyDown(KeyCode.Joystick1Button3)) //Y
		{
			drone.FlipPitch(1);
		}

		if (Input.GetKeyDown(KeyCode.JoystickButton6))  //Back
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
