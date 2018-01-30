using System;
using System.Collections.Generic;
using UnityEngine;


public delegate void EventHandleDelegate(object data);
/// <summary>
/// 事件管理器
/// </summary>
public class EventMgr
{
    /// <summary>
    /// 事件监听池
    /// </summary>
    
    private static Dictionary<EventEnum,  Delegate> delegates = new Dictionary<EventEnum, Delegate>();

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="listenerFunc">监听函数</param>
    public static void addEventListener(EventEnum type, EventHandleDelegate listener)
    {
        //if (type == EventEnum.RefreshBossTip) return;
        try
        {
            if (listener == null)
                return;
            Delegate func;
            if (delegates.TryGetValue(type, out func))
            {
                func = Delegate.Remove(func, listener);//去除重复
                func = Delegate.Combine(func, listener);
            }
            else
            {
                func = listener;
            }
            delegates[type] = func;
        }
        catch(Exception e)
        {
            Debug.LogError("addEventListener error " + e + "  "+type);
        }
    }

    public static bool ContainsEvent(EventEnum type)
    {
        return delegates.ContainsKey(type);
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="listenerFunc">监听函数</param>
    public static void removeEventListener(EventEnum type, EventHandleDelegate listener)
    {
        if (listener == null)
        {
            return;
        }
        Delegate func = null;
        if (delegates.TryGetValue(type, out func))
        {
            func = Delegate.RemoveAll(func, listener);
        }
        delegates[type] = func;
    }

    public static void removeEventListener(EventEnum type)
    {
        if (!delegates.ContainsKey(type))
        {
            return;
        }
        delegates.Remove(type);
    }

    public static void removeAll()
    {
        delegates.Clear();
    }
    /// <summary>
    /// 触发某一类型的事件  并传递数据
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="data">事件的数据(可为null)</param>
    public static void dispatchEvent(EventEnum type, object data = null)
    {
        Delegate func = null;
        delegates.TryGetValue(type, out func);
        if (func != null)
        {
            try
            {
                func.DynamicInvoke(data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            
        }
    }

}

