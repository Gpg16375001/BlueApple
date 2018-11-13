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
/// ListItem: ミッションリスト.
/// </summary>
public class ListItem_Mission : ViewBase
{
	/// <summary>
    /// 初期化.
    /// </summary>
	public void Init(Action<List<int>> didOk)
	{
		m_didOk = didOk;
        this.SetCanvasCustomButtonMsg("bt_CommonS", DidTapGoTo);
		this.SetCanvasCustomButtonMsg("bt_CommonS02", DidTapGet);      
	}

    /// <summary>
	/// TODO : 情報設定.
    /// </summary>
	public void SetInfo(MissionSetting info, MissionAchievement achievement)
	{
		m_info = info;
		m_achivement = achievement;      

        // ラベル類.
		this.GetScript<TextMeshProUGUI>("txtp_Num").text = m_info.reward_item_count.ToString();
		this.GetScript<TextMeshProUGUI>("txtp_PresentName").text = m_info.GetText();
		this.GetScript<TextMeshProUGUI>("txtp_Total").text = m_info.NeedCount().ToString();
		this.GetScript<TextMeshProUGUI>("txtp_CurrentNum").text = m_achivement.ClearCount.ToString();
		this.GetScript<Image>("img_ProgressBar").fillAmount = ((float)m_achivement.ClearCount / (float)m_info.NeedCount());
 
        foreach (Transform child in GetScript<Transform>("ItemIcon")){
            Destroy(child.gameObject);
        }
        GetScript<Transform>("ItemIcon").DetachChildren();
        this.GetScript<Transform>("Item").gameObject.SetActive(true);    // sprite設定ルート

        // アイコン.
        var iconInfo = m_info.reward_item_type.Enum.GetIconInfo(m_info.reward_item_id, true);
        if (iconInfo.IsEnableSprite){
            iconInfo.IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_MaterialIcon");
            iconInfo.IconObject.GetOrAddComponent<ListItem_MaterialIcon>().GetScript<Image>("img_Material").gameObject.SetActive(false);
            var sprite = iconInfo.IconObject.GetOrAddComponent<ListItem_MaterialIcon>().GetScript<uGUISprite>("MaterialIcon");
            sprite.LoadAtlasFromResources(iconInfo.AtlasName, iconInfo.SpriteName);
        }
        iconInfo.IconObject.transform.SetParent(this.GetScript<Transform>("ItemIcon"), false);

        // 今すぐ行く or 受け取る.
        var jumpProc = this.GetGoToSceneProc();
		this.GetScript<RectTransform>("MissionClear").gameObject.SetActive(m_achivement.IsAchieved);
		this.GetScript<RectTransform>("Get").gameObject.SetActive(m_achivement.IsAchieved);
		this.GetScript<RectTransform>("Jump").gameObject.SetActive(!m_achivement.IsAchieved && jumpProc != null);      
	}

	#region ButtonDelegate.

	// ボタン：今すぐいく.
	void DidTapGoTo()
	{
		var jumpProc = this.GetGoToSceneProc();
		if(jumpProc == null){
			Debug.LogError("今すぐ行くシーンが設定されていません。シーン名="+m_info.jump_scene_name);
			return;
		}
		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, jumpProc);
	}

	// ボタン: 受け取る.
    void DidTapGet()
	{
		var missions = new int[] { m_info.id };
		View_MissionItemGetPop.Create(missions.Select(id => MasterDataTable.mission_setting[id]).ToList(), () => { 
			View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.MissionsReceiveItem(missions, (bSuccess, res) => {
                if (!bSuccess || res == null) {
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    LockInputManager.SharedInstance.IsLock = false;
                    return;
                }
				if(m_didOk != null){
					m_didOk(missions.ToList());
				}
				this.UpdateCache(res);
				AwsModule.UserData.UserData = res.UserData;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
				PopupManager.OpenPopupOK("アイテムを受け取りました。");
            });
		});
	}

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

    #endregion

	// シーン遷移ロジック取得.
	private Action GetGoToSceneProc()
	{
		switch (m_info.jump_scene_name) {
            case "MainQuest":
				return () => ScreenChanger.SharedInstance.GoToMainQuestSelect();
			case "Weapon":
				return () => ScreenChanger.SharedInstance.GoToWeapon();
			case "UnitList":
                return () => ScreenChanger.SharedInstance.GoToUnitList();
			case "PVP":
                return () => {
                    if(AwsModule.PartyData.PvPTeam.IsEmpty) {
                        // PVPチーム編成に飛ばす
                        ScreenChanger.SharedInstance.GoToPVPPartyEdit(true, false, null, () => {
                            PopupManager.OpenPopupOK ("PvPチームが編成されていません。編成を行ってください。");
                        });
                    } else {
                        ScreenChanger.SharedInstance.GoToPVP();
                    }
                };           
        }
		return null;
	}

	private Action<List<int>> m_didOk;
	private MissionSetting m_info;
	private MissionAchievement m_achivement;
}
