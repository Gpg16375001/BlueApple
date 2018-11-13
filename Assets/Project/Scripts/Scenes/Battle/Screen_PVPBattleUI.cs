using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;

using BattleLogic;

using UnityEngine.EventSystems;

/// <summary>
/// Screen : バトルのUI部分.
/// </summary>
public class Screen_PVPBattleUI : Screen_BattleUI
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public override void Init()
    {
        BattleProgressManager.Shared.RegistObserverAsFirst(this);
        BattleProgressManager.Shared.BattleUI = this;

        // メニューオブジェクト生成.
        // TODO: PVP用のMenuに変更する
        //m_menu = View_BattleMenu.Create();
        // TODO : エフェクト用のオブジェクトを作成.似たような感じのものが複数出来上がってるのでまとめられないかどうか.
        m_battleStartEffect = View_BattleEffect.Create("PVP/View_PVPStart");
        m_battleStartEffect.gameObject.GetOrAddComponent<View_PVPStart> ().Init ();
        m_battleStartEffect.IsVisible = false;
        m_battleWaveCountEffect = View_BattleEffect.Create("Battle/BattleWave_countup");
        m_battleWaveCountEffect.IsVisible = false;
        m_battleWaveFinishEffect = View_BattleEffect.Create("Battle/BattleWave_finish");
        m_battleWaveFinishEffect.IsVisible = false;

        // ボタン設定
        //this.SetCanvasCustomButtonMsg("bt_BattleMenu", DidTapMenu);
        this.SetCanvasCustomButtonMsg("bt_Speed", DidTapChangeSpeed);
        this.SetCanvasCustomButtonMsg("bt_Auto", DidTapAuto);


        var SpCommandButton = GetScript<CustomButton> ("bt_SPCommand");
        SpCommandButton.m_EnableLongPress = true;
        SpCommandButton.onClick.AddListener(OnSPCommandClick);
        SpCommandButton.onLongPress.AddListener(OnSPCommandLongPress);
        SpCommandButton.onRelease.AddListener(OnSPCommandRelease);

        var AttackCommandButton = GetScript<CustomButton> ("bt_AttackCommand");
        AttackCommandButton.m_EnableLongPress = true;
        AttackCommandButton.onClick.AddListener(OnAttackCommandClick);
        AttackCommandButton.onLongPress.AddListener(OnAttackCommandLongPress);
        AttackCommandButton.onRelease.AddListener(OnAttackCommandRelease);

        m_activeTimeLine = this.GetScript<RectTransform>("GridCountTimeLine").gameObject.GetOrAddComponent<View_ActiveTimeLine> ();
        m_activeTimeLine.Init();
    }

    /// <summary>
    /// バトル開幕.
    /// </summary>
    protected override void CallbackBattleStart()
    {
        LockInputManager.SharedInstance.IsLock = true;


        // バトル開始フェードを開ける.
        m_battleStartEffect.Play(
            () => {
                View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, null);
                // ボイス再生を一緒にする。
                // とりあえず、先頭にいる人のボイスを鳴らしてみる
                var unit = AwsModule.BattleData.AllyParameterList.OrderBy(x => x.PositionIndex).First();
                SoundManager.SharedInstance.PlayVoice(unit.VoiceFileName, SoundVoiceCueEnum.pvp_start);
            }
        );
        PlayInOutUI (true);
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.IsLock = false;
            BattleProgressManager.Shared.WaveStart();
            SetAutoAnimation(AwsModule.BattleData.IsAuto);
            SetSpeedAnimation(AwsModule.BattleData.BattleSpeed);
        });
    }   

    // コールバック : WAVE切り替え.
    protected override void CallbackChangeWave(ListItem_BattleUnit unit)
    {
        // バトル終了.
        if(AwsModule.BattleData.EndBattle) {
            BattleProgressManager.Shared.WinMotionStart(unit, () => {
                // 勝利扱いで終了
                FinishBattle(true);
            });
            return;
        }
        // 演出.
        if(AwsModule.BattleData.WaveCount < AwsModule.BattleData.MaxWaveCount) {
            m_battleWaveCountEffect.gameObject.GetOrAddComponent<View_BattleWaveCountUp> ().SetWaveCount (AwsModule.BattleData.WaveCount);
            m_battleWaveCountEffect.Play();
        } else {
            m_battleWaveFinishEffect.Play();
        }
        BattleScheduler.Instance.AddAction (BattleProgressManager.Shared.WaveStart);
    }

    protected override void CallbackAskContinue ()
    {
        CallbackGameOver ();
    }
    // ゲームオーバー時処理
    protected override void CallbackGameOver()
    {
        // BGM消す
        SoundManager.SharedInstance.StopBGM();

        // すでに溜まっているキューを削除する。
        BattleScheduler.Instance.Clear ();
        BattleScheduler.Instance.AddAction (() => {
            LockInputManager.SharedInstance.ForceUnlockInput();
            // 敗北扱いで終了
            FinishBattle(false);
        });
    }

    void FinishBattle(bool isWin)
    {
        LockInputManager.SharedInstance.IsLock = true;

        if(isWin) {
            SoundManager.SharedInstance.PlayBGM (SoundClipName.Jingle001);
        } else {
            SoundManager.SharedInstance.PlayBGM (SoundClipName.Jingle002);
        }
        SendAPI.PvpFinishBattle(
            AwsModule.BattleData.PVPBattleEntryData.EntryId, isWin,
            (result, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!result) {
                    // reboot
                    ScreenChanger.SharedInstance.Reboot();
                }

                AwsModule.UserData.UserData = response.UserData;
                // resultに移行
                View_PVPBattleResult.Create(response, isWin);

                AwsModule.BattleData.PVPBattleEntryData = null;
                AwsModule.BattleData.StageID = 0;
            }
        );
    }

    public override void UpdateBossInfo(Parameter unit)
    {
    }

    public override void UpdateBossCondition(Parameter unit)
    {
    }

    public override void DisplayBossInfo (bool enable)
    {
    }

    protected override void UpdateViewSettings ()
    {
        this.GetScript<TextMeshProUGUI>("txtp_BattleCount").SetText(AwsModule.BattleData.RestTurnCount);
    }

    public override void DisableBattleMenu ()
    {
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }
}
