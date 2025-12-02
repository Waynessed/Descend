Shader "UI/MinimapWithFog"
{
    Properties{
        _MainTex ("Minimap", 2D) = "white" {}
        _FogTex  ("Fog Mask", 2D) = "white" {}
        _Tint    ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags{ "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
            sampler2D _MainTex; float4 _MainTex_ST;
            sampler2D _FogTex;
            float4 _Tint;

            v2f vert(appdata v){ v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv,_MainTex); return o; }
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 map = tex2D(_MainTex, i.uv) * _Tint;
                fixed  fog = tex2D(_FogTex,  i.uv).a;   // 1=opaque fog, 0=revealed
                // darken by fog (keep alpha)
                map.rgb *= (1.0 - fog);
                return map;
            }
            ENDHLSL
        }
    }
}
