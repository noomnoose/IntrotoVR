/*
The main calculation is carried out in the frag part,
In theory, using this shader will work better, but it will also consume more performance
In the actual test, the difference is not very big, users need to choose according to their own needs
*/
/*
主要的计算放在了片元部分进行,理论上使用这个Shader效果会更加好，但同时也会消耗更多的的性能
实际测试中区别不是很大，需要用户根据自己的需要选择
*/

Shader "Color/ARColorFrag" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}

        //Points Info from C#
        //从C#发送来的点的信息
        _UvTopLeft("point1", Vector) = (0 , 0 , 0 , 0)
        _UvButtomLeft("point2", Vector) = (0 , 0 , 0 , 0)
        _UvTopRight("point3", Vector) = (0 , 0 , 0 , 0)
        _UvBottomRight("point4", Vector) = (0 , 0 , 0 , 0)
    }

    SubShader {
        //transparent,Transparency can be enabled to avoid affecting the accuracy of screenshots
        //透明队列 ，因为初始的时候要给物体透明贴图 消除影响
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        LOD 100

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            //Info from C#
            //C# 脚本传送来的信息
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _UvTopLeft;
            float4 _UvButtomLeft;
            float4 _UvTopRight;
            float4 _UvBottomRight;
			float4x4 _VP;

            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            } ;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);	

             
                return o;
            }
            float4 frag(v2f i) : SV_Target
            {

                 float4 top = lerp(_UvTopLeft, _UvTopRight, i.uv.x);
                 float4 bottom = lerp(_UvButtomLeft, _UvBottomRight, i.uv.x);
                 float4 fixedPos = lerp(bottom, top, i.uv.y);
                 fixedPos = ComputeScreenPos(mul(_VP, fixedPos));

                 //Image sampling The UV coordinate used here is the real UV coordinate in the screenshot after transformation
                 //图片采样
                 fixed4 texColor = tex2D(_MainTex, fixedPos.xy / fixedPos.w);

                 return fixed4(texColor);
            }
            ENDCG
        }
    } 
    FallBack "Legacy Shaders/Transparent/Diffuse"
}