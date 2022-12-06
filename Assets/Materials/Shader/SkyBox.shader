Shader "TohuBohu/SkyBox"
{
    Properties
    {
        _TopColor ("TopColor", Color) = (0,0,0)
        _BottomColor ("BottomColor", Color) = (0,0,0)
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float3 _TopColor;
            float3 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              
                float3 col = lerp(_BottomColor,_TopColor,i.uv.y);
                return float4(col,1.0);
            }
            ENDCG
        }
    }
}
