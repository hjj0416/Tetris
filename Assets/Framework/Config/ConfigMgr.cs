///********************************************************************
//    purpose:	配置管理类，负责配置的加载。
// * 目前配置分两种：
// *  文本配置：TXT格式。第一行为名称，名称必须是规定的，即PID.如果没有，就需要在里面加上。 第二行说明，第三行是类型。
//*********************************************************************/

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.Text;
//using System.IO;



//[ExecuteInEditMode]
//public class ConfigMgr
//{
//    #region Singleton

//    private static ConfigMgr _Instance = null;
//    public static ConfigMgr Instance 
//    {
//        get
//        {
//            if (_Instance == null)
//            {
//                _Instance = new ConfigMgr();

//#if UNITY_EDITOR
//                int count = (int)(ConfigMgr.Table.Max);
//                for (int i = 0; i < count; i++)
//                {
//                    string configName = ((ConfigMgr.Table)i).ToString();
//                    TextAsset data = AssetDatabase.LoadAssetAtPath("Assets/GAME/Package/Config/" + configName + ".txt", typeof(TextAsset)) as TextAsset;
//                    if (data == null)
//                    {
//                        Debug.LogWarning(string.Format("load config:{0} failed!", configName));
//                    }
//                    else
//                    {
//                        _Instance.parse(configName, data.text);
//                    }
//                } 
//#endif
//            }

//            return _Instance;
//        }

//        #if UNITY_EDITOR
//        set
//        {
//            _Instance = value;
//        }
//        #endif

//    }

//    private string[] data;

    
        
//    #endregion

//    private Dictionary<string, Dictionary<int, Property>> configs = new Dictionary<string, Dictionary<int, Property>>();

//    public enum Table
//    {
//        //None = 0,
//        Language,
////        Skill,
////        Hero,
////        Monster,
////        Check,
////        CheckTask,
////        Equip,
////        EventBattle,
////        EventProps,
////        EventSpecial,
////        HeroAttr,
////        HeroLevel,
////        HeroTwoAttr,
////        Item,
////        PlayerLevel,
//        Max
//    }

//    #region 访问接口
//    public List<T> GetColumn<T>(Table table, PID pid, List<T> def)
//    {
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(table.ToString(), out config)) return def;
		
//        List<T> list = new List<T>();
//        foreach(int i in config.Keys)
//        {
//            list.Add(GetColumn<T>(table,pid,  i, default(T)));
//        }
//        return list;
//    }

//    public T GetColumn<T>(string configName,PID pid,  int cfgId, T def)
//    {
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(configName, out config)) return def;
//        Property prop;
//        if (!config.TryGetValue(cfgId, out prop))
//        {
//            return def;
//        }
//        return prop.GetProperty(pid, def);
//    }
    

//    public T GetColumn<T>(Table table, PID pid, int cfgId, T def)
//    {
//        return GetColumn<T>(table.ToString(),pid,  cfgId, def);
//    }

//    /// <summary>
//    /// 查询出field的值等于value的所有记录
//    /// </summary>
//    /// <param name="table"></param>
//    /// <param name="field"></param>
//    /// <param name="value"></param>
//    /// <param name="outList"></param>
//    /// <returns></returns>
//    public void Select<T>(Table table, PID field, T value, List<Property> outList)
//    {
//        outList.Clear();
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(table.ToString(), out config)) return;
//        foreach (Property prop in config.Values)
//        {
//            T val = prop.GetProperty(field, default(T));
//            if (object.Equals(val,value))
//            {
//                outList.Add(prop);
//            }
//        }
//    }

//    /// <summary>
//    /// 查询出field的值等于value的所有记录
//    /// </summary>
//    /// <param name="table"></param>
//    /// <param name="field"></param>
//    /// <param name="value"></param>
//    /// <returns></returns>
//    public Dictionary<int, Property> Select<T>(Table table, PID field, T value)
//    {
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(table.ToString(), out config)) return null;
//        Dictionary<int, Property> ret = new Dictionary<int, Property>();
//        foreach (var prop in config)
//        {
//            T val = prop.Value.GetProperty(field, default(T));
//            if (object.Equals(val, value))
//            {
//                ret[prop.Key] = prop.Value;
//            }
//        }
//        return ret;
//    }

//    public T GetByField<T>(Table table, PID field, string value, PID retFieldID, T def)
//    {
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(table.ToString(), out config)) return def;
//        foreach (var prop in config)
//        {
//            string strID = prop.Value.GetProperty(field, "");
//            if (strID == value)
//            {
//                return prop.Value.GetProperty<T>(retFieldID, def);
//            }
//        }
//        return def;
//    }
//    /// <summary>
//    /// 获取 表中 对应行
//    /// </summary>
//    /// <param name="tableName"></param>
//    /// <param name="cfgId"></param>
//    /// <returns></returns>
//    public Property GetRow(string tableName, int cfgId)
//    {
//        Dictionary<int, Property> config = GetTable(tableName);
//        if (config == null) return null;
//        if (config.ContainsKey(cfgId))
//        {
//            return config[cfgId];
//        }
//        else
//        {
//            //Main.LogError(string.Format("CfgTbl: {0} Not Id: {1}",tableName,cfgId));
//            return null;
//        }
//    }

//    public Property GetRow(Table table, int cfgId)
//    {
//        Dictionary<int, Property> config = GetTable(table.ToString());
//        if (config == null) return null;
//        if (config.ContainsKey(cfgId))
//        {
//            return config[cfgId];
//        }
//        else
//        {
//            //Main.LogError(string.Format("CfgTbl: {0} Not Id: {1}",tableName,cfgId));
//            return null;
//        }
//    }

//    public Property GetRow(Table table, int id1, int id2)
//    {
//        int cfgId = ToCrossID(id1, id2);
//        Dictionary<int, Property> config = GetTable(table.ToString());
//        if (config == null) return null;
//        if (config.ContainsKey(cfgId))
//        {
//            return config[cfgId];
//        }
//        else
//        {
//            //Main.LogError(string.Format("CfgTbl: {0} Not Id: {1}",tableName,cfgId));
//            return null;
//        }
//    }

//    /// <summary>
//    /// 查询指定配置表的配置数据
//    /// </summary>
//    /// <param name="tableName"></param>
//    /// <returns></returns>
//    public Dictionary<int, Property> GetTable(string tableName)
//    {
//        if (configs.ContainsKey(tableName))
//        {
//            return configs[tableName];
//        }
//        else
//        {
//            //Main.LogError("CfgTbl: "+tableName+" is null");
//            return null;
//        }
//    }

//    public Dictionary<int, Property> GetTable(Table table)
//    {
//        return GetTable(table.ToString());
//    }

//    public void ClearTable(Table table)
//    {
//        configs.Remove(table.ToString());
//    }


//    #region load

//    //总的配置数量
//    //public IEnumerator Init()
//    public void Init(VoidCallback cb)
//    {
//        int index = 0;
//        int count = (int) (Table.Max);
//        for (int i = 0; i < count; i++)
//        {
//            Table table = (Table)i;
//            string configName = table.ToString();
//            //AssetBundleManager.Instance.LoadResource<TextAsset>(BundleNameEnum.config.ToString(), configName, (textData) =>
//            //{
//            //    LoadTxtConfig(textData, configName);
//            //    index++;
//            //    if (index >= count)
//            //    {
//            //        if (cb != null)
//            //        {
//            //            cb();
//            //        }
//            //        AssetBundleManager.UnloadAssetBundle(BundleNameEnum.config.ToString());
//            //    }
//            //});
//        }          
        
//    }

//    #endregion


///// <summary>
///// 
///// </summary>
///// <param name="data">表文件文本</param>
///// <param name="configName">表名称</param>
//    private void LoadTxtConfig(TextAsset data, string configName)
//    {
//        if (data == null)
//        {
//            Debug.LogWarning(string.Format("load config:{0} failed!", configName));
//        }
//        else
//        {
//            parse(configName, data.text);
//            //Main.Log("configName " + configName);
//        }
//    }


//    #region text asset loader
//    private static readonly char SplitChar = '\t';
//    private string[] columns;
//    public void parse(string configName, string content)
//    {
//        if (string.IsNullOrEmpty(content)) return;
//        string[] lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
//        try
//        {
//            int lineNo = 0;
//            #region 读取字段名
//            string fieldNames = lines[lineNo++];
//            fieldNames.Trim(new char[] { ' ', '"'});
//            if (string.IsNullOrEmpty(fieldNames)) return; //读取第一行，字段名
//            PID[] fieldIDs = new PID[fieldNames.Length];
//            columns = fieldNames.Split(new char[] { SplitChar });
//            int len = columns.Length;
//            string temp = "";
//            List<string> fieldList = new List<string>();
//            for (int i = 0; i < len; i++)
//            {
//                //Main.Log("read field: " + i);
//                try
//                {
//                    temp = columns[i].Trim();
//                    if (string.IsNullOrEmpty(temp))
//                    {
//                        //Main.Log("空格出现，后面的都忽略不读了！");
//                        len = i;
//                        break;
//                    }
//                    if (fieldList.Contains(temp))
//                    {
//                        Debug.LogError(string.Format("配置:{0} {1}字段名重复", configName, temp));
//                        continue;
//                    }

//                    fieldList.Add(temp);
//                    //if (i == len - 1) temp = temp.Substring(0, temp.Length - 1); //最后一个必须去掉为字符串，不知道为什么会多出一个。好像是回车，不过ANSI和UNITCODE都一样
//                    fieldIDs[i] = (PID)Enum.Parse(typeof(PID), temp, true);

//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError(string.Format("配置:{0}第{1}字段名不合法,必须是PID里枚举定义的, ERROR:{2}", configName, columns[i], ex.Message));
//                }
//            }
//            if (fieldIDs[0] != PID.ID)
//            {
//                //this is a cross table, then parse the id from id1 and id2
//                if(fieldIDs[0] != PID.ID1 || fieldIDs[1] != PID.ID2)
//                {
//                    Debug.LogError(string.Format("配置:{0} ID字段名不合法,必须是:普通表，第一个字段必须为ID，交叉表，第一第二字段必须分别为ID1, ID2", configName));
//                    return;
//                }
//            }
//            #endregion

//            #region 略过字段说明
//            ++lineNo;
//            #endregion

//            #region 读取字段类型
//            string strFieldTypes = lines[lineNo++];
//            strFieldTypes.Trim(new char[] { ' ', '"' });
//            if (string.IsNullOrEmpty(strFieldTypes)) return;
//            string[] strTypes = strFieldTypes.Split(new char[] { SplitChar });
//            DataType[] fieldTypes = new DataType[strTypes.Length];
//            for (int i = 0; i < len; i++)
//            {
//                try
//                {
//                    fieldTypes[i] = (DataType)Enum.Parse(typeof(DataType), strTypes[i], true);
//                    //Main.Log("Debug: add field:" + fieldTypes[i]);
//                }
//                catch (System.Exception ex)
//                {
//                    Debug.LogError(string.Format("配置{0}第{1}字段类型错误, 必须是DataType枚举定义的, ERROR:{2}", configName, i, ex.Message));
//                }
//            }
//            #endregion

//            #region 读取字段配置类型
//            /* 
//            string[] configTypes = null;
//            string strConfigTypes = lines[lineNo++];
//            strConfigTypes.Trim(new char[] { ' ', '"' });
//            if (string.IsNullOrEmpty(strConfigTypes)) return; //读取第4行，配置类型            
//            configTypes = strConfigTypes.Split(new char[] { SplitChar });
//             //* */
//            #endregion

//            //增加一个配置表
//            Dictionary<int, Property> config;
//            if (!configs.TryGetValue(configName, out config))
//            {
//                config = new Dictionary<int, Property>();
//                configs[configName] = config;
//            }
            
//            #region 读取数据
//            for (int n = lineNo; n < lines.Length; ++n)
//            {
//                if (string.IsNullOrEmpty(lines[n])) continue;
//                Property prop = new Property();//装一行信息中的各属性
//                //Main.Log("ffffffffffff: " + lines[n]);
//                string line = lines[n].Trim(new char[] { ' ', '\r','"'});
//                string[] values = line.Split(new char[] { SplitChar });
//                len = Math.Min(strTypes.Length, values.Length);
//                for (int i = 0; i < len; i++)
//                {
//                    values[i] = values[i].Trim(new char[] { '"',' ' }); //取得首尾的引号，EXCEL里有时会多导出引号，不知道为什么。
//                    object val = null;
//                    switch (fieldTypes[i])
//                    {
//                        case DataType.Int:
//                            int iVal = 0;
//                            int.TryParse(values[i], out iVal);
//                            val = iVal;
//                            break;
//                        case DataType.Float:
//                            float fVal = 0;
//                            float.TryParse(values[i], out fVal);
//                            val = fVal;
//                            break;
//                        case DataType.Bool:
//                            val = (values[i] == "1") || (values[i].Equals("true", StringComparison.CurrentCultureIgnoreCase));
//                            break;
//                        case DataType.Short:
//                            short sVal = 0;
//                            short.TryParse(values[i], out sVal);
//                            val = sVal;
//                            break;
//                        case DataType.Long:
//                            long lVal = 0;
//                            long.TryParse(values[i], out lVal);
//                            val = lVal;
//                            break;
//                        case DataType.String:
//                            val = values[i].Replace("\\n", "\n");//为了解决UILABEL回车符显示问题，这里都做一个替换                            
//                            break;
//                        case DataType.IntArray:
//                            double[] ret;
//                            {
//                                string[] strArr = values[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//                                //int count = strArr.Length;
                                

//                                if (strArr.Length <= 1)
//                                {
//                                    int[] intArr = {0};
//                                    int.TryParse(values[i], out intArr[0]);
//                                    val = intArr;
//                                    break;
//                                }
//                                ret = Property.ParseNumArray(values[i]);
//                                if (ret != null) val = Array.ConvertAll<double, int>(ret, d => { return (int)d; });
//                            }
//                            break;
//                        case DataType.FloatArray:
//                            {
//                                //string[] strArr = values[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//                                //int count = strArr.Length;
//                                //if (strArr.Length <= 1)
//                                //{
//                                //    float[] floatArr = { 0 };
//                                //    float.TryParse(values[i], out floatArr[0]);
//                                //    val = floatArr;
//                                //    break;
//                                //}
//                                ret = Property.ParseNumArray(values[i]);
//                                if (ret != null) val = Array.ConvertAll<double, float>(ret, d => { return (float)d; });
//                            }
//                            break;

//                        case DataType.StrArray:
//                            string str = values[i].Replace("\\n", "\n");//为了解决UILABEL回车符显示问题，这里都做一个替换
//                            val = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//                            break;
//                        case DataType.IntArray2D:
//                        {
//                            string[] strArr = values[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
//                            int count = strArr.Length;
//                            List<int[]> arry2d = new List<int[]>(count);
//                            for( int j = 0 ; j < count; j++ )
//                            {
//                                ret = Property.ParseNumArray(strArr[j]);;
//                                if (ret != null) 
//                                    arry2d.Add(Array.ConvertAll<double, int>(ret, d => { return (int)d; }));
//                            }
//                            val = arry2d;
//                        }
//                            break;
//                        case DataType.FloatArray2D:
//                            {
//                                string[] strArr = values[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
//                                int count = strArr.Length;
//                                List<float[]> arry2d = new List<float[]>(count);
//                                for (int j = 0; j < count; j++)
//                                {
//                                    ret = Property.ParseNumArray(strArr[j]);
//                                    if (ret != null)
//                                        arry2d.Add(Array.ConvertAll<double, float>(ret, d => { return (float)d; }));
//                                }
//                                val = arry2d;
//                            }
//                            break;
//                    }
//                    if (val != null)
//                    {
//                        //Main.Log("read field: " + fieldIDs[i] + " type:" + fieldTypes[i] +  " value:" + val);
//                        prop.SetProperty(fieldIDs[i], val);
//                    }
//                }
//                //parse the ID
//                int cfgId = -1;
//                if (fieldIDs[0] != PID.ID)
//                {
//                    //交叉表，用ID1和ID2组合ID
//                    int id1 = -1; int id2 = -1;
//                    if (int.TryParse(values[0], out id1) && int.TryParse(values[1], out id2))
//                    {
//                        cfgId = ToCrossID(id1, id2);
//                    }
//                } 
//                else {
//                    //普通表,直接读取ID 默认用，第一列id做为key
//                    int.TryParse(values[0], out cfgId);
//                }
//                if (cfgId == -1){
//                    Debug.LogError(string.Format("cfg table {0} error id:{1}", configName, values[0]));
//                    continue;
//                }
//                //Main.LogError(key);
//                config[cfgId] = prop;              
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError(string.Format("Load Config Error! ErrorMessage:{0}", ex.Message));
//        }
//        #endregion

//    }

//    private int ToCrossID(int id1, int id2)
//    {
//        return (id1 << 16) + id2;
//    }
//    #endregion

//    #endregion

//    #region save
//    private string ConfigPath = "d:/"; //配置路径，可以在编辑器里配置好
//    public void Save(string configName)
//    {
//        if (!string.IsNullOrEmpty(configName))
//        {
//            //打开文件
//            FileStream mystream = new FileStream(string.Format("{0}/{1}", ConfigPath, configName), FileMode.Create);
//            StreamWriter streamWriter = new StreamWriter(mystream);
//            string strData = save(configName);
//            if (!string.IsNullOrEmpty(strData))
//            {
//                streamWriter.Write(strData);
//                streamWriter.Flush();
//            }
//            streamWriter.Close();
//            mystream.Close();
//        }
//    }


//    private string save(string configName)
//    {
//        //get the config
//        Dictionary<int, Property> config;
//        if (!configs.TryGetValue(configName, out config) || config.Count == 0) return null;
//        StringBuilder stringWriter = new StringBuilder();
//        //get and write the config fields
//        Property properties = config[0];
//        int col = 0;
//        IDictionaryEnumerator propEnumerator = properties.Data.GetEnumerator();
//        while (propEnumerator.MoveNext())
//        {
//            PID propID = (PID)propEnumerator.Key;
//            if (col == 0)
//                stringWriter.Append(propID);
//            else
//                stringWriter.AppendFormat("{0}{1}", SplitChar, propID);
//            ++col;
//        }
//        stringWriter.Append("\r\n");
//        //空出3行，从第5行开始
//        stringWriter.Append("\r\n");
//        stringWriter.Append("\r\n");
//        stringWriter.Append("\r\n");
//        //write the records
//        Dictionary<int, Property>.Enumerator itr = config.GetEnumerator();
//        //int id = 0;
//        while (itr.MoveNext())
//        {
//            //id = itr.Current.Key;
//            properties = itr.Current.Value;
//            col = 0;
//            propEnumerator = properties.Data.GetEnumerator();
//            while (propEnumerator.MoveNext())
//            {
//                if (col == 0)
//                    stringWriter.Append(propEnumerator.Value);
//                else
//                    stringWriter.AppendFormat("{0}{1}", SplitChar, propEnumerator.Value);
//                ++col;
//            }
//        }
//        return stringWriter.ToString();
//    }

//    #endregion



//}
