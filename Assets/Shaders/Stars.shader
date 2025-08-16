Shader "Spacecraft/Stars"
{
    Properties
    {
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _Color("Tint", Color) = (1, 1, 1, 1)

        _StarTex("StarTex", 2D) = "white" {}
        _StarID("StarID", 2D) = "white" {}
        _CameraPos("CameraPos", Vector) = (0, 0, 0, 0)
        _BackColor("Background Color", Color) = (0, 0, 0, 1)
        _Scale("Scale", float) = 3
        _Layer("Layer Count", Int) = 3
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

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

            sampler2D _StarTex;
            sampler2D _StarID;
            float4 _CameraPos;
            float4 _BackColor;
            float _Scale;
            int _Layer;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _BackColor;
                for (int layer = 1; layer <= _Layer; layer++)
                {
                    float strength = (1.0 + float(_Layer) - layer) / _Layer;
                    float2 uv = i.uv;
                    uv -= 0.5;
                    uv *= _Scale / strength;
                    uv += _CameraPos * 0.07 * strength;
                    
                    fixed4 star = tex2D(_StarTex, uv);
                    if (star.a > 0)
                    {
                        float id = tex2D(_StarID, uv).r;
                        id = frac(id + uv.x * 0.123 + uv.y * 0.456);
                        float s = sin(_Time.y + id * 6.28) * 0.5 + 0.5;
                        star *= s;
                        col.rgb += star * strength * 0.2;
                    }
                }
                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}
