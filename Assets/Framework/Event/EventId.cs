
/// <summary>
/// 事件类型
/// (根据需要取名称，不得重复)
/// </summary>
public enum EventEnum
{
    //-----------system---------------------
    //-----------battle---------------------
    TeamRun,
    AutoFight,
    //PassScreen,
    SelectSkill,
    RoleDead,
    ChangeHero,
    LeaveBattle,
    DebugVictory,
    //------------Home--------------------
    UpdateLv,
    UpdateExp,
    UpdateGold,
    UpdateGem,
    UpdatePower,
    UpdatePvePhysical,
    UpdatePvpPhysical,
    UpdateBossPhysical,
    UpdateFriendPoint,
    UpdateHybridStone,
    UpdateWater,
    UpdateMetal,
    UpdatePetroleum,
    UpdateUranium,
    UpdateBuildingLevel,//建筑升级
    UpdatePvpPoint,//竞技场分
    UpdateDice,//骰子更新
    //-------------full screen-------------------
    OpenFullScreen,
    CloseFullScreen,
    //--------------other------------------
    OnHybridChange,
    OnUpdateHatchery,
    UpdateFriends,
    UpdateBuildQueue,
    OnSendAssist,//援助
    OnRcvAssist,//收到援助
    OnShipBattleTouchDown,//点击英雄使用技能
    OnShipBattleTouchUp,
    OnUseHeroSkill,//确认使用英雄技能
    ChangeSBattleSpeed,//更改星舰战斗速度
    //----------PVP-------------------
    UpdatePvpTeam,
    //--------------------------------
    //----------Boss------------------
    FoundRandomBoss,//探索到boss
    RemoveRandomBoss,//移除探索boss
    UpdateBossTeam,
    //--------------------------------
    HeroUpgrade,
    //------------Alliance--------------------
    JoinAlliance,
    ExitAlliance,
    //------------Building--------------------
    RefreshRepairCost,
    BuildingComplete,//建筑完成
    //--------------------------------
    UpdateFriendList,     //刷新好于列表
    UpdateFriendApplyList,//刷新好友申请列表
    UpdateFriendVerifyList,//刷新好友申请列表
    RemoveApplyCell,//取消好友申请
    RemoveSysGiveCell,//申请好友
    //UpdateLetterEmailList,//刷新私信邮箱
    //UpdateGiftEmailList,  //刷新礼物邮箱
    //UpdateFriendPointList,//刷新友情点邮箱

    RefreshPlayerIcon,
    RefreshBossIcon,
    RefreshPlayerName,
    RefreshAccountID,

    RefreshLetterDot,       //刷新私信提示
    RefreshEmailDot,        //刷新信箱提示
    RefreshFriendDot,       //刷新好友提示
    RefreshArenaDot,        //刷新Pvp提示
    RefreshBossDot,         //刷新好友提示
    RefreshShopDot,         //刷新商城提示

    //--------------------------------
    //---------------Chat---------------
    Update_Chat_Common,        //刷新频道聊天状态
    Update_Chat_League,        //刷新联盟聊天状态
    Server_State_Chat,         //聊天服务器状态
    Block_Chat,
    //--------------------------------
    //---------------心跳刷新请求---------------
    Heart_Player,       //心跳刷新玩家基本信息
    Heart_Hero,         //心跳刷新英雄
    Heart_Equip,        //心跳刷新装备
    Heart_Item,         //心跳刷新物品
    Heart_Letter,              //心跳刷新私信
    Heart_FriendList,           //心跳刷好友列表
    Heart_FriendApply,          //心跳刷新好友请求
    Heart_FriendVerify,         //心跳刷新好友确认
    Heart_EventActivity,        //心跳活动事件
    Heart_GiftEmail,            //心跳刷新邮件
    Heart_FriendPoint,          //心跳刷新友情点
    Heart_FightHistory,         //心跳刷新战斗记录
    Heart_Boss,                 //心跳刷新boss信息
    Heart_Arena,                //心跳刷新pvp信息
    Heart_Shop,                 //心跳刷新商城
    Heart_Alliance_Dismiss,     //心跳刷新联盟解散
    Heart_Alliance_Join,        //心跳刷新被加入联盟
    Heart_Alliance_Duty,        //心跳刷新职位变动
    Heart_Alliance_KickOut,     //心跳刷新被提出联盟

    //--------------------------------

    Update_PM_TempList,
    Update_PM_ItemList,
    Update_PM_ItemDetailList,
    Remove_PM_TempList,

}

public class EventId 
{

}
