using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class TimerData
{
    public float TimeCount { get; set; }//计时时间,单位是秒
    public float MaxTime { get; private set; }//最大计时,单位是秒
    public bool IsCountUp { get; private set; }//是否是向上计时


    public TimerData(float time, float maxTime, bool isCountUp)
    {
        this.TimeCount = time;
        this.MaxTime = maxTime;
        this.IsCountUp = isCountUp;
    }

    public bool Tick(float delta)
    {
        bool completeFlag = false;
        if (IsCountUp)
        {
            TimeCount += delta;
            if (TimeCount >= MaxTime)
            {
                completeFlag = true;
            }

        }

        else
        {
            TimeCount -= delta;
            if (TimeCount <= 0)
            {
                completeFlag = true;  
            }
        }

        //NGUIDebug.Log("Timer tick:"+TimeCount);
        return completeFlag;

    }
}
public class TimerMgr : MonoBehaviour
{

    #region 单例
    private static TimerMgr mInstance;
    /// <summary>
    /// 获取资源加载实例
    /// </summary>
    /// <returns></returns>
    public static TimerMgr Instance
    {
        get
        {
            if (mInstance == null)
            {
                string objName = typeof(TimerMgr).Name;
                GameObject go = GameObject.Find(objName);
                if (go == null)
                {
                    go = new GameObject(objName);
                }
                go.transform.parent = null;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                mInstance = go.GetMissingComponent<TimerMgr>();
                mInstance.Init();
                //DontDestroyOnLoad(go);
            }

            return mInstance;
        }
    }

    #endregion

    private static List<Action> tickActions = new List<Action>();

    private float startTime;

    void OnDestroy()
    {
        StopCoroutine("HandleCount");
        tickActions.Clear();
        mInstance = null;
    }

    public void Init()
    {
        startTime = Time.realtimeSinceStartup;
        StartCoroutine("HandleCount");
    }

    public void RegisterTick(Action tickAction)
    {
        if (tickActions.Contains(tickAction))
            return;

        if (tickAction != null)
        {       
            tickActions.Add(tickAction);
            tickAction.Invoke();
        }
        
    }


    private static Queue<Action> waitForRemoveList = new Queue<Action>(); 
    public static void UnregisterTick(Action tickAction)
    {
        if (tickActions.Contains(tickAction))
            waitForRemoveList.Enqueue(tickAction);
    }

    void HandleTick(float delta)
    {
        int count = tickActions.Count;
        for (int i = 0; i < count; i++)
        {
            if(waitForRemoveList.Contains(tickActions[i]))
                continue;
            tickActions[i].Invoke();
        }
        while (waitForRemoveList.Count>0)
        {
            Action a = waitForRemoveList.Dequeue();
            tickActions.Remove(a);
        }
    }

    IEnumerator HandleCount()
    {
        while (true)
        {
            float currentTime = Time.realtimeSinceStartup;
            float delta = currentTime - startTime;
            HandleTick(delta);
            startTime = currentTime;
            yield return new WaitForSeconds(1);
        }
    }
}
