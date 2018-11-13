using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;


/// <summary>
/// Screen : ミッション.
/// </summary>
public class Screen_Mission : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MissionAchievement[] missionAchievements)
	{
		m_missionAchievements = missionAchievements;
		m_missionListPrefab = Resources.Load("Mission/ListItem_Mission") as GameObject;

		// 司書配置.
		this.RequestLibrarianModel();

		// グローバルメニューイベント.
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidTapBackButton += DidTapBack;

		// イベントいる.
		var categories = MasterDataTable.mission_category.DataList;
		var root = this.GetScript<HorizontalLayoutGroup>("TabGrid");
		foreach(var c in categories){
			var go = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_HorizontalTextTab", root.gameObject);
			var tab = go.GetOrAddComponent<ListItem_HorizontalTextTab>();
			tab.Init(c.name, DidTapTab);
		}      

		// ボタン.      
		this.SetCanvasCustomButtonMsg("GetAll/bt_CommonS02", DidTapAllGet);
  
        // 初回リスト更新.
		this.DidTapTab(root.GetComponentsInChildren<ListItem_HorizontalTextTab>().First());

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
	}   
	// 司書キャラモデルリクエスト.
	private void RequestLibrarianModel()
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        this.GetScript<RectTransform>("CharacterAnchor").gameObject.DestroyChildren();
		var loader = new UnitResourceLoader(308001011); // 308001011=司書
        loader.LoadResource(resouce => {
            var go = Instantiate(resouce.Live2DModel) as GameObject;
            go.transform.SetParent(this.GetScript<RectTransform>("CharacterAnchor"));
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            View_FadePanel.SharedInstance.IsLightLoading = false;
        });
    }

    // ミッションリスト更新作成.
    private void UpdateMissionList(string category, List<int> alreadyAchiveIdList = null)
    {
        if (alreadyAchiveIdList != null && alreadyAchiveIdList.Count > 0) {
            m_missionAchievements = m_missionAchievements.Select(a => { a.IsReceived |= alreadyAchiveIdList.Contains(a.MissionId); return a; }).ToArray();
        }
        var categoryInfo = MasterDataTable.mission_category.DataList.Find(c => c.name == category);
        m_list = MasterDataTable.mission_setting.DataList
                                .Where(set => !m_missionAchievements.First(a => a.MissionId == set.id).IsReceived && set.category.Enum == categoryInfo.Enum)
                                .ToList();
        this.GetScript<RectTransform>("NoItem").gameObject.SetActive(m_list.Count <= 0);
        var grid = this.GetScript<InfiniteGridLayoutGroup>("MissionItemGrid");
        grid.gameObject.DestroyChildren();
        grid.OnUpdateItemEvent.RemoveAllListeners();
        if (m_list.Count > 0) {
            m_list.Sort((x, y) => {
                if (m_missionAchievements.First(a => a.MissionId == x.id).IsAchieved && !m_missionAchievements.First(a => a.MissionId == y.id).IsAchieved) {
                    return -1;
                }
                if (!m_missionAchievements.First(a => a.MissionId == x.id).IsAchieved && m_missionAchievements.First(a => a.MissionId == y.id).IsAchieved) {
                    return 1;
                }
                return x.id - y.id;
            });
            grid.OnUpdateItemEvent.AddListener(CallbackUpdateListItem);
            grid.Initialize(m_missionListPrefab, 5, m_list.Count, false);
        } else {
            grid.Initialize(m_missionListPrefab, 1, 1, false);
            foreach (Transform child in grid.transform){
                child.gameObject.SetActive(false);
            }
        }

        // タブ上の受け取れる数表示更新.
        var root = this.GetScript<HorizontalLayoutGroup>("TabGrid");
        foreach (var tab in root.GetComponentsInChildren<ListItem_HorizontalTextTab>()) {
            var cnt = m_missionAchievements.Where(a => MasterDataTable.mission_category[a.MissionCategory].name == tab.CategoryName).Count(a => !a.IsReceived && a.IsAchieved);
            if (cnt > 0) {
                tab.SetBadge(cnt < 100 ? cnt.ToString() : "99+");
            } else {
                tab.SetBadge("");
            }
        }

        m_currentCategoryName = category;
        var info = MasterDataTable.mission_category.DataList.Find(c => c.name == m_currentCategoryName);
        this.GetScript<CustomButton>("GetAll/bt_CommonS02").interactable = m_missionAchievements.Any(a => !a.IsReceived && a.IsAchieved && a.MissionCategory == info.index);

        grid.ResetScrollPosition();
	}
	void CallbackUpdateListItem(int index, GameObject go)
	{
		var achive = Array.Find(m_missionAchievements, a => a.MissionId == m_list[index].id);
		var list = go.GetComponent<ListItem_Mission>();
		if(list == null){
			list = go.AddComponent<ListItem_Mission>();
			list.Init(achiveIdList => UpdateMissionList(m_currentCategoryName, achiveIdList));
		}
		list.SetInfo(m_list[index], achive);
	}
	private List<MissionSetting> m_list;

	#region ButtonDelegate.

	// ボタン: 戻る.
    void DidTapBack()
	{
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => ScreenChanger.SharedInstance.GoToMyPage());
	}

	// ボタン: タブタップ.
	void DidTapTab(ListItem_HorizontalTextTab tab)
	{
		// 表示情報修正.
		var categoryList = MasterDataTable.mission_category.DataList;
		if(tab.CategoryName == categoryList.Find(c => c.Enum == MissionCategoryEnum.Daily).name){
			this.SetActiveRemainTimeLabel(true);
			this.GetScript<TextMeshProUGUI>("txtp_TimeLimit").text = m_remainTodayHours.ToString()+"時間";
		}else if(tab.CategoryName == categoryList.Find(c => c.Enum == MissionCategoryEnum.Normal).name){
			this.SetActiveRemainTimeLabel(false);
		}else if(tab.CategoryName == categoryList.Find(c => c.Enum == MissionCategoryEnum.Event).name) {
			// TODO : イベントはどうするか要検討.
			this.SetActiveRemainTimeLabel(false);
		}
		// タブ選択状態.
		var root = this.GetScript<HorizontalLayoutGroup>("TabGrid");
		root.GetComponentsInChildren<ListItem_HorizontalTextTab>()
		    .Select(t => t.IsSelected = t.CategoryName == tab.CategoryName)
		    .ToList();
		// ミッションリスト.
		this.UpdateMissionList(tab.CategoryName);
	}
    private void SetActiveRemainTimeLabel(bool bActive)
	{
		this.GetScript<TextMeshProUGUI>("txtp_DailyMissionNotes").gameObject.SetActive(bActive);
		this.GetScript<TextMeshProUGUI>("txtp_TimeLimitTitle").gameObject.SetActive(bActive);
		this.GetScript<TextMeshProUGUI>("txtp_TimeLimit").gameObject.SetActive(bActive);
	}

	// ボタン: まとめて受け取る.
    void DidTapAllGet()
	{
		var category = MasterDataTable.mission_category.DataList.Find(c => c.name == m_currentCategoryName);
		var missions = m_missionAchievements.Where(a => !a.IsReceived && a.IsAchieved && a.MissionCategory == category.index).Select(a => a.MissionId).ToArray();
		View_MissionItemGetPop.Create(missions.Select(id => MasterDataTable.mission_setting[id]).ToList(), () => { 
			View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
			SendAPI.MissionsReceiveItem(missions, (bSuccess, res) => {
                if (!bSuccess || res == null) {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    return;
                }
				this.UpdateMissionList(m_currentCategoryName, missions.ToList());
				this.UpdateCache(res);
                AwsModule.UserData.UserData = res.UserData;
                PopupManager.OpenPopupOK("アイテムをまとめて受け取りました。");
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
            });
		});
	}

    #endregion

    // キャッシュ更新処理.
	private void UpdateCache(ReceiveMissionsReceiveItem res)
	{
		if(res.CardDataList != null){
			res.CardDataList.CacheSet();
		}
		if(res.ConsumerDataList != null){
			res.ConsumerDataList.CacheSet();         
		}
		if(res.MagikiteDataList != null){
			res.MagikiteDataList.CacheSet();
		}
		if(res.MaterialDataList != null){
			res.MaterialDataList.CacheSet();
		}
		if(res.WeaponDataList != null){
			res.WeaponDataList.CacheSet();
		}
	}

	void Awake()
	{
		this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
	}

	void OnEnable()
	{
		if(m_coUpdateRemainTime != null){
			this.StopCoroutine(m_coUpdateRemainTime);
		}
		m_coUpdateRemainTime = this.StartCoroutine(this.CoUpdateRemainTime());      
	}
	IEnumerator CoUpdateRemainTime()
	{
		do{
			var tommorow = GameTime.SharedInstance.Today.AddDays(1);
			m_remainTodayHours = (tommorow - GameTime.SharedInstance.Now).Hours;
			yield return new WaitForSeconds(0.6f);         
		}while(true);
	}

	private string m_currentCategoryName;
	private Coroutine m_coUpdateRemainTime;   
	private int m_remainTodayHours;
	private GameObject m_missionListPrefab;
	private MissionAchievement[] m_missionAchievements;
}