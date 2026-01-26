Shader "Custom/DimBackground"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _FillAmount;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main image texture
                fixed4 mainColor = tex2D(_MainTex, i.uv);

                // Sample the mask texture
                fixed4 maskColor = tex2D(_MaskTex, i.uv);

                // Modify alpha based on the mask and fill amount
                if (maskColor.a > 0.5)
                {
                    mainColor.a *= step(_FillAmount, maskColor.a);
                }

                return mainColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}

