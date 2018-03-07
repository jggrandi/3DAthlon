using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(LadderTask))]
public class LadderTaskEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		LadderTask myScript = (LadderTask)target;

		if(GUILayout.Button("LeftFootUp"))
		{
			myScript.LeftFootUp();
		}

		if(GUILayout.Button("LeftFootDown"))
		{
			myScript.LeftFootDown();
		}

		if(GUILayout.Button("LeftHandUp"))
		{
			myScript.LeftHandUp();
		}

		if(GUILayout.Button("LeftHandDown"))
		{
			myScript.LeftHandDown();
		}

		if(GUILayout.Button("RightFootUp"))
		{
			myScript.RightFootUp();
		}

		if(GUILayout.Button("RightFootDown"))
		{
			myScript.RightFootDown();
		}

		if(GUILayout.Button("RightHandUp"))
		{
			myScript.RightHandUp();
		}

		if(GUILayout.Button("RightHandDown"))
		{
			myScript.RightHandDown();
		}
	}
}