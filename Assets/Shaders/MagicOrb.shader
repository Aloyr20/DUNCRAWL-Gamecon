Shader "Custom/MagicOrb"
{
    Properties
    {
        _EnvCube ("Base Cubemap",Cube) = "" {}
        _EnvIntensity ("Reflection Intensity",Range(0,2)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
            };

            TEXTURECUBE(_EnvCube);
            SAMPLER(sampler_EnvCube);

            CBUFFER_START(UnityPerMaterial)
                float _EnvIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // View direction (WS)
                float3 V = SafeNormalize(GetWorldSpaceViewDir(IN.positionWS));

                // Normal (WS)
                float3 N = SafeNormalize(IN.normalWS);

                // Reflection vector (WS) = reflect(incident, normal)
                // HLSL reflect() expects the INCIDENT vector (from surface toward eye), which is -V
                float3 R = reflect(-V, N);

                half3 envColour = SAMPLE_TEXTURECUBE(_EnvCube, sampler_EnvCube, R).rgb * _EnvIntensity;

                return half4(envColour,1);
            }
            ENDHLSL
        }
    }
}
