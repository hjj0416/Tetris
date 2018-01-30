
//using System.Collections;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;
//using System.Text.RegularExpressions;

//public delegate void PropertyChanged(PID Id, object value);

//public enum DataType
//{
//    Unknown,
//    Long,
//    Int,
//    UInt,
//    Short,
//    Float,
//    String,
//    Bool,
//    Byte,
//    Color,
//    Enum,
//    UserData,
//    ArrayStart,
//    //数组类型定义在这后面
//    LongArray,
//    IntArray,
//    ShortArray,
//    FloatArray,
//    StrArray,
//    BoolArray,
//    ByteArray,
//    ColorArray,
//    EnumArray,
//    Vector3,
//    Vector2,
//    UserDataArray,    
//    IntArray2D,//格式“101;1|102,2”,全是int类型
//    FloatArray2D//格式“1.1;1|1.2,2”,全是int类型
//}


//public enum PID
//{
//    Invalid = 0,
//    ID,//ID号
//    ID1,//交叉表第一个ID
//    ID2,//交叉表第二个ID
//    Type,//单位名
//    Name,//显示名

//    us,
//    cn,
//    hk,

//    Icon,
//    Desc,
//    Sound,
//    Lv,
//    CD,

//    Race,
//    Ele,
//    Force,
//    Strength,
//    Intelligence,
//    Agility,
//    Stamina,
//    Health,
//    GrowStrength,
//    GrowIntelligence,
//    GrowAgility,
//    GrowStamina,
//    GrowHealth,
//    Skill,
//    Profession,
//    AttackType,
//    AttackCamp,
//    TargetPriority,
//    TargetNum,
//    CastMagic,
//    FlyMagic,
//    HitMagic,
//    Skills,

//    MonsType,

//    #region attr
//    Hp,
//    Rec,
//    PhyAtk,
//    MagAtk,
//    PhyDef,
//    MagDef,
//    CritRate,
//    CritDmg,
//    HitRate,
//    DodgeRate,
//    Tenacity,
//    Speed,
//    #endregion
//    PreCheck,
//    CheckLv,
//    DropGold,
//    PlayerExp,
//    HeroExp,
//    MapID,
//    BattleEventRate,
//    PropEventRate,
//    SpecialEventRate,
//    Describe,
//    CheckId,
//    EventType,
//    EventValue,
//    StartTime,
//    GrowthTime,
//    GrowthRate,
//    PerformanceType,
//    PerformanceValue,
//    NeedLevel,
//    SellPrice,
//    Description,
//    DropGroupId,
//    Monster,
//    Count,
//    ParameterType,
//    ParameterValue,
//    Two_attribute,
//    Strength_rate,
//    Intelligence_rate,
//    Agility_rate,
//    Stamina_rate,
//    Health_rate,
//    UseExp,
//    LimitedSale,
//    AttackSkills,
//    PassiveSkills,
//    AiGroupID,
//    Immunity,
//    Exp,
//    UseLevel,

//    Max,
    
//}

//public class Property
//{

//    public Property() { }
//    public Property(Dictionary<int,object> data)
//    {
//        Data = data;
//    }

//    #region 属性类型定义
    
//    public readonly static DataType[] Types = new DataType[(int)PID.Max];
//    static Property()
//    {
        
//        Types[(int)PID.ID] = DataType.Int;
//        //TestPropertyToString();
//    }
    
//    #endregion

//    #region 属性相关的工具方法
//    static public bool Parse(string strVal, Dictionary<int,object> data)
//    {
//        return Parse(strVal, data, -1);
//    }
//    static bool Parse(string strVal, Dictionary<int,object> data, int level)
//    {
//        if (string.IsNullOrEmpty(strVal)) return false;
//        //strVal = strVal.TrimStart(new char[] { ' ', '[' });
//        //strVal = strVal.TrimEnd(new char[] { ' ', ']' });
//        try
//        {
//            ++level;
//            string[] strVals = strVal.Split(new string[] { ";" + level }, StringSplitOptions.RemoveEmptyEntries);
//            if (strVals == null || strVals.Length == 0) return false;
//            foreach (string strPair in strVals)
//            {
//                int pos = strPair.IndexOf('=');
//                if (pos < 0)
//                {
//                    UnityEngine.Debug.LogWarning(string.Format("Property value pair:'{0}' is invalid! It must be PID = Value,", strPair));
//                    continue;
//                }
//                string strKey = strPair.Substring(0, pos);
//                strVal = strPair.Substring(pos + 1);
//                //G.Log.Info("vals =++++++{0}, {1}", strKey, strVal);
//                int key = (int)Enum.Parse(typeof(PID), strKey, true);
//                object val = Property.parseProp((PID)key, strVal, level);
//                if (val != null)
//                {
//                    data[key] = val;
//                }
//                //G.Log.Info("key ={0} val={1}", key, val);
//            }
//            return true;
//        }
//        catch (System.Exception ex)
//        {
//            UnityEngine.Debug.LogError(ex.Message);
//            return false;
//        }
//    }

//    static public double[] ParseNumArray(string val)
//    {
//        //if (val.IndexOf(';')<= 0)
//        //{
//        //    Main.LogError("intarray 必须使用分号';'隔开！！！   {0}", val);
//        //}
//        string[] vals = val.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries);
//        if (vals.Length == 0) return null;
//        double[] ret = new double[vals.Length];
//        int i = -1;
//        foreach (string strVal in vals)
//        {
//            ++i;
//            try
//            {
//                ret[i] = double.Parse(strVal);
//            }
//            catch (System.Exception ex)
//            {
//                ret[i] = 0;
//            }
//        }
//        return ret;
//    }


//    /// <summary>
//    /// 分析出属性值
//    /// </summary>
//    /// <param name="ID"></param>
//    /// <param name="strVal"></param>
//    /// <returns></returns>
//    static public object ParseProp(PID ID, string strVal)
//    {
//        return parseProp(ID, strVal, -1);
//    }
//    static object parseProp(PID ID, string strVal, int level)
//    {
//        try
//        {
//            //G.Log.Info("ParseProp {0}, {1}", ID, strVal);
//            object value = null;
//            double[] ret;
//            switch (Types[(int)ID])
//            {
//                case DataType.Long:
//                    long lVal;
//                    if (long.TryParse(strVal, out lVal))
//                    {
//                        value = lVal;
//                    }
//                    break;
//                case DataType.Int:
//                    int iVal;
//                    if (int.TryParse(strVal, out iVal))
//                    {
//                        value = iVal;
//                    }
//                    break;
//                case DataType.UInt:
//                    uint uiVal;
//                    if (uint.TryParse(strVal, out uiVal))
//                    {
//                        value = uiVal;
//                    }
//                    break;
//                case DataType.Short:
//                    short sVal;
//                    if (short.TryParse(strVal, out sVal))
//                    {
//                        value = sVal;
//                    }
//                    break;
//                case DataType.Float:
//                    float fVal;
//                    if (float.TryParse(strVal, out fVal))
//                    {
//                        value = fVal;
//                    }
//                    break;
//                case DataType.Bool:
//                    bool bVal;
//                    if (bool.TryParse(strVal, out bVal))
//                    {
//                        value = bVal;
//                    }
//                    break;
//                case DataType.String:
//                    //value = strVal;
//                    value = strVal.Replace("\r", string.Empty);
//                    if (strVal.EndsWith("\r"))
//                    {
//                        UnityEngine.Debug.LogError(strVal + "   " + value);
//                    }
//                    break;
//                case DataType.Byte:
//                    byte yVal;
//                    if (byte.TryParse(strVal, out yVal))
//                    {
//                        value = yVal;
//                    }
//                    break;
//                case DataType.Enum:
//                case DataType.EnumArray:
//                    if (!string.IsNullOrEmpty(strVal))
//                    {
//                        if (string.IsNullOrEmpty(strVal))
//                        {
//                            value = (byte)0;
//                        }
//                        else
//                        {
//                            byte byteVal;
//                            if (byte.TryParse(strVal, out byteVal))
//                                value = byteVal;
//                            else
//                                value = strVal;
//                        }
//                    }
//                    break;
//                case DataType.Vector3:
//                    if (!string.IsNullOrEmpty(strVal)) value = To(strVal, new short[] { 0, 0, 0 });
//                    break;
//                case DataType.Vector2:
//                    if (!string.IsNullOrEmpty(strVal)) value = To(strVal, new short[] { 0, 0 });
//                    break;
//                case DataType.LongArray:
//                    ret = ParseNumArray(strVal);
//                    //G.Log.Info("pase long array {0}: {1}", ret.Length, ret);
//                    if (ret != null) value = Array.ConvertAll<double, long>(ret, d => { return (long)d; });
//                    break;
//                case DataType.IntArray:
//                    ret = ParseNumArray(strVal);
//                    if (ret != null) value = Array.ConvertAll<double, int>(ret, d => { return (int)d; });
//                    break;
//                case DataType.ShortArray:
//                    ret = ParseNumArray(strVal);
//                    if (ret != null) value = Array.ConvertAll<double, short>(ret, d => { return (short)d; });
//                    break;
//                case DataType.ByteArray:
//                    ret = ParseNumArray(strVal);
//                    if (ret != null) value = Array.ConvertAll<double, byte>(ret, d => { return (byte)d; });
//                    break;
//                case DataType.FloatArray:
//                    ret = ParseNumArray(strVal);
//                    if (ret != null) value = Array.ConvertAll<double, float>(ret, d => { return (float)d; });
//                    break;
//                case DataType.BoolArray:
//                    ret = ParseNumArray(strVal);
//                    if (ret != null) value = Array.ConvertAll<double, bool>(ret, d => { return d != 0; });
//                    break;
//                case DataType.StrArray:
//                    value = strVal.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//                    break;
//                case DataType.UserData:                        
//                    Dictionary<int,object> t = new Dictionary<int,object>();
//                    if (Parse(strVal, t, level)) value = t;
//                    break;
//                case DataType.UserDataArray:
//                    string[] datas = strVal.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
//                    int count = datas.Length;
//                    //G.Log.Info("datas ===={0}, {1}", count, strVal);
//                    if (count > 0)
//                    {
//                        List<Dictionary<int,object>> temp = new List<Dictionary<int,object>>(count);
//                        for (int i = 0; i < datas.Length; i++)
//                        {
//                            t = new Dictionary<int,object>();
//                            if (Parse(strVal, t, level)) temp.Add(t);
//                        }
//                        value = temp.ToArray();
//                    }
//                    break;

//                //case DataType.IntArray2D:
//                //    string[] arrayInts = strVal.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
//                //    int intsCount = arrayInts.Length;
//                //    for (int i = 0; i < intsCount; i++)
//                //    {
//                //       Main.Log("Int array[{0}]={1}",i.ToString(),arrayInts[i]);
//                //    }
//                //    break;
//                default:
//                    UnityEngine.Debug.LogError(string.Format("Property:{0} 's type is not defined!", ID));
//                    break;
//            }
//            return value;
//        }
//        catch (System.Exception ex)
//        {
//            UnityEngine.Debug.LogError(string.Format("Parse property:[key:{0} val={}1] error, {2}", ID, strVal, ex.Message));
//            return null;
//        }
//    }

//    #region ToString

//    static private void ToString<T>(StringBuilder strBuilder, T value, int level)
//    {
//        strBuilder.Append(value.ToString());
//    }
//    static void ToStringArray<T>(StringBuilder strBuilder, T[] vals, int level)
//    {
//        if (vals == null || vals.Length == 0) return;
//        //拼接数组属性
//        foreach (T val in vals)
//        {
//            toString(strBuilder, val, level);
//            strBuilder.Append(',');
//        }
//        strBuilder.Remove(strBuilder.Length - 1, 1);
//    }

//    static private void ToString(StringBuilder strBuilder, Dictionary<int,object> t, int level)
//    {

//        if (t.Count == 0) return;
//        ++level;
//        //插入头
//        //strBuilder.Append('['); //注意：这里不能用‘{’，c#输入的字符串里用‘{’会当成无效字符。报错！
//        IDictionaryEnumerator itr = t.GetEnumerator();
//        while (itr.MoveNext())
//        {
//            ToString(strBuilder, itr.Key, itr.Value, level);
//        }
//        //插入尾
//        //strBuilder.Append(']');
//    }

//    private static void toString(StringBuilder strBuilder, object value, int level)
//    {
//        if (value is bool[]) ToStringArray(strBuilder, (bool[])value, level);
//        else if (value is byte[]) ToStringArray(strBuilder, (byte[])value, level);
//        else if (value is short[]) ToStringArray(strBuilder, (short[])value, level);
//        else if (value is int[]) ToStringArray(strBuilder, (int[])value, level);
//        else if (value is long[]) ToStringArray(strBuilder, (long[])value, level);
//        else if (value is float[]) ToStringArray(strBuilder, (float[])value, level);
//        else if (value is double[]) ToStringArray(strBuilder, (double[])value, level);
//        else if (value is string[]) ToStringArray(strBuilder, (string[])value, level);
//        else if (value is Dictionary<int,object>[]) ToStringArray(strBuilder, (Dictionary<int,object>[])value, level);
//        else if (value is Dictionary<int,object>) ToString(strBuilder, (Dictionary<int,object>)value, level);
//        else ToString(strBuilder, value, level);
//    }


//    static void ToString(StringBuilder strBuilder, object key, object value, int level)
//    {
//        if (value == null || key == null) return;
//        if (key is int) key = (PID)key;
//        strBuilder.AppendFormat("{0}=", key);
//        toString(strBuilder, value, level);
//        strBuilder.Append(";").Append(level).Append(" ");
//    }


//    static public string ToString(object value)
//    {
//        StringBuilder strBuilder = new StringBuilder();
//        toString(strBuilder, value, -1);
//        return strBuilder.ToString();
//    }


//    #endregion

//    #endregion

//    #region 属性变化事件
//    private readonly Dictionary<PID, List<Action<object, object>>> ChangeEvents = new Dictionary<PID, List<Action<object, object>>>();


//    /// <summary>
//    /// 因为同一个ID，同一个事件 一般不进行重复添加
//    /// 所以，一旦onPropChanged依赖的对象被销毁，将会导致[this].onPropChanged中 [this].members 的其他内容被释放，会造成未定义行为
//    /// 故：使用onPropChanged，一般都应该是static methods.
//    /// </summary>
//    /// <param name="ID"></param>
//    /// <param name="onPropChanged"></param>
//    public void AddPropListener(PID ID, Action<object, object> onPropChanged)
//    {
//        if (onPropChanged == null) return;
//        List<Action<object, object>> callbacks;
//        if (!ChangeEvents.TryGetValue(ID, out callbacks))
//        {
//            callbacks = new List<Action<object, object>>();
//            ChangeEvents[ID] = callbacks;
//        }
//        callbacks.Add(onPropChanged);
//    }

//    /// <summary>
//    /// 因为同一个ID，同一个事件 一般不进行重复添加
//    /// 所以，一旦onPropChanged依赖的对象被销毁，将会导致[this].onPropChanged中 [this].members 的其他内容被释放，会造成未定义行为
//    /// 故：使用onPropChanged，一般都应该是static methods.
//    /// </summary>
//    /// <param name="ID"></param>
//    /// <param name="onPropChanged"></param>
//    public void AddPropListenerWithVerify(PID ID, Action<object, object> onPropChanged)
//    {
//        if (HasRegistered(ID, onPropChanged))
//        {
//            return;
//        }
//        else
//        {
//            AddPropListener(ID, onPropChanged);
//        }
//    }
    
//    public bool HasRegistered(PID ID, Action<object, object> onPropChanged)
//    {
//        if (onPropChanged == null) return false;
//         List<Action<object, object>> callbacks;

//        if (!ChangeEvents.TryGetValue(ID, out callbacks))
//        {
//            return false;
//        }

//        return (callbacks.Contains(onPropChanged));
//    }

//    /// <summary>
//    /// 如果onPropChanged依赖的对象有可能被释放，那么，在对象释放的时候移除是安全的, 否则，将会引起其他问题(参考AddPropListener)
//    /// </summary>
//    /// <param name="ID"></param>
//    /// <param name="onPropChanged"></param>
//    public void RemovePropListener(PID ID, Action<object, object> onPropChanged)
//    {
//        if (onPropChanged == null) return;
//        List<Action<object, object>> callbacks;
//        if (ChangeEvents.TryGetValue(ID, out callbacks))
//        {
//            callbacks.Remove(onPropChanged);
//        }
//    }

//    public void FirePropChangeEvent(PID ID, object newValue, object oldValue = null)
//    {
//        List<Action<object, object>> callbacks;
//        if (ChangeEvents.TryGetValue(ID, out callbacks))
//        {
//            foreach (Action<object, object> cb in callbacks)
//            {
//                cb(newValue, oldValue);
//            }
//        }
//    }
//    #endregion

//    #region 属性对象
//    public readonly Dictionary<int, object> Data = new Dictionary<int, object>();
//    virtual public void SetProperty(PID ID, object val)
//    {
//        Data[(int)ID] = val;
//    }

//    #region 读属性函数
//    public bool HasProperty(PID ID)
//    {
//        return (Data.ContainsKey((int)ID));
//    }

//    virtual public T GetProperty<T>(PID ID) { return Get(Data, ID, default(T)); }
//    virtual public T GetProperty<T>(PID ID, T def) { return Get(Data, ID, def); }
//    public T GetEnumProperty<T>(PID ID, T def) { 
//        if (Data == null) return def;
//        object obj = null;
//        if (Data.ContainsKey((int)ID))
//         {
//            obj = Data[(int)ID];
//        }
//        return ToEnum(obj, def); 
//    }


//    #endregion

//    #region 工具方法
//    static public T To<T>(object value, T def)
//    {
//        try
//        {
//            if (value == null) return def;
//            if (value is string && typeof(T) == typeof(int))
//            {
//                value = Convert.ToInt32(value);
//                return (T)value;
//            }
//            return (T)value;
//        }
//        catch (System.Exception ex)
//        {
//            UnityEngine.Debug.LogError(string.Format("Convert {0} to type {1} failed. Error:{2}", value, typeof(T), ex.Message));
//            return def;
//        }

//    }

//    static public short[] To(string value, short[] def)
//    {
//        if (string.IsNullOrEmpty(value)) return def;
//        try
//        {
//            MatchCollection matches = Regex.Matches(value, "[\\d.]+");
//            int i = 0;
//            // Use foreach loop.
//            foreach (Match match in matches)
//            {
//                //Main.Log("Value="+ match.Value);
//                def[i++] = (short)(float.Parse(match.Value));
//            }
//            return def;
//        }
//        catch (System.Exception ex)
//        {
//            return def;
//        }
//    }

//    static public T Get<T>(Dictionary<int,Property> data, int ID, PID PId, T def)
//    {
//        if (data == null) return def;
//        T ret = def;        
//        Property prop = null;
//        if (data.TryGetValue(ID, out prop))
//        {
//            ret = prop.GetProperty<T>(PId);
//        }
//        return ret;
//    }

//    static public object Get(Dictionary<PID, object> data, PID ID, object def)
//    {
//        if (data == null) return def;
//        object ret = null;
//        if (!data.TryGetValue(ID, out ret))
//        {
//            ret = def;
//        }
//        return ret;
//    }

//    static public T Get<T>(Dictionary<int,object> data, PID ID, T def)
//    {
//        if (data == null) return def;
//        object obj = null;
//        if (data.ContainsKey((int)ID))
//         {
//            obj = data[(int)ID];
//        }
//        return To(obj, def);
//    }

//    static public void Set<T>(Dictionary<int, object> data, PID ID, T value)
//    {
//        Set(data, (int)ID, value);
//    }

//    static public void Set<T>(Dictionary<int, object> data, PID ID, T value, T def)
//    {
//        if (object.Equals(value, def)) return;
//        Set(data, (int)ID, value);
//    }

//    static public void Set<T>(Dictionary<int, object> data, byte key, T value)
//    {
//        data[key] = value;
//    }

//    static public void Set<T>(Dictionary<int, object> data, int key, T value)
//    {
//        data[key] = value;
//    }

//    static public void GetSet<T>(Dictionary<int,object> fromData, Dictionary<int,object> toData, PID key)
//    {
//        object val = fromData[(int)key];
//        if (val == null || object.Equals(val, default(T))) return; //等于NULL或者默认的，就不用COPY了
//        toData[(int)key] = val;
//    }

//    public void Into<T>(Dictionary<int,object> target, PID ID)
//    {
//        object val = Data[(int)ID];
//        if (val == null) return;
//        if (object.Equals(val, default(T))) return;
//        target[(byte)ID] = val;
//    }
//    public void From<T>(Dictionary<int,object> target, PID ID)
//    {
//        object val = target[(int)ID];
//        if (val == null) val = default(T);
//        Data[(int)ID] = val;
//    }
//    static public T[] StrToEnumArray<T>(string value, T def)
//    {
//        try
//        {
//            if (!(value is string))
//                return null;
//            else
//            {
//                if (string.IsNullOrEmpty(value)) return null;
//                string[] vals = ((string)value).Split(new char[] { '，' }, StringSplitOptions.RemoveEmptyEntries);
//                if (vals.Length == 0) return null;
//                T[] ret = new T[vals.Length];
//                int i = -1;
//                foreach (string strVal in vals)
//                {
//                    ++i;
//                    try
//                    {
//                        ret[i] = (T)Enum.Parse(typeof(T), strVal, true);
//                    }
//                    catch (System.Exception ex)
//                    {
//                        UnityEngine.Debug.LogException(ex);
//                        ret[i] = def;
//                    }
//                }
//                return ret;
//            }
//        }
//        catch (System.Exception ex)
//        {
//            return null;
//        }
//    }

//    static public T ToEnum<T>(object value, T def)
//    {
//        try
//        {
//            if (value == null)
//                return def;
//            else
//            {

//                if (value is string)
//                {
//                    string val = (string)value;
//                    return (T)Enum.Parse(typeof(T), val, true);
//                }
//                else
//                {
//                    return (T)value;
//                }

//            }
//        }
//        catch (System.Exception ex)
//        {
//            return def;
//        }
//    }
//    #endregion



//    #region test code
//    /*
//        static public void TestPropertyToString()
//        {
//            HybridDictionary temp = new HybridDictionary() { { PID.Pos, new short[] { 10, 10, 10 } }, { PID.Rot, new short[] { 20, 20, 20 } } };
//            HybridDictionary test = new HybridDictionary() { 
//                {PID.Type1, 42},
//                {PID.Pos, new short[]{-119,3,-266}},
//                { PID.Data, new HybridDictionary() { 
//                        { PID.Data, new HybridDictionary() {
//                                { PID.Pos,new short[]{50,50,50}},
//                                { 24, new short[]{150,150,150}}
//                            }
//                        },
//                        {PID.Scale, new short[]{100,100,100}}
//                    }
                    
//                },
//                {PID.Rot, new short[]{-300, 200,-600}},
//                {PID.DataArray, new HybridDictionary[]{temp, temp, temp}}
//            };
//            int count = 100;
//            for (int i = 0; i <= count; ++i )
//            {
//                test[i] = temp;
//            }

//            //FileStream fs = new FileStream("e://aa.dat", FileMode.Create);

//            BinaryFormatter bf = new BinaryFormatter();
//            MemoryStream ms = new MemoryStream();
//            bf.Serialize(ms, test);
//            ms.Flush();
//            //fs.Close();
//            byte[] data = ms.GetBuffer();

//            MemoryStream ms1 = new MemoryStream(data);
//            //fs = new FileStream("e://aa.dat", FileMode.OpenOrCreate);
//            BinaryFormatter af = new BinaryFormatter();
//            HybridDictionary aa = (HybridDictionary)af.Deserialize(ms1);
//            //fs.Close();


////             string bb = Property.ToString(test);
////             HybridDictionary data = new HybridDictionary();
////             Property.Parse(bb, data);
//        }
//         * */
//    #endregion

//    #endregion

//}

///// <summary>
///// 
///// </summary>
//public class PropertyHelper
//{
//    /// <summary>
//    /// 根据PID获得预注册的方法, 传入变参获得返回值
//    /// </summary>
//    /// <typeparam name="T">返回值类型</typeparam>
//    /// <param name="id">方法对应的PID</param>
//    /// <param name="t"></param>
//    /// <param name="args"></param>
//    /// <returns></returns>
//    public bool GetProperty<T>(PID id, out T t, params Object[] args)
//    {
//        Method m;
//        if (properties.TryGetValue(id, out m))
//        {
//            t = (T) m(args);
//            return true;
//        }

//        t = default(T);
//        return false;
//    }

//    /// <summary>
//    /// 设置ID对应的方法, 一般是静态方法
//    /// </summary>
//    /// <param name="id"></param>
//    /// <param name="m"></param>
//    public void SetMethod(PID id, Method m)
//    {
//        properties[id] = m;
//    }

//    public delegate Object Method(params Object[] args);
//    private readonly Dictionary<PID, Method> properties = new Dictionary<PID, Method>();
//}
