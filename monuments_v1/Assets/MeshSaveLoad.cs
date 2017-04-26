using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSaveLoad : MonoBehaviour {

	private Mesh thisMesh;
	private Mesh originalMesh;
	public GameObject player;
	//private ObjImporter objImp;
	// Use this for initialization
	void Start () {
		//objImp = new ObjImporter();
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}

	public void save(int id){
		ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "Assets/SavedMeshes/"+id.ToString()+"_"+gameObject.name+".obj" );
	}

	public void load(int id){
		player = GameObject.Find("New Player Avatar");

		Debug.Log(player.name);
		originalMesh = player.transform.Find(gameObject.name).GetComponent<CopyOnTouch>().thisMesh; 

		Debug.Log("Loading "+"Assets/SavedMeshes/"+id.ToString()+"_"+gameObject.name);
		GameObject newObj = OBJLoader.LoadOBJFile("Assets/SavedMeshes/"+id.ToString()+"_"+gameObject.name+"_copy.obj");
		GetComponent<MeshFilter>().mesh = newObj.GetComponentInChildren<MeshFilter>().mesh;
		GameObject.Destroy(newObj);
		//GetComponent<MeshFilter>().mesh = ObjImporter.ImportFileWithHelp("Assets/SavedMeshes/"+id.ToString()+"_"+gameObject.name+"_copy.obj",originalMesh);
		Debug.Log("Loaded "+gameObject.name);
	}
}
