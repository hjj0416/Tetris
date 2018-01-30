using TheNextMoba.UI;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomUnityEvent
{
}

/// <summary>
/// 可以自定义一些常用unityevent
/// </summary>
public class uintUnityEvent : UnityEvent<uint>{}

public class LongPressUnityEvent : UnityEvent<bool> { }

public class CellEvent : UnityEvent<string, UICell, bool> { }

public class UIToggleEvent : UnityEvent<UIToggle, bool> { }
