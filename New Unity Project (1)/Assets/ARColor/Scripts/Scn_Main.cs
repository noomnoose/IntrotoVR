using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scn_Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button[] _btns = transform.GetComponentsInChildren<Button>();

        foreach (Button btn in _btns)
        {
            btn.onClick.AddListener(
                delegate ()
                {
                    SceneManager.LoadScene(btn.name);
                }
            );
        }
    }


}
