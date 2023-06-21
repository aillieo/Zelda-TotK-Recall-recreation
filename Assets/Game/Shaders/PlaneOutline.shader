Shader "AillieoTech/PlaneOutline"
{
    Properties
    {
        [HDR]_MainColor("Main Color",Color) = (1,1,1,1)
        [HDR]_OutlineColor("Outline Color",Color) = (1,1,1,1)
        _Outline ("Outline",Range(0.01, 0.99)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline"}

        Pass
        {
            ZTest Always
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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _MainColor;
            float4 _OutlineColor;
            float _Outline;
            CBUFFER_END

            Varyings vert (Atributes v)
            {
                Varyings o = (Varyings)0;
                float4 clipPosition = TransformObjectToHClip(v.positionOS.xyz);
                o.positionCS = clipPosition;
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float r = i.uv.y;
                float d = abs(r - 0.5f) * 2;
                float a = smoothstep(_Outline, _Outline*_Outline, d);
                float4 color = lerp(_OutlineColor, _MainColor, a);
                return color;
            }

            ENDHLSL
        }
    }
}
