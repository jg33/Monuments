using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour {

	public int cueNum;
	private int prevCue;
	public GameObject[] watchedObjects;

	public int currentMonumentIndex;

	// Use this for initialization
	void Start () {
		currentMonumentIndex = PlayerPrefs.GetInt("currentMonumentIndex",0);
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)){
			cueNum++;
		} else if (Input.GetKeyDown("z")){
			cueNum--;
		} else if (Input.GetKeyDown("s")){
			GameObject.Find("CopyContainer").SendMessage("saveAll", currentMonumentIndex);
			currentMonumentIndex++;
			PlayerPrefs.SetInt("currentMonumentIndex",currentMonumentIndex);
			PlayerPrefs.Save();
		} else if (Input.GetKeyDown("")){

		}


		if (cueNum != prevCue){
			foreach(GameObject _g in watchedObjects){
				if (_g.GetComponent<Animator>()){
					_g.GetComponent<Animator>().SetInteger("cueNum",cueNum);
				} else {
					_g.SendMessage("setCue",cueNum);
				}
				prevCue = cueNum;
			}
		}
		
	}
}
