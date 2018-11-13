using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;


/// <summary>
/// View : メインクエスト画面の国選択.
/// </summary>
public class View_MainQuest_CountrySelect : ViewBase, IViewMainQuest
{
    /// <summary>
    /// 起動画面.
    /// </summary>
    public MainQuestBootEnum Boot { get { return m_boot; } }
    private MainQuestBootEnum m_boot;

    /// <summary>
    /// 決定した国.
    /// </summary>
    public Belonging DecideBelonging { get; private set; } 


    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MainQuestBootEnum boot, MainQuestCountryData[] countryDatas, Action<MainQuestBootEnum> procChangeView)
    {
        m_boot = boot;
		m_achiveDatas = countryDatas;
        m_procChangeView = procChangeView;
        this.CreateList();

		View_PlayerMenu.DidTapBackButton += BackToMyPage;

		// 初回起動
		this.StartCoroutine(this.PlayAndWaitSlideInForTutorial());

		// TODO : サブシナリオはローンチには出さない想定.
		//this.SetCanvasCustomButtonMsg("bt_SubScenarioSelect", DidTapSubQuest);
		//this.GetScript<CustomButton>("bt_SubScenarioSelect").gameObject.SetActive(AwsModule.ProgressData.ReleaseSubQuestIdList.Count > 0);
    }

	private IEnumerator PlayAndWaitSlideInForTutorial()
    {
        var anim = this.GetScript<Animation>("Root");
        anim.Play("MainQuestCountryIn");
        if (!AwsModule.ProgressData.IsFirstBoot) {
            yield break;
        }
		LockInputManager.SharedInstance.IsLock = true;
        do {
            yield return null;
        } while (anim.isPlaying);
		TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.CountrySelect, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
        LockInputManager.SharedInstance.IsLock = false;
    }

    /// <summary>
    /// 破棄処理.
    /// </summary>
    public void Destroy()
    {
        this.Dispose();
    }
	public override void Dispose()
	{
		View_PlayerMenu.DidTapBackButton -= BackToMyPage;
		base.Dispose();
	}

	// 国一覧リスト作成.
	private void CreateList()
    {
		var valm = MasterDataTable.belonging.DataList.Find(b => b.Enum == BelongingEnum.Varm);
		var achiveList = QuestAchievement.CacheGetAll().Where(a => a.IsAchieved).ToList();
		var belongList = MasterDataTable.belonging.DataList.FindAll(b => b.name != "不明");
		belongList.Sort((x, y) => x.priority_view - y.priority_view);
        foreach (var beloginig in belongList) {
			var questList = MasterDataTable.quest_main.DataList.FindAll(q => q.country == beloginig.name);
			var bReleaseExists = achiveList.Exists(a => questList.Exists(q => q.id == a.QuestId));
			var go = this.GetScript<Transform>(string.Format("Country{0}", (int)beloginig.Enum)).gameObject;
			var c = go.GetOrAddComponent<ListItem_MainQuest>();
			var achiveInfo = Array.Find(m_achiveDatas, a => a.MainQuestCountry == (int)beloginig.Enum);
			c.InitCountry(beloginig, achiveInfo.IsOpen, achiveInfo.IsNew, achiveInfo.IsClear, DidTapCountry);
			c.IsEnableButton = achiveInfo.IsOpen;
			c.GetScript<CustomButton>("bt_NotOpenNote").gameObject.SetActive(!achiveInfo.IsOpen);
			if(!achiveInfo.IsOpen){
				c.SetCanvasCustomButtonMsg("bt_NotOpenNote", () => PopupManager.OpenPopupOK("この国は解放されていません。\n" + valm.name + "のクエストを\n進めましょう。"));
			}         
        }
    }

    #region ButtonDelegate.

    // ボタン : 国.
    void DidTapCountry(Belonging belonging)
    {
        if (AwsModule.ProgressData.IsFirstBoot) {
			TutorialFirstBootModule.DestroyInstance();
        }
		
        DecideBelonging = belonging;
        m_procChangeView(MainQuestBootEnum.Chapter);
    }

    // ボタン : サブクエストタップ.
    void DidTapSubQuest()
    {
		m_procChangeView(MainQuestBootEnum.SubAct);
    }

    #endregion

	// マイページに戻る.
    void BackToMyPage()
    {
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
    }

    private Action<MainQuestBootEnum> m_procChangeView;
	private MainQuestCountryData[] m_achiveDatas;
}
