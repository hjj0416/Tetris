

public static class UIEventType
{
    private static short _MaxIndex = 0;

    private static short GetIndex()
    {
        return _MaxIndex++;
    }

    //------------------------------------------------ui----------------------------------------------------------------------------
    public static readonly short OPEN_POPUP_UI = GetIndex();
    public static readonly short PLAYER_HP_CHANGE = GetIndex();
    public static readonly short ON_CANCEL_REBORN = GetIndex();

    public static readonly short ON_JOYSTICK_MOVE_START = GetIndex();
    public static readonly short ON_JOYSTICK_MOVE = GetIndex();
    public static readonly short ON_JOYSTICK_MOVE_END = GetIndex();

    public static readonly short ON_CLICK_SHOPITEM = GetIndex();

}
