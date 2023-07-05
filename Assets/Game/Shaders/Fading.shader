Shader "AillieoTech/Fading"
{
    Properties {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        [HDR]_Color("Color", color) = (1,1,1,1)
        _FadeValue("FadeValue", float) = 0
        _NoiseLowScale("NoiseLowScale", Range(0, 0.5)) = 0.4
        _NoiseHighScale("NoiseHighScale", Range(0, 0.5)) = 0.1
        _Center("Center",Vector)=(0.5,0.5,0,0)
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
            float _NoiseLowScale;
            float _NoiseHighScale;
            float3 _Center;
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

            float4 gray(float4 color)
            {
                float gray = color.r * 0.299 + color.g * 0.587 + color.b * 0.114;
                return float4(gray, gray, gray, 1.0);
            }

            float4 frag(Varyings i) : SV_Target
            {
                if (SAMPLE_TEXTURE2D(_OutlineMaskRT, sampler_OutlineMaskRT, i.uv).r > 0)
                {
                    discard;
                }

                float4 screenCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float2 distVector = (i.uv - _Center.xy);
                distVector.x *= (_ScreenParams.x / _ScreenParams.y);
                half distance = length(distVector);
                float angle = (atan2(distVector.y, distVector.x) + 3.14) / 6.28;
                float noiseLow = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(angle, 0)).r;
                float noiseHigh = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2((_SinTime.w + 1) * 0.5 * angle, 0)).r;
                float fadeValue = _FadeValue * _FadeValue * 0.75;
                float distanceWithNoise = ((1 - _NoiseLowScale - _NoiseHighScale) + noiseLow * _NoiseLowScale + noiseHigh * _NoiseHighScale) * distance;
                float s1 = step(distanceWithNoise, fadeValue);
                float s2 = smoothstep(distanceWithNoise * 0.9f, distanceWithNoise, fadeValue);
                float circleValue = s2 - s1;
                float4 color = lerp(screenCol, gray(screenCol), s1);
                float4 final = lerp(color, _Color, circleValue);
                return final;
            }

            ENDHLSL
        }
    }
}
