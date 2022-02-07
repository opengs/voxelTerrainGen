/// <summary>
/// Planets generator.
/// Used to generate random planet properies such as:
/// radius, octaves, terraces ...
/// 
/// Versions:
/// [v0.1] Created. PlanetRequest returns static value.
/// </summary>
using UnityEngine;
namespace VoxelPlanets{
	public static class PlanetsGenerator{
		/// <summary>
		/// Generate and return randomized planet properties.
		/// </summary>
		/// <returns>Planet properties</returns>
		public static Planet RequestPlanet(){


		//TODO: Randomize planet generation

		//1 type return
		float radius = 50f;
		Octave[] octaves = new Octave[]{
			//new Octave(0.001f,20f),
			//new Octave(0.02f,10f),
			new Octave(0.04f,10f),
			//new Octave(0.1f,1f),
			//new Octave(1f,0.1f),
			//new Octave(10f,0.01f)
		};
		Terrace[] terraces = new Terrace[]{
		//	new Terrace(5,2f),
		//	new Terrace(8,7f)
		};
		
		Planet planet = new Planet(radius,octaves,terraces);
		//

		return planet;
	}

		/// <summary>
		/// Contains default planet properties.
		/// </summary>
	public class Planet{
		public float Radius;
		public Octave[] Octaves;
		public Terrace[] Terraces;
		public Planet(float Radius, Octave[] Octaves, Terrace[] Terraces){
			this.Radius = Radius;
			this.Octaves = Octaves;
			this.Terraces = Terraces; 
		}
	}

		/// <summary>
		/// Used to add more details to the terrain.
		/// Store details laccunarity and influece(weight).
		/// </summary>
		public struct Octave{
			public float lacunarity;
			public float weight;
				
			public Octave(float lacunarity, float weight){
				this.lacunarity = lacunarity;
				this.weight = weight;
			}
		}

		/// <summary>
		/// Hard Floor on defined height
		/// with defined influence on terran.
		/// </summary>
		public struct Terrace{
			public float height;
			public float weight;

			public Terrace(float height, float weight){
				this.height = height;
				this.weight = weight;
			}
		}
	}
}
