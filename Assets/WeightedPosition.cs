using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPosition : MonoBehaviour {

    public Transform[] referenceTransforms;
    public bool updateX = true, updateY = true, updateZ = true;
    public bool useLocalPosition = false;

    public Vector3 MeanPosition(Transform[] transforms)
    {
        Vector3 mean = Vector3.zero;
        for(int i = 0; i < transforms.Length; i++)
        {
            mean += useLocalPosition ? transforms[i].localPosition : transforms[i].position;
        }
        return 1.0f / transforms.Length * mean;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 meanPos = MeanPosition(referenceTransforms);
        Vector3 currentPos = useLocalPosition ? this.transform.localPosition : this.transform.position;
        if (updateX)
            currentPos.x = meanPos.x;
        if (updateY)
            currentPos.y = meanPos.y;
        if (updateZ)
            currentPos.z = meanPos.z;
        if (useLocalPosition)
            this.transform.localPosition = currentPos;
        else
            this.transform.position = currentPos;
    }
}
