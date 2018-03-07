using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {

	public Transform target;

	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - target.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//transform.rotation = Quaternion.LookRotation(target.position - transform.position);
		//transform.position = target.position + offset;
		transform.position = target.position + Vector3.ProjectOnPlane(-target.forward * offset.magnitude, Vector3.up) + Vector3.up * offset.y;
		transform.rotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up));
		transform.rotation *= Quaternion.Euler(20, 0, 0);
	}
}
