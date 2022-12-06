Shader "TohuBohu/TerrainShader"
{
    Properties
    {
        _FrontTex ("FrontTex", 2D) = "white" {}
        _TopTex ("TopTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _FrontTex;
            float4 _FrontTex_ST;

            sampler2D _TopTex;
            float4 _TopTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal( v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 front = tex2D(_FrontTex, i.uv*_FrontTex_ST.xy + _FrontTex_ST.zw);
                fixed4 top = tex2D(_TopTex, i.uv*_TopTex_ST.xy + _TopTex_ST.zw);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float3 up = float3(0,1,0);

                float NdL = dot(up,i.normal);

                float3 col = lerp(top,front,NdL);
                
                return float4(col,1.0);
            }
            ENDCG
        }
    }
}
