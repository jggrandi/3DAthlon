// Monobehaviour to be attached to race rings. Requires trigger collider.
//
// Detects when the player travels through a ring and calls public events OnScore and OnPenalize
// depending on whether they went through the correct direction or not.
// 
// The correct direction to travel through a ring is from positive to negative along the local Y axis.
//
// The incorrect direction to travel through a ring is from negative to positive along the local Y axis.

using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class RaceRing : MonoBehaviour
{
	//------------------------- PUBLIC -------------------------//

	// Event for when the the player passes through a ring in the correct direction.
	// Passes the ring the player went through as a parameter.
	public static event Action<RaceRing> OnScore;

	// Event for when the the player passes through a ring in the incorrect direction.
	// Passes the ring the player went through as a parameter.
	public static event Action<RaceRing> OnPenalize;

	// Trigger to detect player
	public Collider trigger;


	//------------------------- PRIVATE -------------------------//

	// Holds the position of the player last frame
	private Vector3 lastPlayerPos;

	// Whether or not the player can score, set to false after going through the wrong direction
	private bool canScore;


	private void OnTriggerEnter(Collider other)
	{
		// If its not a player, don't bother
		if (other.transform.root.tag != "Player")
			return;

		// Update lastPlayerPos
		lastPlayerPos = other.transform.position;

		canScore = true;
	}


	private void OnTriggerStay(Collider other)
	{
		// If its not a player, don't bother
		if (other.transform.root.tag != "Player")
			return;

		// If we went through the wrong way already, do nothing
		if (!canScore)
			return;

		// Look at the relative position of the player on the current and previous frame.
		Vector3 rel = transform.InverseTransformPoint(other.transform.position);
		Vector3 prevRel = transform.InverseTransformPoint(lastPlayerPos);

		// If the player's origin passes through the ring's local XY plane
		if (Mathf.Sign(rel.y) != Mathf.Sign(prevRel.y))
		{
			// If the player passes going the correct direction
			if (rel.y < 0)
			{
				// Call OnScore
				if (OnScore != null)
					OnScore(this);

				trigger.enabled = false;
			}
			// If the player passes going the incorrect direction
			else
			{
				// Call OnPenalize
				if (OnPenalize != null)
					OnPenalize(this);

				canScore = false;
			}
		}

		// Update lastPlayerPos
		lastPlayerPos = other.transform.position;
	}

#if UNITY_EDITOR

	// Editor QoL
	private void Reset()
	{
		// Auto detect trigger component
		if (trigger == null)
		{
			trigger = GetComponent<Collider>();
		}

		// Make sure trigger isn't accidently not a trigger
		trigger.isTrigger = true;
	}

#endif
}

// Written by Garrett Eddy: 10/9/2017