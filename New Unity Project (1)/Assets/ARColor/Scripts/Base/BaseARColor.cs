using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseARColor : MonoBehaviour
{
    /*Quadrangular coordinates of the drawing card of the tracked target*/
    /*追踪的目标绘图卡的四角坐标*/
    protected Vector3 pos_TopLeft;    //左上角 top left corner
    protected Vector3 pos_BottomLeft;  //左下角 lower left quarter
    protected Vector3 pos_TopRight;    //右上角 Top right corner
    protected Vector3 pos_BottomRight; //右下角 Lower right corner

    /// <summary>
    /// target image itself
    /// 追踪的目标绘图卡物体本身
    /// </summary>
    public GameObject Card_Track;
    /// <summary>
    /// the center of target image
    /// 追踪目标绘图卡的中心位置
    /// </summary>
    protected Vector3 Center_Card;
    /// <summary>
    /// the width of target image
    /// 追踪的目标绘图卡宽度
    /// </summary>
    protected float Half_W;
    /// <summary>
    /// the height of target image
    /// 追踪的目标绘图卡高度
    /// </summary>
    protected float Half_H;

    /// <summary>
    /// view matrix * projection matrix
    /// 视角矩阵*投影矩阵
    /// </summary>
    protected Matrix4x4 vp;

    /// <summary>
    /// Current screenshot
    /// 当前的截图
    /// </summary>
    protected Texture2D Te;

    /// <summary>
    /// Target model to be colored (one model may have multiple meshes)
    /// 需要上色的目标模型（一个模型可能有多个网格）
    /// </summary>
    public GameObject[] TargetModels;

    public Texture Te_Tran;
    /// <summary>
    /// Whether to render texture when getting the matrix from camera to projection
    /// 在得到从Camera到投影的矩阵时 是否进行纹理渲染
    /// Will affect the obtained matrix, generally select true
    /// 会影响获取到的矩阵  一般选True
    /// Different devices and system versions may use different methods. When the mapping error occurs, changing this value sometimes can solved the error.
    /// 不同的设备及系统版本可能会用到不同的方式，当贴图错误时更改这个值往往可以得到解决
    /// </summary>
    public bool BLrenderIntoTexture = true;


    /// <summary>
    /// Assign the data on the color card to the model
    /// 把涂色卡上的数据赋值给模型
    /// </summary>
    public void ColorTheModelFromImage()
    {
        GetColorData();
        StartCoroutine(ScreenShot());
    }

    /// <summary>
    /// Get the relevant data of coloring
    /// 获取涂色的相关数据
    /// </summary>
    private void GetColorData()
    {
        /*Get the world coordinates of the target tracking drawing card*/
        /*获取的目标追踪绘图卡的世界坐标*/
        Center_Card = Card_Track.transform.position;
        Half_W = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.x * Card_Track.transform.localScale.x * 0.5f;
        Half_H = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.z * Card_Track.transform.localScale.z * 0.5f;
        pos_TopLeft = Center_Card + new Vector3(-Half_W, 0, Half_H);
        pos_BottomLeft = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        pos_TopRight = Center_Card + new Vector3(Half_W, 0, Half_H);
        pos_BottomRight = Center_Card + new Vector3(Half_W, 0, -Half_H);

        Debug.Log(SystemInfo.graphicsDeviceType);

        //GetGPUProjectionMatrix Get the matrix from camera to projection
        //GetGPUProjectionMatrix 得到从Camera到投影的矩阵
        //It deals with the differences between DX and OpenGL, and automatically deals with the differences between platforms
        //是处理DX和OpenGL差异性的 会自动处理平台差异
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, BLrenderIntoTexture);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        vp = P * V;
    }

    /// <summary>
    /// screenshot
    /// 截图
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        //yield return null;  //When public you can use this

        Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        Te.Apply();

        SendPosToShader();
    }

    /// <summary>
    /// Send data to shader
    /// 给Shader发送数据
    /// </summary>
    private void SendPosToShader()
    {
        for (int i = 0; i < TargetModels.Length; i++)
        {
            GameObject _tempGO = TargetModels[i];

            _tempGO.GetComponent<Renderer>().material.mainTexture = Te;

            _tempGO.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(pos_TopLeft.x, pos_TopLeft.y, pos_TopLeft.z, 1f));
            _tempGO.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(pos_BottomLeft.x, pos_BottomLeft.y, pos_BottomLeft.z, 1f));
            _tempGO.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(pos_TopRight.x, pos_TopRight.y, pos_TopRight.z, 1f));
            _tempGO.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(pos_BottomRight.x, pos_BottomRight.y, pos_BottomRight.z, 1f));
            _tempGO.GetComponent<Renderer>().material.SetMatrix("_VP", vp);
        }
    }

    /// <summary>
    /// Assign a transparent map to the model map
    /// 把模型的贴图赋值为透明贴图
    /// </summary>
    public void RemoveTexture()
    {
        for (int i = 0; i < TargetModels.Length; i++)
        {
            GameObject _tempGO = TargetModels[i];
            _tempGO.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        }
    }
}
