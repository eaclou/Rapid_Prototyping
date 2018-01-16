Shader "Hidden/cabBlitShader"
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
			
			float2 _Pixels;
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				_Pixels = float2(8, 8);
				fixed2 uv = round(i.uv * _Pixels) / _Pixels;
				half s = 1 / _Pixels;

				// Neighbour cells
				float cl = round(tex2D(_MainTex, i.uv + fixed2(-s,  0)).x);	// F[x-1, y  ]: Centre Left
				float tc = round(tex2D(_MainTex, i.uv + fixed2( 0, -s)).x);	// F[x,   y-1]: Top Centre
				float cc = round(tex2D(_MainTex, i.uv + fixed2( 0,  0)).x);	// F[x,   y  ]: Centre Centre
				float bc = round(tex2D(_MainTex, i.uv + fixed2( 0, +s)).x);	// F[x,   y+1]: Bottom Centre
				float cr = round(tex2D(_MainTex, i.uv + fixed2(+s,  0)).x);	// F[x+1, y  ]: Centre Right

				float val = (cl + tc + cc + bc + cr) / 5;
				//float val = cc;

				fixed4 col = fixed4(val, val, val, 1);
				//col = fixed4(i.uv, 0, 1);
				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col;
				return col;
			}
			ENDCG
		}
	}
}
