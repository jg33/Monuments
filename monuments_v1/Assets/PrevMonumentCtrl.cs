using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevMonumentCtrl : MonoBehaviour {

    public int latestIndex = 0;
	// Use this for initialization
	void Start () {
		
	}
	

    public void setCue(int _cue){
        if (_cue == 0){
            int currentIndex = latestIndex;
            foreach(SaveTheChildren _child in gameObject.GetComponentsInChildren<SaveTheChildren>()) {
                if (currentIndex < 0) currentIndex = 0;
                _child.setIndex(currentIndex);
                _child.reset();
                currentIndex--;
            }


        }
    }

    public void setLatestIndex(int _i)
    {
        latestIndex = _i;

    }
}
