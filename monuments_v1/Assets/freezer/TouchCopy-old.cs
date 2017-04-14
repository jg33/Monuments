using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCopy : MonoBehaviour {

	public CopyMesh copyMesh;
	public GameObject targetMesh;
	public float effectDistance;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider _c){
	//	Debug.Log("Collided");
		if(_c.gameObject == targetMesh){
			StartCoroutine("CheckVertDist");
		}

	}

	void OnTriggerStay(Collider _c){
		if(_c.gameObject == targetMesh){
			StartCoroutine("CheckVertDist");
		}

	}

	IEnumerator CheckVertDist(){

			Mesh _targetMesh = targetMesh.GetComponent<MeshFilter>().mesh;

			for (int i=0;i<_targetMesh.vertexCount;i++){
				if(Vector3.Distance(_targetMesh.vertices[i], transform.position)<effectDistance){
					copyMesh.CopyVert(i);
				}

			}
		yield return null;

	}

	void OnTriggerExit(Collider _c){


	}



}
