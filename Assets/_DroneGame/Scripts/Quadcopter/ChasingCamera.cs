using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ChasingCamera : MonoBehaviour {

   
    public GameObject player;
    public Vector2 Position = new Vector3(4.0f, -0.5f);


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        player.transform.rotation = Quaternion.Euler( this.gameObject.transform.rotation.eulerAngles - new Vector3(this.gameObject.transform.rotation.eulerAngles.x, 0.0f , this.gameObject.transform.rotation.eulerAngles.z));

        Vector3 rotationAroundDrone = new Vector3(Position[0]*Mathf.Sin(player.transform.rotation.eulerAngles.y * Mathf.Deg2Rad), Position[1], Position[0] * Mathf.Cos(player.transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
   
        player.transform.position = this.gameObject.transform.position - rotationAroundDrone;
    }
}
