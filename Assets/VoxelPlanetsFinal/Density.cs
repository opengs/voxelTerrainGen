/// <summary>
/// DensityGenerator.
/// Generates density for defined point
/// using input parametres.
/// </summary>
/// 
/// Versions:
/// [v1.0] Created.

using UnityEngine;
using System;
using System.Collections;
namespace VoxelPlanets{
	public static class Density{
		const bool RoundPlanet = false;

		/// <summary>
		/// Get density in point.
		/// </summary>
		/// <returns>The density.</returns>
		public static float GetDensity(Vector3 point, Vector3 planetCenter, PlanetsGenerator.Planet planet){
			//Creates sphere with radius=planet.radius  and center in planetCenter;
			float density;
			if(RoundPlanet)
				density = planet.Radius - (point-planetCenter).magnitude;
			else
				density = -point.y;
			//Apply octaves
			for(int i=0,n=planet.Octaves.Length;i<n;i++){
				density+=Noise.Generate(point*planet.Octaves[i].lacunarity)*planet.Octaves[i].weight;
			}

			//Generate terraces
			foreach(PlanetsGenerator.Terrace terrace in planet.Terraces){
				if(RoundPlanet)
					density+=Mathf.Clamp((terrace.height - (point-planetCenter).magnitude)*3,0f,1f)*terrace.weight;
				else
					density+=Mathf.Clamp((terrace.height - point.y)*3,0f,1f)*terrace.weight;
			}
				
			return density;
		}
	}
}
