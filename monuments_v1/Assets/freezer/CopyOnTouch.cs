using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOnTouch : MonoBehaviour {

	public bool didTrigger = false;
	private GameObject duplicateMeshObj;
	public GameObject meshCopyPrefab;
	public GameObject copyContainer;

	MeshCollider thisCollider;

	Mesh thisMesh;
	Vector3[] dyingVerts;
	public int copyCount = 0;

	// Use this for initialization
	void Start () {
		thisMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		dyingVerts = thisMesh.vertices;

		thisCollider = gameObject.GetComponent<MeshCollider>();
		thisCollider.sharedMesh = thisMesh; 
		thisMesh.MarkDynamic();

//		newMesh.triangles = thisMesh.triangles;
//		newMesh.MarkDynamic();

		// copy so we don't overwrite:
		Mesh oldMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		Mesh newMesh = (Mesh)Object.Instantiate(oldMesh);
		GetComponent<SkinnedMeshRenderer>().sharedMesh = newMesh;

		//set to draw as points?
		//thisMesh.SetIndices(thisMesh.GetIndices(0), MeshTopology.Triangles, 0);
		// ^ this fucks up colliderst
	}
	
	// Update is called once per frame
	void Update () {
		if (didTrigger && duplicateMeshObj){
			//duplicateMesh.gameObject.GetComponent<MeshFilter>().mesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;



			if( copyCount < thisMesh.vertexCount ){
				//duplicateMesh.gameObject.GetComponent<MeshFilter>().mesh.vertices. ;

				// COPY //
				Vector3[] tempVerts = duplicateMeshObj.GetComponent<MeshFilter>().mesh.vertices;
				Vector3 thisPoint = transform.TransformPoint(thisMesh.vertices[copyCount]);
			
				// modify by bones
				Matrix4x4[] boneMatrices = new Matrix4x4[gameObject.GetComponent<SkinnedMeshRenderer>().bones.Length];
				for (int i = 0; i < boneMatrices.Length; i++)
					boneMatrices[i] = gameObject.GetComponent<SkinnedMeshRenderer>().bones[i].localToWorldMatrix * gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes[i];

					BoneWeight weight = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.boneWeights[copyCount];

					Matrix4x4 bm0 = boneMatrices[weight.boneIndex0];
					Matrix4x4 bm1 = boneMatrices[weight.boneIndex1];
					Matrix4x4 bm2 = boneMatrices[weight.boneIndex2];
					Matrix4x4 bm3 = boneMatrices[weight.boneIndex3];

					Matrix4x4 vertexMatrix = new Matrix4x4();

					for (int n = 0; n < 16; n++)
					{
						vertexMatrix[n] =
							bm0[n] * weight.weight0 +
							bm1[n] * weight.weight1 +
							bm2[n] * weight.weight2 +
							bm3[n] * weight.weight3;
					}

					thisPoint = vertexMatrix.MultiplyPoint3x4(thisPoint);
					//normals[i] = vertexMatrix.MultiplyVector(mesh.normals[i]);

				tempVerts[copyCount] = thisPoint;

				duplicateMeshObj.GetComponent<MeshFilter>().mesh.vertices =  tempVerts;

				duplicateMeshObj.GetComponent<MeshFilter>().mesh.MarkDynamic();
				duplicateMeshObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
				duplicateMeshObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

				//Debug.Log(duplicateMeshObj.GetComponent<MeshFilter>().mesh.vertices[copyCount]);

				// REMOVE //
				dyingVerts[copyCount] = new Vector3(0,0,0);
//				Mesh tempDeathMesh = new Mesh();
//				tempDeathMesh.triangles = thisMesh.triangles;
//				tempDeathMesh.vertices = dyingVerts;
				gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh.vertices = dyingVerts;

				thisMesh.RecalculateBounds();
				thisMesh.RecalculateNormals();


				// ITERATE //
				copyCount++;

			}



		}
	}

	void OnTriggerEnter(Collider _thisCollider){
		if(!didTrigger && _thisCollider.gameObject.name == "Touch"){
			Debug.Log("Collided");
			CopySelf();
		}


	}

	void CopySelf(){
		//world space
		//Instantiate(meshCopyPrefab, copyContainer.transform);

		//local space
		duplicateMeshObj = Instantiate(meshCopyPrefab, gameObject.transform.position, gameObject.transform.rotation) ;
		duplicateMeshObj.transform.localScale= new Vector3(0.714f,0.714f,0.714f);
		//
		Mesh dupeMesh = duplicateMeshObj.GetComponent<MeshFilter>().mesh;
		dupeMesh.vertices = thisMesh.vertices;
		dupeMesh.triangles = thisMesh.triangles;
		dupeMesh.name = "Dupe Mesh";
		Vector3[] tempVerts = dupeMesh.vertices;

		for(int i = 0; i < tempVerts.Length;i++){
			tempVerts[i] = dupeMesh.vertices[0];
		}
		dupeMesh.vertices = tempVerts;

		dupeMesh.MarkDynamic();

		duplicateMeshObj.GetComponent<MeshFilter>().mesh = dupeMesh;
		duplicateMeshObj.name = gameObject.name+"_copy";
		didTrigger = true;


	}
}
