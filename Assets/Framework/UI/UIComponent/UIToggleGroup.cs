using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/**
* ToggleGroup
* 
* 单选tab控件
* 
* **/
public class UIToggleGroup : ToggleGroup
{
    [SerializeField] int soundID = 3002;
    [SerializeField] protected UIToggle[] toggles;

    public class ToggleEvent : UnityEvent<Toggle, int> { }
    public readonly ToggleEvent onToggleClick = new ToggleEvent();

    public int ToggleCount
    {
        get { return toggles.Length; }
    }

    private bool _notifyEvent = true;


    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < toggles.Length; i++)
        {
            UIToggle tg = toggles[i];
            tg.Index = i;
            tg.onToggleEvent.AddListener(OnClickToggle);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onToggleClick.RemoveAllListeners();
    }

    private void OnClickToggle(UIToggle toggle, bool flag)
    {
        //Debug.Log(toggle.name + "  toggle state : " + toggle.isOn + "  result : " + flag);
        if (!flag) return;
        if (_notifyEvent && onToggleClick != null)
        {
            onToggleClick.Invoke(toggle, toggle.Index);
        }
    }

    /// <summary>
    /// 设置当前toggles状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="notify"></param>
    public void SetDefault(int index = 0, bool notify = true)
    {
        //Debug.Log(gameObject.name + "  set default");
        if (toggles.Length <= index)
            return;
        //判断重复设置
        if (toggles[index].isOn)
        {
            if (notify && onToggleClick != null)
                onToggleClick.Invoke(toggles[index], index);
            return;
        }
        bool oldNotifyState = _notifyEvent;
        _notifyEvent = notify;
        UIToggle toggle;
        for (int i = 0; i < toggles.Length; i++)
        {
            toggle = toggles[i];
            bool isOn = i == index;
            if (toggle.isOn && isOn && _notifyEvent && onToggleClick!=null)
            {
                onToggleClick.Invoke(toggle, i);
            }
            toggle.isOn = isOn;
        }
        _notifyEvent = oldNotifyState;
    }

    public int GetCurIndex()
    {
        int index = 0;
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            if (toggles[i].isOn)
                return index;
            index++;
        }
        return index;
    }

    public int GetMaxIndex()
    {
        return toggles.Length;
    }

    public UIToggle GetCurToggle()
    {
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            if (toggles[i].isOn)
            {
                return toggles[i];
            }
        }
        return null;
    }

    public Toggle GetToggle(int index)
    {
        if (index >= toggles.Length)
        {
            return null;
        }
        return toggles[index];
    }

    [ContextMenu("Init toggles")]
    public void InitToggles()
    {
        toggles = GetComponentsInChildren<UIToggle>();
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            toggles[i].group = this;
        }
    }

}
