Shader "Custom/SineWave1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Pass
        {
            CULL oFF
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                // float phase = _Time * 20.0f;
                // v.vertex.y = v.vertex.y + v.vertex.x;

                v.vertex.y = v.vertex.y + sin(v.uv.x * 6.28);
                // v.vertex.y = v.vertex.y + sin(v.vertex.x);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                float modify = 1.0f;
                col.rgb = fixed3(i.uv.x / modify,i.uv.x / modify,i.uv.x / modify);
                return col;
            }
            ENDCG
        }
    }
}
