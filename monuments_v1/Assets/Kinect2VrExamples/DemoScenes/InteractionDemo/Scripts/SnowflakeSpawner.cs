using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

public class SnowflakeSpawner : MonoBehaviour
{

	public GameObject [] snowPrefabs;

	private float nextSnowTime = 0.0f;
	public float spawnRate = 0.3f;
	public float rangeX = 20f;
	public float rangeZ = 20f;
	public float height = 30f;
	public float _gravity = -1f;
	public float _lifetime = 7f;

	public ParticleSystem explosion;              // Explosion, played when snowflake click is detected

	private int falloutIndex;
	private GameObject snowflake;


//	void Start()
//	{
//		rangeX = 20f;
//		rangeZ = 20f;
//		height = 30f;
//		_gravity = -1;
//	}


	void Update ()
	{
		Physics.gravity = new Vector3 (0, _gravity, 0);

		if (nextSnowTime < Time.time) 
		{
			SpawnSnowflake ();
			nextSnowTime = Time.time + spawnRate;
			spawnRate = Mathf.Clamp(spawnRate, 0.1f, 999f);
		}
	}


	void SpawnSnowflake()
	{
		// Range of spawm x and z	
		float addXPos = Random.Range (-rangeX, rangeX);
		float addZPos = Random.Range (-rangeZ, rangeZ);

		// Scale and position of snowflake
		float _scale =  Random.Range (0.2f,0.8f);
		Vector3 spawnPos = new Vector3 (addXPos, height,  addZPos);

		falloutIndex  = Random.Range(0, snowPrefabs.Length);
		snowflake =  (GameObject)Instantiate( snowPrefabs[falloutIndex], spawnPos, Quaternion.AngleAxis(90, new Vector3(1,0,0)));

		snowflake.transform.localScale = new Vector3(_scale,_scale,_scale) ;
		snowflake.transform.Rotate (new Vector3 (0, 1, 0), 10 * Time.deltaTime);
		snowflake.transform.parent = transform;
		Destroy (snowflake, _lifetime);

		VRInteractiveItem snowInt = snowflake.GetComponent<VRInteractiveItem>();

		if(snowInt)
		{
			snowInt.OnOver += SnowflakeOver;
			snowInt.OnOut += SnowflakeOut;
			snowInt.OnClick += SnowflakeClick;
		}
	}


	private void SnowflakeOver(Transform flakeTrans)
	{
//		Renderer renderer = flakeTrans.GetComponent<Renderer>();
//
//		if(renderer && renderer.material)
//		{
//			renderer.material.color = Color.yellow;
//		}
	}

	private void SnowflakeOut(Transform flakeTrans)
	{
//		Renderer renderer = flakeTrans.GetComponent<Renderer>();
//
//		if(renderer && renderer.material)
//		{
//			renderer.material.color = Color.white;
//		}
	}

	private void SnowflakeClick(Transform flakeTrans)
	{
		if(explosion)
		{
			// show explosion
			if(explosion.isPlaying)
			{
				explosion.Stop();
			}

			explosion.transform.position = flakeTrans.position;
			explosion.Play();
			//Destroy(explosion, explosion.duration);
		}

		// destroy the snowflake
		Destroy(flakeTrans.gameObject);
	}

}