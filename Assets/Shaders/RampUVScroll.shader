Shader "Custom/RampUVScroll"
{
    Properties
    {
        _ScrollMap ("Scrolling Texture",2D) = "white" {}
        _ScrollSpeed ("Scroll Speed",Range(0.1,5)) = 0.1
        _SlopeActive ("Slope Active (Bool)",Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 scrollUv : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 scrollUv : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            TEXTURE2D(_ScrollMap);
            SAMPLER(sampler_ScrollMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _ScrollMap_ST;
                float _ScrollSpeed;
                float _SlopeActive;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.scrollUv = TRANSFORM_TEX(IN.scrollUv, _ScrollMap);
                OUT.normal = IN.normal;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                if (IN.normal.y > 0 && _SlopeActive == 1.0)
                {
                    half4 colour = SAMPLE_TEXTURE2D(_ScrollMap, sampler_ScrollMap, 1 - IN.scrollUv + (float2(0,-_ScrollSpeed) * _Time.y));
                    return colour;
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
