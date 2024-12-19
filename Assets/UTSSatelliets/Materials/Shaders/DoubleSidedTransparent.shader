Shader "Custom/DoubleSidedTransparent"
{
    Properties
    {
        _Color("Color", Color) = (1,0,0,0.2) // 红色，自带透明度
        _EmissionColor("Emission Color", Color) = (1,0,0,1) // 自发光颜色
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            // 双面渲染设置
            Cull Off

            // 设置为透明模式
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;
            fixed4 _EmissionColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 输出自发光颜色，带透明效果
                fixed4 col = _Color;
                col.rgb += _EmissionColor.rgb; // 叠加自发光
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
