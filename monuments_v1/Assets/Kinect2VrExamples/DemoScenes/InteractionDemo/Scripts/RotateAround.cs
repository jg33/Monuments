using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour 
{
	public float _rotSpeed = 50f;

	void Update () 
	{
		this.gameObject.transform.RotateAround(this.transform.position, Vector3.up, _rotSpeed * Time.deltaTime);
	}
}
