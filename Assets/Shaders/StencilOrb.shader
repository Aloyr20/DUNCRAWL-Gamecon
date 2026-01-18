Shader "Custom/StencilOrb"
{
    Properties
    {
        _EnvCube ("Base Cubemap",Cube) = "" {}
        _EnvIntensity ("Reflection Intensity",Range(0,2)) = 1
        _Wave1Complete ("Wave 1 Complete (Bool)",Float) = 0
        _GotHP ("Got HP (Bool)",Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "StencilPass"

            ColorMask 0

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

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
                return half4(0,0,0,0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadePass"
            Tags { "LightMode" = "UniversalForward" } // Necessary for the second pass to work

            Stencil
            {
                Ref 1
                Comp Equal
            }

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
                float _Wave1Complete;
                float _GotHP;
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

                if (_Wave1Complete == 1.0 && _GotHP == 0.0)
                {
                    float3 V = SafeNormalize(GetWorldSpaceViewDir(IN.positionWS));
                
                    half3 envColour = SAMPLE_TEXTURECUBE(_EnvCube, sampler_EnvCube, V).rgb * _EnvIntensity; // Intentionally not-reversed view direction so clouds are visible

                    return half4(envColour,1);
                }
                else
                {
                    return half4(0,0,0,1);
                }
            }
            ENDHLSL
        }
    }
}
