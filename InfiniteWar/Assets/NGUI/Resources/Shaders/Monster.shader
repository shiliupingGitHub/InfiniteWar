Shader "Custom/Monster" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("_Color",Color) = (1.0,1.0,1.0,1.0)
	_Cutout ("Alpha cutoff", Range(0,1)) = 1.0
	_Size("Size",float) = 1.0
}

SubShader {
	Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
	Cull back
	
	ZTest Lequal
	ZWrite on
	Fog { Mode Off }
	CGPROGRAM
	#include "UnityCG.cginc"
	#pragma fragment frag
	#pragma vertex vert
		
		sampler2D _MainTex;
		fixed4 _Color;
		half4 _MainTex_ST;
		float _Cutout;
		float _Size;
		struct appdata_t {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};
		appdata_t vert (appdata_t v)
		{
			appdata_t o;
			
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}
		
		fixed4 frag (appdata_t i) : COLOR
		{
			fixed4 c = tex2D (_MainTex, i.texcoord) * _Color;			
			if (c.a < _Cutout && _Cutout < 0.95)
				discard;

			return c;

				/*			
			if(c.a < 0.001  && _Cutoff < 0.4)
	 			discard;
			c *= _Color;	
			return c;
			*/
		}
	ENDCG
		
	}

}
}
