using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class offsetTex : MonoBehaviour 
{
	public float scrollSpeed = 0.1F;
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		float offset = Time.time * scrollSpeed;
		rend.material.SetTextureOffset("_MainTex", new Vector2(0, -offset));
		rend.material.SetTextureOffset("_AlphaTex", new Vector2(offset/5,0));
	}
}
	