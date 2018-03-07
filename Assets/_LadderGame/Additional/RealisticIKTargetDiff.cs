using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticIKTargetDiff : MonoBehaviour {
    float rungDistance = 0.3f;
    public Transform refTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float stepCompletion = refTarget.localPosition.y % rungDistance / rungDistance;
        //stepCompletion = stepCompletion * stepCompletion;
        float offset = Mathf.Sqrt(Mathf.Sin(stepCompletion * Mathf.PI));
        Vector3 pos = refTarget.localPosition + Vector3.back * offset * 0.15f + Vector3.up * offset * 0.1f;
        if (pos.x == pos.x)
            this.transform.localPosition = pos;

    }
}
