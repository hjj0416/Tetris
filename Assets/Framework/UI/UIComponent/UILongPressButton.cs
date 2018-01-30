using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILongPressButton : Button
{
    private LongPressUnityEvent onLongPressEvent = new LongPressUnityEvent();

    private bool mHandled;
    private bool mPressed;
    private float mPressedTime;

    [SerializeField]
    private float mDuration = 0.9f;

    public float LongPressDuration
    {
        get
        {
            return mDuration;
        }
        set
        {
            mDuration = value;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        mPressed = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        mPressed = true;
        mHandled = false;
        mPressedTime = Time.realtimeSinceStartup;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!mHandled)
        {
            base.OnPointerUp(eventData);
        }
        mPressed = false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!mHandled)
        {
            base.OnPointerClick(eventData);
        }
    }

    private void Update()
    {
        if (!mPressed)
        {
            return;
        }

        if (Time.realtimeSinceStartup - mPressedTime >= LongPressDuration)
        {
            mPressed = false;
            mHandled = true;
            onLongPressEvent.Invoke(true);
        }
    }
}
