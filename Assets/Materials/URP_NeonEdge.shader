Shader "Custom/URP/NeonEdgeGlow"
{
    Properties
    {
        _FaceColor ("Face Color", Color) = (0.15, 0.15, 0.15, 1)
        _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        _EdgeThickness ("Edge Thickness", Range(0.5, 5)) = 1.5
        _EdgeIntensity ("Edge Intensity", Range(1, 10)) = 4
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float3 barycentric : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 barycentric : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _FaceColor;
                float4 _EdgeColor;
                float _EdgeThickness;
                float _EdgeIntensity;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.barycentric = v.barycentric;
                return o;
            }

            float edgeFactor(float3 bary)
            {
                float3 d = fwidth(bary);
                float3 a3 = smoothstep(0.0, d * _EdgeThickness, bary);
                return min(min(a3.x, a3.y), a3.z);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float edge = edgeFactor(i.barycentric);

                float3 face = _FaceColor.rgb;
                float3 edgeCol = _EdgeColor.rgb * _EdgeIntensity;

                float3 color = lerp(edgeCol, face, edge);

                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}
