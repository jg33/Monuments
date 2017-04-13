using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyFBXMesh : MonoBehaviour {

	private Mesh newMesh;
	private Mesh targetMesh;
	private ArrayList newVertices;
	//private ArrayList newTriangles;
	Vector3[] verts ;


	public GameObject targetObject;
	public int startIndex;
	public int endIndex;

	public bool copy;
	public bool clear;


	// Use this for initialization
	void Start () {
		newMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		targetMesh = targetObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;

//		newVertices = new ArrayList(Vector3); 
		//newTriangles = new ArrayList(integer);
		newMesh.MarkDynamic();
		targetMesh.MarkDynamic();

		verts = new Vector3[targetMesh.vertexCount];
		ClearMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if(copy){
			CopyVerts(startIndex,endIndex);
			copy=false;
		}
		if (clear){
			ClearMesh();
			clear=false;
		}

		//newMesh.RecalculateBounds();

//		Debug.Log("new: "+newMesh.vertexCount);
//		Debug.Log("target: "+targetMesh.vertexCount);

	}

	 public void CopyVerts(int _start, int _end){

		int[] tris = new int[targetMesh.triangles.Length];
		tris= targetMesh.triangles;

		Vector3[] targetVerts = targetMesh.vertices;

		//ArrayList vertsToKill = new ArrayList();
		for(int i=_start;i<=_end;i++){
			verts[i] = targetMesh.vertices[i];
			targetVerts[i] = targetMesh.vertices[i+1];
//			for (int j = 0; j< targetMesh.triangles.Length;j++){
//				if(targetMesh.triangles[j]==i){
//					tris[j]= targetMesh.triangles[j];
//				}
//
//			}


		}


		gameObject.GetComponent<MeshFilter>().mesh.vertices = verts;
		gameObject.GetComponent<MeshFilter>().mesh.triangles = tris;

		gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		newMesh.RecalculateBounds();


		targetMesh.vertices= targetVerts;

		targetMesh.RecalculateBounds();
		targetMesh.RecalculateNormals();

	}

	public void CopyVert(int _index){
		CopyVerts(_index,_index);

	}

	void ClearMesh(){
		//newMesh = new Mesh();
		//newMesh.vertices.Clone(targetMesh.vertices);
	//	newMesh.vertices = targetMesh.vertices;
		newMesh.RecalculateBounds();
	}
}
