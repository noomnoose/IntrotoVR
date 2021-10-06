using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseColor : MonoBehaviour
{
    #region variable 变量

    public string CardNm = "";

    public GameObject[] ColorParts;
    /// <summary>
    /// Do you want to render the map
    /// 是否要渲染贴图
    /// </summary>
    public bool BLrenderIntoTexture = false;
    /// <summary>
    /// Transparency texture
    /// 透明贴图
    /// </summary>
    public Texture Te_Tran;
    /// <summary>
    /// Do you want to save the texture
    /// 是否要保存贴图
    /// </summary>
    public bool IfSaveColor = false;

    /// <summary>
    /// The width of track image in Unity Engine
    /// 识别图在Unity中的宽度
    /// </summary>
    public float ImageWidth = 1f;
    /// <summary>
    /// The height of track image in Unity Engine
    /// 识别图在Unity中的高度
    /// </summary>
    public float ImageHeight = 1f;

    #region protected variable 继承类可用变量


    protected Texture2D ColorTe;

    //World coordinate of the points on the card
    protected Vector3 TopLeft_Pl_W;
    protected Vector3 BottomLeft_Pl_W;
    protected Vector3 TopRight_Pl_W;
    protected Vector3 BottomRight_Pl_W;
    protected Matrix4x4 VP; 
    protected Vector3 Center_Card=new Vector3();
    protected float Half_W;
    protected float Half_H;
    #endregion

    #endregion


    protected virtual void Start()
    {
       
        Half_W = 0.5f * ImageWidth;
        Half_H = 0.5f * ImageHeight;

        TopLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, Half_H);
        BottomLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        TopRight_Pl_W = Center_Card + new Vector3(Half_W, 0, Half_H);
        BottomRight_Pl_W = Center_Card + new Vector3(Half_W, 0, -Half_H);
    }


    public void ShotAndColor()
    {
       


        StartCoroutine(ScreenShot());
        StartCoroutine(Get_Position());

        if (IfSaveColor)
        {
            //Current moment
            //当前时刻
            string _time = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            StartCoroutine(Save_Position(_time)) ;
            StartCoroutine(SaveShot(_time));
        }
    }

    //Transfer information to shader
    IEnumerator Get_Position()
    {
        yield return new WaitForEndOfFrame();

        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, BLrenderIntoTexture);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        VP = P * V;


        foreach (var item in ColorParts)
        {
            item.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

            item.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        }
      
    }

    ////ScreenShot
    IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        //yield return null;  //When public you can use this

        ColorTe = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ColorTe.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ColorTe.Apply();


        foreach (var item in ColorParts)
        {
            item.GetComponent<Renderer>().material.mainTexture = ColorTe;
        }
    }


    IEnumerator Save_Position(string time)
    {
        yield return new WaitForEndOfFrame();
        SaveColorUtil.GetInstance().SaveCardPoints(CardNm, time, TopLeft_Pl_W, BottomLeft_Pl_W, TopRight_Pl_W, BottomRight_Pl_W, VP);
    }


    IEnumerator SaveShot(string time)
    {
        yield return new WaitForEndOfFrame();

        if (ColorTe==null)
        {
            ColorTe = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }

        SaveColorUtil.GetInstance().SaveTe(ColorTe, time);
    }

    /// <summary>
    /// Make the model transparent
    /// 把模型变回透明
    /// </summary>
    public void RemoveTexture()
    {
        foreach (var item in ColorParts)
        {
            item.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        }
    }


    public void Set_SavedColor(Texture te, Vector3 topLeft_Pl_W, Vector3 bottomLeft_Pl_W, Vector3 topRight_Pl_W, Vector3 bottomRight_Pl_W, Matrix4x4 vP)
    {
        foreach (var item in ColorParts)
        {
            item.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(topLeft_Pl_W.x, topLeft_Pl_W.y, topLeft_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(bottomLeft_Pl_W.x, bottomLeft_Pl_W.y, bottomLeft_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(topRight_Pl_W.x, topRight_Pl_W.y, topRight_Pl_W.z, 1f));
            item.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(bottomRight_Pl_W.x, bottomRight_Pl_W.y, bottomRight_Pl_W.z, 1f));

            item.GetComponent<Renderer>().material.SetMatrix("_VP", vP);

            item.GetComponent<Renderer>().material.mainTexture = te;
        }
    }

}
