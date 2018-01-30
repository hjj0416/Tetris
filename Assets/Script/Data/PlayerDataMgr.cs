using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataMgr : MonoSingleton<PlayerDataMgr> {
    public PlayerData playerData;
    public EventDispatcher dispatcher;

    public override bool Init()
    {
        base.Init();
        dispatcher = new EventDispatcher();
        playerData = new PlayerData();
        return true;
    }

    public override bool UnInit()
    {
        base.UnInit();

        return true;
    }
}
