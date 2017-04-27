using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueToAnimParam : MonoBehaviour {

	public void setCue(int _cue){

		GetComponent<Animator>().SetInteger("cueNum",_cue);
	}

}
