Shader "AillieoTech/Fresnel"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Power("_Power", Range(1, 10)) = 5
    }

    SubShader
    {

        Tags {"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline"}
    
        Pass
        {

            Blend one OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Atributes
            {
                float4 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                half3 worldPos : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
                half3 worldViewDir : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _Power;
            float4 _MainTex_ST;
            CBUFFER_END

            sampler2D _MainTex;

            Varyings vert(Atributes v)
            {
                Varyings o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex).xyz;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldViewDir = _WorldSpaceCameraPos.xyz - o.worldPos;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half3 worldNormal = normalize(i.worldNormal);
                half3 viewDir = normalize(i.worldViewDir);
                half fresnel = 1.0 - dot(viewDir, worldNormal);
                fresnel = pow(abs(fresnel), _Power);
                half4 color = _Color;
                color *= fresnel;
                return color;
            }

            ENDHLSL
        }
    }
}
