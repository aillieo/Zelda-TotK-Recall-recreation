Shader "AillieoTech/Fading"
{
    Properties {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        [HDR]_Color("_Color", color) = (1,1,1,1)
        _FadeValue("_FadeValue", float) = 0
        _Distortion("Distortion", Range(0, 1)) = 0.1
    }
 
    SubShader
    {

        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Overlay" "DisableBatching"="True" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_OutlineMaskRT);
            SAMPLER(sampler_OutlineMaskRT);

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
            half4 _Color;
            float _FadeValue;
            float _Distortion;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Atributes v)
            {
                Varyings o;
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

                float4 screenCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv).r;
                float2 distVector = (i.uv - 0.5f);
                distVector.x *= (_ScreenParams.x / _ScreenParams.y);
                half distance = length(distVector);
                float fadeValue = _FadeValue;
                float distortedRadius = distance + _Distortion * noise;
                float s1 = step(distortedRadius, fadeValue);
                float s2 = smoothstep(distortedRadius * 0.9f, distortedRadius, fadeValue);
                float circleValue = (s2 - s1) + _Color.a * s1;
                circleValue *= _Color.a;
                float4 final = lerp(screenCol, _Color, circleValue);
                return final;
            }

            ENDHLSL
        }
    }
}
