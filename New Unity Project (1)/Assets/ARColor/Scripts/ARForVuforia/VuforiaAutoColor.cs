using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VuforiaAutoColor : MonoBehaviour {

    public GameObject Earth;
    public GameObject Frame;
    public GameObject EarthSmall;
    public GameObject SolarSystem;
    public GameObject Plane; //the plane show state color
    public Texture Te_Red;
    public Texture Te_Green;
    public GameObject Suc;  // Image prompt success
    public Texture Te_Tran;
    public bool BLrenderIntoTexture = false;


    private Vector3 Center_Card=new Vector3(); //World coordinate of the card
    private float Half_W; //half width
    private float Half_H;
    private float X_Sc;  //Scaling value

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

    //World coordinate of the points on the card
    private Vector3 TopLeft_Pl_W;    
    private Vector3 BottomLeft_Pl_W;
    private Vector3 TopRight_Pl_W;
    private Vector3 BottomRight_Pl_W;

    //Screen coordinate of the points on the card
    private Vector2 TopLeft_Pl_Sc;
    private Vector2 BottomLeft_Pl_Sc;
    private Vector2 TopRight_Pl_Sc;
    private Vector2 BottomRight_Pl_Sc;

    //Scan UI
    private Vector2 TopLeft_Scan;
    private Vector2 BottomLeft_Scan;
    private Vector2 TopRight_Scan;
    private Vector2 BottomRight_Scan;

    private bool B_Re = false;  // has recognized
    private bool B_Shot = false;  // Screenshot already

    private bool Bl_Rotate = false; 
    private bool Bl_ShowSolarSystem = false;



    // Use this for initialization
    void Start () {


        Half_W = 0.5f * ImageWidth;
        Half_H = 0.5f * ImageHeight;

        TopLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, Half_H);
        BottomLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        TopRight_Pl_W = Center_Card + new Vector3(Half_W, 0, Half_H);
        BottomRight_Pl_W = Center_Card + new Vector3(Half_W, 0, -Half_H);

        X_Sc = Screen.width / 1920f;

        TopLeft_Scan = new Vector2(Screen.width - 1300 * X_Sc, Screen.height + 800 * X_Sc) * 0.5f;
        BottomLeft_Scan = new Vector2(Screen.width - 1300 * X_Sc, Screen.height - 800 * X_Sc) * 0.5f;
        TopRight_Scan = new Vector2(Screen.width + 1300 * X_Sc, Screen.height + 800 * X_Sc) * 0.5f;
        BottomRight_Scan = new Vector2(Screen.width + 1300 * X_Sc, Screen.height - 800 * X_Sc) * 0.5f;
    }

    // Update is called once per frame
    void Update () {
        if (B_Shot == false) {
            //Get the Screen coordinates
            TopLeft_Pl_Sc = Camera.main.WorldToScreenPoint(TopLeft_Pl_W);
            BottomLeft_Pl_Sc = Camera.main.WorldToScreenPoint(BottomLeft_Pl_W);
            TopRight_Pl_Sc = Camera.main.WorldToScreenPoint(TopRight_Pl_W);
            BottomRight_Pl_Sc = Camera.main.WorldToScreenPoint(BottomRight_Pl_W);

            //Determine whether the card is within the frame of the scan
            if (TopLeft_Pl_Sc.x > TopLeft_Scan.x && TopLeft_Pl_Sc.y < TopLeft_Scan.y && BottomLeft_Pl_Sc.x > BottomLeft_Scan.x && BottomLeft_Pl_Sc.y > BottomLeft_Scan.y && TopRight_Pl_Sc.x < TopRight_Scan.x && TopRight_Pl_Sc.y < TopLeft_Scan.y && BottomRight_Pl_Sc.x < BottomRight_Scan.x && BottomRight_Pl_Sc.y > BottomRight_Scan.y)
            {
                if (B_Re == false)
                {
                    Plane.GetComponent<Renderer>().material.mainTexture = Te_Green;
                    StartCoroutine(Cancol_Occ());
                    Suc.SetActive(true);
                    B_Re = true;
                }
            }
            else
            {
                Plane.GetComponent<Renderer>().material.mainTexture = Te_Red;
                Suc.SetActive(false);
                B_Re = false;
            }
        }
    }

    //Transfer information to shader
    public void Get_Position() {

        //To Earth shader
        Earth.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //To Frame shader
        Frame.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f)); 
        Frame.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //To EarthSmall shader
        EarthSmall.GetComponent<Renderer>().material.SetVector("_UvTopLeft", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        EarthSmall.GetComponent<Renderer>().material.SetVector("_UvButtomLeft", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        EarthSmall.GetComponent<Renderer>().material.SetVector("_UvTopRight", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        EarthSmall.GetComponent<Renderer>().material.SetVector("_UvBottomRight", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        //Matrix information to shader
        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, BLrenderIntoTexture);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 VP = P * V;
        Earth.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        Frame.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        EarthSmall.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
    }

    //Ready to ScreenShot
    IEnumerator Cancol_Occ() {
        yield return new WaitForSeconds(2.0f);
        Suc.SetActive(false);
        Plane.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        StartCoroutine(ScreenShot());
        Get_Position();
    }

    //ScreenShot
    IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        //yield return null;  //When public you can use this

        if (B_Re == true) {
            Texture2D Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            Te.Apply();

            Earth.GetComponent<Renderer>().material.mainTexture = Te;
            Frame.GetComponent<Renderer>().material.mainTexture = Te;
            EarthSmall.GetComponent<Renderer>().material.mainTexture = Te;
            B_Shot = true;
            Plane.GetComponent<Renderer>().material.mainTexture = Te_Tran;  
        }

    }

    //Button
    public void Btn_Rotate()
    {
        if (!Bl_Rotate)
        {
            Earth.GetComponent<Sun_Rotate>().enabled = true;
        }
        else
        {
            Earth.GetComponent<Sun_Rotate>().enabled = false;
        }
        Bl_Rotate = !Bl_Rotate;
    }

    //Button
    public void Btn_ShowSolarSystem()
    {
        if (!Bl_ShowSolarSystem)
        {
            Earth.SetActive(false);
            Frame.SetActive(false);
            SolarSystem.SetActive(true);
        }
        else
        {
            Earth.SetActive(true);
            Frame.SetActive(true);
            SolarSystem.SetActive(false);
        }
        Bl_ShowSolarSystem = !Bl_ShowSolarSystem;
    }

    public void RemoveTexture()
    {
        Earth.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        Frame.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        EarthSmall.GetComponent<Renderer>().material.mainTexture = Te_Tran;
    }
}
