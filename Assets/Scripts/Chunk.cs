/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Chunk : MonoBehaviour {
	public Vector3 offset = new Vector3(0,0,0);
	static int chunkSize{get{return MapManager.ChunkSize;}}
	object cellsInUnitLocker; 
	public float cellsInUnit{
		get{
			lock(cellsInUnitLocker){
				return _cellsInUnit;
			}
		}
		set{
			lock(cellsInUnitLocker){
				_cellsInUnit = value;
			}
		}
	}
	float _cellsInUnit = -1;
	Node[,,] nodes;
	float[,,] destiny;
	public PlanetsGenerator.Planet planetType;
	MeshFilter _meshFilter;
	MeshCollider _meshCollider;
	Mesh mesh = new Mesh();

	List <Vector3> verticles;
	List <int> triangles;
	void Awake(){
		cellsInUnitLocker = new object();
		_meshFilter = GetComponent<MeshFilter>();
		_meshCollider = GetComponent<MeshCollider>();
		mesh = new Mesh();
	}
	void Initialize(){
		int nodesLenght = (int)((chunkSize*cellsInUnit+1)*2);
		int destinyLenght = (int)(chunkSize*cellsInUnit+1);
		nodes = new Node[nodesLenght,nodesLenght,nodesLenght];
		destiny = new float[destinyLenght,destinyLenght,destinyLenght];
	}
	public void UpdateChunk(float lod){
		lock((object)cellsInUnit){
			if(lod==cellsInUnit)
				return;
			cellsInUnit = lod;
		}
		lock((object)this){
			Initialize();
			DrawChunk();
		}
	}

	void DrawChunk(){
		verticles = new List<Vector3>();
		triangles = new List<int>();
		int nodesLenght = (int)((chunkSize*cellsInUnit+1)*2);
		int destinyLenght = (int)(chunkSize*cellsInUnit+1);
		for(int i = 0; i<nodesLenght;i++)
			for(int j = 0; j<nodesLenght;j++)
				for(int k = 0; k<nodesLenght;k++){
					nodes[i,j,k].position = new Vector3(i,j,k)/2/cellsInUnit;
					nodes[i,j,k].vertexIndex = -1;
				}

		for(int i = 0; i<destinyLenght;i++)
			for(int j = 0; j<destinyLenght;j++)
				for(int k = 0; k<destinyLenght;k++)
					destiny[i,j,k] = Destiny.GetDestiny(new Vector3(offset.x + (float)i/(float)cellsInUnit,offset.y+(float)j/(float)cellsInUnit,offset.z+(float)k/(float)cellsInUnit),Vector3.zero,planetType);
				
		//For all cells
		for(int i = 0; i<destinyLenght-1;i++)
			for(int j = 0; j<destinyLenght-1;j++)
				for(int k = 0; k<destinyLenght-1;k++)
					DrawCell(i,j,k);
		lock((object)MapManager.applyChanges){
		MapManager.applyChanges.Enqueue(this);
			nodes = null;
			destiny = null;
		}
	}
	public void ApplyMesh(){
		lock((object)this){
			_meshFilter.mesh = null;
			mesh.Clear();
			_meshFilter.mesh = mesh;
			mesh.vertices = verticles.ToArray();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();
			_meshCollider.sharedMesh = mesh;
		}
		
	}
	void DrawCell(int x, int y, int z){
		//Debug.Log("Drawing cell " + x.ToString() + " " + y.ToString() + " " + z.ToString());
		int caseParam = 0;

		if(destiny[x,y,z]>=0)
			caseParam+=1;
		if(destiny[x,y+1,z]>=0)
			caseParam+=2;
		if(destiny[x+1,y+1,z]>=0)
			caseParam+=4;
		if(destiny[x+1,y,z]>=0)
			caseParam+=8;
		if(destiny[x,y,z+1]>=0)
			caseParam+=16;
		if(destiny[x,y+1,z+1]>=0)
			caseParam+=32;
		if(destiny[x+1,y+1,z+1]>=0)
			caseParam+=64;
		if(destiny[x+1,y,z+1]>=0)
			caseParam+=128;
		

		byte numPols = TableConstants.case_to_numpolys[caseParam];
		int[][] edgesConList = TableConstants.edge_connect_list[caseParam];
		Vector3 nodeDisp = new Vector3();
		for(int i = 0;i<numPols;i++)
			for(int j = 0;j<3;j++){
				switch(edgesConList[i][j])
				{
				case 0 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y,z],destiny[x,y+1,z]),0);
						AddVerticle(ref nodes[x*2,y*2+1,z*2],nodeDisp);
						AddTrianglePart(nodes[x*2,y*2+1,z*2]);
					}break;
				case 1 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y+1,z],destiny[x+1,y+1,z]),0);
						AddVerticle(ref nodes[x*2+1,y*2+2,z*2],nodeDisp);
					AddTrianglePart(nodes[x*2+1,y*2+2,z*2]);
					}break;
				case 2 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x+1,y,z],destiny[x+1,y+1,z]),0);
						AddVerticle(ref nodes[x*2+2,y*2+1,z*2],nodeDisp);
					AddTrianglePart(nodes[x*2+2,y*2+1,z*2]);
					}break;
				case 3 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y,z],destiny[x+1,y,z]),0);
						AddVerticle(ref nodes[x*2+1,y*2,z*2],nodeDisp);
					AddTrianglePart(nodes[x*2+1,y*2,z*2]);
					}break;
				case 4 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y,z+1],destiny[x,y+1,z+1]),0);
						AddVerticle(ref nodes[x*2,y*2+1,z*2+2],nodeDisp);
					AddTrianglePart(nodes[x*2,y*2+1,z*2+2]);
					}break;
				case 5 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y+1,z+1],destiny[x+1,y+1,z+1]),0);
						AddVerticle(ref nodes[x*2+1,y*2+2,z*2+2],nodeDisp);
					AddTrianglePart(nodes[x*2+1,y*2+2,z*2+2]);
					}break;
				case 6 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x+1,y,z+1],destiny[x+1,y+1,z+1]),0);
						AddVerticle(ref nodes[x*2+2,y*2+1,z*2+2],nodeDisp);
					AddTrianglePart(nodes[x*2+2,y*2+1,z*2+2]);
					}break;
				case 7 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y,z+1],destiny[x+1,y,z+1]),0);
						AddVerticle(ref nodes[x*2+1,y*2,z*2+2],nodeDisp);
					AddTrianglePart(nodes[x*2+1,y*2,z*2+2]);
					}break;
				case 8 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y,z],destiny[x,y,z+1]),0);
						AddVerticle(ref nodes[x*2,y*2,z*2+1],nodeDisp);
					AddTrianglePart(nodes[x*2,y*2,z*2+1]);
					}break;
				case 9 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x,y+1,z],destiny[x,y+1,z+1]),0);
						AddVerticle(ref nodes[x*2,y*2+2,z*2+1],nodeDisp);
					AddTrianglePart(nodes[x*2,y*2+2,z*2+1]);
					}break;
				case 10 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x+1,y+1,z],destiny[x+1,y+1,z+1]),0);
						AddVerticle(ref nodes[x*2+2,y*2+2,z*2+1],nodeDisp);
					AddTrianglePart(nodes[x*2+2,y*2+2,z*2+1]);
					}break;
				case 11 :{
						nodeDisp = new Vector3(0,Interpolate(destiny[x+1,y,z],destiny[x+1,y,z+1]),0);
						AddVerticle(ref nodes[x*2+2,y*2,z*2+1],nodeDisp);
					AddTrianglePart(nodes[x*2+2,y*2,z*2+1]);
					}break;
				default: Debug.Log("Problem");
					break;
				}

		}
		

	}
	void AddVerticle(ref Node node,Vector3 nodeDisp){
		if(node.vertexIndex != -1)
			return;
		verticles.Add(node.position+nodeDisp/cellsInUnit);
		node.vertexIndex = verticles.Count-1;
	}
	void AddTrianglePart(Node node){
		triangles.Add(node.vertexIndex);
	}

	struct Node{
		public Vector3 position;
		public int vertexIndex;
	}
	float Interpolate(float a, float b){
		a = Mathf.Abs(a);
		b = Mathf.Abs(b);
		return a/(a+b)-0.5f;
		return 0;
	}
}
*/
using UnityEngine;

namespace VoxelPlanets{
	public class Chunk:MonoBehaviour{
		public MapManager mapManager;
		public Vector3 position;

		public bool IsEmpty;
		public Mesh mesh;

		public MeshCollider meshCollider{get; private set;}
		public MeshFilter meshFilter{get; private set;}

		void Awake(){
			meshCollider = GetComponent<MeshCollider>();
			meshFilter = GetComponent<MeshFilter>();
			mesh = new Mesh();
		}
	}
}