using UnityEngine;
//using Windows.Kinect;
using System.Collections;
using System;


public class FlyerGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	public TextMesh gestureInfo;

	// result of the gestures
	private float leanAngle = 0f;
	private bool jumpDetected = false;
	private bool squatDetected = false;


	/// <summary>
	/// Gets the lean angle.
	/// </summary>
	/// <returns>The lean angle.</returns>
	public float GetLeanAngle()
	{
		return leanAngle;
	}


	/// <summary>
	/// Determines whether a jump was detected.
	/// </summary>
	/// <returns><c>true</c> if jump was detected; otherwise, <c>false</c>.</returns>
	public bool IsJumpDetected()
	{
		bool bRetVal = jumpDetected;
		jumpDetected = false;
		return bRetVal;
	}


	/// <summary>
	/// Determines whether a squat was detected.
	/// </summary>
	/// <returns><c>true</c> if squat was detected; otherwise, <c>false</c>.</returns>
	public bool IsSquatDetected()
	{
		bool bRetVal = squatDetected;
		squatDetected = false;
		return bRetVal;
	}


	public void UserDetected(long userId, int userIndex)
	{
		if (userIndex == playerIndex) 
		{
			KinectManager manager = KinectManager.Instance;

			manager.DetectGesture(userId, KinectGestures.Gestures.LeanLeft);
			manager.DetectGesture(userId, KinectGestures.Gestures.LeanRight);
			manager.DetectGesture(userId, KinectGestures.Gestures.Jump);
			manager.DetectGesture(userId, KinectGestures.Gestures.Squat);

			if(gestureInfo != null)
			{
				gestureInfo.text = "Lean left/right, jump or squat\nto control the plane.";
			}
		}
	}
	
	public void UserLost(long userId, int userIndex)
	{
		if(userIndex == playerIndex && gestureInfo != null)
		{
			gestureInfo.text = string.Empty;
		}
	}

	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		if (userIndex != playerIndex)
			return;
			
		if((gesture == KinectGestures.Gestures.LeanLeft || gesture == KinectGestures.Gestures.LeanRight) && progress > 0.5f)
		{
			if (gesture == KinectGestures.Gestures.LeanLeft) 
			{
				leanAngle = -screenPos.z;
			} 
			else if (gesture == KinectGestures.Gestures.LeanRight) 
			{
				leanAngle = screenPos.z;
			}
		}
	}

	public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		if (userIndex != playerIndex)
			return false;

		if (gesture == KinectGestures.Gestures.Jump) 
		{
			jumpDetected = true;
			return true;
		} 
		else if (gesture == KinectGestures.Gestures.Squat) 
		{
			squatDetected = true;
			return true;
		}

		return false;
	}

	public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		if (userIndex != playerIndex)
			return false;

		if (gesture == KinectGestures.Gestures.LeanLeft || gesture == KinectGestures.Gestures.LeanRight ||
		   	gesture == KinectGestures.Gestures.Jump || gesture == KinectGestures.Gestures.Squat) 
		{
			return true;
		}

		return false;
	}

}
