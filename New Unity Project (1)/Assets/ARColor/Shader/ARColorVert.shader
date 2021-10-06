/*
The main calculation is carried out in the vertex part. 
Theoretically, the effect of using this shader will be worse, but it can save a lot of performance
In the actual test, the difference is not very big, users need to choose according to their own needs
*/
/*
主要的计算放在了顶点部分进行,理论上使用这个Shader效果会差一些，但可以节省很多的的性能
实际测试中区别不是很大，需要用户根据自己的需要选择
*/


Shader "Color/ARColorVert" {
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

                float4  fixedPos : TEXCOORD0;
            } ;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //Get the uv from model
                //得到模型的uv
                float2 uv = TRANSFORM_TEX(v.texcoord,_MainTex);	

                /*Find the position of each point in space according to the UV layout*/
                /*按照UV排布找到每个点在空间中的位置*/
                //The x position of UV corresponding to the vertex is obtained as the position point between the left and right corners of the top of the tracing drawing card
                //得到一组与顶点对应 使用uv的x位置为差值进度 介于追踪的绘图卡顶部左右角间的位置点
                float4 top = lerp(_UvTopLeft, _UvTopRight, uv.x);
                //The x position of UV corresponding to the vertex is obtained as the position point between the left and right corners of the tracing drawing card
                //得到一组与顶点对应 使用uv的x位置为差值进度 介于追踪的绘图卡底部左右角间的位置点
                float4 bottom = lerp(_UvButtomLeft, _UvBottomRight, uv.x);
                //The Y position of UV corresponding to the vertex is the position point between the top and bottom of the difference
                //得到一组与顶点对应 使用uv的y位置为差值进度 介于之前上下底之间的位置点 
                //即最终获取到了一组在空间uv对应点  这些点所在的平面与绘图卡所在平面相同 即把绘图卡当做是1:1的UV框
                float4 fixedPos = lerp(bottom, top, uv.y);

                //That is to say, a group of UV corresponding points in space are finally obtained. The plane of these points is the same as that of the drawing card, that is, the drawing card is regarded as a 1:1 UV frame
                //把空间中的这组对应uv的坐标 通过矩阵转换到齐次裁剪空间 即取到了截图中的真实uv所在位置(还多了一个w值 在纹理采样的时候处理就可以)
                //VP here and unity in shader_ MATRIX_ The VP is actually the same, but the VP in the shader is always changing, and only the VP in the screenshot is read in C #
                //这里的VP和shader中的UNITY_MATRIX_VP其实是一样的 不过shader中的这个VP始终在变 而C#中只读取了截图时的VP
                o.fixedPos = ComputeScreenPos(mul(_VP, fixedPos));
               
                return o;
            }
            float4 frag(v2f i) : SV_Target
            {
                 //Image sampling
                 //图片采样 这里使用的uv是经过转化过后截图中真正的uv坐标
                 fixed4 texColor = tex2D(_MainTex,  i.fixedPos.xy / i.fixedPos.w);

                 return fixed4(texColor);
            }
            ENDCG
        }
    } 
            FallBack "Legacy Shaders/Transparent/Diffuse"
}