using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// メインクエスト画面のViewを操作するクラス.
/// </summary>
public class MainQuestViewController : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summa
	public void Init(GameObject parentObj, MainQuestCountryData[] countryDatas, Action<QuestDecideInfo> didDecide, Action<QuestDecideInfo, Action> willDecide = null)
    {
        m_rootObj = parentObj;
		m_countryDatas = countryDatas;
        m_didDecide = didDecide;
        m_willDecide = willDecide;
        m_info = new QuestDecideInfo();
    }

    /// <summary>
    /// 決定済みクエスト情報を外部から書き換える処理.強制上書きなので現在のViewを破棄して無視する.
    /// </summary>
	public void ForceUpdateDecideInfo(MainQuestBootEnum boot, IQuestData questInfo)
    {
		Debug.Log("ForceUpdateDecideInfo : boot="+boot);      
		var bootVal = (int)boot;
        m_info.IsMain = questInfo.QuestType == 1;
		m_info.Belonging = questInfo.Country;
		m_info.ChapterNum = !m_info.IsMain || bootVal >= (int)MainQuestBootEnum.Stage ? questInfo.ChapterNum : 0;
		m_info.StageNum = !m_info.IsMain || bootVal >= (int)MainQuestBootEnum.Stage ? questInfo.StageNum: 0;      
		m_info.QuestNum = !m_info.IsMain || bootVal >= (int)MainQuestBootEnum.Stage ? questInfo.QuestNum: 0;
    }

    /// <summary>
    /// View切り替え.引数のデリゲートは単発で切り替え時にやりたい処理を挿入する.
    /// </summary>
	public void ChangeView(MainQuestBootEnum boot, Action didChangeOnce = null)
    {
        this.WillChangeView(next: boot);
        if(m_willDecide != null){
			m_willDecide(m_info, () => this.ChangeViewProc(boot, didChangeOnce));
        }else{
			this.ChangeViewProc(boot, didChangeOnce);
        }
    }

    // View切り替え前にやっときたい処理はここ.
    private void WillChangeView(MainQuestBootEnum next)
    {
        // 国選択=メインクエストルート、サブクエスト幕選択=サブクエストルートシーンにつき毎度クリア.
        if(next == MainQuestBootEnum.Country || next == MainQuestBootEnum.SubAct){
            m_info = new QuestDecideInfo();
            m_info.IsMain = next == MainQuestBootEnum.Country;
            return;
        }
        m_info.IsMain = next != MainQuestBootEnum.SubAct && next != MainQuestBootEnum.SubScene;
        if(m_currentView == null){
            return;
        }
        switch (m_currentView.Boot) {
            case MainQuestBootEnum.Country: 
                {
                    var view = m_currentView as View_MainQuest_CountrySelect;
                    if (next == MainQuestBootEnum.Chapter) {
                        m_info.Belonging = view.DecideBelonging;
                    }
                }
                break;
            case MainQuestBootEnum.Chapter: 
                {
                    var view = m_currentView as View_MainQuest_ChapterSelect;
                    if (next == MainQuestBootEnum.Stage) {
                        m_info.ChapterNum = view.ChapterNum;    // 進む
                    }
                }
                break;
            case MainQuestBootEnum.Stage: 
                {
					if (next == MainQuestBootEnum.Chapter) {
						m_info.StageNum = 0;
                        m_info.ChapterNum = 0;
                    }
                }
                break;


            // 以下サブクエスト.
            case MainQuestBootEnum.SubAct:
                {
                    var view = m_currentView as View_SubQuest_ActSelect;
                    if (next == MainQuestBootEnum.SubScene) {
                        m_info.Belonging = MasterDataTable.belonging[BelongingEnum.Unknown];    // TODO : 一応不明のを一旦入れておく.
                        m_info.StageNum = view.ActNum;
                    }
                }
                break;
            case MainQuestBootEnum.SubScene:
                {
                    if (next == MainQuestBootEnum.SubAct) {
                        m_info.StageNum = 0;
                    }
                }
                break;
        }
    }
    // View切り替え処理.
	private void ChangeViewProc(MainQuestBootEnum boot, Action didChangeOnce = null)
    {
        if(m_currentView != null){
            m_currentView.Destroy();
        }
        // 生成後の処理.非同期対応.
		Action<IViewMainQuest> didCreate = view => {
			m_currentView = view;
			if (m_didDecide != null) {
                m_didDecide(m_info);
            }
			if(didChangeOnce != null){
				didChangeOnce();
			}
		};
        switch (boot) {
            // メインクエスト.
            case MainQuestBootEnum.Country:
				ChangeViewCountrySelect(boot, didCreate);
                break;
            case MainQuestBootEnum.Chapter:
				ChangeViewChapterSelect(boot, didCreate);
                break;
            case MainQuestBootEnum.Stage:
				ChangeViewStageSelect(boot, didCreate);
                break;

            // サブクエスト.
            case MainQuestBootEnum.SubAct:
				ChangeViewSubActSelect(boot, didCreate);
                break;
            case MainQuestBootEnum.SubScene:
				ChangeViewSubSceneSelect(boot, didCreate);
                break;
        }
    }

    #region Internal proc.

    // 国選択画面に切り替え.
	private void ChangeViewCountrySelect(MainQuestBootEnum boot, Action<IViewMainQuest> didCreate)
    {
        if(!m_info.IsMain){
            return;
        }
		var go = Instantiate(Resources.Load("MainQuest/View_MainQuest_CountrySelect")) as GameObject;
        var c = go.GetOrAddComponent<View_MainQuest_CountrySelect>();
		go.transform.SetParent(m_rootObj.transform, false);
		c.Init(boot, m_countryDatas, b => ChangeView(b));
		didCreate(c);
    }

    // 章選択画面に切り替え.
	private void ChangeViewChapterSelect(MainQuestBootEnum boot, Action<IViewMainQuest> didCreate)
    {
        if(m_info.Belonging == null || !m_info.IsMain){
            Debug.LogError("[MainQuestViewController] ChangeViewChapterSelect Error!! : Belonging not select yet.");
            return;
        }
		this.RequestByCountryInfo(m_info.Belonging, () => {
			var go = Instantiate(Resources.Load("MainQuest/View_MainQuest_ChapterSelect")) as GameObject;
            var c = go.GetOrAddComponent<View_MainQuest_ChapterSelect>();
			go.transform.SetParent(m_rootObj.transform, false);
			c.Init(boot, m_info.Belonging, b => ChangeView(b));
			didCreate(c);
		});
    }

    // ステージ選択画面に切り替え.
	private void ChangeViewStageSelect(MainQuestBootEnum boot, Action<IViewMainQuest> didCreate)
    {
        if(m_info.Belonging == null || !m_info.IsMain || m_info.ChapterNum <= 0){
            Debug.LogError("[MainQuestViewController] ChangeViewActSelect Error!! : Belonging or chapter not select yet.");
            return;
        }
		var go = Instantiate(Resources.Load("MainQuest/View_MainQuest_StageSelect")) as GameObject;
        var c = go.GetOrAddComponent<View_MainQuest_StageSelect>();
		go.transform.SetParent(m_rootObj.transform, false);
		c.Init(boot, m_info.Belonging, m_info.ChapterNum, m_info.StageNum, DidChangeStage, b => ChangeView(b));
		didCreate(c);
    }
	void DidChangeStage(MainQuestStageInfo stageInfo)
	{
		m_info.StageNum = stageInfo.stage;
		if(m_didDecide != null){
			m_didDecide(m_info);
		}
	}


    // サブクエスト幕選択画面に切り替え.
	private void ChangeViewSubActSelect(MainQuestBootEnum boot, Action<IViewMainQuest> didCreate)
    {
        if(m_info.IsMain){
            return;
        }
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_SubQuest_StageSelect", m_rootObj);
        var c = go.GetOrAddComponent<View_SubQuest_ActSelect>();
		c.Init(boot, b => ChangeView(b));
		didCreate(c);
    }
    // サブクエストシーン選択画面に切り替え.
	private void ChangeViewSubSceneSelect(MainQuestBootEnum boot, Action<IViewMainQuest> didCreate)
    {
        if(m_info.IsMain || m_info.StageNum <= 0){
            Debug.LogError("is main?="+m_info.IsMain+"/act="+m_info.StageNum);
            return;
        }
        var go = GameObjectEx.LoadAndCreateObject("MainQuest/View_SubQuest_QuestSelect", m_rootObj);
        var c = go.GetOrAddComponent<View_SubQuest_SceneSelect>();
		c.Init(boot, m_info.StageNum, b => ChangeView(b));
		didCreate(c);
    }

    #endregion

	// 通信リクエスト : 国単位のクエスト情報.
	private void RequestByCountryInfo(Belonging belonging, Action didEnd)
	{
		View_FadePanel.SharedInstance.IsLightLoading = true;
		SendAPI.QuestsGetMainAchievementByCountry((int)belonging.Enum, (bSuccess, res) => {
            if (!bSuccess || res == null) {
				PopupManager.OpenPopupOK("エラーが発生しました。\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());
                return;
            }
            res.QuestAchievementList.CacheSet();
            // クリアしているものがあったら止める
            if(AwsModule.ProgressData.IsFirstBoot && res.QuestAchievementList.Any(x => x.IsAchieved)) {
                AwsModule.ProgressData.IsFirstBoot = false;
                // シンクを投げておく
                AwsModule.ProgressData.Sync();
            }
			didEnd();
			View_FadePanel.SharedInstance.IsLightLoading = false; 
        });
	}

    private GameObject m_rootObj;
    private IViewMainQuest m_currentView;
    private Action<QuestDecideInfo, Action> m_willDecide;
    private Action<QuestDecideInfo> m_didDecide;
    private QuestDecideInfo m_info;
	private MainQuestCountryData[] m_countryDatas;
}

/// <summary>
/// struct : メインクエストの選択決定情報.
/// </summary>
public struct QuestDecideInfo
{
    /// <summary>クエストタイプ.メインかサブか</summary>
    public bool IsMain;
    /// <summary>国.</summary>
    public Belonging Belonging;
    /// <summary>チャプター番号.</summary>
    public int ChapterNum;
    /// <summary>幕番号.</summary>
    public int StageNum;
    /// <summary>クエスト番号.</summary>
    public int QuestNum;
}

/// <summary>
/// enum : メインクエスト画面のブートモード一覧列挙.
/// </summary>
public enum MainQuestBootEnum
{
    Country,    // 国選択
    Chapter,    // 章選択
    Stage,      // ステージ選択

    SubAct,     // サブクエスト_幕
    SubScene,   // サブクエスト_シーン
}
