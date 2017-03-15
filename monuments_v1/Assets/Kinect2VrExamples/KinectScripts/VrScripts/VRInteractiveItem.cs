using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // This class should be added to any gameobject in the scene
    // that should react to input based on the user's gaze.
    // It contains events that can be subscribed to by classes that
    // need to know about input specifics to this gameobject.
    public class VRInteractiveItem : MonoBehaviour
    {
		public event Action<Transform> OnOver;             // Called when the gaze moves over this object
        public event Action<Transform> OnOut;              // Called when the gaze leaves this object
		public event Action<Transform> OnClick;            // Called when click input is detected whilst the gaze is over this object.
		public event Action<Transform> OnDoubleClick;      // Called when double click input is detected whilst the gaze is over this object.
        public event Action OnUp;               // Called when Fire1 is released whilst the gaze is over this object.
        public event Action OnDown;             // Called when Fire1 is pressed whilst the gaze is over this object.


        protected bool m_IsOver;


		void OnDisable()
		{
			// remove all event listeners
			OnOver = null;
			OnOut = null;
			OnClick = null;
			OnDoubleClick = null;
			OnUp = null;
			OnDown = null;
		}


        public bool IsOver
        {
            get { return m_IsOver; }              // Is the gaze currently over this object?
        }


        // The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // They in turn call the appropriate events should they have subscribers.
        public void Over()
        {
            m_IsOver = true;

            if (OnOver != null)
				OnOver(transform);
        }


        public void Out()
        {
            m_IsOver = false;

            if (OnOut != null)
				OnOut(transform);
        }


        public void Click()
        {
            if (OnClick != null)
				OnClick(transform);
        }


        public void DoubleClick()
        {
            if (OnDoubleClick != null)
				OnDoubleClick(transform);
        }


        public void Up()
        {
            if (OnUp != null)
                OnUp();
        }


        public void Down()
        {
            if (OnDown != null)
                OnDown();
        }
    }
}