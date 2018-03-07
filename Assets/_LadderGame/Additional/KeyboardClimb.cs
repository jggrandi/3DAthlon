using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardClimb : MonoBehaviour {

    LadderTask lTask;

    // Use this for initialization
    void Start () {
        lTask = FindObjectOfType<LadderTask>();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            lTask.LeftHandUp();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            lTask.RightHandUp();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            lTask.LeftFootUp();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            lTask.RightFootUp();
        }


    }
}
