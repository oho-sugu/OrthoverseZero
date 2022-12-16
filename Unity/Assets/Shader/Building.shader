Shader "Custom/Building"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 200
        Cull Off

        Pass {
            ZWrite On
            ColorMask 0
        }

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}
            Tags{ "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGBA
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 L = normalize(float3(1,1,1));
                float3 N = normalize(i.worldNormal);
                fixed4 col = _Color * max(0, dot(N, L) * 0.5f + (1 - 0.5f));
                return col;
            }
            ENDCG
        }
    }
}
