using System;
using UnityEngine;
using UnityEngine.Events;

public class UICustomTab : MonoBehaviour
{
    public class TabEvent : UnityEvent<int>
    {
        internal void AddListener()
        {
            throw new NotImplementedException();
        }
    }
    public readonly TabEvent onTabClick = new TabEvent();

    [SerializeField] private int defaultIndex = -1;
    [SerializeField] private int soundId = 3002;
    [SerializeField] private UITabItem[] tabItems;

    private int curIndex = -1;
    private bool _init;

    private void Init()
    {
        if (_init) return;
        _init = true;
        curIndex = defaultIndex;
        int count = tabItems.Length;
        for (int i = 0; i < count; i++)
        {
            var item = tabItems[i];
            if (defaultIndex >= 0 && defaultIndex == i)
                item.Init(i, true);
            else
                item.Init(i, false);
            item.OnClickEvent = OnClickTabHandle;
        }
    }

    public void SetDefault(int index, bool notifyEvent = true)
    {
        Init();
        SetTabOn(index);
        if (notifyEvent)
            NotifyEvent();
    }

    public void SetAllOff(bool notifyEvent = true)
    {
        int count = tabItems.Length;
        for (int i = 0; i < count; i++)
        {
            tabItems[i].SetState(false);
        }
        if (notifyEvent)
            NotifyEvent();
    }

    public int GetCurIndex()
    {
        return curIndex;
    }

    void OnClickTabHandle(IUITabItem tabItem, int index)
    {
        if (tabItem.GetState() == false)
        {
            SetTabOn(index);
            NotifyEvent();
        }
    }

    void SetTabOn(int index)
    {
        if (index < 0 || index >= tabItems.Length) return;
        int count = tabItems.Length;
        for (int i = 0; i < count; i++)
        {
            tabItems[i].SetState(i == index);
        }
        curIndex = index;
    }

    void NotifyEvent()
    {
        onTabClick.Invoke(curIndex);
    }

#if UNITY_EDITOR

    void OnValidate()
    {
        if (Application.isPlaying) return;
        Debug.Log("UICustomTab onvalidate");
        tabItems = GetComponentsInChildren<UITabItem>();
    }

#endif

}
