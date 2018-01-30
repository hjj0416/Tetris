using System;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour 
{
    /// <summary>
    /// 预加载资源
    /// </summary>
    /// <param name="cb"></param>
    public virtual void PreLoad(Action cb)
    {
        if (cb != null)
            cb();
    }

    public void Open(Action<bool> cb = null)
    {
        gameObject.SetActive(true);
        OnOpen(cb);
    }


    public void Close(Action cb = null)
    {
        OnClose(() =>
        {
            OnExit();
            gameObject.SetActive(false);
            if (cb != null) cb();
        });
    }

    /// <summary>
    /// 场景被打开前调用
    /// </summary>
    protected virtual void OnOpen(Action<bool> cb)
    {
        if (cb != null) cb(true);
    }
    /// <summary>
    /// 场景进入前调用
    /// </summary>
    public virtual void OnEnter(Action cb)
    {
        if (cb != null)
        {
            cb();
        }
    }

    public virtual void OnEnterComplete()
    {
    }
    /// <summary>
    /// 场景关闭前调用
    /// </summary>
    /// <param name="cb"></param>
    protected virtual void OnExit()
    {
    }
    protected virtual void OnClose(Action cb)
    {
        if (cb != null) cb();
    }


   
}
