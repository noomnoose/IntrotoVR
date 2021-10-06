using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move3DText : MonoBehaviour {


    public void ToLeft()
    {
        transform.position=new Vector3(transform.position.x+1f, transform.position.y, transform.position.z);
    }

    public void ToRight()
    {
        transform.position = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
    }
}
