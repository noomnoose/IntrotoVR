using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VuforiaSquare : MonoBehaviour {

    ////World coordinate of the points on the card
    private Vector3 TopLeft_Pl_W;
    private Vector3 BottomLeft_Pl_W;
    private Vector3 TopRight_Pl_W;
    private Vector3 BottomRight_Pl_W;

    public GameObject Card_Track;
    private Vector3 Center_Card;
    private float Half_W;
    private float Half_H;

    public GameObject Earth;
    public GameObject Frame;
    public Texture Te_Tran;
    public bool BLrenderIntoTexture=false;

    // Use this for initialization
    void Start () {
        //Get the World coordinates
        Center_Card = Card_Track.transform.position;
        Half_W = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.x * Card_Track.transform.localScale.x * 0.5f;
        Half_H = Card_Track.GetComponent<MeshFilter>().mesh.bounds.size.z * Card_Track.transform.localScale.z * 0.5f;
        /* If the color read in the AR scene shows a large deviation on the model, replace the code above with these two lines of code.This is due to Mesh grid data errors caused by the Unity or Vuforia version. Even if the same Unity and Vuforia are created at different times, different results may occur. */
        //Half_W = Card_Track.transform.localScale.x * 0.5f;
        //Half_H = Card_Track.transform.localScale.z * 0.5f;

        TopLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, Half_H);
        BottomLeft_Pl_W = Center_Card + new Vector3(-Half_W, 0, -Half_H);
        TopRight_Pl_W = Center_Card + new Vector3(Half_W, 0, Half_H);
        BottomRight_Pl_W = Center_Card + new Vector3(Half_W, 0, -Half_H);

        Debug.Log(SystemInfo.graphicsDeviceType);
    }

    // Update is called once per frame
    void Update () {

    }

    public void Btn_Color()
    {
        Get_Position();
        StartCoroutine(ScreenShot());
    }

    //Transfer information to shader
    public void Get_Position() {
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Earth.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));

        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1f));
        Frame.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1f));


        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, BLrenderIntoTexture);
        Matrix4x4 V = Camera.main.worldToCameraMatrix;
        Matrix4x4 VP = P * V;
        Earth.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
        Frame.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
    }

    public void RemoveTexture()
    {
        Earth.GetComponent<Renderer>().material.mainTexture = Te_Tran;
        Frame.GetComponent<Renderer>().material.mainTexture = Te_Tran;
    }

    ////ScreenShot
    IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        //yield return null;  //When public you can use this

        Texture2D Te = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Te.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            Te.Apply();
            Earth.GetComponent<Renderer>().material.mainTexture = Te;
            Frame.GetComponent<Renderer>().material.mainTexture = Te;      
     }

    
}
