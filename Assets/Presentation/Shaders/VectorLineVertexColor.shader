Shader "VectorArcade/VectorLineVertexColor"
{
    Properties
    {
        _GlobalTint ("Global Tint (multiply)", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "UnlitVertexColor"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _GlobalTint;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 color        : COLOR;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(positionWS);
                o.color = v.color * _GlobalTint;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}
