using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heightFade : MonoBehaviour {

	float userHeight;
	GameObject user;
	public float planeAlpha;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		user = GameObject.Find("FPSController");
		userHeight = user.transform.position.y;
		if (userHeight > 10) {
			StartCoroutine(FadeTo(0.0f, 100.0f));
//			Debug.Log (userHeight);
//			planeAlpha = 1.0f - userHeight + 10f;
//			GetComponent<Renderer> ().material.color = new Vector4(1f,1f,1f,planeAlpha);
		}
	}

IEnumerator FadeTo(float aValue, float aTime)
{
		planeAlpha = GetComponent<Renderer> ().material.color.a;
	for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
	{
		Color newColor = new Color(0, 0, 0, Mathf.Lerp(planeAlpha,aValue,t));
		GetComponent<Renderer> ().material.color = newColor;
		yield return null;
	}
}
}