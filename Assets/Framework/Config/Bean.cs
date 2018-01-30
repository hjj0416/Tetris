using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class Bean 
{

    public static Dictionary<string, List<List<string>>> TableLists;

    public static List<List<string>> LoadTemplateData(TextAsset data, string TableName)
    {
            if (data == null || data.text == null)
            {
                Debug.Log("Table[" + TableName + "] does not exist!");
                return null;
            }
            string[] lines = data.text.Split(new char[] { '\n'});
            if (lines.Length <= 3)
            {
                Debug.Log("Table[" + TableName + "] line length is less than 3 lines");
                return null;
            }

            string lineField = lines[0].Replace('\r', '\0');
            int fieldCount = lineField.Split(new char[] { '\t' }).Length;

            List<List<string>> lineList = new List<List<string>>();
            for (int i = 3; i < lines.Length; i++)
            {
                if (lines[i] == null || lines[i].Length == 0) continue;
                string line = lines[i];//.Trim();//Trim(new char[] { ' ', '\r', '\n', '\t', '"' });
                if (string.IsNullOrEmpty(line) || line.Length == 0) continue;

                string[] stringList = line.Split(new char[] { '\t' });

                if (fieldCount > stringList.Length)
                {
                    if (! string.Equals(TableName, "Language", StringComparison.CurrentCultureIgnoreCase))
                        Debug.Log("Table["+TableName+"] line["+(i+1)+"] length is not same as field count  ");
                   continue;
                }

                List<string> list = new List<string>();
                for (int j = 0; j < stringList.Length; j++)
                {
                    list.Add(stringList[j]);
                }

                lineList.Add(list);
            }

        return lineList;
    }


    #region load config

    public static void TryLoadConfig(string configName)
    {
        string configPath = String.Format("Config/{1}", PathDefine.DIR_CONFIG, configName);
        TableLists = new Dictionary<string, List<List<string>>>();
        TextAsset data = Resources.Load<TextAsset>(configPath);
        List<List<string>> arraylist = LoadTemplateData(data, configPath);
        TableLists[configName.ToLower()] = arraylist;
        Type ex = Type.GetType(configName + "Bean", false, true);
        if (ex != null)
        {
            MethodInfo mi = ex.GetMethod("Init");
            mi.Invoke(null, null);
        }
        else
        {
            Debug.Log("Fai to Init:" + configName);
        }
    }


    #endregion




    public enum DataType
    {
        Long,
        Int,
        UInt,
        Short,
        Float,
        String,
        Bool,
        Byte,
        Color,
        Enum,
        DateTime,
        Hex,//16进制数据,以0x开头
        //数组类型定义在这后面
        LongArray,
        IntArray,
        ShortArray,
        FloatArray,
        StringArray,
        BoolArray,
        ByteArray,
        Vector3,
        Vector2,
        IntArray2D,//格式“101;1|102,2”,全是int类型
        FloatArray2D//格式“1.1;1|1.2,2”,全是int类型
    }

   


    public class Convertor
    {
        private static Convertor _Instance;
        public static Convertor Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Convertor();
                }

                return _Instance;
            }
        }

        private Dictionary<DataType, string> TypeDict = new Dictionary<DataType, string>();
        
        public Convertor()
        {
            foreach (var dataStruct in DataStructs)
            {
                TypeDict[dataStruct.type] = dataStruct.desc;
            }
        }

        public DataType Str2Type(string strType)
        {
            try
            {
                DataType type = (DataType) Enum.Parse(typeof (DataType), strType, true);
                return type;
            }
            catch(Exception e)
            {
                Debug.LogError("Fail to convert type:" + strType);
              return DataType.String;  
            }
        }

        public string Type2Str(DataType dataType)
        {
            if (TypeDict.ContainsKey(dataType))
            {
                return TypeDict[dataType];
            }

            return TypeDict[DataType.String];
        }


        //表格的数据类型转换为C#的数据类型
        public string Tabletype2Ctype(string tableType)
        {
            return Type2Str(Str2Type(tableType));
        }



        public class DataStruct
        {
            public DataType type;
            public string desc;

            public DataStruct(DataType type, string desc)
            {
                this.type = type;
                this.desc = desc;
           
            }
        }

        public static DataStruct[] DataStructs = new[]
        { 
            new DataStruct(DataType.Long, "long"),
            new DataStruct(DataType.Int,"int"),
            new DataStruct(DataType.UInt, "uint"),
            new DataStruct(DataType.Short,"ushort"),
            new DataStruct(DataType.Float, "float"),
            new DataStruct(DataType.String, "string"),
            new DataStruct(DataType.Bool, "bool"),
            new DataStruct(DataType.Byte, "byte"),
            new DataStruct(DataType.Enum, "enum"),
            new DataStruct(DataType.DateTime, "DateTime"),
            new DataStruct(DataType.Hex, "int"),
            //数组类型定义在这后面
            new DataStruct(DataType.LongArray, "long[]"),
            new DataStruct(DataType.IntArray, "int[]"),
            new DataStruct(DataType.ShortArray, "short[]"),
            new DataStruct(DataType.FloatArray, "float[]"),
            new DataStruct(DataType.StringArray, "string[]"),
            new DataStruct(DataType.BoolArray, "bool[]"),
            new DataStruct(DataType.ByteArray, "byte[]"),
            new DataStruct(DataType.Vector3, "Vector3"),
            new DataStruct(DataType.Vector2, "Vector2"),
            new DataStruct(DataType.IntArray2D, "List<int[]>"),//格式“101;1|102;2”,全是int类型
            new DataStruct(DataType.FloatArray2D, "List<float[]>")//格式“1.1;1|1.2;2”,全是float类型

        };
    }

    

    //Double Array
    public static double[] ParseNumArray(string val, char splitChar = ';')
    {
        if (string.IsNullOrEmpty(val))
            return null;

        string[] vals = val.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);
        if (vals.Length <= 0) return null;
        double[] ret = new double[vals.Length];
        int i = 0;
        foreach (string strVal in vals)
        {
            try
            {
                ret[i] = double.Parse(strVal);
            }
            catch (System.Exception ex)
            {
                ret[i] = 0;
            }
            i++;
        }
        return ret;

    }

    public static T ParseData<T>(DataType type, string strVal, T def = default(T))
    {
        return Convert<T>(ParseProp(type, strVal), def);
    }

    static T Convert<T>(object value, T def = default(T))
    {
        try
        {
            if (value == null) return def;
            if (value is string && typeof(T) == typeof(int))
            {
                value = System.Convert.ToInt32(value);
                return (T)value;
            }
            return (T)value;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Convert {0} to type {1} failed. Error:{2}", value, typeof(T), ex.Message));
            return def;
        }
    }

    static object ParseProp(DataType type, string strVal)
    {
        try
        {
            object value = null;
            double[] ret;
            switch (type)
            {
                case DataType.Long:
                    long lVal;
                    if (long.TryParse(strVal, out lVal))
                    {
                        value = lVal;
                    }
                    break;
                case DataType.Int:
                    int iVal;
                    if (int.TryParse(strVal, out iVal))
                    {
                        value = iVal;
                    }
                    break;
                case DataType.UInt:
                    uint uiVal;
                    if (uint.TryParse(strVal, out uiVal))
                    {
                        value = uiVal;
                    }
                    break;
                case DataType.Short:
                    short sVal;
                    if (short.TryParse(strVal, out sVal))
                    {
                        value = sVal;
                    }
                    break;
                case DataType.Float:
                    float fVal;
                    if (float.TryParse(strVal, out fVal))
                    {
                        value = fVal;
                    }
                    break;
                case DataType.Bool:
                    bool bVal;
                    if (bool.TryParse(strVal, out bVal))
                    {
                        value = bVal;
                    }
                    break;
                case DataType.String:
                    value = strVal.Trim();/*.Replace("\r", string.Empty);
                    if (strVal.EndsWith("\r"))
                    {
                        Main.LogError(strVal + "   " + value);
                    }*/
                    break;
                case DataType.Byte:
                    byte yVal;
                    if (byte.TryParse(strVal, out yVal))
                    {
                        value = yVal;
                    }
                    break;
                case DataType.DateTime:
                    DateTime dateTime;
                    DateTime.TryParse(strVal, out dateTime);
                    value = dateTime;
                    break;

                case DataType.Hex:
                    if(strVal.StartsWith("0x") || strVal.StartsWith("0X"))
                    {
                        strVal = strVal.Substring(2);
                    }
                    value = int.Parse(strVal, System.Globalization.NumberStyles.HexNumber);
                    break;

                case DataType.Enum:
                    if (!string.IsNullOrEmpty(strVal))
                    {
                        if (string.IsNullOrEmpty(strVal))
                        {
                            value = (byte)0;
                        }
                        else
                        {
                            byte byteVal;
                            if (byte.TryParse(strVal, out byteVal))
                                value = byteVal;
                            else
                                value = strVal;
                        }
                    }
                    break;
                case DataType.Vector3:
                    ret = ParseNumArray(strVal);
                    
                    if (ret.Length < 3)
                    {
                        value = Vector3.zero;
                    }
                        
                    else
                    {
                        value = new Vector3((float) ret[0], (float) ret[1], (float) ret[2]);
                    }
                    break;
                case DataType.Vector2:
                        ret = ParseNumArray(strVal);
                        
                        if (ret.Length < 2)
                        {
                            value = Vector2.zero;
                        }
                        
                        else
                        {
                            value = new Vector2((float) ret[0], (float) ret[1]);
                        }
                    break;
                case DataType.LongArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => (long)d);
                    break;
                case DataType.IntArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => (int)d);
                    break;
                case DataType.ShortArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => (short)d);
                    break;
                case DataType.ByteArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => (byte)d);
                    break;
                case DataType.FloatArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => (float)d);
                    break;
                case DataType.BoolArray:
                    ret = ParseNumArray(strVal);
                    if (ret != null) value = Array.ConvertAll(ret, d => d != 0);
                    break;
                case DataType.StringArray:
                    value = strVal.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                case DataType.IntArray2D:
                    string[] arrayInts = strVal.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    int intsCount = arrayInts.Length;
                    List<int[]> intArray2D = new List<int[]>();
                    for (int i = 0; i < intsCount; i++)
                    {
                        double[] temp = ParseNumArray(arrayInts[i]);
                        intArray2D.Add(Array.ConvertAll<double, int>(temp, d =>
                        {
                            return (int)d;
                        }));
                    }

                    value = intArray2D;
                    break;

                case DataType.FloatArray2D:
                    string[] arrayFloats = strVal.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    int floatsCount = arrayFloats.Length;
                    List<float[]> floatArray2D = new List<float[]>();
                    for (int i = 0; i < floatsCount; i++)
                    {
                        double[] temp = ParseNumArray(arrayFloats[i]);

                        floatArray2D.Add(Array.ConvertAll<double, float>(temp, d =>
                        {
                            return (float)d;
                        }));
                    }

                    value = floatArray2D;
                    break;


                default:
                    Debug.LogError(string.Format("Property:{0} 's type is not defined!", type));
                    break;
            }
            return value;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Parse property:[key:{0} val={1}] error, {2}", type, strVal, ex.Message));
            return null;
        }
    }
}
