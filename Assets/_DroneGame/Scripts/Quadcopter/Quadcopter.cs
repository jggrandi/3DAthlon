// Monobehaviour to be attatched to Quadcopter, requires rigidbody.
//
// Performs a basic simulation of a quadcopter using a rigidbody.
//
// Recieves control input through Drive(), then modifies the power of each
// motor based on the input to fly.
//
// FlipRoll() and FlipPitch() perform an automatic 180 degree flip along the X / Z axis respectively.
//
// Provides a gyroStabilization option which automatically balances the quadcopter in the air and limits
// the pitch and roll attitude to gyroTiltLimit. Needs polish, but works well enough for now.
//
// Recommended values: gyroTiltLimit = 0.8, flipSpeed = 5
//
// Recommended rigidbody values: Drag = 0.5, AngularDrag = 10

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Quadcopter : MonoBehaviour
{
	//------------------------- PUBLIC -------------------------//

	// Determines whether gyroStabilization feature is on or off
	public bool gyroStabilization;

	// The value to limit the pitch and roll attitudes when gyroStabilization is on
	public float gyroTiltLimit;

	// Determines how quickly the quadcopter flips when using FlipRoll() or FlipPitch()
	public float flipSpeed;

	// The quadcopter's four motors and recommended spin direction for each motor
	public Motor MotorNE;	// clockwise
	public Motor MotorNW;	// counter clockwise
	public Motor MotorSE;	// counter clockwise
	public Motor MotorSW;	// clockwise

	// The control input for power, goes from 0 to 1
	// 0 = no power, 1 = full power
	public float Power { get; private set; }

	// The control input for pitch, goes from -1 to 1
	// -1 = pitch back, 1 = pitch forward
	public float Pitch { get; private set; }

	// The control input for yaw, goes from -1 to 1
	// -1 = yaw left, 1 = yaw right
	public float Yaw { get; private set; }

	// The control input for roll, goes from -1 to 1
	// -1 = roll left, 1 = roll right
	public float Roll { get; private set; }

	// The main function which sets the control input for the quadcopter
	public void Drive(float power, float pitch, float yaw, float roll)
	{
		Power = Mathf.Clamp01(power);
		Pitch = Mathf.Clamp(pitch, -1, 1);
		Yaw = Mathf.Clamp(yaw, -1, 1);
		Roll = Mathf.Clamp(roll, -1, 1);
	}

	// Instructs the quadcopter to perform a quick flip via rolling in the direction of axis
	// -1 = roll left, 1 = roll right
	public void FlipRoll(int axis)
	{
		// If a flip is already in progress, ignore
		if (flip != Vector2.zero)
			return;

		// Sets vectors for physics calculations
		flip = Vector2.right * Mathf.Clamp(axis, -1, 1) * flipSpeed;
		flipStart = transform.up;
		flipTime = Time.time;
	}

	// Instructs the quadcopter to perform a quick flip via pitching in the direction of axis
	// -1 = pitch back, 1 = pitch forward
	public void FlipPitch(int axis)
	{
		// If a flip is already in progress, ignore
		if (flip != Vector2.zero)
			return;

		// Sets vectors for physics calculations
		flip = Vector2.up * Mathf.Clamp(axis, -1, 1) * flipSpeed;
		flipStart = transform.up;
		flipTime = Time.time;
	}

	// Sets the control input for power
	[System.Obsolete("Individually modifying control inputs is not recommended, please use Drive()", false)]
	public void SetPower(float power)
	{
		Power = Mathf.Clamp01(power);
	}

	// Sets the control input for pitch
	[System.Obsolete("Individually modifying control inputs is not recommended, please use Drive()", false)]
	public void SetPitch(float pitch)
	{
		Pitch = Mathf.Clamp(pitch, -1, 1);
	}

	// Sets the control input for yaw
	[System.Obsolete("Individually modifying control inputs is not recommended, please use Drive()", false)]
	public void SetYaw(float yaw)
	{
		Yaw = Mathf.Clamp(yaw, -1, 1);
	}

	// Sets the control input for roll
	[System.Obsolete("Individually modifying control inputs is not recommended, please use Drive()", false)]
	public void SetRoll(float roll)
	{
		Roll = Mathf.Clamp(roll, -1, 1);
	}


	//------------------------- PRIVATE -------------------------//

	// Stores whether to perform a flip or not, as well as the direction and speed to flip in
	private Vector2 flip;

	// Stores the upwards direction of the quadcopter when a flip is started
	private Vector3 flipStart;

	// Timer to stop flip if it exceeds time limit
	private float flipTime;


	private void FixedUpdate()
	{
		// Makes moving diagonally the same speed as forward and back
		Vector3 horizontal =  Vector3.ClampMagnitude(new Vector3(Roll, 0, Pitch), 1);

		// Temporary variables for control inputs
		float power = Power;
		float pitch = horizontal.z;
		float yaw = Yaw;
		float roll = horizontal.x;

		// Power values for each motor
		float ne = power;
		float nw = power;
		float se = power;
		float sw = power;

		// If we want to, or already are performing a flip
		if (flip != Vector2.zero)
		{
			// do pitch
			se += flip.y;
			sw += flip.y;
			ne -= flip.y;
			nw -= flip.y;

			// do roll
			nw += flip.x;
			sw += flip.x;
			ne -= flip.x;
			se -= flip.x;

			// Set the power for each motor
			MotorNE.SetPower(ne);
			MotorNW.SetPower(nw);
			MotorSE.SetPower(se);
			MotorSW.SetPower(sw);

			// Check if the flip is complete
			if (Time.time - flipTime > 3 / flipSpeed || Vector3.Dot(flipStart, transform.up) < -0.9f)
			{
				// Stop flipping
				flip = Vector2.zero;
			}

			// Don't do anything else in a flip
			return;
		}

		// Measures how tilted the quadcopter is compared to Vector3.up, used for stabilization
		Vector3 flat = transform.InverseTransformVector( Vector3.ProjectOnPlane(transform.up, Vector3.up) );
		flat.y = 0;

		// if stabilization is on and the quadcopter is flying
		if (gyroStabilization && Mathf.Abs(power + pitch + roll + yaw) > 0)
		{
			// Amount of correction needed to not surpass tiltLimit
			Vector3 correct = flat * flat.magnitude / gyroTiltLimit;

			// Apply roll correction
			ne += correct.x;
			se += correct.x;
			nw -= correct.x;
			sw -= correct.x;

			// Apply pitch correction
			ne += correct.z;
			nw += correct.z;
			se -= correct.z;
			sw -= correct.z;
		}

		// do pitch
		if (pitch > 0)
		{
			se += pitch / 2;
			sw += pitch / 2;
		}
		else
		{
			ne -= pitch / 2;
			nw -= pitch / 2;
		}

		// do yaw
		nw += yaw / 2;
		se += yaw / 2;
		ne -= yaw / 2;
		sw -= yaw / 2;

		// do roll
		if (roll > 0)
		{
			nw += roll / 2;
			sw += roll / 2;
		}
		else
		{
			ne -= roll / 2;
			se -= roll / 2;
		}

		// Set the power for each motor
		MotorNE.SetPower(ne);
		MotorNW.SetPower(nw);
		MotorSE.SetPower(se);
		MotorSW.SetPower(sw);
	}

#if UNITY_EDITOR

	// Editor QoL
	private void Reset()
	{
		// Set Tag
		tag = "Player";

		// Auto detect motors
		Motor[] motors = GetComponentsInChildren<Motor>();

		if (MotorNE == null)
		{
			foreach (Motor motor in motors)
			{
				if (motor.name == "MotorNE")
					MotorNE = motor;
			}
		}
		if (MotorNW == null)
		{
			foreach (Motor motor in motors)
			{
				if (motor.name == "MotorNW")
					MotorNW = motor;
			}
		}
		if (MotorSE == null)
		{
			foreach (Motor motor in motors)
			{
				if (motor.name == "MotorSE")
					MotorSE = motor;
			}
		}
		if (MotorSW == null)
		{
			foreach (Motor motor in motors)
			{
				if (motor.name == "MotorSW")
					MotorSW = motor;
			}
		}

		// Auto create motors
		if (MotorNE == null)
		{
			GameObject g = new GameObject();
			g.name = "MotorNE";
			g.transform.parent = transform;
			g.transform.localPosition = new Vector3(1, 0, 1);
			MotorNE = g.AddComponent<Motor>();
		}
		if (MotorNW == null)
		{
			GameObject g = new GameObject();
			g.name = "MotorNW";
			g.transform.parent = transform;
			g.transform.localPosition = new Vector3(-1, 0, 1);
			MotorNW = g.AddComponent<Motor>();
			MotorNW.direction = Motor.MotorDirection.COUNTERCLOCKWISE;
		}
		if (MotorSE == null)
		{
			GameObject g = new GameObject();
			g.name = "MotorSE";
			g.transform.parent = transform;
			g.transform.localPosition = new Vector3(1, 0, -1);
			MotorSE = g.AddComponent<Motor>();
			MotorSE.direction = Motor.MotorDirection.COUNTERCLOCKWISE;
		}
		if (MotorSW == null)
		{
			GameObject g = new GameObject();
			g.name = "MotorSW";
			g.transform.parent = transform;
			g.transform.localPosition = new Vector3(-1, 0, -1);
			MotorSW = g.AddComponent<Motor>();
		}
	}

#endif
}

// Written by Garrett Eddy: 10/9/2017