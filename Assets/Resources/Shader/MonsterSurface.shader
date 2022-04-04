Shader "Custom/MonsterSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _ColorVein ("ColorVein", Color) = (1,0,0,0)
        _ColorVeinEnd ("ColorVeinEnd", Color) = (0,1,0,0)
        _VeinThresh ("VeinThresh", Range(0,1)) = 0.5

        _SurfaceBumpScale ("SurfaceBumpScale", Range(1, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 vertex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        fixed4 _ColorVein;
        fixed4 _ColorVeinEnd;
        half _VeinThresh;

        half _SurfaceBumpScale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertex = v.vertex.xyz;

            float x = sin(v.vertex.x * 1000.0f);
            float y = sin(v.vertex.y * 1000.0f);
            float z = sin(v.vertex.z * 1000.0f);

            float pulseTime = sin(_Time.x * 40.0f) + 0.5f * sin(_Time.x * 60.0f);
            float pulseStep = step(0.0, pulseTime);
            float pulse = 1.0f + pulseStep * pulseTime + (1.0 - pulseStep) * pulseTime * 0.3f;

            v.vertex.xyz += v.normal * x * z * y * 0.001f * _SurfaceBumpScale * pulse;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

            half veinStep = step(_VeinThresh, tex.r);
            half veinColorStep = smoothstep(_VeinThresh, 1.0f, tex.r);

            fixed4 bodyColor = (1.0 - veinStep) * _Color + veinStep * (_ColorVein * (1 - veinColorStep) + _ColorVeinEnd * veinColorStep);

            float s = 0.002f;
            float occlusion = 1.0 - 0.7 * (smoothstep(s, s + 0.001, IN.vertex.z) + smoothstep(s, s + 0.001, -IN.vertex.z));

            o.Albedo = bodyColor * occlusion + _ColorVein * (1 - occlusion);

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
