Shader "Custom/DP_NormalShade"
{
    Properties
    {
        _xColour1 ("x Colour 1", Color) = (1,1,1,1)
        _yColour1 ("y Colour 1", Color) = (1,1,1,1)
        _zColour1 ("z Colour 1", Color) = (1,1,1,1)
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
                float3 normal: NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normal : TEXCOORD0;
            };


            CBUFFER_START(UnityPerMaterial)
                half4 _xColour1;
                half4 _yColour1;
                half4 _zColour1;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normal = IN.normal;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalnorm = normalize(IN.normal);
                
                float dpX = abs(dot(IN.normal.x, float3(1,0,0)));
                float dpY = abs(dot(IN.normal.y, float3(0,1,0)));
                float dpZ = abs(dot(IN.normal.z, float3(0,0,1)));

                return half4(_xColour1.rgb*dpX + _yColour1.rgb*dpY + _zColour1.rgb*dpZ,1);
            }
            ENDHLSL
        }
    }
}
