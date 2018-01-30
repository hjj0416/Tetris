using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUITabItem
{
    Action<IUITabItem, int> OnClickEvent { get; set; }
    void Init(int index, bool isOn);
    void SetState(bool isOn);
    bool GetState();
}

public class UITabItem : MonoBehaviour, IUITabItem, IPointerClickHandler
{
    public Action<IUITabItem, int> OnClickEvent { get; set; }
    private int _index;
    private bool _isOn;
    public void Init(int index, bool isOn)
    {
        _index = index;
        _isOn = !isOn;//force update
        SetState(isOn);
        OnInit();
    }

    public void SetState(bool isOn)
    {
        if (_isOn == isOn) return;
        _isOn = isOn;
        OnStateChanged(_isOn);
    }

    public bool GetState()
    {
        return _isOn;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickEvent != null) OnClickEvent(this, _index);
    }

    #region virtual

    protected virtual void OnInit()
    {
    }

    protected virtual void OnStateChanged(bool isOn)
    {
        
    }

    #endregion

}