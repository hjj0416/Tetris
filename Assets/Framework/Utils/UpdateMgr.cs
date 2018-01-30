using UnityEngine;
using System;
using System.Collections.Generic;


/// <summary>
/// delaycall管理器
/// 方法：
/// UpdateManager.Instance.Delay()
/// UpdateManager.Instance.Repeat()
/// 
/// 注意：本方法提供给非mono对象
/// </summary>
public class UpdateMgr : MonoSingleton<UpdateMgr>
{
    private static int handleID = 0;
    private static bool _isPause = false;
    #region repeat call

    private Dictionary<int, RepeatData> repeatDic = new Dictionary<int, RepeatData>();

    public int Repeat(float interval, Action cb)
    {
        handleID++;
        if (handleID == int.MaxValue)
        {
            handleID = 1;
        }
        RepeatData data = new RepeatData(handleID, interval, cb);
        repeatDic.Add(handleID, data);
        return handleID;
    }

    public void CancelRepeat(int ID)
    {
        if (repeatDic.ContainsKey(ID))
        {
            repeatDic.Remove(ID);
        }
    }

    private void RepeatUpdate()
    {
        if (repeatDic == null || repeatDic.Count <= 0) return;
        if (repeatDic.Values.Count <= 0) return;
        float curTime = Time.realtimeSinceStartup;
        foreach(RepeatData d in repeatDic.Values)
        {
            if(d.triggleTime <= curTime)
            {
                d.cb();
                d.times++;
                d.triggleTime = d.startTime + d.interval * (d.times + 1);
            }
        }
    }

    

    #endregion

    #region delay call
    private List<CallData> cacheDelayList = new List<CallData>();
    private Dictionary<int, CallData> dic = new Dictionary<int,CallData>();
    List<int> removeList = new List<int>();
    /// <summary>
    /// 注册一个delaycall
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="cb"></param>
    /// <param name="times"></param>
    /// <returns></returns>
    public int Delay(float delayTime, Action cb, int times = 1)
    {
        handleID++;
        if (handleID == int.MaxValue)
        {
            handleID = 1;
        }
        CallData data = new CallData(handleID, delayTime, cb, times);
        cacheDelayList.Add(data);
        //Debug.Log("add call " + handleID);
        return handleID;
    }
    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="ID"></param>
    public void Cancel(int ID)
    {
        if(dic.ContainsKey(ID))
        {
            dic.Remove(ID);
        }
    }

    public void RemoveAll()
    {
        dic.Clear();
    }

    private void DelayCallUpdate()
    {
        if (cacheDelayList.Count >0)
        {
            CallData data;
            for(int i = 0; i < cacheDelayList.Count ; i++)
            {
                data = cacheDelayList[i];
                dic.Add(data.id, data);
            }
            cacheDelayList.Clear();
        }
        if (dic == null || dic.Count <= 0) return;
        float curTime = Time.realtimeSinceStartup;
        foreach (CallData d in dic.Values)
        {
            if (d.triggleTime <= curTime)
            {
                //Debug.Log("call "+ d.id);
                if (d.cb != null) d.cb();
                d.times--;
                if (d.times <= 0)
                {
                    removeList.Add(d.id);
                }
                else
                {
                    d.triggleTime += d.delayTime;
                }
            }
        }
        //remove
        if (removeList.Count > 0)
        {
            for (int i = 0; i < removeList.Count; i++)
            {
                //Debug.Log("remove " + removeList[i]);
                dic.Remove(removeList[i]);
            }
            removeList.Clear();
        }
    }

    #endregion delay call

    public static void Pause()
    {
        _isPause = true;
    }

    public static void Resume()
    {
        _isPause = false;
    }
    
    void Update()
    {
        if (_isPause) return;
        DelayCallUpdate();
        RepeatUpdate();
    }
}

class CallData
{
    public int id;
    public int times = 1;//触发次数
    public float delayTime = 0;//每次延时时间
    public float triggleTime = 0;
    public Action cb = null;

    public CallData(int id, float delayTime, Action cb, int times)
    {
        this.id = id;
        this.times = times;
        this.delayTime = delayTime;
        this.triggleTime = Time.realtimeSinceStartup + delayTime;
        this.cb = cb;
    }
}

class RepeatData
{
    public int id;
    public int times = 0;//已触发次数
    public float triggleTime = 0;
    public float startTime = 0;
    public float interval = 0;
    public Action cb = null;

    public RepeatData(int id, float interval, Action cb)
    {
        this.id = id;
        this.interval = interval;
        triggleTime = interval + Time.realtimeSinceStartup;
        this.cb = cb;
        startTime = Time.realtimeSinceStartup;
    }
}