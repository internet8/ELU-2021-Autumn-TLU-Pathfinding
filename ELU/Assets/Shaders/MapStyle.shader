Shader "Custom/MapStyle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _DarkTehem ("Dark Theme Switch", Int) = 0
        _Sharpen ("Sharpen Switch", Int) = 0
        _Brightness ("Brightness", Range(0.0, 0.5)) = 0
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface surf NoLighting noambient

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

        sampler2D _MainTex;
        fixed _Brightness;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        int _DarkTehem;
        int _Sharpen;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 result;
            // sharpen
            if (_Sharpen) {
                // red
                if (pow(pow(0.7 - c.r, 2) + pow(0.1 - c.g, 2) + pow(0.2 - c.b, 2), 0.5) < 0.3) {
                    result = half4(0.8157, 0.0157, 0.235, 1);
                }
                if (pow(pow(0.78 - c.r, 2) + pow(0.61 - c.g, 2) + pow(0.58 - c.b, 2), 0.5) < 0.3) {
                    result = half4(0.8157, 0.0157, 0.235, 1);
                }
                if (pow(pow(0.83 - c.r, 2) + pow(0.72 - c.g, 2) + pow(0.7 - c.b, 2), 0.5) < 0.3) {
                    result = half4(0.8157, 0.0157, 0.235, 1);
                }
                // white
                if (pow(pow(1 - c.r, 2) + pow(1 - c.g, 2) + pow(1 - c.b, 2), 0.5) < 0.2) {
                    result = 1;
                }
                // light grey
                if (pow(pow(0.788 - c.r, 2) + pow(0.788 - c.g, 2) + pow(0.788 - c.b, 2), 0.5) < 0.2) {
                    result = 0.788;
                }
                // grey
                if (pow(pow(0.635 - c.r, 2) + pow(0.635 - c.g, 2) + pow(0.635 - c.b, 2), 0.5) < 0.2) {
                    result = 0.635;
                }
                // dark grey
                if (pow(pow(0.533 - c.r, 2) + pow(0.533 - c.g, 2) + pow(0.533 - c.b, 2), 0.5) < 0.28) {
                    result = 0.533;
                }
                // black
                if (pow(pow(0 - c.r, 2) + pow(0 - c.g, 2) + pow(0 - c.b, 2), 0.5) < 0.65) {
                    result = 0;
                }
                if (pow(pow(0.2 - c.r, 2) + pow(0.2 - c.g, 2) + pow(0.2 - c.b, 2), 0.5) < 0.38) {
                    result = 0;
                }
                // green
                if (pow(pow(0.42 - c.r, 2) + pow(0.67 - c.g, 2) + pow(0.31 - c.b, 2), 0.5) < 0.2) {
                    result = half4(0.42, 0.67, 0.31, 1);
                }
            } else {
                result = c;
            }
            // post processing
            if (_DarkTehem) {
                if ((result.r > 0.5 && result.b < 0.5 && result.g < 0.5) || (result.g > 0.5 && result.b < 0.5 && result.r < 0.5)) {
                    result = result.rgba;
                } else {
                    result = 1 - result.rgba + _Brightness;
                }
            } else {
                if ((result.r > 0.5 && result.b < 0.5 && result.g < 0.5) || (result.g > 0.5 && result.b < 0.5 && result.r < 0.5)) {
                    result = result.rgba;
                } else {
                    result = result.rgba - _Brightness; // 0.065 for light theme
                }
            }
            o.Albedo = result;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
