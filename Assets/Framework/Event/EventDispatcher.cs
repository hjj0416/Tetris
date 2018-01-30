using System;
using System.Collections.Generic;

public delegate void EventHandler(short type);
public delegate void EventHandler<T>(short type, T msg);

public class EventDispatcher
{
    private Dictionary<short, List<Delegate>> eventHandlerDic = new Dictionary<short, List<Delegate>>(10);
    private Dictionary<short, List<Delegate>> oneShotHandlerDic = new Dictionary<short, List<Delegate>>(10);
    private List<List<Delegate>> tempHandlerList = new List<List<Delegate>>();

    public void ClearEventHandler()
    {
        //直接将字典Clear会将事件回掉函数数组大小无法复用，从而造成每次进池出池时造成gc。
        //            eventHandlerDic.Clear();
        //            oneShotHandlerDic.Clear();
        Dictionary<short, List<Delegate>>.Enumerator etor = eventHandlerDic.GetEnumerator();
        while (etor.MoveNext())
        {
            etor.Current.Value.Clear();
        }

        etor = oneShotHandlerDic.GetEnumerator();
        while (etor.MoveNext())
        {
            etor.Current.Value.Clear();
        }


        tempHandlerList.Clear();
    }

    private void AddEventHandlerInternal(Dictionary<short, List<Delegate>> dic, short type, Delegate handler)
    {
        List<Delegate> eventListeners = GetEventHandlerList(dic, type, true);
        bool newEvent = true;
#if DEBUG
            if (eventListeners.Contains(handler))
            {
                UnityEngine.Debug.LogError(string.Format( "repeat to add = {0}, type = {1}" , handler,type));
                newEvent = false;
            }
            
#endif
        if (newEvent)
        {
            //Debug.SyncLog (string.Format ("[{0}]AddEventHandlerInternal type={1} handler={2}", this.GetHashCode (), type, handler.GetHashCode ()));
            eventListeners.Add(handler);
        }

        //Debug.Assert(eventListeners.Count < 300);
    }

    private void RemoveEventHandlerInternal(Dictionary<short, List<Delegate>> dic, short type, Delegate handler)
    {
        List<Delegate> eventListeners = GetEventHandlerList(dic, type, false);
        if (eventListeners == null)
            return;

        int index = eventListeners.IndexOf(handler);
        if (index != -1)
        {
            //				Debug.SyncLog (string.Format ("[{0}]RemoveEventHandlerInternal type={1} handler={2}", this.GetHashCode (), type, handler.GetHashCode ()));
            eventListeners[index] = null;
            if (!tempHandlerList.Contains(eventListeners)) tempHandlerList.Add(eventListeners);
        }
    }

    public void PresizeHandler(short type, int size)
    {
        if (!eventHandlerDic.ContainsKey(type))
        {
            eventHandlerDic.Add(type, new List<Delegate>(size));
        }
    }

    //不能在SendEvent中移除，只能在Update中移除
    public void ClearNullEventHandler()
    {
        int count = tempHandlerList.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                List<Delegate> eventListeners = tempHandlerList[i];
                int len = eventListeners.Count;
                int newLen = 0;
                for (int j = 0; j < len; j++)
                {
                    if (eventListeners[j] != null)
                    {
                        if (newLen != j)
                            eventListeners[newLen] = eventListeners[j];
                        newLen++;
                    }
                }
                eventListeners.RemoveRange(newLen, len - newLen);
            }
            tempHandlerList.Clear();
        }
    }

    private List<Delegate> GetEventHandlerList(Dictionary<short, List<Delegate>> dic, short type, bool autoCreate)
    {
        List<Delegate> eventList;
        if (dic.TryGetValue(type, out eventList))
            return eventList;
        else
        {
            if (autoCreate)
            {
                eventList = new List<Delegate>();
                dic.Add(type, eventList);
                return eventList;
            }
            return null;
        }
    }

    #region 无参数事件
    public void AddEventHandler(short type, EventHandler handler)
    {
        AddEventHandlerInternal(eventHandlerDic, type, handler);
    }

    public void RemoveEventHandler(short type, EventHandler handler)
    {
        RemoveEventHandlerInternal(eventHandlerDic, type, handler);
    }

    public void AddOneShotEventHandler(short type, EventHandler handler)
    {
        AddEventHandlerInternal(oneShotHandlerDic, type, handler);
    }

    public void RemoveOneShotEventHandler(short type, EventHandler handler)
    {
        RemoveEventHandlerInternal(oneShotHandlerDic, type, handler);
    }

    private void DoSend(short type, List<Delegate> eventListeners)
    {
        int len = eventListeners.Count;
        for (int i = 0; i < len; i++)
        {
            if (eventListeners[i] == null)
                continue;

            try
            {
                EventHandler tEvent = (EventHandler)eventListeners[i];
                if (tEvent != null)
                    tEvent(type);
                else
                    UnityEngine.Debug.LogError(string.Format("NextEventDispatcher.SendEvent eventID:{0} cast type error", type));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
    }

    public void SendEvent(short type)
    {
        List<Delegate> eventListeners = GetEventHandlerList(eventHandlerDic, type, false);
        if (eventListeners != null)
            DoSend(type, eventListeners);

        List<Delegate> oneShotListeners = GetEventHandlerList(oneShotHandlerDic, type, false);
        if (oneShotListeners != null)
        {
            DoSend(type, oneShotListeners);
            oneShotListeners.Clear();
        }
    }
    #endregion

    #region 单参数事件
    public void AddEventHandler<T>(short type, EventHandler<T> handler)
    {
        AddEventHandlerInternal(eventHandlerDic, type, handler);
    }

    public void RemoveEventHandler<T>(short type, EventHandler<T> handler)
    {
        RemoveEventHandlerInternal(eventHandlerDic, type, handler);
    }

    public void AddOneShotEventHandler<T>(short type, EventHandler<T> handler)
    {
        AddEventHandlerInternal(oneShotHandlerDic, type, handler);
    }

    public void RemoveOneShotEventHandler<T>(short type, EventHandler<T> handler)
    {
        RemoveEventHandlerInternal(oneShotHandlerDic, type, handler);
    }

    private void DoSend<T>(short type, List<Delegate> eventListeners, T msg)
    {
        int len = eventListeners.Count;
        for (int i = 0; i < len; i++)
        {
            if (eventListeners[i] == null)
                continue;

            try
            {
                EventHandler<T> tEvent = (EventHandler<T>)eventListeners[i];
                if (tEvent != null)
                    tEvent(type, msg);
                else
                    UnityEngine.Debug.LogError(string.Format("NextEventDispatcher.SendEvent eventID:{0} has error msg type:{1}", type, msg.GetType()));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }
    }

    public void SendEvent<T>(short type, T msg)
    {
        List<Delegate> eventListeners = GetEventHandlerList(eventHandlerDic, type, false);
        if (eventListeners != null)
            DoSend(type, eventListeners, msg);

        List<Delegate> oneShotListeners = GetEventHandlerList(oneShotHandlerDic, type, false);
        if (oneShotListeners != null)
        {
            DoSend(type, oneShotListeners, msg);
            oneShotListeners.Clear();
        }
    }
    #endregion
}