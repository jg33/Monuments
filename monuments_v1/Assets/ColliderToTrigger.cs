using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderToTrigger : MonoBehaviour {
    public GameObject meshToTrigger;
    public float delay=0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider _c) {
        if (_c.name == "Touch"){
            if (delay > 0) {
                StartCoroutine(startWithDelay(delay));
            }
            else {
                meshToTrigger.SendMessage("CopySelf");
            }
        }
    }

    IEnumerator startWithDelay(float _secs)
    {
        yield return new WaitForSeconds(_secs);
        meshToTrigger.SendMessage("CopySelf");

    }
}
