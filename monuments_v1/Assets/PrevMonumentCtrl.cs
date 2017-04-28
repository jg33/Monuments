using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevMonumentCtrl : MonoBehaviour {

    public int latestIndex = 0;

    public int numDuplicates;
	// Use this for initialization
	void Start () {
		
	}
	

    public void setCue(int _cue){
        if (_cue == 0){
            // Load Monuments from disk //
            int currentIndex = latestIndex;
            foreach(SaveTheChildren _child in gameObject.GetComponentsInChildren<SaveTheChildren>()) {
                if (currentIndex < 0) currentIndex = 0;
                _child.setIndex(currentIndex);
                _child.reset();
                currentIndex--;
            }
            // Duplicate and instantiate //
            for(int i = 0; i < numDuplicates; i++)
            {
                GameObject newMon = GameObject.Instantiate(transform.GetChild((int)Random.Range(0,transform.childCount-1)).gameObject, GameObject.Find("Environment").transform );
                newMon.transform.position = new Vector3(Random.Range(-50f, 50f), Random.Range(0f, 10f), Random.Range(-50f, 50f));
                newMon.transform.position *= Random.Range(5f, 20f);
                newMon.transform.localScale *= Random.Range(1f, 5f);

            }
        }
    }

    public void setLatestIndex(int _i)
    {
        latestIndex = _i;

    }
}
