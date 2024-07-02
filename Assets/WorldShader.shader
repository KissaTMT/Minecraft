Shader "WorldShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TextureScale("Texture scale", float) = 1
        //[NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 250

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
        float _TextureScale;
        //sampler2D _BumpMap;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
        };
        float positive_fmod(float a,float b){
            return frac(abs(a/b))*abs(b);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float x = IN.worldPos.x * _TextureScale;
            float y = IN.worldPos.y * _TextureScale;
            float z = IN.worldPos.z * _TextureScale;

            float isUp = abs(IN.worldNormal.y);

            float2 offset = float2(positive_fmod(z + x*(1-isUp),0.0625),positive_fmod(y + x * isUp,0.0625));
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex + offset);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            //o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        }
    ENDCG
    }

    FallBack "Mobile/Diffuse"
}