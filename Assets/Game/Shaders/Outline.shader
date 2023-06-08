Shader "AillieoTech/Outline"
{
    Properties
    {
        [HDR]_OutlineColor("Outline Color",Color) = (1,1,1,1)
        _Outline ("Outline",Range(0,0.5)) = 0.1
    }

    SubShader
    {

        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            LOD 100
            Cull Front
            // ZTest Always

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float _Outline;
            CBUFFER_END

            Varyings vert (Atributes v)
            {
                Varyings o = (Varyings)0;
                float4 viewPos = mul(UNITY_MATRIX_MV, v.positionOS);
                float3 viewNorm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normalOS);
                float3 offset = normalize(viewNorm) * _Outline;
                viewPos.xyz += offset;
                o.positionCS = mul(UNITY_MATRIX_P, viewPos);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}
