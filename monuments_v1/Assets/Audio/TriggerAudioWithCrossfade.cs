using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioWithCrossfade : MonoBehaviour {


	public AudioSource source1;
	public AudioSource source2;
	public AudioClip[] clips;

	public int currentSourceNum = 1;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void setCue(int _i){
		AudioSource currentSource =  new AudioSource();;
		AudioSource targetSource = new AudioSource();;
		if(clips[_i]){
			switch(currentSourceNum){
				case 1:
					currentSource = source2;
					targetSource = source1;
					gameObject.GetComponent<Animator>().SetInteger("source",currentSourceNum);
					currentSourceNum = 2;
					break;

				case 2:
					currentSource = source1;
					targetSource = source2;
					gameObject.GetComponent<Animator>().SetInteger("source",currentSourceNum);
					currentSourceNum = 1;
					break;

				default:
					Debug.LogAssertion("Audio Source error");
					break;

			}


			targetSource.clip= clips[_i];
			targetSource.Play();




		}


	}
}

