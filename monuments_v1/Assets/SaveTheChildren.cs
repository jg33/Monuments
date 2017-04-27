using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTheChildren : MonoBehaviour {

	public int id;
	public GameObject meshCopyPrefab;
	public bool loadOnStart;
	public float postScale;
	public Vector3 postLocation;

	private GameObject player;


	// Use this for initialization
	void Start () {
		player = GameObject.Find("New Player Avatar");


		if(loadOnStart){
			populateAndLoad(id);
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetKey("s")){
//			saveAll(id);
//		} else if (Input.GetKey("l")){
//			populateAndLoad(id);
//		}
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
		transform.localScale = new Vector3(postScale,postScale,postScale);
		transform.position = postLocation;
	}

	void killKids(){
		GameObject dummy = new GameObject();
		for (int i=0;i<transform.childCount;i++){
			transform.GetChild(i).SetParent(dummy.transform);
		}
		GameObject.Destroy(dummy);

	}

	public void setCue (int _cue){
		switch(_cue){
		case 0:
			killKids();
			populateAndLoad(id);
			break;

		default: 
			break;


		}

	}

    public void setIndex(int _i)
    {
        id = _i;

    }
}
