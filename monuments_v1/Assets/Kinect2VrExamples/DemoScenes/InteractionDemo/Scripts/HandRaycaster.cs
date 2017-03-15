using System;
using UnityEngine;
using VRStandardAssets.Utils;

public class HandRaycaster : MonoBehaviour
{
    public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user is raycasting on an object.

	[Tooltip("Special raycast source. If not set, avatar hands will be used.")]
	public Transform raycastSource;

	[Tooltip("Layers to exclude from the raycast.")]
    public LayerMask m_ExclusionLayers;

	[Tooltip("The reticle, if applicable.")]
    public Reticle m_Reticle;

//    public bool m_ShowDebugRay;                   // Optionally show the debug ray.
//    public float m_DebugRayLength = 5f;           // Debug ray length.
//    public float m_DebugRayDuration = 0f;         // How long the Debug ray will remain visible.

	[Tooltip("How far into the scene the ray is cast.")]
    public float m_RayLength = 100f;

	[Tooltip("Whether to require closed hand for shooting. Otherwise just point your arm to shoot.")]
	public bool makeFistToShoot = true;

	[Tooltip("Laser renderer prefab - for shooting, if any.")]
	public LineRenderer laserPrefab;


	[Tooltip("GUI-Text to display information messages.")]
	public TextMesh infoText;

    
    private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
    private VRInteractiveItem m_LastInteractible;                   //The last interactive item

	private HandInteractionListener intetactionListener;
	private bool isLeftHandInteracting;
	private bool isRightHandInteracting;

	//private GameObject player;
	private AvatarController avatarController;

	private Transform leftElbowTrans;
	private Transform leftWristTrans;
	private Transform rightElbowTrans;
	private Transform rightWristTrans;

	private Vector3 raycastPos;
	private Vector3 raycastDir;
	private RaycastHit raycastHit;
	private bool isShooting;

	private LineRenderer laser;


    // Utility for other classes to get the current interactive item
    public VRInteractiveItem CurrentInteractible
    {
        get { return m_CurrentInteractible; }
    }

    
	void Start()
	{
		//player = GameObject.FindGameObjectWithTag("Player");

		//if(player)
		{
			//avatarController = player.GetComponent<AvatarController>();
			avatarController = GetComponent<AvatarController>();

			if(avatarController)
			{
				leftElbowTrans = avatarController.GetBoneTransform(avatarController.GetBoneIndexByJoint(KinectInterop.JointType.ElbowLeft, false));
				leftWristTrans = avatarController.GetBoneTransform(avatarController.GetBoneIndexByJoint(KinectInterop.JointType.WristLeft, false));
				rightElbowTrans = avatarController.GetBoneTransform(avatarController.GetBoneIndexByJoint(KinectInterop.JointType.ElbowRight, false));
				rightWristTrans = avatarController.GetBoneTransform(avatarController.GetBoneIndexByJoint(KinectInterop.JointType.WristRight, false));
			}
		}

		// find the interaction listener
		intetactionListener = GetComponent<HandInteractionListener>();
	}


    void Update()
    {
		HandRaycast();
    }

  
	private void HandRaycast()
    {
		// get raycast position and direction
		isLeftHandInteracting = (intetactionListener && leftWristTrans) ? intetactionListener.IsLeftHandInteracting() : false;
		isRightHandInteracting = (intetactionListener && rightWristTrans) ? intetactionListener.IsRightHandInteracting() : false;
		bool isInteracting = isLeftHandInteracting || isRightHandInteracting;

		isShooting = isLeftHandInteracting ? (makeFistToShoot ? (intetactionListener.GetLeftHandEvent() == InteractionManager.HandEventType.Grip) : !makeFistToShoot) :
			isRightHandInteracting ? (makeFistToShoot ? (intetactionListener.GetRightHandEvent() == InteractionManager.HandEventType.Grip) : !makeFistToShoot) : false;

		if (raycastSource) 
		{
			raycastPos = raycastSource.position;
			raycastDir = raycastSource.forward;
			isInteracting = true;  // eye interaction - always true

			if (!makeFistToShoot) 
			{
				isShooting = true;
			}
		} 
		else if (isInteracting)
		{
			// left or right arm
			raycastPos = isLeftHandInteracting ? leftWristTrans.position : rightWristTrans.position;
			raycastDir = isLeftHandInteracting ? (leftWristTrans.position - leftElbowTrans.position).normalized : 
				(rightWristTrans.position - rightElbowTrans.position).normalized;
		}


//        // Show the debug ray if required
//        if (m_ShowDebugRay)
//        {
//			Debug.DrawRay(raycastPos, raycastDir * m_DebugRayLength, Color.blue, m_DebugRayDuration);
//        }

		if(infoText)
		{
			KinectDataClient dataClient = KinectDataClient.Instance;
			bool dataClientConnected = dataClient ? dataClient.IsConnected : false;

			if(dataClientConnected && intetactionListener)
			{
				if(isShooting)
				{
					if (raycastSource && !makeFistToShoot) 
					{
						string sMessage = "Eyes shooting";
						infoText.text = sMessage;
					}
					else if (isLeftHandInteracting || isRightHandInteracting) 
					{
						string sMessage = (isLeftHandInteracting ? "Left" : "Right") + " hand shooting";
						infoText.text = sMessage;
					}
				}
				else if(isLeftHandInteracting || isRightHandInteracting)
				{
					string sMessage = (isLeftHandInteracting ? "Left" : "Right") + (!raycastSource ? " hand pointing" : " hand ready");
					infoText.text = sMessage;
				}
				else 
				{
					string sMessage = !raycastSource ? "Point at snowflakes with your hand" : "Look at the snowflakes";
					sMessage += (makeFistToShoot ? "\nClose your hand to shoot them" : "\nto shoot them");
					infoText.text = sMessage;
				}
			}
		}

        // Create a ray that points forwards from the camera.
		bool raySuccess = false;

		if (isInteracting) 
		{
			Ray ray = new Ray(raycastPos, raycastDir);
			//raySuccess = Physics.Raycast (ray, out raycastHit, m_RayLength, ~m_ExclusionLayers);
			raySuccess = Physics.SphereCast (ray, 0.5f, out raycastHit, m_RayLength, ~m_ExclusionLayers);
		}
        
        // Do the raycast forweards to see if we hit an interactive item
		if (raySuccess)
        {
			VRInteractiveItem interactible = raycastHit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
            m_CurrentInteractible = interactible;

            // If we hit an interactive item and it's not the same as the last interactive item, then call Over
            if (interactible && interactible != m_LastInteractible)
			{
                interactible.Over();
			}

            // Deactive the last interactive item 
            if (interactible != m_LastInteractible)
			{
                DeactiveLastInteractible();

				if(interactible && isShooting)
				{
					// instantiate the laser beam
					if (laserPrefab) 
					{
						laser = Instantiate(laserPrefab) as LineRenderer;
						laser.transform.parent = transform;

						Vector3 laserStartPos = isLeftHandInteracting ? leftWristTrans.position : rightWristTrans.position;
						if (raycastSource && !makeFistToShoot)
							laserStartPos = raycastPos;

						laser.SetPosition(0, laserStartPos);
						laser.SetPosition(1, raycastHit.point);
					}

					interactible.Click();
				}
			}

            m_LastInteractible = interactible;

            // Something was hit, set at the hit position.
            if (m_Reticle)
			{
				m_Reticle.SetPosition(raycastHit);
			}

            if (OnRaycasthit != null)
			{
				OnRaycasthit(raycastHit);
			}
        }
        else
        {
            // Nothing was hit, deactive the last interactive item.
            DeactiveLastInteractible();
            m_CurrentInteractible = null;

            // Position the reticle at default distance.
            if (m_Reticle)
			{
				m_Reticle.SetPosition(raycastPos, raycastDir);
			}
        }
    }


    private void DeactiveLastInteractible()
    {
		if (laser) 
		{
			// destroy the laser beam with 0.5s delay
			Destroy(laser.gameObject, 0.5f);
			laser = null;
		}

		if (m_LastInteractible) 
		{
			m_LastInteractible.Out();
			m_LastInteractible = null;
		}
    }

}

