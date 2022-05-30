Shader "Unlit/GrassShader"
{
    Properties
    {
        _FloorColor("Floor Color",Color) = (1,1,1)
        _Factor("factor",float) = 1
        _TopColor("TopColor",Color) = (1,1,1)
        _BottomColor("TopColor",Color) = (1,1,1)
        _BendRotationRandom("Bend Rotation Random",Range(0,1)) = 0.2

        _BladeWidth("Blade Width",float) = 0.5
        _BladeWidthRandom("Blade Width random",float) = 0.02
        _BladeHeight("Blade Height",float) = 0.5
        _BladeHeightRandom("Blade Height Random",float) = 0.3

        _WindDistortionMap("Wind Distortion Map",2D) = "white" {}
        _WindFrequency("Wind Frequency",Vector)=(0.05,0.05,0,0)

        _WindStrength("Wind Strength",float) = 1

        _BladeForward("Blade Forward Amount",float) = 0.38
        _BladeCurve("Blade Curvature Amount",Range(1,4)) = 2
        
        _ShadowStrength("shadow strenght",Range(-5,5))=0

        _TranslucentGain("Translucent Gain",float) = 0.5
        _ColorLightInpact("Color Light Inpact",float) = 0.5
        
    }
    CGINCLUDE

        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"
        #pragma multi_compile _SHADOWS_SCREEN
        #pragma multi_compile_fwdbase_fullforwardshadows

        #define  BLADE_SEGMENTS 3

        struct v2g
        {
            float4 vertex : SV_POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;

        };

        struct g2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
            unityShadowCoord4 _ShadowCoord : TEXCOORD1;
        };

        float3 _FloorColor;
        float _Factor;
        float3 _BottomColor;
        float3 _TopColor;
        float _BendRotationRandom;

        sampler2D _WindDistortionMap;
        float4 _WindDistortionMap_ST;

        float2 _WindFrequency;

        float _WindStrength;

        float _BladeWidth;
        float _BladeWidthRandom;
        float _BladeHeight;
        float _BladeHeightRandom;
        float _BladeForward;
        float _BladeCurve;
        float _ShadowStrength;

        // Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
        // Extended discussion on this function can be found at the following link:
        // https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
        // Returns a number in the 0...1 range.
        float rand(float3 co)
        {
            return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
        }

        // Construct a rotation matrix that rotates around the provided axis, sourced from:
        // https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
        float3x3 AngleAxis3x3(float angle, float3 axis)
        {
            float c, s;
            sincos(angle, s, c);

            float t = 1 - c;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;

            return float3x3(
                t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                t * x * z - s * y, t * y * z + s * x, t * z * z + c
                );
        }


        v2g vert(appdata_full  v)
        {
            v2g o;
            o.vertex = (v.vertex);
            o.normal = v.normal;
            o.tangent = v.tangent;


            return o;
        }


        g2f VertexOutput(float3 pos, float2 uv,float3 normal)
        {
            g2f o;
            o.pos = UnityObjectToClipPos(pos);
            o.uv = uv;
            o._ShadowCoord = ComputeScreenPos(o.pos);
            o.normal = UnityObjectToWorldNormal( normal);
            
            #if UNITY_PASS_SHADOWCASTER
	            // Applying the bias prevents artifacts from appearing on the surface.
	            o.pos = UnityApplyLinearShadowBias(o.pos);
             #endif
            return o;
        }

        g2f GenerateGrassVertex(float3 vertexPosition, float width, float height, float forward, float2 uv, float3x3 transformationMatrix)
        {
            float3 tangentPoint = float3(width, forward, height);
            float3 localPosition = vertexPosition + mul(transformationMatrix, tangentPoint);
            float3 tangentNormal = normalize(float3(0,-1,0));
            float3 loclaNormal = mul(transformationMatrix,tangentNormal);
   
            return VertexOutput(localPosition, uv,loclaNormal);

        }

    
        [maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
        void geom(point v2g input[1], inout TriangleStream<g2f> triStream)
        {
            
            float3 pos = input[0].vertex;
            float3 vNormal = input[0].normal;
            float4 vTangent = input[0].tangent;
            float3 vBinormal = normalize(cross(vNormal, vTangent));
            float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
            float3x3 bendRotationMatrix = AngleAxis3x3(rand(pos.zzx) * _BendRotationRandom * UNITY_PI * 0.5, float3(-1, 0, 0));

            float3x3 tangentToLocal = float3x3(
                vTangent.x, vBinormal.x, vNormal.x,
                vTangent.y, vBinormal.y, vNormal.y,
                vTangent.z, vBinormal.z, vNormal.z
                );

            g2f o;

            facingRotationMatrix = mul(tangentToLocal, facingRotationMatrix);

            float2 uv = pos.xz * _WindDistortionMap_ST.xy + _WindDistortionMap_ST.zw + _WindFrequency * _Time.y;
            float2 windSample = (tex2Dlod(_WindDistortionMap, float4(uv, 0, 0)).xy * 2 - 1) * _WindStrength;

            float3 wind = normalize(float3(windSample.x, windSample.y, 0));
            float3x3 windRotation = AngleAxis3x3(UNITY_PI * windSample, wind);
            float3x3 transformationMatrixFacing = mul(tangentToLocal, facingRotationMatrix);
            float3x3 transformationMatrix = mul(mul(facingRotationMatrix, bendRotationMatrix), windRotation);

            float height = (rand(pos.zyx) * 2 - 1) * _BladeHeightRandom + _BladeHeight;
            float width = (rand(pos.xzy) * 2 - 1) * _BladeWidthRandom + _BladeWidth;
            float forward = rand(pos.yyz) * _BladeForward;


            for (int i = 0; i < 3; i++)
            {
                float t = i / (float)BLADE_SEGMENTS;

                float segmentHeight = height * t;
                float segementWidth = width * (1 - t);
                float segmentForward = pow(t, _BladeCurve) * forward;
                float3x3 transformMatrix = i == 0 ? transformationMatrixFacing : transformationMatrix;

                triStream.Append(GenerateGrassVertex(pos, segementWidth, segmentHeight, segmentForward, float2(0, t), transformMatrix));
                triStream.Append(GenerateGrassVertex(pos, -segementWidth, segmentHeight, segmentForward, float2(1, t), transformMatrix));

            }
            
            triStream.Append(GenerateGrassVertex(pos, 0, height, forward, float2(0.5, 1), transformationMatrix));
             TRANSFER_VERTEX_TO_FRAGMENT(o);
        }

    ENDCG

    SubShader
    {
        Tags { 
            
            "RenderType"="Opaque" 
            "LightMode" = "ForwardBase"
        }

        Pass
        {
            Cull Off     

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"

	        float _TranslucentGain;
            float _ColorLightInpact;
            
            float4 frag(g2f i,fixed facing : VFACE) : SV_Target
            {
                float3 normal = facing > 0 ? i.normal : - i.normal;
                float atten =   SHADOW_ATTENUATION(i);
                float NdotL = ((1-dot(normal,_WorldSpaceLightPos0)) + _TranslucentGain ) * atten   ;

                //NdotL = step(0.5,NdotL);
               
                float3 ambient = ShadeSH9(float4(normal, 1));
                float3 diffuse ;
                float3 color = lerp(_BottomColor,_TopColor,i.uv.y);
                diffuse = lerp(color-_ShadowStrength,color,NdotL);

                //return float4(normal * 0.5 + 0.5,1);
                return  float4(atten.xxx,0);
            }
            ENDCG
        }

         Pass // shadow pass
            {
                Cull Off
                
                Tags
                {
                    "LightMode" = "ShadowCaster"
                }
                
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma geometry geom
                    #pragma fragment frag

                    #pragma target 4.6
                    #pragma multi_compile_fwdbase
                    #pragma multi_compile_shadowcaster
                  
                    float4 frag(g2f i) : SV_Target
                    {
                        SHADOW_CASTER_FRAGMENT(i)
                    }
                    ENDCG
            }

    }
        Fallback "VertexLit"
}
