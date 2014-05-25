Shader "GrassOnMeshGenerator/BillboardWavingDoublePass" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
		_CameraPosition ("Camera Position", Vector) = (0, 0, 0, 1) // .xyz = camera position, .w = 1 / (max sqr distance)
		_CameraRight ("Camera Right", Vector) = (1, 0, 0, 0)
		_CameraUp ("Camera Up", Vector) = (0, 1, 0, 0)
		_Cutoff ("Cutoff", float) = 0.5
		_Cutoff2 ("Cutoff2", float) = 0.5
	}
	
CGINCLUDE
	#include "UnityCG.cginc"
	#include "GrassOnMeshGenerator.cginc"
	#pragma glsl_no_auto_normalization

	struct v2f {
		float4 pos : POSITION;
		fixed4 color : COLOR;
		float4 uv : TEXCOORD0;
	};
	v2f BillboardVert (appdata_full v) {
		v2f o;
		WavingGrassOMGBillboardVert (v);
		o.color = v.color;
		
		o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);
			
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
		o.uv = v.texcoord;
		return o;
	}
ENDCG

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}
		Cull Off
		LOD 200
		ColorMask RGB
				
		CGPROGRAM
			#pragma surface surf Lambert vertex:WavingGrassOMGBillboardVert addshadow alphatest:_Cutoff2
			#pragma exclude_renderers flash
						
			sampler2D _MainTex;
			fixed _Cutoff;

			struct Input {
				float2 uv_MainTex;
				fixed4 color : COLOR;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
				fixed4 d = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = d.a;
				clip (o.Alpha - _Cutoff);
			}
		ENDCG			
	}

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}

		ColorMask RGB
		Cull Off
		Lighting On
		
		Pass {
			CGPROGRAM
			#pragma vertex BillboardVert
			#pragma exclude_renderers shaderonly
			ENDCG

			AlphaTest Greater [_Cutoff]

			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture * primary }
		}
	}
	
	Fallback Off
}
