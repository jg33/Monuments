using UnityEngine;
//using Windows.Kinect;
using System.Collections;
using System;


public class GameRestartListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Selection redial that needs to be filled for game restart.")]
	public VRStandardAssets.Utils.SelectionRadial selectionRedial;

	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	public TextMesh gestureInfo;

	// result of the gestures
	private float restartProgress = 0f;
	//private bool restartDetected = false;


	/// <summary>
	/// Gets the restart gesture progress.
	/// </summary>
	/// <returns>The raise-hand progress.</returns>
	public float GetRestartGestureProgress()
	{
		return restartProgress;
	}


//	/// <summary>
//	/// Determines whether a restart-game gesture was detected.
//	/// </summary>
//	/// <returns><c>true</c> if restart game gesture was detected; otherwise, <c>false</c>.</returns>
//	public bool IsRestartGestureDetected()
//	{
//		bool bRetVal = restartDetected;
//		restartDetected = false;
//		return bRetVal;
//	}


	public void UserDetected(long userId, int userIndex)
	{
		if (userIndex == playerIndex) 
		{
			KinectManager manager = KinectManager.Instance;

			//manager.DetectGesture(userId, KinectGestures.Gestures.RaiseLeftHand);
			//manager.DetectGesture(userId, KinectGestures.Gestures.RaiseRightHand);
			manager.DetectGesture(userId, KinectGestures.Gestures.Tpose);
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
			
		//if(gesture == KinectGestures.Gestures.RaiseLeftHand || gesture == KinectGestures.Gestures.RaiseRightHand)
		if (gesture == KinectGestures.Gestures.Tpose)
		{
			restartProgress = progress;

			if (selectionRedial) 
			{
				selectionRedial.SetRedialProgress(progress);
			}
		}
	}

	public bool GestureCompleted(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		if (userIndex != playerIndex)
			return false;

		//if (gesture == KinectGestures.Gestures.RaiseLeftHand || gesture == KinectGestures.Gestures.RaiseRightHand)
		if (gesture == KinectGestures.Gestures.Tpose)
		{
			//restartDetected = true;
			restartProgress = 1f;

			if (selectionRedial) 
			{
				selectionRedial.SetRedialFilled();
			}
		}

		return true;
	}

	public bool GestureCancelled(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		if (userIndex != playerIndex)
			return false;

		//if (gesture == KinectGestures.Gestures.RaiseLeftHand || gesture == KinectGestures.Gestures.RaiseRightHand) 
		if (gesture == KinectGestures.Gestures.Tpose)
		{
			//restartDetected = false;
			restartProgress = 0f;

			if (selectionRedial) 
			{
				selectionRedial.ResetRedialProgress();
			}
		}

		return true;
	}

}
