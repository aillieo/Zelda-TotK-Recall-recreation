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

        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Overlay" "Queue"="Transparent" "DisableBatching"="True" }

        Pass
        {
            Blend one OneMinusSrcAlpha

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
            float4 _MainTex_ST;
            half4 _Color;
            float _FadeValue;
            float _Distortion;
            CBUFFER_END

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            Varyings vert(Atributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 screenCol = tex2D(_MainTex, i.uv);
                float noise = tex2D(_NoiseTex, i.uv).r;
                float2 distVector = i.uv - 0.5f;
                half4 color = 0;
                half2 symmetryUv = i.uv - 0.5;
                half distance = length(symmetryUv);
                float fadeValue = _FadeValue;
                float distortedRadius = 1.0 + _Distortion * noise;
                float s1 = step(distortedRadius, fadeValue);
                float s2 = smoothstep(distortedRadius - 1.0, distortedRadius, fadeValue);
                float circleValue = (s2 - s1);
                float4 final = lerp(screenCol, _Color, circleValue);
                return final;
            }

            ENDHLSL
        }
    }
}
