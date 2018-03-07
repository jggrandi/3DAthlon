// Monobehaviour to be attatched to a container object which holds the rings to fly through
//
// Has an array of rings which the player must fly through in order
//
// Handles events for the player passing through a ring in the correct direction and the incorrect direction
//
// Tracks the amount of time the player takes to complete the course

using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceGameMode : MonoBehaviour
{
    //------------------------- PUBLIC -------------------------//

    // Singleton
    public static RaceGameMode S;

    // The set of rings in the couse, ordered in the sequence the player must fly through
    public RaceRing[] rings;

    // The amount of time which gets added to the stopwatch when the player flys through a ring
    // in the wrong direction
    public float timePenalty;

    // The amount of time the player has taken on the course so far / their final time
    public float Stopwatch
    {
        get
        {
            if (!finished)
                return Time.time - timeStart;
            else
                return finalTime;
        }
    }

    // The next active ring the player must fly through
    public RaceRing nextRing
    {
        get
        {
            if (nextIndex < rings.Length)
                return rings[nextIndex];
            else
                return null;
        }
    }


    //------------------------- PRIVATE -------------------------//

    // time at which the race started
    private float timeStart;

    // index of the next ring
    private int nextIndex;

    // whether or no the playre completed the race
    private bool finished;

    // The player's final time for the race
    private float finalTime;


    private void Start()
    {
        S = this;

        // Disable all the rings
        foreach (RaceRing ring in rings)
        {
            ring.trigger.enabled = false;
        }

        nextIndex = 0;

        // enable the first ring
        rings[nextIndex].trigger.enabled = true;

        // Subscribe to ring events
        RaceRing.OnScore -= Checkpoint;
        RaceRing.OnScore += Checkpoint;
        RaceRing.OnPenalize -= Penalty;
        RaceRing.OnPenalize += Penalty;
    }

    // Called when the player goes through a ring in the correct direction
    private void Checkpoint(RaceRing ring)
    {
        // increment the index
        nextIndex++;

        // Check if the course was completed
        if (nextIndex > rings.Length - 1)
        {
            finished = true;
            finalTime = Time.time - timeStart;

            // Added on 2/7/2018 : after task is complete, load the next scene in the build order.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            return;
        }

        // activate the next ring
        rings[nextIndex].trigger.enabled = true;
    }

    // Called when the player goes through a ring in the incorrect direction
    private void Penalty(RaceRing ring)
    {
        // Add a time penalty to the clock
        timeStart -= timePenalty;
    }


    private void Update()
    {
        //temp
        //Debug.Log(Stopwatch);
    }

#if UNITY_EDITOR

    // Editor QoL
    private void Reset()
    {
        // Autofill rings array
        if (rings == null)
            rings = new RaceRing[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            rings[i] = transform.GetChild(i).GetComponent<RaceRing>();
        }
    }

#endif
}

// Written by Garrett Eddy: 10/9/2017