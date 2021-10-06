// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Color/Special" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}

        //Points Info from C#
        _Uvpoint1("point1", Vector) = (0 , 0 , 0 , 0)
        _Uvpoint2("point2", Vector) = (0 , 0 , 0 , 0)
        _Uvpoint3("point3", Vector) = (0 , 0 , 0 , 0)
        _Uvpoint4("point4", Vector) = (0 , 0 , 0 , 0)

    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            //Info from C#
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Uvpoint1;
            float4 _Uvpoint2;
            float4 _Uvpoint3;
            float4 _Uvpoint4;
			float4x4 _VP;

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4  fixedPos : TEXCOORD2;
            } ;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);	
                return o;
            }
            float4 frag (v2f i) : COLOR
            {    
			    float4 top = lerp(_Uvpoint1, _Uvpoint3, i.uv.x);
                float4 bottom = lerp(_Uvpoint2, _Uvpoint4, i.uv.x);
                float4 fixedPos = lerp(bottom, top, i.uv.y);
				fixedPos = ComputeScreenPos(mul(_VP, fixedPos));
                return tex2D(_MainTex, fixedPos.xy / fixedPos.w);			
            }
            ENDCG
        }
    } 
}