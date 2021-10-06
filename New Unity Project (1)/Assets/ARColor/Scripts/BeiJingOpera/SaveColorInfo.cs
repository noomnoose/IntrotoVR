using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;


/// <summary>
/// Store color information
/// 储存颜色信息
/// </summary>
public class SaveColorUtil 
{
    /// <summary>
    /// Previously stored color information
    /// 之前已经存储过的涂色信息
    /// </summary>
    public ColorData[] oldColorData;


    public static SaveColorUtil instance=null;

    public static SaveColorUtil GetInstance()
    {
        if (instance==null)
        {
            instance = new SaveColorUtil();
        }
        return instance;
    }

    #region 保存涂色信息

    /// <summary>
    /// Save color information保存涂色信息
    /// </summary>
    /// <param name="cardNm">Color card information - one file is stored separately for each color card涂色卡信息-每个涂色卡单独存储一个文件</param>
    /// <param name="time">Current save time - used to distinguish the information saved each time当前的保存时间-用来区别每次保存的信息</param>
    /// <param name="TopLeft_Pl_W"></param>
    /// <param name="BottomLeft_Pl_W"></param>
    /// <param name="TopRight_Pl_W"></param>
    /// <param name="BottomRight_Pl_W"></param>
    /// <param name="VP"></param>
    public void SaveCardPoints(string cardNm, string time, Vector3 TopLeft_Pl_W, Vector3 BottomLeft_Pl_W, Vector3 TopRight_Pl_W, Vector3 BottomRight_Pl_W, Matrix4x4 VP)
    {
        //Full name of TXT file
        //txt文件全名
        string _fileFullNm = cardNm + ".txt";

        //Current color information to be saved
        //当前要保存的涂色信息
        ColorData _colorData = new ColorData();
        _colorData.TimeDate = time;
        _colorData.TopLeft_Pl_W = TopLeft_Pl_W;
        _colorData.BottomLeft_Pl_W = BottomLeft_Pl_W;
        _colorData.TopRight_Pl_W = TopRight_Pl_W;
        _colorData.BottomRight_Pl_W = BottomRight_Pl_W;
        _colorData.VP = VP;

        /*Put the color information into the array*/
        /*把涂色信息放入数组中*/
        //New full color information
        //新的全部涂色信息
        ColorDatas _NewColorDatas = new ColorDatas();
        //Color information content
        //涂色信息内容
        ColorData[] _colorDatasSave;

        //Get the previous coloring information
        //得到之前的着色信息
        ColorDatas _oldColorDatas = LoadColorInfo(_fileFullNm);
        if (_oldColorDatas != null) //There is already coloring information 已有着色信息
        {
            int _oldNum = _oldColorDatas.Datas.Length;

            _colorDatasSave = new ColorData[_oldNum + 1];

            //First, input all the old information 先将旧的信息全部录入
            for (int i = 0; i < _colorDatasSave.Length - 1; i++)
            {
                _colorDatasSave[i] = _oldColorDatas.Datas[i];
            }

            //Information to add information to the last position 在最后位置加入信息的信息
            _colorDatasSave[_colorDatasSave.Length - 1] = _colorData;
        }
        else //If there is no coloring information before, all the coloring information is itself 之前无着色信息 则全部涂色信息就是本身
        {
            _colorDatasSave = new ColorData[] { _colorData };
        }


        _NewColorDatas.Datas = _colorDatasSave;

        string _str = JsonUtility.ToJson(_NewColorDatas);

        CreateFile(Application.persistentDataPath, _fileFullNm, _str);
    }

    /// <summary>
    /// Save picture
    /// 保存图片
    /// </summary>
    /// <param name="te"></param>
    /// <param name="directory"></param>
    /// <param name="location"></param>
    public void SaveTe(Texture2D te, string time)
    {
        //JPG file full path
        //jpg文件全路径
        string filename = time + ".jpg";
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        //Path location to save
        //要保存的路径位置
        string directory = Application.persistentDataPath;

        //Converting textur2d to binary data
        //把Texture2D转化为二进制数据
        byte[] bytes = te.EncodeToJPG(97);

        //Whether the path exists, and if not, create it
        //是否存在该路径，如果不存在则创建该路径
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        //create a file
        //创建文件
        FileStream file = File.Open(filePath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        Debug.Log(filePath);


    }

    #endregion

    #region Read color information 读取涂色信息

    /// <summary>
    /// Read the file to save the color information and get the corresponding relationship between the storage time and the information
    /// 读取保存涂色信息的文件得到保存时间与信息的对应关系
    /// </summary>
    /// <param name="fileFullName"></param>
    /// <returns></returns>
    public Dictionary<string, ColorData> LoadColorTimeAndCtt(string fileFullName)
    {
        //Read the original information in the file first
        //先读取文件中的原始信息
        ColorDatas _colorDatas = LoadColorInfo(fileFullName);
        if (_colorDatas == null)//If the data has not been saved 如果没有保存过数据
        {
            return null;
        }

        if (_colorDatas.Datas.Length <= 0)//If the saved data content is empty 如果保存的数据内容为空
        {
            return null;
        }

        //Organize the original information into the corresponding relationship between time and specific coloring content
        //把原始信息整理为时间与具体涂色内容的对应关系
        Dictionary<string, ColorData> _dic_DateAndData = new Dictionary<string, ColorData>();
        ColorData[] _datas = _colorDatas.Datas;
        for (int i = 0; i < _datas.Length; i++)
        {
            string _timeDate = _datas[i].TimeDate;
            _dic_DateAndData.Add(_timeDate, _datas[i]);
        }

        return _dic_DateAndData;
    }

    /// <summary>
    /// Read the corresponding relationship between all the time and the picture through the time information
    /// 通过时间信息读取到所有的时间与图片的对应关系
    /// </summary>
    /// <param name="timeDatas"></param>
    /// <returns></returns>
    public Dictionary<string, Texture2D> LoadColorTimeAndTe(List<string> timeDatas)
    {
        //If the time information passed in is empty
        //如果传入的时间信息为空
        if (timeDatas==null){return null; }
        //If the number of incoming time information is less than or equal to 0
        //如果传入的时间信息数量小于等于0
        if (timeDatas.Count<=0){return null;}

        //Read the picture and match the time to the picture
        //读取图片 并把时间和图片一一对应
        Dictionary<string, Texture2D> _dic_TimeAndTe=new Dictionary<string, Texture2D>();

        List<Texture2D> _tes= LoadColorTestrues(timeDatas);

        for (int i = 0; i < timeDatas.Count; i++)
        {
            
            string _timeDate = timeDatas[i];
            Texture2D _te = _tes[i];
            _dic_TimeAndTe.Add(_timeDate, _te);
        }

        return _dic_TimeAndTe;
    }



    /// <summary>
    /// Read the corresponding color picture through the time information
    /// 通过时间信息读取对应的涂色图片
    /// </summary>
    /// <param name="timeDatas"></param>
    /// <returns></returns>
    public List<Texture2D> LoadColorTestrues(List<string> timeDatas)
    {
        //If the time information passed in is empty
        //如果传入的时间信息为空
        if (timeDatas == null)
        {
            return null;
        }

        //If the number of incoming time messages is less than or equal to 0
        //如果传入的时间信息数量小于等于0
        if (timeDatas.Count <= 0)
        {
            return null;
        }

        List<Texture2D> _tes = new List<Texture2D>();

        //All colored content saved
        //加载所有保存过的涂色内容
        for (int i = 0; i < timeDatas.Count; i++)
        {
            //Build add in
            //生成加载项
            string _timeDate = timeDatas[i];

            //The whole path of the picture corresponding to time
            //时间所对应图片的全路径
            string filePath = Application.persistentDataPath + @"/" + _timeDate + ".jpg";

            //Texture2D tex = new Texture2D(1920, 1080);
            Texture2D _te = new Texture2D(Screen.width, Screen.height);

            byte[] _pic = File.ReadAllBytes(filePath);

            _te.LoadImage(_pic);

            _tes.Add(_te);
        }

        return _tes;
    }

    /// <summary>
    /// Read color information
    /// 读取涂色信息
    /// </summary>
    /// <param name="fileFullName">The full name of the file, including the extension文件的全称 包括扩展名</param>
    /// <returns></returns>
    public ColorDatas LoadColorInfo(string fileFullName)
    {
        string path = Application.persistentDataPath;
        string data = LoadFile(path, fileFullName);
        if (data == null || data == "")
        {
            return null;
        }

        ColorDatas _colorDatas = JsonUtility.FromJson<ColorDatas>(data);


        return _colorDatas;     
    }


    #endregion

    #region Delete coloring information删除涂色信息

    /// <summary>
    /// Delete all information 删除所有信息
    /// </summary>
    /// <param name="directoryPath"></param>
    public void DeleteAllColorInfo()
    {

        string directoryPath = Application.persistentDataPath;

        DirectoryInfo direction = new DirectoryInfo(directoryPath);

        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            string FilePath = directoryPath + "/" + files[i].Name;

            File.Delete(FilePath);
        }

        //Debug.Log("there is files："+ Directory.GetFiles(directoryPath).ToList().Count);

       
    }

    //IEnumerator DeleteOneByOne()
    //{
    //    yield return new wai
    //}

    public void DeleteOneColor(string cardNm, string timeData)
    {
        DeleteColorData(cardNm, timeData);
        DeleteColorTe(timeData);
    }

    /// <summary>
    /// Delete the color information of the specified entry
    /// 删除指定条目的涂色信息
    /// </summary>
    /// <param name="cardNm"></param>
    /// <param name="timeData"></param>
    public void DeleteColorData(string cardNm,string timeData)
    {
        //Full name of TXT file
        //txt文件全名
        string _fileFullNm = cardNm + ".txt";
        //Get the previous coloring information
        //得到之前的着色信息
        ColorDatas _oldColorDatas = LoadColorInfo(_fileFullNm);

        //New full color information
        //新的全部涂色信息
        ColorDatas _NewColorDatas = new ColorDatas();

        //The old color information set is used for filtering
        //旧涂色信息集合 用于筛选
        List<ColorData> _newColors = new List<ColorData>();

        //Delete the information to be removed from the old coloring information set
        //从旧的涂色信息集合中删除要去除的信息
        for (int i = 0; i < _oldColorDatas.Datas.Length; i++)
        {
            ColorData _data = _oldColorDatas.Datas[i];

            if (_data.TimeDate== timeData)
            {
                continue;
            }

            _newColors.Add(_data);
        }

        //The new color information content is used for assignment
        //新涂色信息内容 用于赋值
        ColorData[] _colorDatasSave= new ColorData[_newColors.Count];
        for (int i = 0; i < _colorDatasSave.Length; i++)
        {
            _colorDatasSave[i] = _newColors[i];
        }

        _NewColorDatas.Datas = _colorDatasSave;


        string _str = JsonUtility.ToJson(_NewColorDatas);

        CreateFile(Application.persistentDataPath, _fileFullNm, _str);
    }

    /// <summary>
    /// Removes the specified paint map
    /// 删除指定的涂色贴图
    /// </summary>
    /// <param name="timeData"></param>
    public void DeleteColorTe(string timeData)
    {
        //Full name of JPG file
        //jpg文件全名
        string _fileFullNm = timeData + ".jpg";

        string _fullPaht = Application.persistentDataPath+@"/"+ _fileFullNm;

        File.Delete(_fullPaht);
    }

    #endregion

    #region File operation文件操作

    /// <summary>
    /// Read file returns the information read from the file as a string
    /// 读取文件 把从文件中读取到的信息以字符串的形式返回
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string LoadFile(string path, string filename)
    {

        if (!IsExistsFile(path, filename))
        {
            Debug.Log("file not exist 文件不存在");
            return null;
        }

        //Read file in the form of file stream
        //文件流的形式读取文件
        StreamReader sr = File.OpenText(path + "//" + filename);
        ArrayList arr = new ArrayList();
        while (true)
        {
            //Read the file stream information line by line and read it as a string
            //逐行读取文件流信息读取成字符串
            string line = sr.ReadLine();
            //When the value read is empty, jump out of the loop to read
            //读取到的数值为空时跳出循环读取
            if (line == null){ break;}
            //Add the read string to the ArrayList
            //把读取到的字符串添加到ArrayList中
            arr.Add(line);
        }
        string str = "";
        foreach (string i in arr)
            str += i;
        sr.Close();
        sr.Dispose();
        return str;
    }

    /// <summary>
    /// create file
    /// 创建文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filename"></param>
    /// <param name="info"></param>
    public static void CreateFile(string path, string filename, string info)
    {

        StreamWriter steamWrite;
        FileInfo finfo = new FileInfo(path + "//" + filename);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (finfo.Exists)
            finfo.Delete();
        steamWrite = finfo.CreateText();
        steamWrite.WriteLine(info);
        steamWrite.Close();
        steamWrite.Dispose();
    }

    /// <summary>
    /// Determine if the folder exists
    /// 判断文件夹是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool IsExistsFile(string path, string filename)
    {
        if (!Directory.Exists(path))
            return false;
        if (!File.Exists(path + "//" + filename))
            return false;
        return true;
    }
    #endregion


}
