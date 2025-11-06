Shader "Hidden/ScreenFade"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,1)
		_MainTex("Texture", 2D) = "white" {}
		_FadeAmount("Screen Fade Amount", Range(0, 1)) = 0
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					UNITY_VERTEX_OUTPUT_STEREO //Insert
				};

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v); //Insert
					UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				//sampler2D _MainTex;
				UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
				float4 _MainTex_ST;
				float _FadeAmount;
				uniform float4 _Color;

				fixed4 frag(v2f i) : SV_Target
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert
					float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
					fixed4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv); //Insert
					//fixed4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
					col.rgb = lerp(col,_Color,_FadeAmount);
					return col;
				}
				ENDCG
			}
		}
}
