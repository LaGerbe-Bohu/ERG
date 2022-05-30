// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ShaderDebugPA"
{
    Properties
    {
        _semiMajorAxis ("semiMajorAxis", range(-1,1)) =0
        _semiMinorAxis("_semiMinorAxis", range(-1,1)) = 0
        _angle("Angle",float)=0
        _ColorOfEllipse("",Color) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue="="Transparent" }
  

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            fixed3 _centerEllipse;
            fixed3 _centerOfMass;
            fixed4 _ColorOfEllipse;
            
            float _semiMajorAxis;
            float _semiMinorAxis;
            float _angle;
            
            int isInEllipse(float2 pos,float2 center)
            {
              
                int val = pow(center.x - pos.x,2)/pow(_semiMajorAxis,2) +  pow(center.y - pos.y,2)/pow(_semiMinorAxis,2) <= 1;
                return val;
            }


            // (a²*sin²d + b²*cos²d) * x² + 2(b²-a²)*sin(d)*cos(d)*x*y + (a²*cos²d + b²*sin²d)*y² == a²*b²
            int RotatedEllipse(fixed2 pos,fixed2 center,fixed2 d)
            {

                float2 origin = float2(center.x - pos.x,center.y - pos.y);
            
                
               return (pow(_semiMajorAxis,2)*pow(sin(d),2) + pow(_semiMinorAxis,2)*pow(cos(d),2))* pow(origin.x,2) + 2*(pow(_semiMinorAxis,2)-pow(_semiMajorAxis,2))
                *sin(d)*cos(d)*origin.x*origin.y + (pow(_semiMajorAxis,2) * pow(cos(d),2) + pow(_semiMinorAxis,2)*pow(sin(d),2) )*pow(origin.y,2) <= pow(_semiMajorAxis,2)*pow(_semiMinorAxis,2);

           
            }
            
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldPos :TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 postest :TEXCOORD2;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xz;
                o.postest = UnityObjectToClipPos(_centerEllipse);
                o.uv = v.uv;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 uv = (i.worldPos- 0.5)*2;

                //float draw =pow(saturate(1-distance(i.worldPos,_centerEllipse.xz)),100);


                float4 v =  (distance(i.worldPos,_centerOfMass.xz) < 0.005 ) * float4(1,0,1,1) ;
                fixed val = RotatedEllipse(i.worldPos,_centerEllipse.xz,_angle * (UNITY_PI/180) ) ;

                fixed4 ellipse = val * _ColorOfEllipse;
                ellipse = lerp(ellipse,v,v.x);

                float t = pow(0.5,2);
                
                return fixed4(ellipse.xyzw);
            }
            ENDCG
        }
    }
}
