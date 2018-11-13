using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// View : メインクエスト画面の章選択.
/// </summary>
public class View_MainQuest_ChapterSelect : ViewBase, IViewMainQuest
{
    /// <summary>
    /// 起動画面.
    /// </summary>
    public MainQuestBootEnum Boot { get { return m_boot; } }
    private MainQuestBootEnum m_boot;

    /// <summary>
    /// 章番号.
    /// </summary>
    public int ChapterNum { get; private set; }

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(MainQuestBootEnum boot, Belonging belonging, Action<MainQuestBootEnum> procChangeView)
    {      
        m_boot = boot;
        m_belonging = belonging;
        m_procChangeView = procChangeView;
		this.CreateList();
		Invoke("LoadLive2D", 0.001f);   // ローディングフェードが前処理と被り正しく表示がされないことがあるため少し遅らせる.

        // 初回起動.
		this.StartCoroutine(this.PlayAndWaitSlideInForTutorial());      

		// ラベル.
		this.GetScript<TextMeshProUGUI>("txtp_SelectCountry").text = belonging.name;
		this.GetScript<uGUISprite>("EmblemIcon").ChangeSprite(((int)belonging.Enum).ToString());

        // ボタン.
		this.SetCanvasCustomButtonMsg("bt_BackCountrySelect", DidTapClose);
        this.SetCanvasButtonMsg("bt_BackList", DidTapBack);
        this.SetCanvasButtonMsg("bt_CountryInfo", DidTapCountryDetail);
		View_PlayerMenu.DidTapBackButton += DidTapBack;
    }

	private IEnumerator PlayAndWaitSlideInForTutorial()
	{
		var anim = this.GetScript<Animation>("AnimParts");
        anim.Play("MainQuestSlideIn");
		if (!AwsModule.ProgressData.IsFirstBoot) {
			yield break;
        }      
		LockInputManager.SharedInstance.IsLock = true;
        do {
            yield return null;
        } while (anim.isPlaying);
		TutorialFirstBootModule.CreateIfMissing(TutorialFirstBootModule.ViewMode.ChapterSelect, this, View_GlobalMenu.CreateIfMissing(), View_PlayerMenu.CreateIfMissing());
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
		View_PlayerMenu.DidTapBackButton -= DidTapBack;
		base.Dispose();
	}

	// 章一覧リスト作成.
	private void CreateList()
    {
		var root = this.GetScript<ScrollRect>("ChapterScrollView").content.gameObject;
        root.DestroyChildren();
		var chapterList = MasterDataTable.quest_main_chapter_info.GetListThisCountry(m_belonging);
		chapterList.Sort((x, y) => y.id - x.id);
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(m_belonging);
		foreach (var info in chapterList) { 
			var bReleaseQuest = AwsModule.ProgressData.IsReleaseMainQuest(m_belonging, info.chapter);
			if (!bReleaseQuest && (latest == null || info.chapter > latest.ChapterNum)) {
				continue;
            }
			var list = MasterDataTable.quest_main.DataList.FindAll(q => q.Country.Enum == m_belonging.Enum && q.ChapterNum == info.chapter);
			var firstQuestNum = list.Select(i => i.QuestNum).Min();
			var firstQuest = list.Find(i => i.QuestNum == firstQuestNum);
			var go = GameObjectEx.LoadAndCreateObject("MainQuest/ListItem_MainQuest_Chapter", root);
            var c = go.GetOrAddComponent<ListItem_MainQuest>();
            c.InitChapter(info, DidTapChapter);
			c.IsEnableButton = !firstQuest.is_force_lock;
			c.GetScript<TextMeshProUGUI>("txtp_Other").gameObject.SetActive(!firstQuest.is_force_lock);
			c.GetScript<Image>("img_StoryIconNew").gameObject.SetActive(c.GetScript<Image>("img_StoryIconNew").gameObject.activeSelf && !firstQuest.is_force_lock);
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockChapterNote").gameObject.SetActive(firstQuest.is_force_lock);
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockChapterNote").enabled = c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockChapterNote").interactable = firstQuest.is_force_lock;
			c.GetScript<SmileLab.UI.CustomButton>("bt_ForceLockChapterNote").onClick.RemoveAllListeners();
            if (firstQuest.is_force_lock) {
				c.SetCanvasCustomButtonMsg("bt_ForceLockChapterNote", () => PopupManager.OpenPopupOK("新章はまだ解放されておりません。\nアップデートされるまで\nお待ちください。"));
            }
		}
    }

    // 適宜対応するLive2Dモデルのロード.心情も設定する.
    private void LoadLive2D()
	{
		// 章ごとのモデルと心情を表示.ロードを挟む.
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(m_belonging);
        this.GetScript<TextMeshProUGUI>("txtp_ScenarioOutline").text = latest.stage_info.chapter_info.feeling_text;    // 心情.
        this.GetScript<RectTransform>("CharacterAnchor").gameObject.DestroyChildren();
        View_FadePanel.SharedInstance.IsLightLoading = true;
        var loader = new UnitResourceLoader(latest.stage_info.chapter_info.feeling_unit_id);
        loader.LoadResource(resouce => {
            var go = Instantiate(resouce.Live2DModel) as GameObject;
            go.transform.SetParent(this.GetScript<RectTransform>("CharacterAnchor"));
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            View_FadePanel.SharedInstance.IsLightLoading = false;
            this.GetScript<RectTransform>("ScenarioInfo").gameObject.SetActive(true);
        });
	}

    #region ButtonDelegate.

    // ボタン : 閉じるボタン.
    void DidTapClose()
    {
		this.PlaySlideOut(() => m_procChangeView(MainQuestBootEnum.Country));
    }

    // ボタン : 戻るボタン.
    void DidTapBack()
    {
		this.PlaySlideOut(() => m_procChangeView(MainQuestBootEnum.Country));
    }

    // ボタン : 章.
	void DidTapChapter(MainQuestChapterInfo chapterInfo)
    {
		if (AwsModule.ProgressData.IsFirstBoot) {
			TutorialFirstBootModule.DestroyInstance();
        }      
		
        this.PlaySlideOut(() => {
			this.ChapterNum = chapterInfo.chapter;
            m_procChangeView(MainQuestBootEnum.Stage);
        });
    }

    // ボタン : 国詳細再表示
    void DidTapCountryDetail()
    {
		this.PlaySlideOut(() => View_CountryInfoPop.Create(m_belonging));
        
    }
    #endregion

	private void PlaySlideOut(Action didEnd)
	{
		this.StartCoroutine(this.CoPlaySlideOut(didEnd));
	}
	private IEnumerator CoPlaySlideOut(Action didEnd)
	{
		LockInputManager.SharedInstance.IsLock = true;
		var anim = this.GetScript<Animation>("AnimParts");
		anim.Play("MainQuestSlideOut");
		do {
			yield return null;
		} while (anim.isPlaying);
		if(didEnd != null){
			didEnd();
		}
		LockInputManager.SharedInstance.IsLock = false;
	}

    private Belonging m_belonging;
    Action<MainQuestBootEnum> m_procChangeView;
}
