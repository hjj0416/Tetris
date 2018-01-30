using TheNextMoba.UI;

public interface ICellNotifier
{
    void NotifyEvent(string eventName, UICell cell, bool flag);
}
