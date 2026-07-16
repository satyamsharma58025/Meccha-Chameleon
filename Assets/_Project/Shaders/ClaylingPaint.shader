Shader "HueSeek/ClaylingPaint"
{
    Properties
    {
        _BaseMap ("Base Albedo", 2D) = "white" {}
        _PaintMap ("Paint Accumulation", 2D) = "black" {}
        _PaintColor ("Active Stroke Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0
        _Roughness ("Roughness", Range(0,1)) = 0.5
        _Imperfection ("Imperfection Noise", Range(0,1)) = 0.12
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_PaintMap);
            SAMPLER(sampler_PaintMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _PaintColor;
                half _Metallic;
                half _Roughness;
                half _Imperfection;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half4 paintMask = SAMPLE_TEXTURE2D(_PaintMap, sampler_PaintMap, input.uv);

                half noise = frac(sin(dot(input.uv, float2(12.9898, 78.233))) * 43758.5453);
                half imperfection = lerp(1.0h, 0.85h, _Imperfection * noise);

                half3 albedo = lerp(baseCol.rgb, _PaintColor.rgb, paintMask.r) * imperfection;

                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = float3(0,0,0);
                lightingInput.normalWS = normalize(input.normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(float3(0,0,0));

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedo;
                surfaceData.metallic = _Metallic * paintMask.r;
                surfaceData.smoothness = 1.0h - _Roughness;
                surfaceData.alpha = 1.0h;

                return half4(albedo, 1.0h);
            }
            ENDHLSL
        }
    }
}
