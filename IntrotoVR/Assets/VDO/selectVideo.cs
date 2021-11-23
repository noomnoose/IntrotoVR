using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class selectVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClip;

    int videoIndex = 0;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }
    void Update()
    {
        videoPlayer.clip = videoClip[videoIndex];
    }
    public void NextVideo()
    {
        videoIndex = videoIndex + 1;
       // Debug.Log(" " + videoIndex);
    }
    public void PassVideo()
    {
        videoIndex = videoIndex - 1;
        //Debug.Log(" " + videoIndex);
    }
}
