using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelPlanets{
	/// <summary>
	/// Generates main mesh properties for chunk.
	/// </summary>
	public static class TerrainGenerator{
		/// <summary>
		/// Simplifies the mesh into new 
		/// level of detail and updates chunk with them.
		/// Thread safe.
		/// </summary>
		public static void SimplifyMesh(SimplifyMeshParams _params){
			Answer answer = new Answer();

			//TODO: Mesh simplifiing
			//Maybe just invoke GenerateMesh with anoughter LoD

			//"Constant return"
			answer.chunk =_params.chunk;
			answer.vertices = new List<Vector3>(_params.chunk.mesh.vertices);
			answer.triangles = new List<int>(_params.chunk.mesh.triangles);
			//

			_params.chunk.mapManager.chunkUpdateQeue.Enqueue(answer);
		}

		public struct SimplifyMeshParams{
			public Chunk chunk;
			public float LoD;

			public SimplifyMeshParams(Chunk chunk, float LoD){
				this.chunk = chunk;
				this.LoD = LoD;
			}
		}

		/// <summary>
		/// Generates main properties and updates chunk with them.
		/// Thread safe.
		/// </summary>
		public static void GenerateMesh(GenerateMeshParams _params){
			//Creating data lists for mesh parametrs
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			//Arrays for storing every node in chunk and density in points
			int nodesLenght = (int)((_params.chunkSize*_params.LoD+1)*2);
			int densityLenght = (int)(_params.chunkSize*_params.LoD+1);
			VerticeNode[,,] nodes = new VerticeNode[nodesLenght,nodesLenght,nodesLenght];
			float[,,] density = new float[densityLenght,densityLenght,densityLenght];


			//Initializing nodes array
			for(int i = 0; i<nodesLenght;i++){
				for(int j = 0; j<nodesLenght;j++){
					for(int k = 0; k<nodesLenght;k++){
						nodes[i,j,k].position = new Vector3(i,j,k)/2/_params.LoD;
						nodes[i,j,k].verticeIndex = -1;
					}
				}
			}
			//Generating density for each point
			for(int i = 0; i<densityLenght;i++){
				for(int j = 0; j<densityLenght;j++){
					for(int k = 0; k<densityLenght;k++){
						density[i,j,k] = Density.GetDensity(_params.chunk.position + new Vector3(i,j,k)/_params.LoD,_params.planetCenter,_params.planet);
					}
				}
			}
			/// <summary>
			/// Chunks are virtually separated into cells.
			/// Cell contains 8 nodes and looks like a cube.
			/// We need to calculate every cell separately.
			/// </summary>
			for(int i = 0; i<densityLenght-1;i++){
				for(int j = 0; j<densityLenght-1;j++){
					for(int k = 0; k<densityLenght-1;k++){
						CalculateCell(i,j,k,_params.LoD,ref vertices,ref triangles,ref density, ref nodes);
					}
				}
			}


			//Creating answer for this request
			Answer answer = new Answer();
			answer.chunk = _params.chunk;
			answer.vertices = vertices;
			answer.triangles = triangles;

			_params.chunk.mapManager.chunkUpdateQeue.Enqueue(answer);
		}
		public struct GenerateMeshParams{
			public float LoD;
			public int chunkSize;
			public Chunk chunk;
			public Vector3 planetCenter;
			public PlanetsGenerator.Planet planet;

			public GenerateMeshParams(float LoD,int chunkSize,Chunk chunk,Vector3 planetCenter,PlanetsGenerator.Planet planet){
				this.LoD = LoD;
				this.chunkSize = chunkSize;
				this.chunk = chunk;
				this.planetCenter = planetCenter;
				this.planet = planet;
			}
		}	

		/// <summary>
		/// Calculates individual cell.
		/// </summary>
		private static void CalculateCell(int x, int y, int z,float LoD, ref List<Vector3> vertices, ref List<int> triangles, ref float[,,] density, ref VerticeNode [,,] nodes){
			//Calculating cell type
			#region CellType
			int cellType = 0;

			if(density[x,y,z]>=0)
				cellType+=1;
			if(density[x,y+1,z]>=0)
				cellType+=2;
			if(density[x+1,y+1,z]>=0)
				cellType+=4;
			if(density[x+1,y,z]>=0)
				cellType+=8;
			if(density[x,y,z+1]>=0)
				cellType+=16;
			if(density[x,y+1,z+1]>=0)
				cellType+=32;
			if(density[x+1,y+1,z+1]>=0)
				cellType+=64;
			if(density[x+1,y,z+1]>=0)
				cellType+=128;
			#endregion

			///<summary>
			/// Finding data in static tables, according to the CellType:
			///  [1]case_to_numpolys: have many polygons should be in this cell
			///  [2]edge_connect_list: whitch edges should be connected
			/// </summary>
			byte numPols = TableConstants.case_to_numpolys[cellType];
			int[][] edgesConList = TableConstants.edge_connect_list[cellType];


			///<summary>
			/// Calculates witch vertices and thiangles we need to add
			/// in this cell.
			/// </summary>
			Vector3 nodeDisp = new Vector3();
			for(int i = 0;i<numPols;i++)
				for(int j = 0;j<3;j++){
					switch(edgesConList[i][j])
					{
					case 0 :{
							nodeDisp = new Vector3(0,Interpolate(density[x,y,z],density[x,y+1,z]),0);
							AddVerticle(ref nodes[x*2,y*2+1,z*2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2,y*2+1,z*2],ref triangles);
						}break;
					case 1 :{
							nodeDisp = new Vector3(Interpolate(density[x,y+1,z],density[x+1,y+1,z]),0,0);
							AddVerticle(ref nodes[x*2+1,y*2+2,z*2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+1,y*2+2,z*2],ref triangles);
						}break;
					case 2 :{
							nodeDisp = new Vector3(0,Interpolate(density[x+1,y,z],density[x+1,y+1,z]),0);
							AddVerticle(ref nodes[x*2+2,y*2+1,z*2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+2,y*2+1,z*2],ref triangles);
						}break;
					case 3 :{
							nodeDisp = new Vector3(Interpolate(density[x,y,z],density[x+1,y,z]),0,0);
							AddVerticle(ref nodes[x*2+1,y*2,z*2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+1,y*2,z*2],ref triangles);
						}break;
					case 4 :{
							nodeDisp = new Vector3(0,Interpolate(density[x,y,z+1],density[x,y+1,z+1]),0);
							AddVerticle(ref nodes[x*2,y*2+1,z*2+2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2,y*2+1,z*2+2],ref triangles);
						}break;
					case 5 :{
							nodeDisp = new Vector3(Interpolate(density[x,y+1,z+1],density[x+1,y+1,z+1]),0,0);
							AddVerticle(ref nodes[x*2+1,y*2+2,z*2+2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+1,y*2+2,z*2+2],ref triangles);
						}break;
					case 6 :{
							nodeDisp = new Vector3(0,Interpolate(density[x+1,y,z+1],density[x+1,y+1,z+1]),0);
							AddVerticle(ref nodes[x*2+2,y*2+1,z*2+2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+2,y*2+1,z*2+2],ref triangles);
						}break;
					case 7 :{
							nodeDisp = new Vector3(Interpolate(density[x,y,z+1],density[x+1,y,z+1]),0,0);
							AddVerticle(ref nodes[x*2+1,y*2,z*2+2],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+1,y*2,z*2+2],ref triangles);
						}break;
					case 8 :{
							nodeDisp = new Vector3(0,0,Interpolate(density[x,y,z],density[x,y,z+1]));
							AddVerticle(ref nodes[x*2,y*2,z*2+1],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2,y*2,z*2+1],ref triangles);
						}break;
					case 9 :{
							nodeDisp = new Vector3(0,0,Interpolate(density[x,y+1,z],density[x,y+1,z+1]));
							AddVerticle(ref nodes[x*2,y*2+2,z*2+1],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2,y*2+2,z*2+1],ref triangles);
						}break;
					case 10 :{
							nodeDisp = new Vector3(0,0,Interpolate(density[x+1,y+1,z],density[x+1,y+1,z+1]));
							AddVerticle(ref nodes[x*2+2,y*2+2,z*2+1],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+2,y*2+2,z*2+1],ref triangles);
						}break;
					case 11 :{
							nodeDisp = new Vector3(0,0,Interpolate(density[x+1,y,z],density[x+1,y,z+1]));
							AddVerticle(ref nodes[x*2+2,y*2,z*2+1],nodeDisp,ref vertices,LoD);
							AddTrianglePart(nodes[x*2+2,y*2,z*2+1],ref triangles);
						}break;
					default: Debug.Log("Error!!! Can't create cell! Wrong parametres from the table!");
						break;
					}

				}
		
		}

		//If this vertice isn't assign to the mesh, assign it.
		private static void AddVerticle(ref VerticeNode node,Vector3 nodeDisp,ref List<Vector3> vertices,float Lod){
			if(node.verticeIndex != -1)
				return;
			vertices.Add(node.position+nodeDisp/Lod);
			node.verticeIndex = vertices.Count-1;
		}

		//Add one vertice to the triangle vertice list
		private static void AddTrianglePart(VerticeNode node,ref List<int> triangles){
			triangles.Add(node.verticeIndex);
		}

		//Just a math method :)
		private static float Interpolate(float a, float b){
			//return (a+b)/2;
			a = Mathf.Abs(a);
			b = Mathf.Abs(b);
			return a/(a+b)-0.5f;
		}

		/// <summary>
		/// Describes vertice with its position
		/// and current index in mesh.
		/// </summary>
		struct VerticeNode{
			public Vector3 position;
			public int verticeIndex;
		}

		///<summary>
		/// Stores TerrainGenerator answer.
		/// </summary>
		public struct Answer{
			public Chunk chunk;
			public List<Vector3> vertices;
			public List<int> triangles;
		}
	}
}