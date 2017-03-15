using UnityEngine;
using System.Collections;

public class CameraRollFixer : MonoBehaviour 
{
	void Update () 
	{
		Vector3 cameraUp = transform.up; // Camera.main ? Camera.main.transform.up : transform.up;
		Quaternion invPitchRot = Quaternion.FromToRotation(cameraUp, Vector3.up);
		transform.rotation = transform.rotation * invPitchRot;
	}
}
