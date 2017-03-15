using UnityEngine;
using System.Collections;

public class HandTrigger : MonoBehaviour 
{
	public enum WhichHand : int { LeftHand, RightHand }
	[Tooltip("To which hand is the trigger attahced.")]
	public WhichHand whichHand;

	[Tooltip("If the trigger has been triggered.")]
	public bool triggered = false;

	[Tooltip("At which time the trigger was triggered.")]
	public float triggeredAt = 0f;

	[Tooltip("Transform which caused the collision/trigger.")]
	public Transform collidedWith;


	void OnTriggerEnter(Collider other)
	{
		if (triggered)
			return;
		
		triggered = true;
		triggeredAt = Time.time;
		collidedWith = other.transform;
	}

	void OnTriggerStay(Collider other)
	{
		if (other.transform.Equals (collidedWith))
		{
			triggered = true;
			triggeredAt = Time.time;
			collidedWith = other.transform;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.transform.Equals (collidedWith)) 
		{
			triggered = false;
			//triggeredAt = 0f;
		}
	}

}
