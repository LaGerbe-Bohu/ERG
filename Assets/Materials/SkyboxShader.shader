Shader "Custom/SkyboxShader"
{
    Properties
    {
        _BottomColor ("BottomColor", color) = (0,0,0)
        _TopColor ("BottomColor", color) = (0,0,0)
        _Threshold("Threshold",range(-1,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
     

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


            float _Threshold;
            float3 _BottomColor;
            float3 _TopColor;

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

            fixed4 frag (v2f i) : SV_Target
            {
                float UV = saturate(  i.uv.y + _Threshold);
                fixed3 col = lerp(_BottomColor,_TopColor,UV);

                return float4(col,1);
            }
            ENDCG
        }
    }
}
