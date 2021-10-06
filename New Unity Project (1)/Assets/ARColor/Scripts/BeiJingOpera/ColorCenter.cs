using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Color and save
/// 涂色并保存
/// </summary>
public class ColorCenter : BaseColor
{
    /// <summary>
    /// UI to display painting history
    /// 显示涂色历史的UI
    /// </summary>
    public GameObject Atlas;
    /// <summary>
    /// Displays the root node of the saved colored list
    /// 显示保存过的涂色的列表根节点
    /// </summary>
    public Transform ImList = null;

    /// <summary>
    /// The corresponding relationship between save time and texture
    /// 保存时间与图片的对应关系
    /// </summary>
    public Dictionary<string, Texture2D> Dic_DateAndTe;
    /// <summary>
    /// The corresponding relationship between storage time and color data
    /// 保存时间与涂色数据的对应关系
    /// </summary>
    public Dictionary<string, ColorData> Dic_DateAndData;

    /// <summary>
    /// Is coloring allowed
    /// 是否允许着色
    /// </summary>
    private bool ifCanColor = true;

    /// <summary>
    /// Color button
    /// 着色按钮
    /// </summary>
    public void Btn_Color()
    {
        if (ifCanColor)
        {
            ShotAndColor();
            ifCanColor = false;
            StartCoroutine(delayColor());
        }
        else
        {
            Debug.Log("点的太快了");
        }
       
    }

    /// <summary>
    /// Clear the influence of existing maps
    /// 清除现有贴图的影响
    /// </summary>
    public void Btn_Clean()
    {
        RemoveTexture();
    }

    /// <summary>
    /// Turn off the painting history Atlas
    /// 关闭涂色历史图集
    /// </summary>
    public void Btn_AtlasClose()
    {
        Atlas.SetActive(false);
    }

    /// <summary>
    /// Loading saved coloring information
    /// 加载保存过的涂色信息
    /// </summary>
    public void LoadSavedColorDatas()
    {
        Dic_DateAndTe = new Dictionary<string, Texture2D>();
        Dic_DateAndData = new Dictionary<string, ColorData>();

        string _fileNm =CardNm+ ".txt";

        ColorDatas _colorDatas = SaveColorUtil.GetInstance().LoadColorInfo(_fileNm);

        if (_colorDatas == null)//If the data has not been saved 如果没有保存过数据 
        {
            return;
        }

        if (_colorDatas.Datas.Length <= 0)//If the saved data content is empty
        {
            return;
        }

        GameObject _imPre = Resources.Load<GameObject>("Im_Saved");

        ColorData[] _datas = _colorDatas.Datas;

        //Find all currently loaded content and destroy them
        //找到所有当前的已加载过的内容 销毁他们
        Image[] _oldIms = ImList.GetComponentsInChildren<Image>();
        if (_oldIms != null)
        {
            if (_oldIms.Length > 0)
            {
                for (int i = 0; i < _oldIms.Length; i++)
                {
                    Destroy(_oldIms[i].gameObject);
                }
            }
        }

        //Collection of all color history time information
        //所有涂色历史时间信息的集合
        List<string> _timeDatas = new List<string>();
        for (int i = 0; i < _datas.Length; i++)
        {
            _timeDatas.Add(_datas[i].TimeDate);
        }

        //Read all pictures
        //读取所有图片
        List<Texture2D> _tes = SaveColorUtil.GetInstance().LoadColorTestrues(_timeDatas);

        //All colored content saved
        //加载所有保存过的涂色内容
        for (int i = 0; i < _datas.Length; i++)
        {
            //Build add in
            //生成加载项
            string _timeDate = _datas[i].TimeDate;

            GameObject _temp = Instantiate(_imPre);
            _temp.transform.SetParent(ImList);

            //Load text
            //加载文字
            _temp.transform.Find("Tx_Date").GetComponent<Text>().text = _timeDate;

            Texture2D tex = _tes[i];

            //Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            
            _temp.GetComponent<Image>().sprite = _sprite;

            //Dictionary
            //字典
            Dic_DateAndTe.Add(_timeDate, tex);
            Dic_DateAndData.Add(_timeDate, _datas[i]);

            //Thumbnail button
            //缩略图片按钮
            Button _btn = _temp.GetComponent<Button>();
            //Load button
            //加载按钮
            Button _btn_Color = _temp.transform.Find("Pn_Btns/Btn_Load").GetComponent<Button>();
            //Delete button
            //删除按钮
            Button _btn_Delete= _temp.transform.Find("Pn_Btns/Btn_Delete").GetComponent<Button>();

            
            _btn.onClick.AddListener(delegate ()
            {
                string _date = _btn.transform.Find("Tx_Date").GetComponent<Text>().text;
                Texture2D _savedTe = Dic_DateAndTe[_date];
                ColorData _savedDate = Dic_DateAndData[_date];

                Vector3 TopLeft_Pl_W = _savedDate.TopLeft_Pl_W;
                Vector3 BottomLeft_Pl_W = _savedDate.BottomLeft_Pl_W;
                Vector3 TopRight_Pl_W = _savedDate.TopRight_Pl_W;
                Vector3 BottomRight_Pl_W = _savedDate.BottomRight_Pl_W;
                Matrix4x4 VP = _savedDate.VP;

                Set_SavedColor(_savedTe, TopLeft_Pl_W, BottomLeft_Pl_W, TopRight_Pl_W, BottomRight_Pl_W, VP);

                Atlas.SetActive(false);
            });
            _btn_Color.onClick.AddListener(delegate ()
            {
                string _date = _btn.transform.Find("Tx_Date").GetComponent<Text>().text;
                Texture2D _savedTe = Dic_DateAndTe[_date];
                ColorData _savedDate = Dic_DateAndData[_date];

                Vector3 TopLeft_Pl_W = _savedDate.TopLeft_Pl_W;
                Vector3 BottomLeft_Pl_W = _savedDate.BottomLeft_Pl_W;
                Vector3 TopRight_Pl_W = _savedDate.TopRight_Pl_W;
                Vector3 BottomRight_Pl_W = _savedDate.BottomRight_Pl_W;
                Matrix4x4 VP = _savedDate.VP;

                Set_SavedColor(_savedTe, TopLeft_Pl_W, BottomLeft_Pl_W, TopRight_Pl_W, BottomRight_Pl_W, VP);

                Atlas.SetActive(false);
            });
            _btn_Delete.onClick.AddListener(delegate ()
            {
                string _date = _btn.transform.Find("Tx_Date").GetComponent<Text>().text;

                SaveColorUtil.GetInstance().DeleteOneColor(CardNm, _date);

                Destroy(_btn_Delete.transform.parent.parent.gameObject);
            });

            _temp.GetComponent<RectTransform>().localScale = new Vector3(1f,1f,1f);
        }

        Atlas.SetActive(true);
    }

    /// <summary>
    /// Delay 1.1 seconds after painting to allow painting again
    /// 涂色后延迟1.1秒才允许再次涂色
    /// </summary>
    /// <returns></returns>
    IEnumerator delayColor()
    {
        yield return new WaitForSeconds(1.1f);
        ifCanColor = true;
    }


    public void Btn_DeleteAll()
    {
        SaveColorUtil.GetInstance().DeleteAllColorInfo();
    }
}
