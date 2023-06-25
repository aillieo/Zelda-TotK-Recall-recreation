Shader "AillieoTech/Highlight"
{
    Properties
    {
        _Color("Color", color) = (1,1,1,1)
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _Speed("_Speed", Range(1, 10)) = 5
        
    }

    SubShader
    {

        Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            ZTest LEqual
            ZWrite Off

            Blend one OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributes
            {
                float2 uv : TEXCOORD0;
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _Speed;
            half4 _NoiseTex_ST;
            CBUFFER_END

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Atributes v)
            {
                Varyings o = (Varyings)0;
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.positionOS = v.positionOS.xyz;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv).r;

                noise = pow(noise, 3);

                float c1 = distance(i.positionOS.z, 0) - _Time.y * _Speed;
                c1 = c1 + noise * 20;

                float c2 = distance(i.positionWS.z, 0) - _Time.y * _Speed;
                c2 = c2 + noise * 40;

                float c = c1 + c2;

                c = fmod(c, 3);
                if (c < 0)
                {
                    c = c + 3;
                }
    
                float threshold = 3;
                float v = smoothstep(threshold - 1.5, threshold, c);
                float4 final = _Color * v;
                return final;
            }

            ENDHLSL
        }
    }
}
