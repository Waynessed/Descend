Shader "Hidden/FOWReveal"
{
    Properties { }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        ZWrite Off Blend One Zero // we overwrite into a temporary, then copy back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float2 verts[4] = { float2(-1,-1), float2(1,-1), float2(1,1), float2(-1,1) };
                float2 uvs[4]   = { float2(0,0),  float2(1,0),  float2(1,1),  float2(0,1)  };
                o.pos = float4(verts[id], 0, 1);
                o.uv  = uvs[id];
                return o;
            }

            Texture2D _Source;        SamplerState sampler_Source;
            float2 _BrushUV;          // 0..1 (x,y)
            float  _BrushRadius;      // normalized (0..1 of RT)
            float  _Softness;         // 0..1

            float4 frag(v2f i) : SV_Target
            {
                float4 cur = _Source.Sample(sampler_Source, i.uv); // current mask (a=alpha)
                // distance to brush center (in UV)
                float d = distance(i.uv, _BrushUV);
                // Circle: 0 = fully revealed at center, 1 = untouched
                float reveal = smoothstep(_BrushRadius, _BrushRadius - _Softness, d);
                // We want to keep the MIN of current and reveal (once revealed stays revealed)
                cur.a = min(cur.a, reveal);
                return cur;
            }
            ENDHLSL
        }
    }
}
