using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Checker : MonoBehaviour {

	public int[] HcurrentPos = {4, 4}; // [LeftHand,RightHand]
    public int[] FcurrentPos = {0, 0}; // [LeftFoot,RightFoot]
	private int Hmax;
	private int Hmin;
	private int Fmin;
	private int Fmax;
	public bool LimbMoving = false;
	private float NextTime = 0f;
	private LadderTask Movement; // Ladder Logic

    public Transform[] limbs;

	void Awake () {

	    Movement = transform.GetComponent<LadderTask> ();
	 
	}

	void Update () {	
			
		if (Input.GetKeyDown("r")) {
			this.LeftHand("UP");
		}
		if (Input.GetKeyDown("t")) {
			this.RightHand("UP");	
		}
		if (Input.GetKeyDown("f")) {
			this.LeftFoot("UP");
		}
		if (Input.GetKeyDown("g")) {
			this.RightFoot("UP");
		}		
	}

    private void LateUpdate()
    {
        /*LimbMoving = false;
        for (int i = 0; i < limbs.Length; i++)
        {
            if (limbs[i].hasChanged)
            {
                LimbMoving = true;
                limbs[i].hasChanged = false;
            }
        }

        this.LeftHand("UP");
        this.RightHand("UP");
        this.LeftFoot("UP");
        this.RightFoot("UP");
        */
        if (HcurrentPos[0] + HcurrentPos[1] + FcurrentPos[0] + FcurrentPos[1] > 40)
        {
//            Debug.Log(Time.time);
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#endif
        }

    }

    /// <summary>
    /// Avoid more than one limb movement 
    /// Modifies the variable LimbMoving
    /// To let us know if any limb was activated
    /// the time of movement was calculated with 
    /// rungDistance/speed variables in LadderTask.cs
    /// </summary>

    IEnumerator Zerocondition() {

		// Two limbs are moving at the same time 
		// (e.g., LeftHandUp() and RightFootUp() 
		// are called simultaneously).

		LimbMoving = true;
		yield return new WaitForSeconds(0.6f);
		LimbMoving = false;
        //yield return new WaitForSeconds(0);
    }

	/// <summary>
	/// Handles Left Hand movement
	/// </summary>
	/// <param name="direction"></param>
	
	public void LeftHand(string direction){

		if (direction == "UP" && CheckNextDistance(Lhand:1) && !LimbMoving){
			Movement.LeftHandUp();
			HcurrentPos[0]+=1;
			StartCoroutine(Zerocondition());
		}
		
		else if(direction == "DOWN" && CheckNextDistance(Lhand:-1) && !LimbMoving){
			Movement.LeftHandDown();
			HcurrentPos[0]-=1;
			StartCoroutine(Zerocondition());
		}
	}

	/// <summary>
	/// Handles Right Hand movement
	/// </summary>
	/// <param name="direction"></param>
	 
	public void RightHand(string direction){

		if (direction == "UP" && CheckNextDistance(Rhand:1) && !LimbMoving){
			Movement.RightHandUp();
			HcurrentPos[1] +=1;
			StartCoroutine(Zerocondition());
		}
		else if(direction == "DOWN" && CheckNextDistance(Rhand:-1) && !LimbMoving){
			Movement.RightHandDown();
			HcurrentPos[1] -=1;
			StartCoroutine(Zerocondition());
		}
	}

	/// <summary>
	/// Handles Right Foot Movement
	/// </summary>
	/// <param name="direction"></param>
 
	public void RightFoot(string direction){

		if (direction == "UP" && CheckNextDistance(Rfoot:1) && !LimbMoving){
			Movement.RightFootUp();
			FcurrentPos[1] += 1;
			StartCoroutine(Zerocondition());
		}
		else if(direction == "DOWN" && CheckNextDistance(Rfoot:-1) && !LimbMoving){
			Movement.RightFootDown();
			FcurrentPos[1] -= 1;
			StartCoroutine(Zerocondition());
		}
	}

	/// <summary>
	/// Handles Left Foot Movement
	/// </summary>
	/// <param name="direction"></param>
	 
	public void LeftFoot(string direction){

		if (direction == "UP" && CheckNextDistance(Lfoot:1) && !LimbMoving){
			FcurrentPos[0] += 1;
			Movement.LeftFootUp();
			StartCoroutine(Zerocondition());
		}
		else if(direction == "DOWN" && CheckNextDistance(Lfoot:-1) && !LimbMoving){
			FcurrentPos[0] -= 1;
			Movement.LeftFootDown();
			StartCoroutine(Zerocondition());	
		}
	}

	/// <summary>
	/// Check validity of next step.
	/// if no parameter is passed, return current status.
	/// </summary>
	/// <param name="Lhand"></param>
	/// <param name="Rhand"></param>
	/// <param name="Lfoot"></param>
	/// <param name="Rfoot"></param>
	/// <returns>Bool: Next step is valid or not</returns>

	public bool CheckNextDistance(int Lhand=0, int Rhand=0, int Lfoot=0, int Rfoot=0){
	
		Hmax = Mathf.Max(HcurrentPos[0]+Lhand, HcurrentPos[1]+Rhand);
		Hmin = Mathf.Min(HcurrentPos[0]+Lhand, HcurrentPos[1]+Rhand);
		Fmax = Mathf.Max(FcurrentPos[0]+Lfoot, FcurrentPos[1]+Rfoot);
		Fmin = Mathf.Min(FcurrentPos[0]+Lfoot, FcurrentPos[1]+Rfoot);
	
		// Any foot is within 2 rungs of any hand 
		// (i.e., a foot can never be above a hand 
		// and must remain at least 3 rungs below).
		
		bool firstCondition = ((Hmin - Fmax) > 2);
		
		// Any hand is 6 or more rungs above any foot 
		// (i.e., the task cannot be completed by 
		// climbing first with the hands and then the feet).
		
		bool secondCondition = ((Hmax - Fmin) < 6);

        // prevent going through the floor
        bool aboveFloor = Fmin >= 0;

        // If any condition is violated, player will fall

        return firstCondition && secondCondition && aboveFloor;


	}
}


