Shader "Custom/IDPlantShader1"
{
    Properties
    {
        _Id("Index", Range (0, 255)) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetalicTex ("Metalic/Smoothness", 2D) = "black" {}
        _NormalMapIntensity ("Normal Intensity", Float) = 1
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags {
            "Queue"      = "AlphaTest"
			"RenderType" = "TransparentCutout"}

        AlphaToMask On
        LOD 200
        
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetalicTex;
        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
            float IsFacing:VFACE;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _NormalMapIntensity;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            fixed4 mg = tex2D (_MetalicTex, IN.uv_MainTex);
            o.Metallic = mg.r * _Metallic;
            o.Smoothness = mg.a * _Glossiness;
            o.Alpha = c.a;
            float vv = IN.IsFacing>0?1:-1;
            o.Normal = lerp( float3(0,0,1), UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)), _NormalMapIntensity) * vv;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/Diffuse"
}
