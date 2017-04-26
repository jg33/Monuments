﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class AnimateSky : MonoBehaviour
{  
	//An array of Objects that stores the results of the Resources.LoadAll() method  
	private Object[] objects;  
	//Each returned object is converted to a Texture and stored in this array  
	private Texture[] textures;  
	//With this Material object, a reference to the game object Material can be stored  
	private Material goMaterial;  
	//An integer to advance frames  
	public int frameCounter = 0;   
	public Color glowColor;

	void Awake()  
	{  
		//Get a reference to the Material of the game object this script is attached to  
		this.goMaterial = gameObject.GetComponent<Renderer>().material;  
	}  

	void Start ()  
	{  
		goMaterial = gameObject.GetComponent<Renderer> ().material;
		goMaterial.EnableKeyword("_EmissionColor");
		goMaterial.SetColor("_EmissionColor",Color.black);

		//Load all textures found on the Sequence folder, that is placed inside the resources folder  
		this.objects = Resources.LoadAll("MoBSkyNew", typeof(Texture));  

		//Initialize the array of textures with the same size as the objects array  
		this.textures = new Texture[objects.Length];  

		//Cast each Object to Texture and store the result inside the Textures array  
		for(int i=0; i < objects.Length;i++)  
		{  
			this.textures[i] = (Texture)this.objects[i];  
		}  
		goMaterial.mainTexture = textures[0];  
	}  
	void Update (){
		
		//Call the 'PlayLoop' method as a coroutine with a 0.04 delay  
		StartCoroutine("PlayLoop",0.04f);  
		//Set the material's texture to the current value of the frameCounter variable  
		goMaterial.mainTexture = textures[frameCounter];

	}


	void OnTriggerEnter (Collider other)  
	{  
		if (other.gameObject.tag == "Player") {
			goMaterial = gameObject.GetComponent<Renderer> ().material;
			goMaterial.EnableKeyword ("_EmissionColor");
			goMaterial.SetColor ("_EmissionColor", glowColor);
		}

	}  
	  
	void OnTriggerExit (Collider other)  
	{ 
		if (other.gameObject.tag == "Player") {
			goMaterial = gameObject.GetComponent<Renderer> ().material;
			goMaterial.EnableKeyword ("_EmissionColor");
			goMaterial.SetColor ("_EmissionColor", Color.black);
		}
	}  
	//The following methods return a IEnumerator so they can be yielded:  
	//A method to play the animation in a loop  
	IEnumerator PlayLoop(float delay)  
	{  
		//Wait for the time defined at the delay parameter  
		yield return new WaitForSeconds(delay);    

		//Advance one frame  
		frameCounter = (++frameCounter)%textures.Length;  

		//Stop this coroutine  
		StopCoroutine("PlayLoop");  
	}    

	//A method to play the animation just once  
	IEnumerator Play(float delay)  
	{  
		//Wait for the time defined at the delay parameter  
		yield return new WaitForSeconds(delay);    

		//If the frame counter isn't at the last frame  
		if(frameCounter < textures.Length-1)  
		{  
			//Advance one frame  
			++frameCounter;  
		}  

		//Stop this coroutine  
		StopCoroutine("PlayLoop");  
	}   

}  