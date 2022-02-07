/*using UnityEngine;
using System.Collections;

public class Destiny{
	static float scale = 5;
	public static float GetDestiny(Vector3 point,Vector3 center,PlanetsGenerator.Planet planetType){
		
		//float destiny = -point.y +1;
		///destiny = 40 - (point-new Vector3(64,45,64)).magnitude;
		destiny+=Mathf.Abs(Noise.Generate(point/1000)*30f);
		destiny+=Mathf.Abs(Noise.Generate(point/200)*15f);
		destiny+=Noise.Generate(point/40)*6f;
		destiny+=Noise.Generate(point/10)*1f;
		destiny+=Noise.Generate(point)*0.1f;
		//destiny+=Mathf.Clamp((50-(point-new Vector3(64,64,64)).magnitude)*3,0f,1f)*1.5f;
		destiny+=Mathf.Clamp((6-point.y)*3,0f,1f)*2;
		//destiny+=Noise.Generate(point.x/1.01f,point.y/1.01f,point.z/1.01f)*1.0f;

	
		destiny+=Noise.Generate(point.x,point.y,point.z)*0.5f;	
		float destiny = planetType.radius - (point-center).magnitude;
		foreach(PlanetsGenerator.Octave octave in planetType.octaves){
			destiny+=Noise.Generate(point*octave.lacunarity)*octave.weight;
		}
		foreach(PlanetsGenerator.Cascade cascade in planetType.cascades){
			destiny+=Mathf.Clamp((cascade.height - (point-center).magnitude)*3,0f,1f)*cascade.weight;
		}

		return destiny;
	}
}
*/