/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MapViewer : MonoBehaviour {
	[SerializeField]
	Transform viewer;
	float visibleDistance{get{return (float)MapManager.ChunkSize*4;}}
	int chunksVisibleInViewDst;
	[SerializeField]
	Vector3 latestChunk;

	void Start(){
		latestChunk = viewer.position*MapManager.ChunkSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(visibleDistance/MapManager.ChunkSize);
		StartCoroutine(VisiblityUpdate());
	}
	IEnumerator VisiblityUpdate(){
		while(true){
			int currentChunkCoordX = Mathf.RoundToInt(viewer.position.x/MapManager.ChunkSize);
			int currentChunkCoordY = Mathf.RoundToInt(viewer.position.y/MapManager.ChunkSize);
			int currentChunkCoordZ = Mathf.RoundToInt(viewer.position.z/MapManager.ChunkSize);

			Vector3 currentChunk = new Vector3(currentChunkCoordX,currentChunkCoordY,currentChunkCoordZ);
			if(Vector3.Distance(currentChunk,latestChunk)>2){
				latestChunk=currentChunk;
				for(int xOffset = -chunksVisibleInViewDst;xOffset<=chunksVisibleInViewDst;xOffset++)
					for(int yOffset = -chunksVisibleInViewDst;yOffset<=chunksVisibleInViewDst;yOffset++){
						for(int zOffset = -chunksVisibleInViewDst;zOffset<=chunksVisibleInViewDst;zOffset++){
							Vector3 viewedChunkCoord = new Vector3(currentChunkCoordX+xOffset,currentChunkCoordY+yOffset,currentChunkCoordZ+zOffset)*MapManager.ChunkSize;
							if(!MapManager.chunks.ContainsKey(viewedChunkCoord))
								MapManager.CreateNewChunk(viewedChunkCoord);

							Chunk viewedChunk = MapManager.chunks[viewedChunkCoord];
							if(viewedChunk.cellsInUnit != GetLOD(viewedChunkCoord)){
								WaitCallback callBack = new WaitCallback(state => viewedChunk.UpdateChunk((float)state));
								ThreadPool.QueueUserWorkItem(callBack,GetLOD(viewedChunkCoord));
							}

						}
						yield return null;
					}
			}
			yield return null;
		}
	}

	float GetLOD(Vector3 pos){
		pos+=new Vector3(1,1,1)*MapManager.ChunkSize/2;
		float distance = (pos - viewer.position).magnitude;
		/*if(distance<MapManager.ChunkSize) return 1f;
		if(distance<MapManager.ChunkSize*2) return 1f;
		if(distance<MapManager.ChunkSize*4) return 1f;
		if(distance<MapManager.ChunkSize*6) return 1f;
		if(distance<MapManager.ChunkSize*10) return 0.25f;
		if(distance<MapManager.ChunkSize*18) return 0.125f;
		return 0.0625f;
	}
}
*/

using UnityEngine;
using System.Collections;
namespace VoxelPlanets{
	public class MapViewer:MonoBehaviour{
		[SerializeField]
		MapManager mapManager;
		void Start(){
			StartCoroutine(DrawPlanet());
		}
		IEnumerator DrawPlanet(){
			for(int i = -10;i<5;i++)
				for(int j = -3;j<5;j++)
					for(int k = -10;k<5;k++){
						mapManager.RequestChunkUpdate(new Vector3(i*16,j*16,k*16),1f);
						yield return null;
					}
		}
	}
}