Shader "URP/Particles/FogVolumetric2DNoise"
{
    Properties
    {
        [MainColor] _Tint("Fog Tint (RGBA= density color+alpha)", Color) = (0.85,0.90,0.95,0.35)
        [MainTexture]_BaseMap("Optional Base Map (RGBA)", 2D) = "white" {}

        _NoiseTex("Main Noise (R)", 2D) = "white" {}
        _DetailTex("Detail Noise (R)", 2D) = "gray" {}
        _BlueNoise("Blue Noise (R)", 2D) = "gray" {}

        _NoiseTiling("Main Tiling (per meter)", Float) = 0.35
        _DetailTiling("Detail Tiling", Float) = 1.2
        _SliceScale("Vertical Slice Scale", Float) = 0.25
        _NoiseScroll("Main Scroll (xy)", Vector) = (0.03, 0.00, 0, 0)
        _DetailScroll("Detail Scroll (xy)", Vector) = (-0.015, 0.02, 0, 0)

        _Contrast("Noise Contrast", Range(0.5, 4)) = 1.6
        _Mix("Detail Mix", Range(0, 1)) = 0.45
        _Density("Global Density", Range(0, 3)) = 1.0

        _HeightStart("Height Start (WS)", Float) = 0.0
        _HeightEnd("Height End (WS)", Float) = 2.2

        // soft particles & camera fade
        _SoftNear("Soft Near", Float) = 0.4
        _SoftFar ("Soft Far",  Float) = 1.4
        _CamNear ("Camera Near Fade", Float) = 0.6
        _CamFar  ("Camera Far Fade",  Float) = 1.2

        _DitherStrength("BlueNoise Dither Strength", Range(0,1)) = 0.25

        _EdgeFeather("Edge Feather (0-1)", Range(0.0, 1.0)) = 0.6
        _EdgePower("Edge Power", Range(0.5, 6.0)) = 2.5
        _RandomJitter("Noise Jitter Strength", Range(0, 0.8)) = 0.25
        _SliceThickness("Parallax Thickness", Range(0, 1.0)) = 0.25
        _ParallaxSteps("Parallax Steps (1-4)", Range(1, 4)) = 2
        _UsePremul("Use Premultiplied", Float) = 1
    }

    SubShader
    {
        Tags{
            "Queue"="Transparent" "RenderType"="Transparent"
            "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline"
        }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "Forward"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            
            // WebGL specific defines
            #if defined(UNITY_WEBGL)
                #define WEBGL_BUILD
            #endif
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            TEXTURE2D(_BaseMap);      SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseTex);     SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_DetailTex);    SAMPLER(sampler_DetailTex);
            TEXTURE2D(_BlueNoise);    SAMPLER(sampler_BlueNoise);

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float4 _NoiseScroll;
                float4 _DetailScroll;
                float  _NoiseTiling, _DetailTiling, _SliceScale;
                float  _Contrast, _Mix, _Density;
                float  _HeightStart, _HeightEnd;
                float  _SoftNear, _SoftFar, _CamNear, _CamFar;
                float  _DitherStrength;
                float  _EdgeFeather;
                float  _EdgePower;
                float  _RandomJitter;
                float  _SliceThickness;
                float  _ParallaxSteps;
                float  _UsePremul;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 posCS   : SV_POSITION;
                float3 posWS   : TEXCOORD0;
                float2 uv      : TEXCOORD1;
                float4 col     : COLOR;
                float2 uvScreen: TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                o.posCS = TransformWorldToHClip(o.posWS);

                float2 screenUV = o.posCS.xy / o.posCS.w;
                screenUV = screenUV * 0.5 + 0.5;
                o.uvScreen = screenUV;

                o.uv = v.uv;
                o.col = v.color;
                return o;
            }

            // remap & contrast helper
            float remap01(float x) { return saturate(x); }

            float softParticleFade(float2 uv, float3 posWS)
            {
                #if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(UNITY_WEBGL)
                    // depth texture may be unavailable on mobile and WebGL
                    return 1.0;
                #else
                    float scene01 = SampleSceneDepth(uv);
                    float sceneEye = LinearEyeDepth(scene01, _ZBufferParams);
                    float partEye  = LinearEyeDepth(posWS, GetWorldToViewMatrix());
                    float dist = sceneEye - partEye;
                    float fade = saturate( (dist - _SoftNear) / max(1e-5, (_SoftFar - _SoftNear)) );
                    return fade;
                #endif
            }

            float camFade(float2 uv)
            {
                #if defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(UNITY_WEBGL)
                    // depth texture not available on mobile and WebGL
                    return 1.0;
                #else
                    // distance in clip space Z (eye space depth also ok; keep simple via depth texture)
                    float d = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
                    return saturate( (d - _CamNear) / max(1e-5, (_CamFar - _CamNear)) );
                #endif
            }

            float heightFactor(float yWS)
            {
                return saturate( (yWS - _HeightStart) / max(1e-4, (_HeightEnd - _HeightStart)) );
            }

            float noiseMask(float3 posWS, float2 uv0)
            {
                // world-space XZ + vertical slice to fit billboard
                float2 uvMain   = posWS.xz * _NoiseTiling + _Time.y * _NoiseScroll.xy;
                uvMain         += posWS.y * _SliceScale;
                float n1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uvMain).r;

                float2 uvDet    = posWS.xz * (_NoiseTiling * _DetailTiling) + _Time.y * _DetailScroll.xy;
                uvDet          += posWS.y * (_SliceScale * 1.7);
                float n2 = SAMPLE_TEXTURE2D(_DetailTex, sampler_DetailTex, uvDet).r;

                // mix & contrast
                float n = lerp(n1, n1 * n2, _Mix);
                // map to 0~1 and contrast
                n = pow(saturate(n), _Contrast);
                return n;
            }

            float blueDither(float2 uvScreen)
            {
                float2 uv = uvScreen * float2(_ScreenParams.xy); // screen space
                float v = SAMPLE_TEXTURE2D(_BlueNoise, sampler_BlueNoise, uv / 128.0).r; // assuming 128x128 texture
                return (v - 0.5) * 2.0 * _DitherStrength;
            }

            // pseudo-random generator for jitter
            float hash21(float2 p) {
                p = frac(p*float2(123.34, 456.21));
                p += dot(p, p+45.32);
                return frac(p.x*p.y);
            }

            half4 frag (v2f i) : SV_Target
            {
                // -------- 1) Quad edge softening, removing visible square edge --------
                // map (0,0)->(1,1) UV to centered 0 at [-1,1], distance from center
                float2 uv01 = i.uv;
                float2 uvC  = uv01*2.0 - 1.0;
                // circular mask, can also use radial/ellipse, controlled by _EdgeFeather
                float edgeMask = 1.0 - smoothstep(_EdgeFeather, 1.0, length(uvC));
                edgeMask = pow(saturate(edgeMask), _EdgePower);

                // -------- 2) Soft particles & camera fade & height --------
                float nSoft = softParticleFade(i.uvScreen, i.posWS);
                float nCam  = camFade(i.uvScreen);
                float nHeight = heightFactor(i.posWS.y);

                // -------- 3) Parallax / volume jittering / ray marching --------
                // per-particle random ID for jittering fragments
                float rnd = hash21(i.posWS.xz);
                float2 jitter = (rnd - 0.5) * _RandomJitter;

                // View ray direction (world space)
                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.posWS);
                // ray marching step length, unit scale appropriate
                float stepLen = _SliceThickness * 0.35;
                int   steps   = (int)round(_ParallaxSteps);     // 1~4

                // Main noise / detail noise composition
                float accum = 0.0;
                float weight = 1.0 / max(1, steps);
                float3 posW = i.posWS;

                #ifdef UNITY_WEBGL
                    // WebGL: simplified version, no unroll, fewer steps
                    steps = min(steps, 2);  // max 2 steps for WebGL
                    for (int s = 0; s < 2; ++s)
                    {
                        if (s >= steps) break;
                        // each step samples a new UV for noise accumulation
                        float2 uvMain = posW.xz * _NoiseTiling + _Time.y * _NoiseScroll.xy + jitter;
                        uvMain += posW.y * _SliceScale;

                        float2 uvDet  = posW.xz * (_NoiseTiling * _DetailTiling) + _Time.y * _DetailScroll.xy + jitter * 1.7;
                        uvDet  += posW.y * (_SliceScale * 1.7);

                        float n1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uvMain).r;
                        float n2 = SAMPLE_TEXTURE2D(_DetailTex, sampler_DetailTex, uvDet).r;
                        float n  = lerp(n1, n1 * n2, _Mix);
                        n = pow(saturate(n), _Contrast);
                        accum += n * weight;

                        posW += V * stepLen; // move along view ray
                    }
                #else
                    // Non-WebGL: full version with unroll optimization
                    [unroll(4)]
                    for (int s = 0; s < 4; ++s)
                    {
                        if (s >= steps) break;
                        // each step samples a new UV for noise accumulation
                        float2 uvMain = posW.xz * _NoiseTiling + _Time.y * _NoiseScroll.xy + jitter;
                        uvMain += posW.y * _SliceScale;

                        float2 uvDet  = posW.xz * (_NoiseTiling * _DetailTiling) + _Time.y * _DetailScroll.xy + jitter * 1.7;
                        uvDet  += posW.y * (_SliceScale * 1.7);

                        float n1 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uvMain).r;
                        float n2 = SAMPLE_TEXTURE2D(_DetailTex, sampler_DetailTex, uvDet).r;
                        float n  = lerp(n1, n1 * n2, _Mix);
                        n = pow(saturate(n), _Contrast);
                        accum += n * weight;

                        posW += V * stepLen; // move along view ray
                    }
                #endif

                // Add dithering and blue noise
                float dith = blueDither(i.uvScreen);
                accum = saturate(accum + dith * _DitherStrength);

                // -------- 4) Final alpha, pre-multiplying is optional --------
                float baseA = _Tint.a * _Density * i.col.a;
                float alpha = baseA * accum * nHeight * nSoft * nCam * edgeMask;
                alpha = saturate(alpha);

                float3 rgb = (_Tint.rgb) * SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;

                // Pre-multiply produces clearer edges
                if (_UsePremul > 0.5)
                    return half4(rgb * alpha, alpha);
                else
                    return half4(rgb, alpha);
            }

            ENDHLSL
        }
    }
    FallBack Off
}
