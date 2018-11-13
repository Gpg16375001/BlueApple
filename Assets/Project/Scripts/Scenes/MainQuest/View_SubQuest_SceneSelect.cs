using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// View : サブクエスト画面のシーン選択.
/// </summary>
public class View_SubQuest_SceneSelect : ViewBase, IViewMainQuest
{
    /// <summary>
    /// 起動画面.
    /// </summary>
    public MainQuestBootEnum Boot { get { return m_boot; } }
    private MainQuestBootEnum m_boot;

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(MainQuestBootEnum boot, int act, Action<MainQuestBootEnum> procChangeView)
    { 
        m_boot = boot;
        m_actNum = act;
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

    // シーン一覧リスト作成.
    private void CreateList()
    {
        var root = this.GetScript<ScrollRect>("StageScrollView").content.gameObject;
        root.DestroyChildren();
        var questList = AwsModule.ProgressData.ReleaseSubQuestIdList.Select(id => MasterDataTable.quest_sub[id]).ToList();
        var sceneList = questList.FindAll(q => q.index == m_actNum);
        foreach(var quest in sceneList){
            var go = GameObjectEx.LoadAndCreateObject("MainQuest/ListItem_SubQuest_Scene", root);
            var c = go.GetOrAddComponent<ListItem_MainQuest>();
            var stage = MasterDataTable.stage[quest.stage_id];
            c.InitSubScene(quest, stage.name, DidTapScene);
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
        m_procChangeView(MainQuestBootEnum.SubAct);
    }

    // ボタン : シーン.
    void DidTapScene(SubQuest quest)
    {
        AwsModule.ProgressData.CurrentQuest = quest;
        AwsModule.ProgressData.CurrentQuestAchievedMissionIdList = null;
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            ScreenChanger.SharedInstance.GoToFriendSelect();
        });
    }

    #endregion

    private int m_actNum;
    Action<MainQuestBootEnum> m_procChangeView;
}
