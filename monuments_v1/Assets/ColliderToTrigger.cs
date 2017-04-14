using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderToTrigger : MonoBehaviour {
    public GameObject meshToTrigger;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider _c)
    {
        if(_c.name == "Touch") meshToTrigger.SendMessage("CopySelf");

    }
}
