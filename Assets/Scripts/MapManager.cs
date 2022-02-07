/*Old version
 * using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {
	[SerializeField]
	private GameObject _chunkPref;
	static public GameObject chunkPref;
	static Transform mapManager = null;

	public const int ChunkSize = 16;
	public static int mapSize = 4;

	static PlanetsGenerator.Planet planetType;

	public static Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3, Chunk>();

	public static Queue<Chunk> applyChanges = new Queue<Chunk>();

	void Start(){
		if(mapManager != null)
			Destroy(this);
		mapManager = transform;
		chunkPref = _chunkPref;

		planetType = PlanetsGenerator.RequestPlanet();
		//CreateChuncks();
	}
	void Update(){
		Chunk _ch = null;
		lock((object)applyChanges){
		if(applyChanges.Count!=0)
			_ch = applyChanges.Dequeue();
		}
		if(_ch!=null){
			_ch.ApplyMesh();
		}
	}
	public static void CreateNewChunk(Vector3 pos){
		GameObject _ch = Instantiate(chunkPref,pos,Quaternion.identity) as GameObject;
		_ch.GetComponent<Chunk>().offset = pos;
		_ch.GetComponent<Chunk>().planetType = planetType;
		_ch.transform.parent = mapManager;

		chunks.Add(pos,_ch.GetComponent<Chunk>());
	}

	void CreateChuncks(){
		Vector3 pos = new Vector3();
		for(int i = 0; i<mapSize;i++)
			for(int j = 0; j<mapSize;j++)
				for(int k = 0; k<mapSize;k++){
					pos.x = (ChunkSize)*i;
					pos.y = (ChunkSize)*j;
					pos.z = (ChunkSize)*k;
					GameObject _ch = Instantiate(chunkPref,pos,Quaternion.identity) as GameObject;
					_ch.GetComponent<Chunk>().offset = pos;
					_ch.transform.parent = transform;
					chunks.Add(pos,_ch.GetComponent<Chunk>());
				}
	}
	public struct MeshChanges{
		List<Vector3> verticles;
		List<int> triangles;
	}
}
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace VoxelPlanets{
	public class MapManager:MonoBehaviour{
		public PlanetsGenerator.Planet planet{get; private set;}
		public Vector3 planetCenter{get; private set;}

		[SerializeField]
		GameObject chunkPrefab;
		const int chunksize = 16;
		public Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3, Chunk>();

		void Awake(){
			StartCoroutine(UpdateChunks());
			planet = PlanetsGenerator.RequestPlanet();
			planetCenter = transform.position;
		}

		/// <summary>
		/// Try to find chunk. If not created, create it.
		/// Updates chunk mesh with new LoD. 
		/// </summary>
		/// <param name="chunkPos">Chunk position.</param>
		/// <param name="LoD">Level of details</param>
		public void RequestChunkUpdate(Vector3 chunkPos, float LoD){
			if(chunks.ContainsKey(chunkPos)){
				WaitCallback callBack = new WaitCallback(state => TerrainGenerator.SimplifyMesh((TerrainGenerator.SimplifyMeshParams)state));
				ThreadPool.QueueUserWorkItem(callBack,new TerrainGenerator.SimplifyMeshParams(chunks[chunkPos],LoD));
			}else{
				Chunk chunk = CreateChunk(chunkPos);
				WaitCallback callBack = new WaitCallback(state => TerrainGenerator.GenerateMesh((TerrainGenerator.GenerateMeshParams)state));
				ThreadPool.QueueUserWorkItem(callBack,new TerrainGenerator.GenerateMeshParams(LoD,chunksize,chunk,planetCenter,planet));
			}
		}

		Chunk CreateChunk(Vector3 chunkPos){
			GameObject chunkObj = Instantiate(chunkPrefab,chunkPos,Quaternion.identity) as GameObject;
			Chunk chunk = chunkObj.GetComponent<Chunk>();
			chunk.mapManager = this;
			chunk.position = chunkPos;
			return chunk;
		}

		/// <summary>
		/// If there are chunks, that needs to be updated
		/// with new mesh, update them
		/// </summary>
		public LockedQeue<TerrainGenerator.Answer> chunkUpdateQeue = new LockedQeue<TerrainGenerator.Answer>();
		IEnumerator UpdateChunks(){
			while(true){
				if(chunkUpdateQeue.Count>0){
					TerrainGenerator.Answer answer = chunkUpdateQeue.Dequeue();
					answer.chunk.meshFilter.sharedMesh = null;
					answer.chunk.meshCollider.sharedMesh = null;
					answer.chunk.mesh.vertices = answer.vertices.ToArray();
					answer.chunk.mesh.triangles = answer.triangles.ToArray();
					answer.chunk.mesh.RecalculateNormals();
					answer.chunk.meshFilter.sharedMesh = answer.chunk.mesh;
					//answer.chunk.meshCollider.sharedMesh = answer.chunk.mesh;
				}
				yield return null;
			}
		}


	}
}