Shader "URP/TriplanarConcrete_Lite"
{
    Properties
    {
        _BaseColorTint ("Base Color Tint", Color) = (1,1,1,1)

        _AlbedoTex ("Base (Albedo)", 2D) = "white" {}
        _NormalTex ("Normal", 2D) = "bump" {}

        _Tiling ("Tiling (tex repeats per meter)", Float) = 1.0
        _BlendSharpness ("Triplanar Blend Sharpness", Range(1,8)) = 6.0
        _NormalStrength ("Normal Strength", Range(0,2)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness (base)", Range(0,1)) = 0.25
        _Occlusion ("Occlusion (base)", Range(0,1)) = 1.0

        // Secondary Albedo blend
        _AlbedoTex2 ("Secondary Albedo", 2D) = "white" {}
        _BlendNoise ("Blend Noise", 2D) = "gray" {}
        _BlendScale ("Blend Scale", Float) = 2.0
        _BlendStrength ("Blend Strength", Range(0,1)) = 0.5

        // Roughness map, can blend with base Smoothness
        _UseRoughnessTex ("Use Roughness Map", Float) = 1
        _RoughnessTex ("Roughness (white=rough / black=smooth, linear)", 2D) = "white" {}
        _RoughnessInvert ("Invert to Smoothness (1-rough)", Float) = 1
        _RoughnessStrength ("Roughness Influence", Range(0,1)) = 0.4
        _RoughnessGamma    ("Roughness Gamma",   Range(0.3,3)) = 1.4
        _RoughnessRemapMin ("Roughness In Min",  Range(0,1))   = 0.2
        _RoughnessRemapMax ("Roughness In Max",  Range(0,1))   = 0.8

        // AO Map
        _UseAOTex ("Use AO Map", Float) = 1
        _AOTex ("Ambient Occlusion (linear)", 2D) = "white" {}
        _AOIntensity ("AO Intensity", Range(0,2)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP

            // WebGL specific defines
            #if defined(UNITY_WEBGL)
                #define WEBGL_BUILD
            #endif

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            float3 _WorldAnchor;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColorTint;
                float _Tiling;
                float _BlendSharpness;
                float _NormalStrength;
                float _Metallic;
                float _Smoothness;
                float _Occlusion;

                float _BlendScale;
                float _BlendStrength;

                // Roughness
                float _UseRoughnessTex;
                float _RoughnessInvert;
                float _RoughnessStrength;
                float _RoughnessGamma;
                float _RoughnessRemapMin;
                float _RoughnessRemapMax;

                // AO
                float _UseAOTex;
                float _AOIntensity;
            CBUFFER_END

            // Textures
            TEXTURE2D(_AlbedoTex);   SAMPLER(sampler_AlbedoTex);
            TEXTURE2D(_NormalTex);   SAMPLER(sampler_NormalTex);
            TEXTURE2D(_AlbedoTex2);  SAMPLER(sampler_AlbedoTex2);
            TEXTURE2D(_BlendNoise);  SAMPLER(sampler_BlendNoise);
            TEXTURE2D(_RoughnessTex); SAMPLER(sampler_RoughnessTex);
            TEXTURE2D(_AOTex);        SAMPLER(sampler_AOTex);

            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float4 tangentOS:TANGENT; float2 uv0:TEXCOORD0; };
            struct Varyings {
                float4 positionHCS:SV_POSITION;
                float3 positionWS:TEXCOORD0;
                float3 normalWS:TEXCOORD1;
                float  fogCoord:TEXCOORD2;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV,vertexSH,3);
                float3 viewDirWS:TEXCOORD4;
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    float4 shadowCoord:TEXCOORD5;
                #endif
            };

            float3 TriplanarWeights(float3 nWS, float sharp){ float3 w=pow(abs(nWS),sharp); return w/(w.x+w.y+w.z+1e-5); }
            float2 ProjectX(float3 p){ return p.zy; } float2 ProjectY(float3 p){ return p.xz; } float2 ProjectZ(float3 p){ return p.xy; }

            float4 Sample_AlbedoA(float3 pos,float3 w){
                float2 uvX=ProjectX(pos)*_Tiling, uvY=ProjectY(pos)*_Tiling, uvZ=ProjectZ(pos)*_Tiling;
                float4 cx=SAMPLE_TEXTURE2D(_AlbedoTex,sampler_AlbedoTex,uvX);
                float4 cy=SAMPLE_TEXTURE2D(_AlbedoTex,sampler_AlbedoTex,uvY);
                float4 cz=SAMPLE_TEXTURE2D(_AlbedoTex,sampler_AlbedoTex,uvZ);
                return cx*w.x+cy*w.y+cz*w.z;
            }
            float4 Sample_AlbedoB(float3 pos,float3 w){
                float2 uvX=ProjectX(pos)*_Tiling, uvY=ProjectY(pos)*_Tiling, uvZ=ProjectZ(pos)*_Tiling;
                float4 cx=SAMPLE_TEXTURE2D(_AlbedoTex2,sampler_AlbedoTex2,uvX);
                float4 cy=SAMPLE_TEXTURE2D(_AlbedoTex2,sampler_AlbedoTex2,uvY);
                float4 cz=SAMPLE_TEXTURE2D(_AlbedoTex2,sampler_AlbedoTex2,uvZ);
                return cx*w.x+cy*w.y+cz*w.z;
            }

            float3 UnpackNormalRGorAG(float4 c){ float3 n; n.xy=c.wy*2-1; n.z=sqrt(saturate(1-dot(n.xy,n.xy))); return n; }
            float3 NormalFromAxis(float3 nTS,uint axis){
                float3 T,B,N; if(axis==0){T=float3(0,0,1);B=float3(0,1,0);N=float3(1,0,0);}
                else if(axis==1){T=float3(1,0,0);B=float3(0,0,1);N=float3(0,1,0);}
                else {T=float3(1,0,0);B=float3(0,1,0);N=float3(0,0,1);}
                return normalize(T*nTS.x + B*nTS.y + N*nTS.z);
            }
            float3 Sample_Normal(float3 pos,float3 w){
                float2 uvX=ProjectX(pos)*_Tiling, uvY=ProjectY(pos)*_Tiling, uvZ=ProjectZ(pos)*_Tiling;
                float3 nx=UnpackNormalRGorAG(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,uvX));
                float3 ny=UnpackNormalRGorAG(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,uvY));
                float3 nz=UnpackNormalRGorAG(SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,uvZ));
                float3 wx=NormalFromAxis(nx,0), wy=NormalFromAxis(ny,1), wz=NormalFromAxis(nz,2);
                float3 n=normalize(wx*w.x+wy*w.y+wz*w.z);
                return normalize(lerp(float3(0,0,1), n, _NormalStrength));
            }

            float Sample_Roughness(float3 pos,float3 w){
                float2 uvX=ProjectX(pos)*_Tiling, uvY=ProjectY(pos)*_Tiling, uvZ=ProjectZ(pos)*_Tiling;
                float rx=SAMPLE_TEXTURE2D(_RoughnessTex,sampler_RoughnessTex,uvX).r;
                float ry=SAMPLE_TEXTURE2D(_RoughnessTex,sampler_RoughnessTex,uvY).r;
                float rz=SAMPLE_TEXTURE2D(_RoughnessTex,sampler_RoughnessTex,uvZ).r;
                return rx*w.x + ry*w.y + rz*w.z;
            }
            float Sample_AO(float3 pos,float3 w){
                float2 uvX=ProjectX(pos)*_Tiling, uvY=ProjectY(pos)*_Tiling, uvZ=ProjectZ(pos)*_Tiling;
                float ax=SAMPLE_TEXTURE2D(_AOTex,sampler_AOTex,uvX).r;
                float ay=SAMPLE_TEXTURE2D(_AOTex,sampler_AOTex,uvY).r;
                float az=SAMPLE_TEXTURE2D(_AOTex,sampler_AOTex,uvZ).r;
                return ax*w.x + ay*w.y + az*w.z;
            }

            struct Varyings vert (Attributes v){
                Varyings o; VertexPositionInputs p=GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs n=GetVertexNormalInputs(v.normalOS, v.tangentOS);
                o.positionHCS=p.positionCS; o.positionWS=p.positionWS; o.normalWS=n.normalWS;
                o.viewDirWS=GetWorldSpaceViewDir(p.positionWS); o.fogCoord=ComputeFogFactor(p.positionCS.z);
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    o.shadowCoord=GetShadowCoord(p);
                #endif
                OUTPUT_LIGHTMAP_UV(v.uv0, unity_LightmapST, o.lightmapUV);
                OUTPUT_SH(n.normalWS, o.vertexSH);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 weights = TriplanarWeights(normalize(i.normalWS), _BlendSharpness);
                float3 samplePos = i.positionWS - _WorldAnchor;

                // Albedo + secondary blend
                float4 colA = Sample_AlbedoA(samplePos, weights) * _BaseColorTint;
                float4 colB = Sample_AlbedoB(samplePos, weights);
                float  nz   = SAMPLE_TEXTURE2D(_BlendNoise, sampler_BlendNoise, samplePos.xz * _BlendScale).r;
                float4 baseCol = lerp(colA, colB, saturate(nz * _BlendStrength));

                float3 nWS = Sample_Normal(samplePos, weights);

                // Metallic
                float metallic = saturate(_Metallic);

                // Smoothness from Roughness map
                float smoothness = _Smoothness;
                if (_UseRoughnessTex > 0.5)
                {
                    float rough = Sample_Roughness(samplePos, weights);
                    float denom = max(1e-5, _RoughnessRemapMax - _RoughnessRemapMin);
                    rough = saturate((rough - _RoughnessRemapMin) / denom);
                    rough = pow(rough, _RoughnessGamma);
                    float toSmooth = (_RoughnessInvert > 0.5) ? (1.0 - rough) : rough;
                    smoothness = lerp(_Smoothness, toSmooth, _RoughnessStrength);
                }
                smoothness = saturate(smoothness);

                // AO
                float occlusion = _Occlusion;
                if (_UseAOTex > 0.5)
                {
                    occlusion = saturate(Sample_AO(samplePos, weights) * _AOIntensity);
                }

                // Build surface
                SurfaceData s; ZERO_INITIALIZE(SurfaceData, s);
                s.albedo     = baseCol.rgb;
                s.metallic   = metallic;
                s.specular   = 0;
                s.smoothness = smoothness;
                s.normalTS   = float3(0,0,1);
                s.occlusion  = occlusion;
                s.emission   = 0;
                s.alpha      = 1;
                s.clearCoatMask = 0;
                s.clearCoatSmoothness = 0;

                InputData input; ZERO_INITIALIZE(InputData, input);
                input.positionWS      = i.positionWS;
                input.normalWS        = nWS;
                input.viewDirectionWS = SafeNormalize(i.viewDirWS);
                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                    input.shadowCoord  = i.shadowCoord;
                #endif
                input.fogCoord        = i.fogCoord;
                input.vertexLighting  = 0;
                input.bakedGI         = SAMPLE_GI(i.lightmapUV, i.vertexSH, nWS);

                half4 col = UniversalFragmentPBR(input, s);
                col.rgb = MixFog(col.rgb, i.fogCoord);
                return col;
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
