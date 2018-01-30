using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour 
{
    [SerializeField] private float Percent = 0;
    [SerializeField] private Slider slider;
    [SerializeField] private Text ValueLabel;

    private float _duration = 1f;
    private float _targetPercent = 0;
    private Tween tw;
    private bool _pauseWhenFull;
    private int _capacity = 0;
    /// <summary>
    /// 进度条满时回调
    /// </summary>
    public Action OnFullCb;
    /// <summary>
    /// 进度条完成时回调
    /// </summary>
    public Action OnCompleteCb;

    void Awake()
    {
        if (slider != null)
            slider.value = Percent;
    }

    private float value
    {
        get
        {
            if (slider != null)
                return slider.value;
            return 0;
        }
        set
        {
            if (slider != null)
            {
                slider.value = value;
            }
        }
    }

    public string text
    {
        set { ValueLabel.text = value; }
    }



    public void SetDuration(float time)
    {
        _duration = time;
    }

    /// <summary>
    /// 设置初始值
    /// </summary>
    /// <param name="pValue"></param>
    /// <param name="capacity"></param>
    public void SetStartValue(int pValue, int capacity = 0)
    {
        if (ValueLabel != null)
            ValueLabel.text = pValue.ToString();
        if (capacity != 0)
            _capacity = capacity;
        value = (float)pValue / _capacity;
    }

    public void SetStartPercent(float pValue)
    {
        value = pValue;
    }

    /// <summary>
    /// 设置百分比
    /// </summary>
    /// <param name="value"></param>
    public void SetTargetPercent(float pValue, bool pauseWhenFull = false)
    {
        //目标值
        if (Mathf.Abs(value-pValue)<0.01f)
            return;
        _targetPercent = pValue;
        _pauseWhenFull = pauseWhenFull;
        if (tw != null)
            tw.Kill();
        DoProgress();
    }

    public void GoOn()
    {
        DoProgress();
    }

    void DoProgress()
    {
        if (slider.value >= 1)
            slider.value = 0;
        if (_targetPercent > 1)
        {
            _targetPercent -= 1;
            OnProgress(1);
        }
        else
        {
            OnProgress(_targetPercent);
            _targetPercent = 0;
        }
    }

    void OnProgress(float endValue)
    {
        float percent = slider.value;
        float duration = (endValue - percent) * _duration;
        tw = DOTween.To(() => percent, x => percent = x, endValue, duration);
        tw.OnUpdate(() =>
        {
            slider.value = percent;
        });
        tw.OnComplete(() =>
        {
            slider.value = endValue;
            if (endValue == 1)
            {
                if (OnFullCb != null) OnFullCb();
            }
            if (_targetPercent > 0)
            {
                if (!_pauseWhenFull)
                    DoProgress();
            }
            else
            {
                if (OnCompleteCb != null) OnCompleteCb();
            }
        });
    }

}
