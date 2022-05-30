Shader "Custom/GrassShader"
{
    Properties
    {
        _HeightGrasse("Taille du brin", range(0.001,1)) = 1
        _WidthGrasse("largeur de l'herbe",range(0.001,1))= 1
        _NomberOfDetail("nombre de division de brin",int)=1
        _waveTexture ("Texture Wind", 2D) = "" {}
        _speedWind ("Vitesse du vent", float) = 1
        _WindForce ("Force du vent", float) = 1
        _bladeWind("torsion de certain blade",float) = 1 
        
        _grassTexture ("Grass texture", 2D) = "white" {}
        _Color("Color",Color) = (0,0,0)
        
    }
    
    CGINCLUDE
    #include "UnityCG.cginc" 
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    #pragma multi_compile _SHADOWS_SCREEN
    #pragma multi_compile_fwdbase_fullforwardshadows

              float _HeightGrasse;
            float _WidthGrasse;
            float _NomberOfDetail;
            float _WindForce;
            
            float4 _waveTexture_ST;
            sampler2D _waveTexture;

            float _speedWind;

            float _bladeWind;

            
            float rand(float3 co)
            {
                return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
            }

            float3x3 AngleAxis3x3(float angle, float3 axis)
            {
                float c, s;
                sincos(angle, s, c);

                float t = 1 - c;
                float x = axis.x;
                float y = axis.y;
                float z = axis.z;

                return float3x3(
                    t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
                    t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
                    t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
                );
            }
            
            struct appdata
            {
             
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float4 tangent : TANGENT;

            };

            struct v2g
            {
        
                float4 vertex : SV_POSITION;
                float4 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct g2f
            {
            	float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                unityShadowCoord4 _ShadowCoord : TEXCOORD1;
                
            };



            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = (v.vertex);
                o.normal = v.normal;
                o.tangent = v.tangent;
             
                return o;
            }


            g2f createPoint(float3 position,float2 uv)
            {
                g2f g;

				g.normal =             	
                g.vertex = UnityObjectToClipPos(position);
                g.uv = uv;
                g._ShadowCoord = ComputeScreenPos(g.vertex);

                   #if UNITY_PASS_SHADOWCASTER
	                // Applying the bias prevents artifacts from appearing on the surface.
	                g.vertex = UnityApplyLinearShadowBias(g.vertex);
                 #endif
                
                return g;
            }
            

            [maxvertexcount(30)]
            void geom(point  v2g inputs[1], inout TriangleStream<g2f> triStream)
            {
                float4 originVertex =inputs[0].vertex;

                float2 uv = originVertex.xy * _waveTexture_ST.xy + _waveTexture_ST.zw + _speedWind *_Time.x;
                float2 waveTexture = (tex2Dlod(_waveTexture, float4(uv,0,0)).xy*2 - 1 )* _WindForce;
                float3 windDirection  = normalize(float3(waveTexture.xy,0));

                float3x3 wind = AngleAxis3x3(waveTexture * UNITY_TWO_PI,windDirection);
        
                
                float4 normal = inputs[0].normal;
                float4 tangent = inputs[0].tangent;
                float3 bnormal = cross(normal,tangent) * tangent.w;

                float3x3 matriceLocal = float3x3(
                tangent.x,bnormal.x,normal.x,
                tangent.y,bnormal.y,normal.y,
                tangent.z,bnormal.z,normal.z
                );

         
                float3x3 matrice = matriceLocal;
                
                float3x3 rotation = AngleAxis3x3(rand(originVertex) * UNITY_TWO_PI,float3(0,0,1));

                
                float3x3 blandeRotation = AngleAxis3x3(rand(originVertex.xxy) * _bladeWind * UNITY_PI * 0.5, float3(-1, 0, 0));
                

     
                
                matrice = mul(matrice,wind);
                matrice = mul(matrice, rotation);
                matrice = mul(matrice,blandeRotation);
                

            
                float4 pos;
                g2f p;
               
                float3 tangentSpace;

               for(fixed i = 0; i <=_HeightGrasse; i+= (fixed)_HeightGrasse/(fixed)_NomberOfDetail)
                {
                    for(fixed j = 0; j <=_WidthGrasse; j+=_WidthGrasse)
                    {

                        tangentSpace = float3(0,pow(i,2)*1,i);

                        
                        p._ShadowCoord = originVertex;
                        p =  createPoint( originVertex + mul(matrice, float3(j - (_WidthGrasse/2),tangentSpace.y,i)),float2(j/_WidthGrasse,i/_HeightGrasse));
                        triStream.Append(p);
                    }
                }

                
             
                
                
            }

    
    ENDCG
    
    SubShader
    {
        Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True"  }


        Pass
        {
            Cull Off
            Tags {"LightMode" = "ForwardBase" }
            AlphaToMask On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            

            float3 _Color;


            
            float4 _grassTexture_ST;
            sampler2D _grassTexture;
            
            
            fixed4 frag (g2f i) : SV_Target
            {
                float2 uv = i.uv*2 - 1;
                float dist = length(uv - float2(0,0));
                float atten =   SHADOW_ATTENUATION(i);
                float3 grassTexture = tex2D( _grassTexture, i.uv *_grassTexture_ST.xy + _grassTexture_ST.zw );
                float3 color = lerp(_Color - .3,_Color,i.uv.y);

                float v = sqrt(pow(uv.x,2) + pow(uv.y,2) );
                 v = smoothstep(.99,1,v);

                if(uv.y < 0.2) v = 0;

                v = 1-v;

                if(atten.x <= 0)
                {
                    color = _Color - 0.3;
                }

                
                return float4(color,v.x);
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
