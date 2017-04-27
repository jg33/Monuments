using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour {

	public int cueNum = 0;
	private int prevCue = -1;
	public GameObject[] watchedObjects;

	public int currentMonumentIndex;

	// Use this for initialization
	void Start () {
		currentMonumentIndex = PlayerPrefs.GetInt("currentMonumentIndex",0);
        GameObject.Find("CopyContainer").SendMessage("setIndex", currentMonumentIndex);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)){
			cueNum++;
		} else if (Input.GetKeyDown("z")){
			cueNum--;
		} else if (Input.GetKeyDown("s")){
			// save current model //
			GameObject.Find("CopyContainer").SendMessage("saveAll", currentMonumentIndex);
			currentMonumentIndex++;
			PlayerPrefs.SetInt("currentMonumentIndex",currentMonumentIndex);
			PlayerPrefs.Save();
		} else if (Input.GetKeyDown("l")){
			// load? //
		} else if (Input.GetKeyDown("0")){
			// PRESHOW, RESET, LOAD MODELS //
			cueNum = 0;
		} else if (Input.GetKeyDown("1")){
			// ENTER, FLY IN //
			cueNum = 1;
		} else if (Input.GetKeyDown("2")){
			// FADE IN BODY //
			cueNum = 2;
		} else if (Input.GetKeyDown("3")){
			// HOST APPEARS //
			cueNum = 3;
		} else if (Input.GetKeyDown("4")){
			// CONTACT, DISSOLVE BODY //
			cueNum = 4;
		} else if (Input.GetKeyDown("5")){
			// DEPARTURE //
			cueNum = 5;
		} else if (Input.GetKeyDown("6")){
			// save
			cueNum = 6;
		}


		if (cueNum != prevCue){
			foreach(GameObject _g in watchedObjects){
//				if (_g.GetComponent<Animator>()){
//					_g.GetComponent<Animator>().SetInteger("cueNum",cueNum);
//				} else {
					_g.SendMessage("setCue",cueNum);
//				}
				prevCue = cueNum;
			}




		}
		
	}
}
