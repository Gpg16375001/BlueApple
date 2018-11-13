using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ScreenController : パーティ編成.
/// </summary>
public class PartyEditSController : ScreenControllerBase
{
    /// <summary>
    /// バトル準備か否か.
    /// </summary>
    public bool IsBattleInit { get; set; }
    /// <summary>
    /// PvP用編成か
    /// </summary>
    public bool IsPvP { get; set; }
    /// <summary>
    /// 戻る画面がPvpの対戦相手選択か？
    /// </summary>
    public bool IsPvPOpponentSelect { get; set; }

    /// <summary>
    /// サポートカード情報.フレンド選択して入れば設定.   
    /// </summary>
    public SupporterCardData SupportCard { get; set; }

    public string PrevSceneName { get; set; }


    /// <summary>
    /// 起動画面の生成.
    /// </summary>
    public override void CreateBootScreen()
    {
		SoundManager.SharedInstance.PlayBGM(SoundClipName.bgm012);  // TODO : シチュエーションに応じた選曲が必要か否か
        var go = GameObjectEx.LoadAndCreateObject("PartyEdit/Screen_PartyEdit", this.gameObject);
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<Screen_PartyEdit>();
        c.Init(IsBattleInit, IsPvP, IsPvPOpponentSelect, SupportCard, PrevSceneName == null ? ScreenChanger.SharedInstance.PrevSceneName : PrevSceneName);
    }
}