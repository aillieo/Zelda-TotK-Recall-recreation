Shader "AillieoTech/Scanning"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        [HDR]_ScanLineColor("_ScanLineColor", color) = (1,1,1,1)
        _ScanSpeed("_ScanSpeed", float) = 0
        _ScanLineWidth("ScanLineWidth", Range(0, 10)) = 1
        _ScanLineInterval("_ScanLineInterval", Range(1, 30)) = 30
        _ScanLightStrength("ScanLightStrength", float) = 1
        _Radius("Radius", float) = 1
        _Distortion("Distortion", Range(0, 1)) = 0.1
    }
 
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Overlay" "Queue"="Transparent" "DisableBatching"="True" }
        LOD 100
        ZTest Always Cull Off ZWrite Off
        Blend one OneMinusSrcAlpha

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half4 _ScanLineColor;
        float _ScanSpeed;
        float _ScanLineWidth;
        float _ScanLineInterval;
        float _ScanLightStrength;
        float _DistortFactor;
        float3 _Center;
        float _Radius;
        float _Distortion;
        CBUFFER_END
    
        sampler2D _MainTex;
        sampler2D _NoiseTex;
        TEXTURE2D(_CameraDepthTexture);
        SAMPLER(sampler_CameraDepthTexture);
    
        struct appdata {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
    
        struct v2f {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 viewRayWorld : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };
    
        v2f vert(appdata v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            float sceneRawDepth = 1;
#if defined(UNITY_REVERSED_Z)
            sceneRawDepth = 1 - sceneRawDepth;
#endif
            float3 worldPos = ComputeWorldSpacePosition(v.uv, sceneRawDepth, UNITY_MATRIX_I_VP);
            o.viewRayWorld = worldPos - _WorldSpaceCameraPos.xyz;
            o.uv = v.uv;
            return o;
        }
    
        float4 frag(v2f i) : SV_Target
        {
            float4 screenCol = tex2D(_MainTex, i.uv);
            float noise = tex2D(_NoiseTex, i.uv).r;
            float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv);
            float linear01Depth = Linear01Depth(sceneRawDepth, _ZBufferParams);
            float3 worldPos = _WorldSpaceCameraPos.xyz + (linear01Depth)*i.viewRayWorld;

            float3 distVector = worldPos - _Center;
            float distance = sqrt(distVector.x * distVector.x + distVector.z * distVector.z);
            float scanValue = _Time.y * _ScanSpeed;
            distance = fmod(distance + scanValue, _ScanLineInterval);

            float distortedRadius = _Radius * (1.0 + _Distortion * noise);
            float s1 = step(distortedRadius, distance);
            float s2 = smoothstep(distortedRadius - _ScanLineWidth, distortedRadius, distance);
            float circleValue = (s2 - s1) ;
            float4 final = lerp(screenCol, _ScanLineColor, circleValue);
            return final;
        }
    
        ENDHLSL

        Pass
        {
            Name "Scanning"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
