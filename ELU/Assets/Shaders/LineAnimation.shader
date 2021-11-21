Shader "Custom/LineAnimation"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AnimationSpeed ("Animation Speed", Range(0.0, 5)) = 0.5
        _Tiling ("Tiling", Range(0.0, 20)) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting noambient

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

        sampler2D _MainTex;
        float _Tiling;
        float _AnimationSpeed;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 animation = float2(_Time.y * _AnimationSpeed,  0);
            float2 tiling = float2(_Tiling, 1);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex * tiling + animation) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
