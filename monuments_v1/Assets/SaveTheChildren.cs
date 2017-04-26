using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTheChildren : MonoBehaviour {

	public int id;
	public GameObject meshCopyPrefab;
	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.Find("New Player Avatar");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("s")){
			saveAll(id);
		} else if (Input.GetKey("l")){
			populateAndLoad(id);
		}
	}

	void saveAll(int _id){
		for (int i =0; i< gameObject.GetComponentsInChildren<MeshSaveLoad>().Length;i++ ){
			MeshSaveLoad _childSaver = gameObject.GetComponentsInChildren<MeshSaveLoad>()[i];
			_childSaver.save(_id);
		}
	}

	void populateAndLoad(int _id){
		if(player){
			for(int i =0; i< player.transform.GetChildCount();i++){
				GameObject thisOriginal = player.transform.GetChild(i).gameObject;
				if (thisOriginal.name != "U" && !thisOriginal.name.Contains("Eye")){
					GameObject thisCopy = GameObject.Instantiate(meshCopyPrefab);
					thisCopy.transform.SetParent(transform);
					thisCopy.name = thisOriginal.name;
					MeshSaveLoad loader = thisCopy.GetComponent<MeshSaveLoad>();
					loader.load(_id);
					Debug.Log(thisOriginal.name);
				}
			}


		}


	}
}
