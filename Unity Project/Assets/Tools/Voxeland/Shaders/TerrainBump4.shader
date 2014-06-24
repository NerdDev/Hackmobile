Shader "Voxeland/TerrainBump4"  
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse1(RGB)", 2D) = "gray" {}
		_BumpMap ("Bump1(CA)", 2D) = "bump" {}
		_MainTex2 ("Diffuse2(RGB)", 2D) = "gray" {}
		_BumpMap2 ("Bump2(CA)", 2D) = "bump" {}
		_MainTex3 ("Diffuse3(RGB)", 2D) = "gray" {}
		_BumpMap3 ("Bump3(CA)", 2D) = "bump" {}
		_MainTex4 ("Diffuse4(RGB)", 2D) = "gray" {}
		_BumpMap4 ("Bump4(CA)", 2D) = "bump" {}
		_Ambient ("Additional Ambient", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		
	} 


	SubShader 
	{
		CGPROGRAM
		
		#pragma surface surf BlinnPhong
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;        
			float3 worldPos;
			float3 screenPos;
			float4 color : Color;
		};
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MainTex2;
		sampler2D _BumpMap2; 
		sampler2D _MainTex3;
		sampler2D _BumpMap3;
		sampler2D _MainTex4;
		sampler2D _BumpMap4;
		half3 _Ambient;
		fixed4 _Color;
		half _Shininess;
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			//getting last color
			half4 color4 = 1-IN.color.r-IN.color.b-IN.color.g;
			
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex)*IN.color.r 
				+ tex2D(_MainTex2, IN.uv_MainTex)*IN.color.g 
				+ tex2D(_MainTex3, IN.uv_MainTex)*IN.color.b 
				+ tex2D(_MainTex4, IN.uv_MainTex)*color4;
			
			fixed4 norm = tex2D(_BumpMap, IN.uv_MainTex)*IN.color.r 
				+ tex2D(_BumpMap2, IN.uv_MainTex)*IN.color.g 
				+ tex2D(_BumpMap3, IN.uv_MainTex)*IN.color.b 
				+ tex2D(_BumpMap4, IN.uv_MainTex)*color4; 
			
			o.Normal = UnpackNormal(norm);

			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = tex.a;
			o.Alpha = tex.a * _Color.a;
			o.Specular = _Shininess;
			
			o.Emission = o.Albedo * IN.color.a * _Ambient;
		}
     
		ENDCG
    }
    
    Fallback "VertexLit"
}