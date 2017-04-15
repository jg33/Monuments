using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOnTouch : MonoBehaviour {

     bool didTrigger = false;
	private GameObject duplicateMeshObj;
	public GameObject meshCopyPrefab;
	public GameObject copyContainer;

	Collider thisCollider;

	Mesh thisMesh;
	Vector3[] dyingVerts;
	int copyCount = 0;

    public Transform dupeVertexOrigin;

  //  Mesh originalMesh;

	// Use this for initialization
	void Start () {
		thisMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		dyingVerts = thisMesh.vertices;

		//thisCollider = gameObject.GetComponent<MeshCollider>();
		//thisCollider.sharedMesh = thisMesh; 
		thisMesh.MarkDynamic();

//		newMesh.triangles = thisMesh.triangles;
//		newMesh.MarkDynamic();

		// copy so we don't overwrite:
		Mesh oldMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        Mesh newMesh = (Mesh)Instantiate(oldMesh);

		GetComponent<SkinnedMeshRenderer>().sharedMesh = newMesh;
        thisMesh = newMesh;
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
             
            SkinnedMeshRenderer skin = gameObject.GetComponent<SkinnedMeshRenderer>();
            Mesh mesh = skin.sharedMesh;
 
            int vertexCount = mesh.vertexCount;
            Vector3[] vertices = new Vector3[vertexCount];

            Matrix4x4[] boneMatrices = new Matrix4x4[skin.bones.Length];
            for (int i = 0; i < boneMatrices.Length; i++) boneMatrices[i] = skin.bones[i].localToWorldMatrix * mesh.bindposes[i];
 
        
            BoneWeight weight = mesh.boneWeights[copyCount];
 
            Matrix4x4 bm0 = boneMatrices[weight.boneIndex0];
            Matrix4x4 bm1 = boneMatrices[weight.boneIndex1];
            Matrix4x4 bm2 = boneMatrices[weight.boneIndex2];
            Matrix4x4 bm3 = boneMatrices[weight.boneIndex3];
 
            Matrix4x4 vertexMatrix = new Matrix4x4();
 
            for (int n = 0; n < 16; n++){
                vertexMatrix[n] =
                    bm0[n] * weight.weight0 +
                    bm1[n] * weight.weight1 +
                    bm2[n] * weight.weight2 +
                    bm3[n] * weight.weight3;
             }
 
                thisPoint = vertexMatrix.MultiplyPoint3x4(mesh.vertices[copyCount]);
                thisPoint *= 1.4f;
                thisPoint += dupeVertexOrigin.transform.position;

            // normals[i] = vertexMatrix.MultiplyVector(mesh.normals[i]);
            
                


                tempVerts[copyCount] = thisPoint;

				duplicateMeshObj.GetComponent<MeshFilter>().mesh.vertices =  tempVerts;

				duplicateMeshObj.GetComponent<MeshFilter>().mesh.MarkDynamic();
				duplicateMeshObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
				duplicateMeshObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

				//Debug.Log(duplicateMeshObj.GetComponent<MeshFilter>().mesh.vertices[copyCount]);

				// REMOVE //
				dyingVerts[copyCount] = dyingVerts[0];
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

    void CopySelf()
    {
        //world space
        //Instantiate(meshCopyPrefab, copyContainer.transform);
        if (!didTrigger)
        {
            //local space
            duplicateMeshObj = Instantiate(meshCopyPrefab);
            duplicateMeshObj.transform.localPosition = new Vector3(-gameObject.transform.parent.position.x * 0.714f, -gameObject.transform.parent.position.y * 0.714f, -gameObject.transform.parent.position.z * 0.714f);
            duplicateMeshObj.transform.localScale = new Vector3(0.714f, 0.714f, 0.714f);
            duplicateMeshObj.transform.parent = copyContainer.transform;
            duplicateMeshObj.name = gameObject.name + "_copy";

            //
            Mesh dupeMesh = duplicateMeshObj.GetComponent<MeshFilter>().mesh;
            dupeMesh.vertices = thisMesh.vertices;
            dupeMesh.triangles = thisMesh.triangles;
            dupeMesh.name = "Dupe Mesh";
            Vector3[] tempVerts = dupeMesh.vertices;

            Debug.Log("tempVerts: " + tempVerts.Length);


            for (int i = 0; i < tempVerts.Length; i++)
            {
                tempVerts[i] = (dupeMesh.vertices[0] + dupeVertexOrigin.position);
            }
            dupeMesh.vertices = tempVerts;

            dupeMesh.MarkDynamic();

            duplicateMeshObj.GetComponent<MeshFilter>().mesh = dupeMesh;

            didTrigger = true;


        }
    }
}
