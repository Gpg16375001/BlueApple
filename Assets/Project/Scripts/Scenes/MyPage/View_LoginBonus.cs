using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_LoginBonus : ViewBase
{
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_LoginBonus Create(LoginbonusData loginbonusData, Action didTapClose = null)
    {
        var go = GameObjectEx.LoadAndCreateObject("Mypage/View_LoginBonusPop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var c = go.GetOrAddComponent<View_LoginBonus>();
        c.InitInternal(loginbonusData, didTapClose);
        return c;
    }
    // 内部初期化.
    private void InitInternal(LoginbonusData loginbonusData, Action didTapClose)
    {
        m_Close = false;
        m_didTapClose = didTapClose;

        // ボタン設定.
        this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
        this.SetCanvasCustomButtonMsg("Bg", DidTapClose);

        // リストのルートオブジェクトを取得
        RectTransform WeeklyList = GetScript<RectTransform>("WeeklyList");

        LoginbonusDistItem todayItem = null;

        // アイテム情報を登録
        foreach (var loginbonusDayCount in loginbonusData.CurrentRoundDayCountList) {
            var loginbonusItem = MasterDataTable.loginbonus_dist_item.Get (loginbonusData.LoginbonusId, loginbonusDayCount);
            if(loginbonusItem != null) {
                if (loginbonusDayCount == loginbonusData.LastReceivedDayCount) {
                    todayItem = loginbonusItem;
                }
                // anchorの下に並べる
                var go = GameObjectEx.LoadAndCreateObject ("Mypage/ListItem_LoginBonusDailyItem",
                    GetScript<RectTransform>(string.Format("ListItemAnchor{0}", loginbonusDayCount)).gameObject);
                var item = go.GetComponent<ListItem_LoginBonusDailyItem> ();
                item.Init(loginbonusData.LastReceivedDayCount == loginbonusDayCount,
                    loginbonusData.LastReceivedDayCount > loginbonusDayCount,
                    loginbonusDayCount,
                    loginbonusItem
                );
            }
        }

        // 今日取得分のアイテムを設定
        if (todayItem != null) {
            var txtpItemName = GetScript<TextMeshProUGUI> ("txtp_ItemName");
            txtpItemName.SetText (todayItem.item_type.GetName(todayItem.item_id));

            var itemRectTrans = GetScript<RectTransform> ("Item");
            var go = GameObjectEx.LoadAndCreateObject ("Mypage/ListItem_LoginBonusDailyItem", itemRectTrans.gameObject);
            var item = go.GetComponent<ListItem_LoginBonusDailyItem> ();
            item.Init(false, false, null, todayItem);
        }

        // ニエンテLive2dの表示
        this.GetScript<RectTransform>("CharacterAnchor").gameObject.DestroyChildren();
        loader = new UnitResourceLoader(308001011); // 308001011=司書
        loader.LoadFlagReset();
        loader.IsLoadLive2DModel = true;
        loader.IsLoadVoiceFile = true;

        loader.LoadResource (
            (resource) => {
                var go = Instantiate (resource.Live2DModel) as GameObject;
                var canvas = this.GetScript<Canvas> ("CharacterAnchor");
                go.transform.SetParent (canvas.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                var cubismRender = go.GetComponentsInChildren<Live2D.Cubism.Rendering.CubismRenderController> () [0];
                cubismRender.SortingLayerId = canvas.sortingLayerID;
                cubismRender.SortingOrder = canvas.sortingOrder;

                m_Live2dVoicePlayer = go.GetOrAddComponent<Live2dVoicePlayer> ();
                m_Live2dVoicePlayer.Play ("VOICE_308001011", string.Format ("greeting_00{0}", UnityEngine.Random.Range (1, 4)));
            }
        );

        bgLoaded = false;
        GetScript<ScreenBackground> ("img_LoginBonusBg").CallbackLoaded (() => {
            bgLoaded = true;
        });
    }

    /// <summary>
    /// ポップアップの開くアニメーションを実行
    /// Enableになったタイミングだと暗転時だったりするので明示的に再生できるように
    /// </summary>
    public void Open()
    {
        if (!m_Close) {
            return;
        }
        m_Close = true;
        PlayOpenCloseAnimation (true, () => {
            m_Close = false;
        });
    }

    // ボタン : 閉じる.
    void DidTapClose()
    {
        if (m_Close) {
            return;
        }

        m_Close = true;

        PlayOpenCloseAnimation (false, () => {
            if (m_didTapClose != null) {
                m_didTapClose ();
            }
            this.Dispose ();
        });
    }

    // 開閉アニメーション処理.
    private void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
        do{
            yield return null;
        }while(anim.isPlaying);
        if(didEnd != null){
            didEnd();
        }
    }


    public override void Dispose ()
    {
        if (m_Live2dVoicePlayer != null) {
            m_Live2dVoicePlayer.Stop ();
            m_Live2dVoicePlayer.gameObject.SetActive (false);
        }
        loader.Dispose ();
        base.Dispose ();
    }

    public bool IsLoaded()
    {
        return loader.IsLoaded && bgLoaded;
    }

    void Awake()
    {
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

    bool m_Close = false;
    private Action m_didTapClose;
    private Live2dVoicePlayer m_Live2dVoicePlayer;
    UnitResourceLoader loader;
    bool bgLoaded;
}
