// IMPORTANT: In Unity's script execution order settings, this script must execute before the quadcopter scripts / default time
//
// Monobehaviour to be attatched to an empty gameobject
//
// Detects cheating by predicting the change in position, and change in velocity of the quadcopter based on the power of each motor,
// then comparing it to the actual change in position and change in velocity each frame.
//
// To compensate for any false positives, there is a cheating tolerence setting.
// The tolerence setting is the maximum percentage of time spent cheating this script will allow before taking action.
//
// When enough cheating is detected, the scene is reset.


using UnityEngine;
using UnityEngine.SceneManagement;

public class AntiCheat : MonoBehaviour
{
	//------------------------- PUBLIC -------------------------//

	// The maximum percentage of time the user can cheat before this script takes action
	// Default setting is 15%, so the scene will be reset only if the user cheats more than 15% of the time
	public float CheatingTolerence = .15f;



	//------------------------- PRIVATE -------------------------//

	// Reference to the quadcopter in the current scene
	private Quadcopter drone;

	// The average amount of thrust produced by a single motor on the quadcopter
	private float motorThrust;

	// Reference to the rigidbody attatched to the quadcopter
	private Rigidbody rBody;

	// The position (worldspace) of the quadcopter last frame
	private Vector3 lastPos;

	// The velocity of the quadcopter's rigidbody last frame
	private Vector3 lastVel;

	// The power setting for each of the quadcopter's motors last frame
	private float[] lastMotors;

	// The total amount of time AntiCheat has been running
	private float totalTime;

	// The total amount of time the user is determined to be cheating
	private float invalidTime;


	private void Start()
	{
		// Get all quadcopters in the scene
		Quadcopter[] drones = FindObjectsOfType<Quadcopter>();

		// Throw an exception if there is more than one quadcopter
		if (drones.Length > 1)
		{
			throw new System.Exception("Multiple quadcopters detected.");
		}

		// Initialization
		drone = drones[0];
		motorThrust = (drone.MotorNE.maxThrust + drone.MotorNW.maxThrust + drone.MotorSE.maxThrust + drone.MotorSW.maxThrust) / 4.0f;
		rBody = drone.GetComponent<Rigidbody>();


		// Make sure the quadcopter has a rigidbody
		if (rBody == null)
		{
			throw new System.Exception("Rigidbody on quadcopter is missing.");
		}


		// Make sure the rigidbody parameters have not been modified
		if (rBody.isKinematic == true || rBody.useGravity == false || rBody.constraints != RigidbodyConstraints.None)
		{
			throw new System.Exception("Quadcopter rigidbody paramters do not match.");
		}


		// Initiazation
		lastPos = drone.transform.position;
		lastVel = rBody.velocity;
		lastMotors = new float[] { drone.MotorNE.Power, drone.MotorNW.Power, drone.MotorSE.Power, drone.MotorSW.Power };
	}


	// Handles reseting the scene if the amount of cheating exceeds the tolerence setting
	private void Update()
	{
		// To give a valid sample size, don't do anything until 90 seconds in
		if (totalTime < 90)
			return;

		// If the percentage of time cheating exceeds the tolerence setting, reset the current scene
		if (invalidTime / totalTime > CheatingTolerence)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			Debug.Log("Cheating Threshold Exceeded: Scene Reset");
		}
	}


	// Handles movement prediction to detect cheating
	private void FixedUpdate()
	{
		// Update total time
		totalTime += Time.fixedDeltaTime;


		// Look for potential physics collisions near the quadcopter
		int count = 0;

		foreach (Collider collider in Physics.OverlapSphere(drone.transform.position, 2))
		{
			if (!collider.transform.root.CompareTag("Player"))
			{
				count++;
			}
		}

		// If there are potential collisions nearby, we turn off the cheat detection
		if (count > 0)
		{
			UpdateValues();
			return;
		}


		// Cheat Detection Part 1 - Velocity validation
		// Checks if the quadcopter's position is being modified by another script

		// The movement that is expected from last frame based on the rigidbody's velocity last frame
		Vector3 expectedVel = lastVel;

		// The actual movement which occured from last frame
		Vector3 trueVel = drone.transform.position - lastPos;

		// As an approximation, check to see if expectedVel and trueVel are in the same relative direction
		if ( Vector3.Dot(expectedVel, trueVel) < 0)
		{
			// if expectedVel and trueVel are NOT in the same relative direction, cheating has occurred
			invalidTime += Time.fixedDeltaTime;
			UpdateValues();
			Debug.Log("Potential Cheating: Velocity Mismatch");
			return;
		}



		// Cheat Detection Part 1 - Acceleration validation
		// Checks if other forces are being applied to the quadcopter's rigidbody

		// The change in velocity that was expected last frame based on gravity, drag, and the power of the motors last frame
		// Note: if the thrust calculation in Motor.cs is modified, this calculation also must be modified
		Vector3 expectedAccel = Physics.gravity * Time.fixedDeltaTime
			+ -lastVel + lastVel * (1 - Time.fixedDeltaTime * rBody.drag)
			+ (lastMotors[0] + lastMotors[1] + lastMotors[2] + lastMotors[3]) * motorThrust * Time.deltaTime * Time.fixedDeltaTime * drone.transform.up / rBody.mass;

		// The actual change in velocity from the last frame
		Vector3 trueAccel = rBody.velocity - lastVel;

		// As an approximation, check to see if expectedAccel and trueAccel are in the same relative direction
		if (Vector3.Dot(expectedAccel, trueAccel) < 0)
		{
			// if expectedAccel and trueAccel are NOT in the same relative direction, cheating has occurred
			invalidTime += Time.fixedDeltaTime;
			Debug.Log("Potential Cheating: Acceleration Mismatch");
		}

		UpdateValues();
	}


	// Updates the tracked values from the last frame
	private void UpdateValues()
	{
		lastPos = drone.transform.position;
		lastVel = rBody.velocity;
		lastMotors[0] = drone.MotorNE.Power;
		lastMotors[1] = drone.MotorNW.Power;
		lastMotors[2] = drone.MotorSE.Power;
		lastMotors[3] = drone.MotorSW.Power;
	}
}

// Written by Garrett Eddy: 10/21/2017