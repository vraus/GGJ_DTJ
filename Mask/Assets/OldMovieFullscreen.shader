Shader "Hidden/OldMovieFullscreen"
{
    Properties
    {
        _Grain ("Grain Amount", Range(0,1)) = 0.35
        _Flicker ("Flicker Strength", Range(0,1)) = 0.15
        _Scratches ("Scratches", Range(0,1)) = 0.4
        _Vignette ("Vignette", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "OldMoviePass"
            ZTest Always
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _Grain;
            float _Flicker;
            float _Scratches;
            float _Vignette;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;

                // Fullscreen triangle
                o.uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.positionHCS = float4(o.uv * 2.0 - 1.0, 0.0, 1.0);
                o.uv.y = 1.0 - o.uv.y;

                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            half4 Frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv);

                float time = _Time.y * 24.0;

                // Flicker
                float flicker = lerp(1.0, rand(float2(time, time)), _Flicker);
                col.rgb *= flicker;

                // Grain
                float grain = rand(uv * time) * 2.0 - 1.0;
                col.rgb += grain * _Grain;

                // Vertical scratches
                float scratch = step(0.995, rand(float2(uv.x * 120.0, time)));
                col.rgb -= scratch * _Scratches;

                // Vignette
                float2 center = uv - 0.5;
                float vign = smoothstep(0.5, 0.15, length(center));
                col.rgb *= lerp(1.0, vign, _Vignette);

                return col;
            }
            ENDHLSL
        }
    }
}
