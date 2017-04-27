using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour {

	public AudioClip[] clips;
//	// Use this for initialization
//	void Start () {
//		
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	public void setCue(int _i){

		if(clips[_i]){

			gameObject.GetComponent<AudioSource>().PlayOneShot(clips[_i]);

		}




	}
}
