using UnityEngine;
using System.Collections;

public class HandInteractionListener : MonoBehaviour, InteractionListenerInterface
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	// current left and right hand interaction
	private bool isLeftHandInteracting = false;
	private bool isRightHandInteracting = false;

	private InteractionManager.HandEventType leftHandEvent = InteractionManager.HandEventType.Release;
	private InteractionManager.HandEventType rightHandEvent = InteractionManager.HandEventType.Release;


	/// <summary>
	/// Determines whether the left hand is interacting.
	/// </summary>
	/// <returns><c>true</c> if this the left hand is interacting; otherwise, <c>false</c>.</returns>
	public bool IsLeftHandInteracting()
	{
		return isLeftHandInteracting;
	}

	/// <summary>
	/// Determines whether the right hand is interacting.
	/// </summary>
	/// <returns><c>true</c> if this the right hand is interacting; otherwise, <c>false</c>.</returns>
	public bool IsRightHandInteracting()
	{
		return isRightHandInteracting;
	}

	/// <summary>
	/// Gets the left hand event.
	/// </summary>
	/// <returns>The left hand event.</returns>
	public InteractionManager.HandEventType GetLeftHandEvent()
	{
		return leftHandEvent;
	}

	/// <summary>
	/// Gets the right hand event.
	/// </summary>
	/// <returns>The right hand event.</returns>
	public InteractionManager.HandEventType GetRightHandEvent()
	{
		return rightHandEvent;
	}


	public void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
	{
		if (userIndex != playerIndex)
			return;

		if (!isRightHand) 
		{
			isLeftHandInteracting = isHandInteracting;
			leftHandEvent = InteractionManager.HandEventType.Grip;
		} 
		else 
		{
			isRightHandInteracting = isHandInteracting;
			rightHandEvent = InteractionManager.HandEventType.Grip;
		}
	}

	public void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
	{
		if (userIndex != playerIndex)
			return;

		if (!isRightHand) 
		{
			isLeftHandInteracting = isHandInteracting;
			leftHandEvent = InteractionManager.HandEventType.Release;
		} 
		else 
		{
			isRightHandInteracting = isHandInteracting;
			rightHandEvent = InteractionManager.HandEventType.Release;
		}
	}

	public bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
	{
		if (userIndex != playerIndex)
			return false;

		return true;
	}

}
