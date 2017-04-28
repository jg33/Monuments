using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMov : MonoBehaviour {

	float userHeight;
	GameObject user;

		public MovieTexture movTexture;

	void Start(){
		GetComponent<Renderer> ().material.mainTexture = movTexture;
		movTexture.Pause ();
	}
		void Update() {
		user = GameObject.Find("FPSController");
		userHeight = user.transform.position.y;
		if (userHeight > 10) {
			GetComponent<Renderer> ().material.mainTexture = movTexture;
			movTexture.Play ();
		}
		}
	}