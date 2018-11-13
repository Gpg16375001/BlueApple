using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// View : サブクエスト画面の幕選択.
/// </summary>
public class View_SubQuest_ActSelect : ViewBase, IViewMainQuest
{
    /// <summary>
    /// 起動画面.
    /// </summary>
    public MainQuestBootEnum Boot { get { return m_boot; } }
    private MainQuestBootEnum m_boot;

    /// <summary>
    /// 幕番号.
    /// </summary>
    public int ActNum { get; private set; }


    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(MainQuestBootEnum boot, Action<MainQuestBootEnum> procChangeView)
    {
        m_boot = boot;
        m_procChangeView = procChangeView;
        this.CreateList();

        // ボタン.
        this.SetCanvasButtonMsg("bt_CloseList", DidTapClose);
        this.SetCanvasButtonMsg("bt_BackList", DidTapBack);
    }

    /// <summary>
    /// 破棄処理.
    /// </summary>
    public void Destroy()
    {
        this.Dispose();
    }

    // 幕一覧リスト作成.
    private void CreateList()
    {
        var root = this.GetScript<ScrollRect>("StageScrollView").content.gameObject;
        root.DestroyChildren();
        var releaseIdList = AwsModule.ProgressData.ReleaseSubQuestIdList;
        var questList = MasterDataTable.quest_sub.DataList.FindAll(q => releaseIdList.Exists(id => id == q.id));
        var indexList = questList.Select(q => q.index).Distinct().ToList();
        foreach(var idx in indexList){
            var go = GameObjectEx.LoadAndCreateObject("MainQuest/ListItem_SubQuest_Act", root);
            var c = go.GetOrAddComponent<ListItem_MainQuest>();
            c.InitSubAct(idx, DidTapAct);
        }
    }

    #region ButtonDelegate.

    // ボタン : 閉じるボタン.
    void DidTapClose()
    {
        m_procChangeView(MainQuestBootEnum.Country);
    }

    // ボタン : 戻るボタン.
    void DidTapBack()
    {
        m_procChangeView(MainQuestBootEnum.Country);
    }

    // ボタン : 幕.
    void DidTapAct(int act)
    {
        this.ActNum = act;
        m_procChangeView(MainQuestBootEnum.SubScene);
    }

    #endregion

    Action<MainQuestBootEnum> m_procChangeView;
}
