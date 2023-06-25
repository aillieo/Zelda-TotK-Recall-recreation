Shader "AillieoTech/OutlineMask"
{
    Properties
    {
    }

    SubShader
    {

        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert (Atributes v)
            {
                Varyings o = (Varyings)0;
                float4 clipPosition = TransformObjectToHClip(v.positionOS.xyz);
                o.positionCS = clipPosition;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                return float4(1, 1, 1, 1);
            }

            ENDHLSL
        }
    }
}
