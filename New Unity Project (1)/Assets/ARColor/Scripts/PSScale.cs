using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSScale : MonoBehaviour
{

    ParticleSystem[] ps;
    public float psScaleFloat = 0.1f;

    void Reset()
    {
        foreach (var item in transform.GetComponentsInChildren<ParticleSystem>())
        {
            var main = item.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            item.transform.localScale = new Vector3(psScaleFloat, psScaleFloat, psScaleFloat);
            Debug.Log("------------->");
        }
    }
}