// Monobehaviour to be attatched to Quadcopter as child gameobjects.
//
// Performs a basic simulation of a single quadcopter motor.
//
// Quadcopter.cs modifies the motor power to control thrust and torque.
//
// Recommended values: maxThrust = 250, maxTorque = 300

using UnityEngine;

public class Motor : MonoBehaviour
{
	// Enum for the direction motors spin in
	public enum MotorDirection
	{
		CLOCKWISE,
		COUNTERCLOCKWISE
	}


	//------------------------- PUBLIC -------------------------//

	// Quadcopter rigidbody component
	public Rigidbody rBody;

	// Direction the motor spins in
	public MotorDirection direction;

	// Maxaimum amount of thrust the motor produces at full power
	public float maxThrust;

	// Maximum amount of torque the motor produces at full power
	public float maxTorque;

	// The current power setting for the motor, 0 = off, 1 = full power
	public float Power { get; private set; }

	// Sets the power setting for the motor, used by Quadcopter.cs
	public void SetPower(float value)
	{
		Power = Mathf.Max(0, value);
	}


    //------------------------- PRIVATE -------------------------//


    private void FixedUpdate()
    {
        // Adds thrust force
        rBody.AddForceAtPosition(transform.up * maxThrust * Power, transform.position);

        // Calculates the torque the motor performs on the chassis
        Vector3 torque = ((direction == MotorDirection.CLOCKWISE) ? transform.up : -transform.up) * Power * maxTorque;

        // Adds torque to the chassis
        rBody.AddTorque(torque);
    }


#if UNITY_EDITOR

    // Editor QoL
    private void Reset()
	{
		// Auto detect rigidbody
		if (rBody == null)
		{
			rBody = GetComponentInParent<Rigidbody>();
		}
	}

#endif
}

// Written by Garrett Eddy: 10/9/2017