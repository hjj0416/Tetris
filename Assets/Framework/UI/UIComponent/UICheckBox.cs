using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 复选框
/// 支持多个选中
/// 
/// </summary>
public class UICheckBox : MonoBehaviour
{
    [SerializeField] private Toggle[] toggles;

    public class ToggleEvent : UnityEvent<int> { }
    public readonly ToggleEvent onToggleClick = new ToggleEvent();

    public int curValue { get; private set; }
    public int maxValue { get; private set; }
    private bool _sendEvent;

    void Awake()
    {
        maxValue = 0;
        for (int i = 0; i < toggles.Length; i++)
        {
            Toggle tp = toggles[i];
            tp.isOn = false;
            tp.onValueChanged.AddListener(OnClickToggle);
            maxValue += 1 << i;
        }
        _sendEvent = true;
    }

    void OnDestroy()
    {
        onToggleClick.RemoveAllListeners();
    }

    public void ToggleOffAll(bool sendEvent = true)
    {
        _sendEvent = false;
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            toggles[i].isOn = false;
        }
        if(sendEvent)
            onToggleClick.Invoke(0);
        _sendEvent = true;
    }

    public void ToggleOnAll(bool sendEvent = true)
    {
        _sendEvent = false;
        int ret = 0;
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            toggles[i].isOn = true;
            ret += 1 << i;
        }
        if (sendEvent)
            onToggleClick.Invoke(ret);
        _sendEvent = true;
    }


    private void OnClickToggle(bool flag)
    {
        if (!_sendEvent) return;
        curValue = 0;
        int count = toggles.Length;
        for (int i = 0; i < count; i++)
        {
            if (toggles[i].isOn)
            {
                curValue += 1 << i;
            }
        }
        onToggleClick.Invoke(curValue);
    }


    #region tool

    [ContextMenu("bind toggles")]
    void BindToggles()
    {
        toggles = GetComponentsInChildren<Toggle>();
    }

    #endregion

}