using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Single stored color information
/// 单次存储的颜色信息
/// </summary>
[Serializable]
public class ColorData 
{
    public string TimeDate;
    public Vector3 TopLeft_Pl_W;
    public Vector3 BottomLeft_Pl_W;
    public Vector3 TopRight_Pl_W;
    public Vector3 BottomRight_Pl_W;
    public Matrix4x4 VP;
}
