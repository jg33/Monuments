using UnityEngine;
using System.Collections;
using System;

public class ObjectSpawner : MonoBehaviour 
{
	// events
	public event Action<Transform> OnNewObject;


	[Tooltip("Prefab used to instantiate balls in the scene.")]
    public Transform ballPrefab;

	[Tooltip("Prefab used to instantiate cubes in the scene.")]
	public Transform cubePrefab;

	[Tooltip("User transform used for spawning objects. If not set, Kinect user position will be used.")]
	public Transform userTransform;
	
	[Tooltip("Range in meters of newly instantiated prefabs around the user position.")]
	public Vector3 instantiateInRange = new Vector3(1.5f, 0f, 1.5f);

	[Tooltip("How many objects do we want to spawn.")]
	public int numberOfObjects = 20;

	private Vector3 posUser = Vector3.zero;
    private float nextSpawnTime = 0.0f;
    private float spawnRate = 1.5f;
	private int ballsCount = 0;
 	

	void Update () 
	{
        if (nextSpawnTime < Time.time)
        {
            SpawnBalls();
            nextSpawnTime = Time.time + spawnRate;

			spawnRate = UnityEngine.Random.Range(0f, 1f);
			//numberOfBalls = Mathf.RoundToInt(Random.Range(1f, 10f));
        }
	}

    void SpawnBalls()
    {
		KinectManager manager = KinectManager.Instance;

		if(ballPrefab && cubePrefab && ballsCount < numberOfObjects &&
		   manager && manager.IsInitialized() && manager.IsUserDetected())
		{
			long userId = manager.GetPrimaryUserID();
			posUser = userTransform ? userTransform.position : manager.GetUserPosition(userId);

			float xPos = UnityEngine.Random.Range(-instantiateInRange.x, instantiateInRange.x);
			float zPos = UnityEngine.Random.Range(-instantiateInRange.z, instantiateInRange.z);
			Vector3 spawnPos = new Vector3(posUser.x + xPos, posUser.y + instantiateInRange.y, posUser.z + zPos);

			int ballOrCube = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));

			Transform ballTransform = Instantiate(ballOrCube > 0 ? ballPrefab : cubePrefab, spawnPos, Quaternion.identity) as Transform;
			ballTransform.GetComponent<Renderer>().material.color = new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), 1f);
			ballTransform.parent = transform;

			ballsCount++;

			if (OnNewObject != null) 
			{
				OnNewObject (ballTransform);
			}
		}
    }

}
