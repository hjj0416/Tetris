using System;
using System.Collections.Generic;
using System.Linq;

public class ActionParam
{
    #region value

    private object _value;
    public ActionParam(object value)
    {
        _value = value;
        HasValue = true;
    }

    public bool HasValue { get; private set; }


    public void SetValue<T>(T value) where T : new()
    {
        _value = value;
    }

    public T GetValue<T>() where T : new()
    {
        return (T)_value;
    }

    #endregion

    #region dic

    private Dictionary<string, object> _param;


    public ActionParam()
    {
        _param = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
    }

    public KeyValuePair<string, object>[] ToArray()
    {
        return _param != null ? _param.ToArray() : new KeyValuePair<string, object>[0];
    }

    public void Foreach(Func<string, object, bool> func)
    {
        if (_param == null) return;
        foreach (KeyValuePair<string, object> pair in _param)
        {
            if (!func(pair.Key, pair.Value))
            {
                break;
            }
        }
    }

    public T Get<T>(string name)
    {
        return (T)this[name];
    }

    /// <summary>
    /// Find param
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public object this[string name]
    {
        get
        {
            return _param != null && _param.ContainsKey(name) ? _param[name] : null;
        }
        set
        {
            if (_param != null)
            {
                _param[name] = value;
            }
        }
    }

    #endregion

}
