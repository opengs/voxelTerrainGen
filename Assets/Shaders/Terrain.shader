Shader "Custom/Terrain" {
	Properties {

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;


		float minHeight = 0;
		float maxHeight = 32;

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float InverseLerp(float a, float b, float value){
			return saturate((value-a)/(b-a));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = InverseLerp(minHeight,maxHeight,IN.worldPos.y);
			o.Albedo = float3(saturate(IN.worldPos.y/64),saturate(IN.worldPos.y/64),saturate(IN.worldPos.y/64));
			//o.Albedo = float3(0.5,0.5,0.5);

		}
		ENDCG
	}
	FallBack "Diffuse"
}
