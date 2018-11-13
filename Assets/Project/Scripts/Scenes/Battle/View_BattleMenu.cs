using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_BattleMenu : ViewBase
{
    /// <summary>
    /// 表示中？
    /// </summary>
    public bool IsVisible 
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    /// <summary>
    /// 自身の全ボタンの有効/無効設定.
    /// </summary>
    public bool IsEnableButtons 
    { 
        set {
            if(DidSetEnableButton != null){
                DidSetEnableButton(value);
            }
            IsEnableButton = value; 
        } 
    }

    /// <summary>
    /// Event : メニュー側でボタンロックを掛ける際に追従してロックしたいViewがあればこちらに登録しておく.
    /// </summary>
    public event Action<bool/*bActive*/> DidSetEnableButton;

    private string AndroidBackButtonInfoId = string.Empty;
    /// <summary>
    /// 生成メソッド.
    /// </summary>
    public static View_BattleMenu Create()
    {
        var go = GameObjectEx.LoadAndCreateObject("Battle/View_BattleMenu");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var instance = go.GetOrAddComponent<View_BattleMenu>();
        instance.InitInternal();
        return instance;
    }

    public void Open(bool bImmediate = false, Action didClose = null)
    {
        if (m_bOpen && gameObject.activeSelf) {
            return;
        }
        m_didClose = didClose;
        // オープン時はアクティブをあげる
        gameObject.SetActive (true);

        if (string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButtonInfoId = AndroidBackButton.SetEventInThisScene (DidTapClose);
        }
        this.StartCoroutine( this.PlayOpenClose(bImmediate) );
    }

    public void Close(bool bImmediate = false, Action didClose = null)
    {
        if (!m_bOpen && !gameObject.activeSelf) {
            return;
        }
        if (!string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButton.DeleteEvent (AndroidBackButtonInfoId);
            AndroidBackButtonInfoId = null;
        }
        this.StartCoroutine( this.PlayOpenClose(bImmediate, didClose) );
    }

    // 内部初期化.
    private void InitInternal()
    {
		// サポート表示.
		var rootObj = this.GetScript<RectTransform>("UnitIcon").gameObject;
        var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", rootObj);
		go.GetOrAddComponent<ListItem_UnitIcon>().Init(AwsModule.BattleData.BattleEntryData.SupporterCardData.ConvertCardData());

		// ミッション内容.
		for (var i = 1; i <= AwsModule.BattleData.MissionProgress.MissionList.Count; ++i){
			var label = this.GetScript<TextMeshProUGUI>(string.Format("Mission{0}/txtp_MissionText", i));
			if(i > AwsModule.BattleData.MissionProgress.MissionList.Count){
				label.gameObject.SetActive(false);
				continue;
			}
			var mission = AwsModule.BattleData.MissionProgress.MissionList[i-1];         
			label.SetText(mission.ExplanatoryText);
			this.GetScript<Image>(string.Format("Mission{0}/WhitePanel", i)).color = mission.IsAlreadyAchived ? Color.red : Color.white;
		}

        // ボタン設定
		this.SetCanvasCustomButtonMsg("bt_close", DidTapClose);
		this.SetCanvasCustomButtonMsg("Retire/bt_Common", DidTapRetire);

        // 初期化時は閉じておく
        Close(true);
    }

    #region ButtonDelegate.

    // ボタン：クローズ
    void DidTapClose()
    {
        Close(false, m_didClose);
    }
    // ボタン：撤退
    void DidTapRetire()
    {
        IsEnableButtons = false;
        // 撤退確認ポップアップ
        PopupManager.OpenPopupYN (TextData.GetText ("confirm_retire"),
            () => {
                // 自分自信を閉じる
                Close();

                PopupManager.OpenPopupOK(TextData.GetText ("retire"), () => {
                    BattleProgressManager.Shared.RetireProc(() => {
                        IsEnableButtons = true;
                        if(m_didClose != null){
                            m_didClose();
                        }
                    });
                });
            },
            () => {
                IsEnableButtons = true;
            }
        );
    }

    #endregion

    // アニメーション開閉処理.
    private IEnumerator PlayOpenClose(bool bImmediate = false, Action didClose = null)
    {
        LockInputManager.SharedInstance.IsLock = true;

        if(m_anim == null){
            m_anim = this.GetComponent<IgnoreTimeScaleAnimation>();
        }
        if(bImmediate){
            if(m_bOpen) {
                m_anim["PopupClose"].normalizedTime = 1f;
            }else{
                m_anim["PopupOpen"].normalizedTime = 1f;
            }
        }
        if(m_bOpen){
            m_anim.Play("PopupClose");
        }else{
            m_anim.Play("PopupOpen");
        }
        yield return null;

        m_bOpen = !m_bOpen;

        // アニメーション終了待ち
        while (m_anim.isPlaying) {
            yield return null;
        }
        if (!m_bOpen) {
            // クローズ時はアクティブを落とす
            gameObject.SetActive (m_bOpen);
            if (didClose != null) {
                didClose();
            }
        }
        LockInputManager.SharedInstance.IsLock = false;
    }

    void OnDestory()
    {
        if (!string.IsNullOrEmpty (AndroidBackButtonInfoId)) {
            AndroidBackButton.DeleteEvent (AndroidBackButtonInfoId);
            AndroidBackButtonInfoId = null;
        }
    }

    private bool m_bOpen = true;        // 開閉状態.
    private Action m_didClose;
    private IgnoreTimeScaleAnimation m_anim;
}