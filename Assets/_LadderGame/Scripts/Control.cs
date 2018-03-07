


using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Control : MonoBehaviour {

	private Checker Target;
	private GameObject Hand1;
	private GameObject Hand2;
	private float Threshold = 5f;
	private Vector3 LastHand1Pos = new Vector3(0, 0, 0);
	private Vector3 LastHand2Pos = new Vector3(0, 0, 0);
	private bool HMD = false;
	private bool next1 = false;
	private bool next2 = false;
	private int activeLibm = 1;

	void Start () {
		
		Target = transform.GetComponent<Checker>();
		GameObject player = GameObject.Find("Player");
		
		if(HMD){
			
			GameObject Objs = player.transform.GetChild(0).gameObject;
			Hand1 = Objs.transform.GetChild(2).gameObject;
			Hand2 = Objs.transform.GetChild(3).gameObject;
			Debug.Log(Hand2.name);
		}
		else{

			GameObject Objs = player.transform.GetChild(1).gameObject;
			Debug.Log(Objs.name);
			Hand1 = Objs.transform.GetChild(1).gameObject;
			Hand2 = Objs.transform.GetChild(1).gameObject;
			Debug.Log(Hand2.name);
		}
	}
	
	void Update () {

		// Debug.Log("Hand1: " + (Threshold<GetHand1Velocity()));
		// Debug.Log("Hand2: " + (Threshold<GetHand2Velocity()));
		// Debug.Log("ActiveLimb: " + activeLibm);
		// Debug.Log("Is Moving?: " + Target.LimbMoving);
		this.Climb();
	}

	/// <summary>
	/// Calculate Velocity Of First Controler Shake
	/// </summary>
	/// <returns> Magnitude of Velocity</returns>

	private float GetHand1Velocity(){

		Vector3 CurrentHandPos = Hand1.transform.position;
		Vector3 Distance = CurrentHandPos - LastHand1Pos;
		LastHand1Pos = CurrentHandPos;
		float Velocity = Distance.magnitude / Time.deltaTime;

		return Velocity;
	}


	/// <summary>
	/// Calculate Velocity Of Second Controler Shake
	/// </summary>
	/// <returns> Magnitude of Velocity</returns>

	private float GetHand2Velocity(){

		Vector3 CurrentHandPos = Hand2.transform.position;
		Vector3 Distance = CurrentHandPos - LastHand2Pos;
		LastHand2Pos = CurrentHandPos;
		float Velocity = Distance.magnitude / Time.deltaTime;
		return Velocity;
	}

	/// <summary>
	/// Controles User Climbing
	/// </summary>

	private void Climb(){

		next1 = Threshold<GetHand1Velocity();
		next2 = Threshold<GetHand2Velocity();

		if(next1 && (activeLibm == 1) && !Target.LimbMoving){
			Target.LeftHand("UP");	
			activeLibm++;
			Debug.Log("Im In 1");
		}

		if(next2 && (activeLibm == 2) && !Target.LimbMoving){
			Target.RightFoot("UP");
			activeLibm++;
			Debug.Log("Im In 2");
		}

		if(next2 && (activeLibm == 3) && !Target.LimbMoving){
			Target.RightHand("UP");
			activeLibm++;
			Debug.Log("Im In 3");
		}

		if(next1 && (activeLibm == 4) && !Target.LimbMoving){
			Target.LeftFoot("UP");
			activeLibm = 1;
			Debug.Log("Im In 4");
		}
		

	}
}

