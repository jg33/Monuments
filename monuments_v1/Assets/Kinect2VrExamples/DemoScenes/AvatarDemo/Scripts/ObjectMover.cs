using UnityEngine;
using System.Collections;
using System;


public class ObjectMover : MonoBehaviour 
{
	public event Action<Transform> OnDestroyObject;

	void Update () 
	{
        if (transform.position.y < -2f)
        {
			if (OnDestroyObject != null) 
			{
				OnDestroyObject (transform);
			}

            Destroy(gameObject);
        }
	}
}
