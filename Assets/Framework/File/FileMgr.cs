//using System.Net.Mime;
//using DotNet.Utilities;
//using UnityEngine;

//public class FileMgr
//{
//    public static string key = "helloworld!";

//    private static DESAlgorithm _des;
//    private static DESAlgorithm DES
//    {
//        get
//        {
//            if(_des == null)
//                _des = new DESAlgorithm();
//            return _des;
//        }
//    }


//    public static string DataPath
//    {
//        get { return Application.persistentDataPath; }
//    }

//    public static string CachePath
//    {
//        get { return Application.temporaryCachePath; }
//    }

//    /// <summary>
//    /// 写文件
//    /// </summary>
//    /// <param name="path"></param>
//    /// <param name="str"></param>
//    /// <param name="key"></param>
//    public static void WriteFile(string path, string str)
//    {
//        str = DES.EncodePwd(str, key);
//        //NGUIDebug.Log(str);
//        FileOperate.WriteFile(path, str);
//    }
//    /// <summary>
//    /// 读文件
//    /// </summary>
//    /// <param name="path"></param>
//    /// <param name="key"></param>
//    /// <returns></returns>
//    public static string ReadFile(string path)
//    {
//        string str = FileOperate.ReadFile(path);
//        //NGUIDebug.Log(str);
//        str = DES.DecodePwd(str, key);
//        return str;
//    }

//    public static void DeleteFile(string path)
//    {
        
//    }

//}
