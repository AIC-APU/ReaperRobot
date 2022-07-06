Shader "IndexTexture"
{
    Properties
    {
    }
        SubShader
    {
        Cull Off
        ZTest LEqual
        ZWrite On

        Tags {"AnnotationID"="ID"}

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            float _Id;

            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 annoID  : COLOR0;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.annoID.r = _Id;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return i.annoID.r / 255;
            }
            ENDCG
        }
    }
}