Shader "AillieoTech/Outline"
{
    Properties
    {
        [HDR]_OutlineColor("Outline Color",Color) = (1,1,1,1)
        [IntRange]_Outline ("Outline",Range(0,16)) = 8
    }

    SubShader
    {

        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Overlay" "Queue" = "Transparent" "DisableBatching" = "True" }

        Pass
        {
            ZTest Always
            ZWrite Off

            Cull Off

            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            int _Outline;
            CBUFFER_END

            TEXTURE2D(_OutlineMaskRT);
            SAMPLER(sampler_OutlineMaskRT);
            float4 _OutlineMaskRT_TexelSize;

            Varyings vert (Atributes v)
            {
                Varyings o;
                // UNITY_SETUP_INSTANCE_ID(v);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                if (SAMPLE_TEXTURE2D(_OutlineMaskRT, sampler_OutlineMaskRT, i.uv).r > 0)
                {
                    discard;
                }

                float ColorIntensityInRadius = 0;

                float dx = _OutlineMaskRT_TexelSize.x;
                float dy = _OutlineMaskRT_TexelSize.y;
                float x0 = i.uv.x - _Outline * 0.5 * dx;
                float y0 = i.uv.y - _Outline * 0.5 * dy;

                [unroll(16)]
                for (int x = 0; x < _Outline; x += 1)
                {
                    [unroll(16)]
                    for (int y = 0; y < _Outline; y += 1)
                    {
                        float u = x0 + x * dx;
                        float v = y0 + y * dy;

                        float2 uv = float2(u, v);

                        float r = SAMPLE_TEXTURE2D(_OutlineMaskRT, sampler_OutlineMaskRT, uv).r;
                        ColorIntensityInRadius += r;
                    }
                }

                ColorIntensityInRadius = ColorIntensityInRadius / (_Outline * _Outline) * 4;


                float4 color = _OutlineColor;
                color.a *= ColorIntensityInRadius;
                return color;
            }

            ENDHLSL
        }
    }
}
