using UnityEngine;
//using Windows.Kinect;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.IO;


/// <summary>
/// This interface needs to be implemented by all interaction listeners
/// </summary>
public interface InteractionListenerInterface
{
	/// <summary>
	/// Invoked when hand grip is detected.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="isRightHand">Whether it is the right hand or not</param>
	/// <param name="isHandInteracting">Whether this hand is the interacting one or not</param>
	/// <param name="handScreenPos">Hand screen position, including depth (Z)</param>
	void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos);

	/// <summary>
	/// Invoked when hand release is detected.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="isRightHand">Whether it is the right hand or not</param>
	/// <param name="isHandInteracting">Whether this hand is the interacting one or not</param>
	/// <param name="handScreenPos">Hand screen position, including depth (Z)</param>
	void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos);

	/// <summary>
	/// Invoked when hand click is detected.
	/// </summary>
	/// <returns><c>true</c>, if the click detection must be restarted, <c>false</c> otherwise.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="isRightHand">Whether it is the right hand or not</param>
	/// <param name="handScreenPos">Hand screen position, including depth (Z)</param>
	bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos);
}


/// <summary>
/// Interaction manager is the component that deals with hand interactions.
/// </summary>
public class InteractionManager : MonoBehaviour 
{
	/// <summary>
	/// The hand event types.
	/// </summary>
	public enum HandEventType : int
    {
        None = 0,
        Grip = 1,
        Release = 2
    }

	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Camera used for screen-to-world calculations. This is usually the main camera.")]
	public Camera screenCamera;

	[Tooltip("Whether to show the hand-moved cursor on the screen or not. The following textures need to be set too.")]
	public UnityEngine.UI.Image guiHandCursor;
	
	[Tooltip("Hand-cursor texture for the hand-grip state.")]
	public Sprite gripHandTexture;
	[Tooltip("Hand-cursor texture for the hand-release state.")]
	public Sprite releaseHandTexture;
	[Tooltip("Hand-cursor texture for the non-tracked state.")]
	public Sprite normalHandTexture;

	[Tooltip("Smooth factor for cursor movement.")]
	public float smoothFactor = 10f;
	
	[Tooltip("Whether hand clicks (hand not moving for ~2 seconds) are enabled or not.")]
	public bool allowHandClicks = false;
	
	[Tooltip("Whether hand cursor and interactions control the mouse cursor or not.")]
	private bool controlMouseCursor = false;

	[Tooltip("Whether hand grips and releases control mouse dragging or not.")]
	private bool controlMouseDrag = false;

	// Bool to specify whether to convert Unity screen coordinates to full screen mouse coordinates
	//public bool convertMouseToFullScreen = false;
	
	[Tooltip("List of the available interaction listeners. They must implement InteractionListenerInterface. If the list is empty, the available interaction listeners will be detected at start up.")]
	public List<MonoBehaviour> interactionListeners;

	[Tooltip("GUI-Text to display the interaction-manager debug messages.")]
	public TextMesh debugText;
	
	private long playerUserID = 0;
	private long lastUserID = 0;
	
	private bool isLeftHandPrimary = false;
	private bool isRightHandPrimary = false;
	
	private bool isLeftHandPress = false;
	private bool isRightHandPress = false;
	
	private Vector3 cursorScreenPos = Vector3.zero;
	private bool dragInProgress = false;
	
	private KinectInterop.HandState leftHandState = KinectInterop.HandState.Unknown;
	private KinectInterop.HandState rightHandState = KinectInterop.HandState.Unknown;
	
	private HandEventType leftHandEvent = HandEventType.None;
	private HandEventType lastLeftHandEvent = HandEventType.Release;

	private Vector3 leftHandPos = Vector3.zero;
	private Vector3 leftHandScreenPos = Vector3.zero;
	private Vector3 leftIboxLeftBotBack = Vector3.zero;
	private Vector3 leftIboxRightTopFront = Vector3.zero;
	private bool isleftIboxValid = false;
	private bool isLeftHandInteracting = false;
	private float leftHandInteractingSince = 0f;
	
	private Vector3 lastLeftHandPos = Vector3.zero;
	private float lastLeftHandTime = 0f;
	private bool isLeftHandClick = false;
	private float leftHandClickProgress = 0f;
	
	private HandEventType rightHandEvent = HandEventType.None;
	private HandEventType lastRightHandEvent = HandEventType.Release;

	private Vector3 rightHandPos = Vector3.zero;
	private Vector3 rightHandScreenPos = Vector3.zero;
	private Vector3 rightIboxLeftBotBack = Vector3.zero;
	private Vector3 rightIboxRightTopFront = Vector3.zero;
	private bool isRightIboxValid = false;
	private bool isRightHandInteracting = false;
	private float rightHandInteractingSince = 0f;
	
	private Vector3 lastRightHandPos = Vector3.zero;
	private float lastRightHandTime = 0f;
	private bool isRightHandClick = false;
	private float rightHandClickProgress = 0f;
	
	// Bool to keep track whether Kinect and Interaction library have been initialized
	private bool interactionInited = false;
	
	// The single instance of FacetrackingManager
	private static InteractionManager instance;


	// distance from gui-cursor to camera
	private float fCursorToCameraDist = 0f;


	/// <summary>
	/// Gets the single InteractionManager instance.
	/// </summary>
	/// <value>The InteractionManager instance.</value>
    public static InteractionManager Instance
    {
        get
        {
            return instance;
        }
    }
	
	/// <summary>
	/// Determines whether the InteractionManager was successfully initialized.
	/// </summary>
	/// <returns><c>true</c> if InteractionManager was successfully initialized; otherwise, <c>false</c>.</returns>
	public bool IsInteractionInited()
	{
		return interactionInited;
	}
	
	/// <summary>
	/// Gets the current user ID, or 0 if no user is currently tracked.
	/// </summary>
	/// <returns>The user ID</returns>
	public long GetUserID()
	{
		return playerUserID;
	}
	
	/// <summary>
	/// Gets the current left hand event (none, grip or release).
	/// </summary>
	/// <returns>The current left hand event.</returns>
	public HandEventType GetLeftHandEvent()
	{
		return leftHandEvent;
	}
	
	/// <summary>
	/// Gets the last detected left hand event (grip or release).
	/// </summary>
	/// <returns>The last left hand event.</returns>
	public HandEventType GetLastLeftHandEvent()
	{
		return lastLeftHandEvent;
	}
	
	/// <summary>
	/// Gets the current normalized viewport position of the left hand, in range [0, 1].
	/// </summary>
	/// <returns>The left hand viewport position.</returns>
	public Vector3 GetLeftHandScreenPos()
	{
		return leftHandScreenPos;
	}
	
	/// <summary>
	/// Determines whether the left hand is primary for the user.
	/// </summary>
	/// <returns><c>true</c> if the left hand is primary for the user; otherwise, <c>false</c>.</returns>
	public bool IsLeftHandPrimary()
	{
		return isLeftHandPrimary;
	}
	
	/// <summary>
	/// Determines whether the left hand is pressing.
	/// </summary>
	/// <returns><c>true</c> if the left hand is pressing; otherwise, <c>false</c>.</returns>
	public bool IsLeftHandPress()
	{
		return isLeftHandPress;
	}
	
	/// <summary>
	/// Determines whether a left hand click is detected, false otherwise.
	/// </summary>
	/// <returns><c>true</c> if a left hand click is detected; otherwise, <c>false</c>.</returns>
	public bool IsLeftHandClickDetected()
	{
		if(isLeftHandClick)
		{
			isLeftHandClick = false;
			leftHandClickProgress = 0f;
			lastLeftHandPos = Vector3.zero;
			lastLeftHandTime = Time.realtimeSinceStartup;
			
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Gets the left hand click progress, in range [0, 1].
	/// </summary>
	/// <returns>The left hand click progress.</returns>
	public float GetLeftHandClickProgress()
	{
		return leftHandClickProgress;
	}
	
	/// <summary>
	/// Gets the current right hand event (none, grip or release).
	/// </summary>
	/// <returns>The current right hand event.</returns>
	public HandEventType GetRightHandEvent()
	{
		return rightHandEvent;
	}
	
	/// <summary>
	/// Gets the last detected right hand event (grip or release).
	/// </summary>
	/// <returns>The last right hand event.</returns>
	public HandEventType GetLastRightHandEvent()
	{
		return lastRightHandEvent;
	}
	
	/// <summary>
	/// Gets the current normalized viewport position of the right hand, in range [0, 1].
	/// </summary>
	/// <returns>The right hand viewport position.</returns>
	public Vector3 GetRightHandScreenPos()
	{
		return rightHandScreenPos;
	}
	
	/// <summary>
	/// Determines whether the right hand is primary for the user.
	/// </summary>
	/// <returns><c>true</c> if the right hand is primary for the user; otherwise, <c>false</c>.</returns>
	public bool IsRightHandPrimary()
	{
		return isRightHandPrimary;
	}
	
	/// <summary>
	/// Determines whether the right hand is pressing.
	/// </summary>
	/// <returns><c>true</c> if the right hand is pressing; otherwise, <c>false</c>.</returns>
	public bool IsRightHandPress()
	{
		return isRightHandPress;
	}
	
	/// <summary>
	/// Determines whether a right hand click is detected, false otherwise.
	/// </summary>
	/// <returns><c>true</c> if a right hand click is detected; otherwise, <c>false</c>.</returns>
	public bool IsRightHandClickDetected()
	{
		if(isRightHandClick)
		{
			isRightHandClick = false;
			rightHandClickProgress = 0f;
			lastRightHandPos = Vector3.zero;
			lastRightHandTime = Time.realtimeSinceStartup;
			
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Gets the right hand click progress, in range [0, 1].
	/// </summary>
	/// <returns>The right hand click progress.</returns>
	public float GetRightHandClickProgress()
	{
		return rightHandClickProgress;
	}
	
	/// <summary>
	/// Gets the current cursor normalized viewport position.
	/// </summary>
	/// <returns>The cursor viewport position.</returns>
	public Vector3 GetCursorPosition()
	{
		return cursorScreenPos;
	}


	//----------------------------------- end of public functions --------------------------------------//


	void Awake()
	{
		instance = this;
	}

	void Start() 
	{
		interactionInited = true;

		// try to automatically detect the available gesture listeners in the scene
		if(interactionListeners.Count == 0)
		{
			MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

			foreach(MonoBehaviour monoScript in monoScripts)
			{
//				if(typeof(InteractionListenerInterface).IsAssignableFrom(monoScript.GetType()) &&
//					monoScript.enabled)
				if((monoScript is InteractionListenerInterface) && monoScript.enabled)
				{
					interactionListeners.Add(monoScript);
				}
			}
		}

		// by default set the main-camera to be screen-camera
		if (screenCamera == null) 
		{
			screenCamera = Camera.main;
		}

		if (screenCamera != null && guiHandCursor != null) 
		{
			Vector3 vCursorToCam = screenCamera.transform.InverseTransformPoint(guiHandCursor.transform.position);
			fCursorToCameraDist = vCursorToCam.z;
		}
	}
	
	void OnDestroy()
	{
		interactionInited = false;
		instance = null;
	}
	
	void Update () 
	{
		KinectManager kinectManager = KinectManager.Instance;
		
		// update Kinect interaction
		if(kinectManager && kinectManager.IsInitialized())
		{
			playerUserID = kinectManager.GetUserIdByIndex(playerIndex);
			
			if(playerUserID != 0)
			{
				lastUserID = playerUserID;
				HandEventType handEvent = HandEventType.None;
				
				// get the left hand state
				leftHandState = kinectManager.GetLeftHandState(playerUserID);
				
				// check if the left hand is interacting
				isleftIboxValid = kinectManager.GetLeftHandInteractionBox(playerUserID, ref leftIboxLeftBotBack, ref leftIboxRightTopFront, isleftIboxValid);
				//bool bLeftHandPrimaryNow = false;

				// was the left hand interacting till now
				bool wasLeftHandInteracting = isLeftHandInteracting;

				if(isleftIboxValid && //bLeftHandPrimaryNow &&
				   kinectManager.GetJointTrackingState(playerUserID, (int)KinectInterop.JointType.HandLeft) != KinectInterop.TrackingState.NotTracked)
				{
					leftHandPos = kinectManager.GetJointPosition(playerUserID, (int)KinectInterop.JointType.HandLeft);

					leftHandScreenPos.x = Mathf.Clamp01((leftHandPos.x - leftIboxLeftBotBack.x) / (leftIboxRightTopFront.x - leftIboxLeftBotBack.x));
					leftHandScreenPos.y = Mathf.Clamp01((leftHandPos.y - leftIboxLeftBotBack.y) / (leftIboxRightTopFront.y - leftIboxLeftBotBack.y));
					leftHandScreenPos.z = Mathf.Clamp01((leftIboxLeftBotBack.z - leftHandPos.z) / (leftIboxLeftBotBack.z - leftIboxRightTopFront.z));

					isLeftHandInteracting = (leftHandPos.x >= (leftIboxLeftBotBack.x - 1.0f)) && (leftHandPos.x <= (leftIboxRightTopFront.x + 0.5f)) &&
						(leftHandPos.y >= (leftIboxLeftBotBack.y - 0.1f)) && (leftHandPos.y <= (leftIboxRightTopFront.y + 0.7f)) &&
						(leftIboxLeftBotBack.z >= leftHandPos.z) && (leftIboxRightTopFront.z * 0.8f <= leftHandPos.z);
					//bLeftHandPrimaryNow = isLeftHandInteracting;

					// start interacting?
					if(!wasLeftHandInteracting && isLeftHandInteracting)
					{
						leftHandInteractingSince = Time.realtimeSinceStartup;
					}

					// check for left press
					isLeftHandPress = ((leftIboxRightTopFront.z - 0.1f) >= leftHandPos.z);
					
					// check for left hand click
					if(allowHandClicks && !dragInProgress && isLeftHandInteracting && 
						((leftHandPos - lastLeftHandPos).magnitude < KinectInterop.Constants.ClickMaxDistance))
					{
						if((Time.realtimeSinceStartup - lastLeftHandTime) >= KinectInterop.Constants.ClickStayDuration)
						{
							if(!isLeftHandClick)
							{
								isLeftHandClick = true;
								leftHandClickProgress = 1f;

								foreach(InteractionListenerInterface listener in interactionListeners)
								{
									if (listener.HandClickDetected (playerUserID, playerIndex, false, leftHandScreenPos)) 
									{
										isLeftHandClick = false;
										leftHandClickProgress = 0f;
										lastLeftHandPos = Vector3.zero;
										lastLeftHandTime = Time.realtimeSinceStartup;
									}
								}

								if(controlMouseCursor)
								{
									MouseControl.MouseClick();

									isLeftHandClick = false;
									leftHandClickProgress = 0f;
									lastLeftHandPos = Vector3.zero;
									lastLeftHandTime = Time.realtimeSinceStartup;
								}
							}
						}
						else
						{
							leftHandClickProgress = (Time.realtimeSinceStartup - lastLeftHandTime) / KinectInterop.Constants.ClickStayDuration;
						}
					}
					else
					{
						isLeftHandClick = false;
						leftHandClickProgress = 0f;
						lastLeftHandPos = leftHandPos;
						lastLeftHandTime = Time.realtimeSinceStartup;
					}
				}
				else
				{
					isLeftHandInteracting = false;
					isLeftHandPress = false;
				}
				
				// get the right hand state
				rightHandState = kinectManager.GetRightHandState(playerUserID);

				// check if the right hand is interacting
				isRightIboxValid = kinectManager.GetRightHandInteractionBox(playerUserID, ref rightIboxLeftBotBack, ref rightIboxRightTopFront, isRightIboxValid);
				//bool bRightHandPrimaryNow = false;

				// was the right hand interacting till now
				bool wasRightHandInteracting = isRightHandInteracting;

				if(isRightIboxValid && //bRightHandPrimaryNow &&
				   kinectManager.GetJointTrackingState(playerUserID, (int)KinectInterop.JointType.HandRight) != KinectInterop.TrackingState.NotTracked)
				{
					rightHandPos = kinectManager.GetJointPosition(playerUserID, (int)KinectInterop.JointType.HandRight);

					rightHandScreenPos.x = Mathf.Clamp01((rightHandPos.x - rightIboxLeftBotBack.x) / (rightIboxRightTopFront.x - rightIboxLeftBotBack.x));
					rightHandScreenPos.y = Mathf.Clamp01((rightHandPos.y - rightIboxLeftBotBack.y) / (rightIboxRightTopFront.y - rightIboxLeftBotBack.y));
					rightHandScreenPos.z = Mathf.Clamp01((rightIboxLeftBotBack.z - rightHandPos.z) / (rightIboxLeftBotBack.z - rightIboxRightTopFront.z));

					isRightHandInteracting = (rightHandPos.x >= (rightIboxLeftBotBack.x - 0.5f)) && (rightHandPos.x <= (rightIboxRightTopFront.x + 1.0f)) &&
						(rightHandPos.y >= (rightIboxLeftBotBack.y - 0.1f)) && (rightHandPos.y <= (rightIboxRightTopFront.y + 0.7f)) &&
						(rightIboxLeftBotBack.z >= rightHandPos.z) && (rightIboxRightTopFront.z * 0.8f <= rightHandPos.z);
					//bRightHandPrimaryNow = isRightHandInteracting;
					
					if(!wasRightHandInteracting && isRightHandInteracting)
					{
						rightHandInteractingSince = Time.realtimeSinceStartup;
					}
					
					// check for right press
					isRightHandPress = ((rightIboxRightTopFront.z - 0.1f) >= rightHandPos.z);
					
					// check for right hand click
					if(allowHandClicks && !dragInProgress && isRightHandInteracting && 
						((rightHandPos - lastRightHandPos).magnitude < KinectInterop.Constants.ClickMaxDistance))
					{
						if((Time.realtimeSinceStartup - lastRightHandTime) >= KinectInterop.Constants.ClickStayDuration)
						{
							if(!isRightHandClick)
							{
								isRightHandClick = true;
								rightHandClickProgress = 1f;
								
								foreach(InteractionListenerInterface listener in interactionListeners)
								{
									if (listener.HandClickDetected (playerUserID, playerIndex, true, rightHandScreenPos)) 
									{
										isRightHandClick = false;
										rightHandClickProgress = 0f;
										lastRightHandPos = Vector3.zero;
										lastRightHandTime = Time.realtimeSinceStartup;
									}
								}

								if(controlMouseCursor)
								{
									MouseControl.MouseClick();

									isRightHandClick = false;
									rightHandClickProgress = 0f;
									lastRightHandPos = Vector3.zero;
									lastRightHandTime = Time.realtimeSinceStartup;
								}
							}
						}
						else
						{
							rightHandClickProgress = (Time.realtimeSinceStartup - lastRightHandTime) / KinectInterop.Constants.ClickStayDuration;
						}
					}
					else
					{
						isRightHandClick = false;
						rightHandClickProgress = 0f;
						lastRightHandPos = rightHandPos;
						lastRightHandTime = Time.realtimeSinceStartup;
					}
				}
				else
				{
					isRightHandInteracting = false;
					isRightHandPress = false;
				}

				// if both hands are interacting, check which one interacts longer than the other
				if(isLeftHandInteracting && isRightHandInteracting)
				{
					if(rightHandInteractingSince <= leftHandInteractingSince)
						isLeftHandInteracting = false;
					else
						isRightHandInteracting = false;
				}

				// if left hand just stopped interacting, send extra non-interaction event
				if (wasLeftHandInteracting && !isLeftHandInteracting) 
				{
					foreach(InteractionListenerInterface listener in interactionListeners)
					{
//						if(lastLeftHandEvent == HandEventType.Grip)
//							listener.HandGripDetected (playerUserID, playerIndex, false, isLeftHandInteracting, leftHandScreenPos);
//						else //if(lastLeftHandEvent == HandEventType.Release)
//							listener.HandReleaseDetected (playerUserID, playerIndex, false, isLeftHandInteracting, leftHandScreenPos);
						if(lastLeftHandEvent == HandEventType.Grip)
							listener.HandReleaseDetected (playerUserID, playerIndex, false, true, leftHandScreenPos);
					}
				}


				// if right hand just stopped interacting, send extra non-interaction event
				if (wasRightHandInteracting && !isRightHandInteracting) 
				{
					foreach(InteractionListenerInterface listener in interactionListeners)
					{
//						if(lastRightHandEvent == HandEventType.Grip)
//							listener.HandGripDetected (playerUserID, playerIndex, true, isRightHandInteracting, rightHandScreenPos);
//						else //if(lastRightHandEvent == HandEventType.Release)
//							listener.HandReleaseDetected (playerUserID, playerIndex, true, isRightHandInteracting, rightHandScreenPos);
						if(lastRightHandEvent == HandEventType.Grip)
							listener.HandReleaseDetected (playerUserID, playerIndex, true, true, rightHandScreenPos);
					}
				}


				// process left hand
				handEvent = HandStateToEvent(leftHandState, lastLeftHandEvent);

				if((isLeftHandInteracting != isLeftHandPrimary) || (isRightHandInteracting != isRightHandPrimary))
				{
					if(controlMouseCursor && dragInProgress)
					{
						MouseControl.MouseRelease();
						dragInProgress = false;
					}
					
					lastLeftHandEvent = HandEventType.Release;
					lastRightHandEvent = HandEventType.Release;
				}
				
				if(controlMouseCursor && (handEvent != lastLeftHandEvent))
				{
					if(controlMouseDrag && !dragInProgress && (handEvent == HandEventType.Grip))
					{
						dragInProgress = true;
						MouseControl.MouseDrag();
					}
					else if(dragInProgress && (handEvent == HandEventType.Release))
					{
						MouseControl.MouseRelease();
						dragInProgress = false;
					}
				}
				
				leftHandEvent = handEvent;
				if(handEvent != HandEventType.None)
				{
					if (leftHandEvent != lastLeftHandEvent) 
					{
						foreach(InteractionListenerInterface listener in interactionListeners)
						{
							if(leftHandEvent == HandEventType.Grip)
								listener.HandGripDetected (playerUserID, playerIndex, false, isLeftHandInteracting, leftHandScreenPos);
							else if(leftHandEvent == HandEventType.Release)
								listener.HandReleaseDetected (playerUserID, playerIndex, false, isLeftHandInteracting, leftHandScreenPos);
						}
					}

					lastLeftHandEvent = handEvent;
				}
				
				// if the hand is primary, set the cursor position
				if(isLeftHandInteracting)
				{
					isLeftHandPrimary = true;

					if((leftHandClickProgress < 0.8f) /**&& !isLeftHandPress*/)
					{
						float smooth = smoothFactor * Time.deltaTime;
						if(smooth == 0f) smooth = 1f;
						
						cursorScreenPos = Vector3.Lerp(cursorScreenPos, leftHandScreenPos, smooth);
					}

					// move mouse-only if there is no cursor texture
					if(controlMouseCursor && 
					   (!guiHandCursor || (!gripHandTexture && !releaseHandTexture && !normalHandTexture)))
					{
						MouseControl.MouseMove(cursorScreenPos, debugText);
					}
				}
				else
				{
					isLeftHandPrimary = false;
				}


				// process right hand
				handEvent = HandStateToEvent(rightHandState, lastRightHandEvent);

				if(controlMouseCursor && (handEvent != lastRightHandEvent))
				{
					if(controlMouseDrag && !dragInProgress && (handEvent == HandEventType.Grip))
					{
						dragInProgress = true;
						MouseControl.MouseDrag();
					}
					else if(dragInProgress && (handEvent == HandEventType.Release))
					{
						MouseControl.MouseRelease();
						dragInProgress = false;
					}
				}
				
				rightHandEvent = handEvent;
				if(handEvent != HandEventType.None)
				{
					if (rightHandEvent != lastRightHandEvent) 
					{
						foreach(InteractionListenerInterface listener in interactionListeners)
						{
							if(rightHandEvent == HandEventType.Grip)
								listener.HandGripDetected (playerUserID, playerIndex, true, isRightHandInteracting, rightHandScreenPos);
							else if(rightHandEvent == HandEventType.Release)
								listener.HandReleaseDetected (playerUserID, playerIndex, true, isRightHandInteracting, rightHandScreenPos);
						}
					}

					lastRightHandEvent = handEvent;
				}	
				
				// if the hand is primary, set the cursor position
				if(isRightHandInteracting)
				{
					isRightHandPrimary = true;

					if((rightHandClickProgress < 0.8f) /**&& !isRightHandPress*/)
					{
						float smooth = smoothFactor * Time.deltaTime;
						if(smooth == 0f) smooth = 1f;
						
						cursorScreenPos = Vector3.Lerp(cursorScreenPos, rightHandScreenPos, smooth);
					}

					// move mouse-only if there is no cursor texture
					if(controlMouseCursor && 
					   (!guiHandCursor || (!gripHandTexture && !releaseHandTexture && !normalHandTexture)))
					{
						MouseControl.MouseMove(cursorScreenPos, debugText);
					}
				}
				else
				{
					isRightHandPrimary = false;
				}

			}
			else
			{
				// send release events
				if (lastLeftHandEvent == HandEventType.Grip || lastRightHandEvent == HandEventType.Grip) 
				{
					foreach(InteractionListenerInterface listener in interactionListeners)
					{
						if(lastLeftHandEvent == HandEventType.Grip)
							listener.HandReleaseDetected (lastUserID, playerIndex, false, true, leftHandScreenPos);
						if(lastRightHandEvent == HandEventType.Grip)
							listener.HandReleaseDetected (lastUserID, playerIndex, true, true, leftHandScreenPos);
					}
				}

				leftHandState = KinectInterop.HandState.NotTracked;
				rightHandState = KinectInterop.HandState.NotTracked;
				
				isLeftHandPrimary = isRightHandPrimary = false;
				isLeftHandInteracting = isRightHandInteracting = false;
				leftHandInteractingSince = rightHandInteractingSince = 0f;

				isLeftHandClick = isRightHandClick = false;
				leftHandClickProgress = rightHandClickProgress = 0f;
				lastLeftHandTime = lastRightHandTime = Time.realtimeSinceStartup;

				isLeftHandPress = false;
				isRightHandPress = false;
				
				leftHandEvent = HandEventType.None;
				rightHandEvent = HandEventType.None;
				
				lastLeftHandEvent = HandEventType.Release;
				lastRightHandEvent = HandEventType.Release;

				if(controlMouseCursor && dragInProgress)
				{
					MouseControl.MouseRelease();
					dragInProgress = false;
				}
			}
		}
		
	}

	// converts hand state to hand event type
	public static HandEventType HandStateToEvent(KinectInterop.HandState handState, HandEventType lastEventType)
	{
		switch(handState)
		{
			case KinectInterop.HandState.Open:
				return HandEventType.Release;

			case KinectInterop.HandState.Closed:
			case KinectInterop.HandState.Lasso:
				return HandEventType.Grip;
			
			case KinectInterop.HandState.Unknown:
				return lastEventType;
		}

		return HandEventType.None;
	}
	

	void OnGUI()
	{
		if(!interactionInited)
			return;
		
		// display debug information
		if(debugText)
		{
			string sGuiText = string.Empty;

			//if(isLeftHandPrimary)
			{
				sGuiText += "LCursor" + (isLeftHandInteracting ? "*: " : " : ") + leftHandScreenPos.ToString();
				
				if(lastLeftHandEvent == HandEventType.Grip)
				{
					sGuiText += "  LeftGrip";
				}
				else if(lastLeftHandEvent == HandEventType.Release)
				{
					sGuiText += "  LeftRelease";
				}
				
				if(isLeftHandClick)
				{
					sGuiText += "  LeftClick";
				}
//				else if(leftHandClickProgress > 0.5f)
//				{
//					sGuiText += String.Format("  {0:F0}%", leftHandClickProgress * 100);
//				}
				
				if(isLeftHandPress)
				{
					sGuiText += "  LeftPress";
				}
			}
			
			//if(isRightHandPrimary)
			{
				sGuiText += "\nRCursor" + (isRightHandInteracting ? "*: " : " : ") + rightHandScreenPos.ToString();
				
				if(lastRightHandEvent == HandEventType.Grip)
				{
					sGuiText += "  RightGrip";
				}
				else if(lastRightHandEvent == HandEventType.Release)
				{
					sGuiText += "  RightRelease";
				}
				
				if(isRightHandClick)
				{
					sGuiText += "  RightClick";
				}
//				else if(rightHandClickProgress > 0.5f)
//				{
//					sGuiText += String.Format("  {0:F0}%", rightHandClickProgress * 100);
//				}

				if(isRightHandPress)
				{
					sGuiText += "  RightPress";
				}
			}
			
			debugText.text = sGuiText;
		}
		
		// display the cursor status and position
		if(guiHandCursor)
		{
			Sprite texture = null;
			
			if(isLeftHandPrimary)
			{
				if(lastLeftHandEvent == HandEventType.Grip)
					texture = gripHandTexture;
				else if(lastLeftHandEvent == HandEventType.Release)
					texture = releaseHandTexture;
			}
			else if(isRightHandPrimary)
			{
				if(lastRightHandEvent == HandEventType.Grip)
					texture = gripHandTexture;
				else if(lastRightHandEvent == HandEventType.Release)
					texture = releaseHandTexture;
			}
			
			if(texture == null)
			{
				texture = normalHandTexture;
			}
			
			//if(useHandCursor)
			{
				if((texture != null) && (isLeftHandPrimary || isRightHandPrimary))
				{
//					Rect rectTexture; 
//
//					if(controlMouseCursor)
//					{
//						MouseControl.MouseMove(cursorScreenPos, debugText);
//						rectTexture = new Rect(Input.mousePosition.x - texture.width / 2, Screen.height - Input.mousePosition.y - texture.height / 2, 
//						                       texture.width, texture.height);
//					}
//					else 
//					{
//						rectTexture = new Rect(cursorScreenPos.x * Screen.width - texture.width / 2, (1f - cursorScreenPos.y) * Screen.height - texture.height / 2, 
//						                       texture.width, texture.height);
//					}
//
//					GUI.DrawTexture(rectTexture, texture);

					if (guiHandCursor.sprite != texture) 
					{
						guiHandCursor.sprite = texture;
					}

					if (screenCamera) 
					{
						Vector3 vCursorPos = new Vector3 (cursorScreenPos.x, cursorScreenPos.y, fCursorToCameraDist);
						guiHandCursor.rectTransform.position = screenCamera.ViewportToWorldPoint(vCursorPos);
					}
				}
			}
		}
	}

}
