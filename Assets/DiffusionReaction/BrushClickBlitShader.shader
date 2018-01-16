Shader "Hidden/BrushClickBlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float _CoordX;
			float _CoordY;
			float _PaintA;
			float _PaintB;
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 mouseCoords = float2(_CoordX, _CoordY);

				float2 pixToMouse = i.uv - mouseCoords;
				float distToMouse = length(pixToMouse);

				fixed4 col = tex2D(_MainTex, i.uv);

				if(distToMouse < 0.05) {
					col.y = _PaintA;
					col.x = _PaintB;
				}
				// just invert the colors
				//col = 1 - col;
				return col;
			}
			ENDCG
		}
	}
}
