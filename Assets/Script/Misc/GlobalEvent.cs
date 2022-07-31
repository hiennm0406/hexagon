using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalEvent
{
    // khởi tạo game
    public static readonly StationEvent startGame = new StationEvent();

    // bắt đầu turn mới - count turn
    public static readonly StationEvent IncreaseTurn = new StationEvent();

    // kết thúc lượt xoay - merge xong
    public static readonly StationEvent EndTurn = new StationEvent();

    // tạo ô xong, chờ player xoay
    public static readonly StationEvent WaitPlayerMove = new StationEvent();

    /// <summary>
    /// Trigger when any change in user inventory
    /// </summary>
    public static readonly StationEvent GetChangePlayerInventory = new StationEvent();


    public static readonly StationEvent OnMazeInitDone = new StationEvent();
    /// <summary>
    /// Save the process of player
    /// </summary>
    public static readonly StationEvent GetUserStatic = new StationEvent();

    /// <summary>
    /// Save the process of player
    /// </summary>
    public static readonly StationEvent InitTeam = new StationEvent();


    /// <summary>
    /// Trigger when start of new turn
    /// </summary>
    public static readonly StationEvent OnPreTurn = new StationEvent();


    /// <summary>
    /// Trigger when start of new turn
    /// </summary>
    public static readonly StationEvent OnBattlePhase = new StationEvent();

    /// <summary>
    /// Trigger when start done cast skill
    /// </summary>
    public static readonly StationEvent OnCastSkillDone = new StationEvent();


    /// <summary>
    /// Trigger when start of new turn
    /// </summary>
    public static readonly StationEvent OnEndBattleTurn = new StationEvent();

    /// <summary>
    /// Trigger when win of map
    /// </summary>
    public static readonly StationEvent OnWinBattle = new StationEvent();


    /// <summary>
    /// Trigger when win of map
    /// </summary>
    public static readonly StationEvent OnWinMap = new StationEvent();



    /// <summary>
    /// trigger when changing scene, the previous scene will be removed next
    /// </summary>
    public static readonly StationEvent OnSceneStartLoad = new StationEvent();

    /// <summary>
    /// the scene just finished loading, we load all the save for that area
    /// </summary>
    public static readonly StationEvent OnSceneInitialize = new StationEvent();

    /// <summary>
    /// trigger when it is time to initialize the npc, items, or other objects in the scene
    /// </summary>
    public static readonly StationEvent OnSceneLoadObjects = new StationEvent();

    /// <summary>
    /// the scene is ready, can hide loading screen
    /// </summary>
    public static readonly StationEvent OnSceneReady = new StationEvent();

    /// <summary>
    /// we are leaving this area, use this to cancel all pending actions, animation...
    /// </summary>
    public static readonly StationEvent OnBeforeLeaveScene = new StationEvent();


    //    #region TEAM
    //    public static StationEvent<BaseCharacter> OnCharacterAdded = new StationEvent<BaseCharacter>();
    //    public static StationEvent<BaseCharacter> OnCharacterRemoved = new StationEvent<BaseCharacter>();
    //    public static StationEvent<BaseCharacter> OnLeaderChanged = new StationEvent<BaseCharacter>();
    //    #endregion

    //    #region UI
    //    public static readonly StationEvent<UiEventData> OnUiEvent = new StationEvent<UiEventData>();
    //    #endregion
    //}
}
