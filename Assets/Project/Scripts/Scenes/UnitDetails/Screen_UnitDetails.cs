using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;
using Live2D.Cubism.Rendering;

/// <summary>
/// Screen : ユニット詳細画面.
/// </summary>
public class Screen_UnitDetails : ViewBase
{
    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(CardData displayCard, bool dispGlobalMenu, bool supportCard, System.Action backSceneCallback, UnitResourceLoader unitResource, WeaponResourceLoader weaponResource)
    {
		m_displayCard = displayCard;
		m_unitResource = unitResource;
        m_BackSceneCallback = backSceneCallback;
        m_DispGlobalMenu = dispGlobalMenu;
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButon;

        // ユニットカードの表示
        var unitRoot = this.GetScript<RectTransform> ("CardAnchor");
        var unitCard = GameObjectEx.LoadAndCreateObject ("_Common/View/UnitCard", unitRoot.gameObject);
		unitCard.GetComponent<UnitCard> ().Init(m_displayCard, null, unitResource);

        SetInfo (displayCard);

        if (!supportCard) {
            SetProfile (displayCard, unitResource, weaponResource);
        }

		//
        this.SetCanvasCustomButtonMsg("GoProfile/bt_Tab2Off", DidTapGoProfile, !supportCard);
		this.SetCanvasCustomButtonMsg("GoStatus/bt_Tab2Off", DidTapGoStatus);
        this.SetCanvasCustomButtonMsg("Reinforcement/bt_Common", DidTapReinforcement, !displayCard.IsMaxLevel && !supportCard); // 強化.
        this.SetCanvasCustomButtonMsg("Evolution/bt_Common", DidTapEvolution, !displayCard.IsMaxRarity && !supportCard);        // 進化.
        this.SetCanvasCustomButtonMsg("Equip/bt_Sub", DidTapEquip, !supportCard);                                               // 装備変更.
        this.SetCanvasCustomButtonMsg("TrainingBoard/bt_Common", DidTrainingBoard, !supportCard);                               // 育成ボード    

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }

    public void Init(SupporterCardData displayCard, bool dispGlobalMenu, bool supportCard, System.Action backSceneCallback, UnitResourceLoader unitResource, WeaponResourceLoader weaponResource)
    {
        m_displayCard = null;
        m_unitResource = unitResource;
        m_BackSceneCallback = backSceneCallback;
        m_DispGlobalMenu = dispGlobalMenu;

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButon;

        // ユニットカードの表示
        var unitRoot = this.GetScript<RectTransform> ("CardAnchor");
        var unitCard = GameObjectEx.LoadAndCreateObject ("_Common/View/UnitCard", unitRoot.gameObject);
        unitCard.GetComponent<UnitCard> ().Init(displayCard.ConvertCardData(), null, unitResource);

        SetInfo (displayCard);

        //
        this.SetCanvasCustomButtonMsg("GoProfile/bt_Tab2Off", DidTapGoProfile, false);
        this.SetCanvasCustomButtonMsg("GoStatus/bt_Tab2Off", DidTapGoStatus);
        this.SetCanvasCustomButtonMsg("Reinforcement/bt_Common", DidTapReinforcement, false);   // 強化.
        this.SetCanvasCustomButtonMsg("Evolution/bt_Common", DidTapEvolution, false);           // 進化.
        this.SetCanvasCustomButtonMsg("Equip/bt_Sub", DidTapEquip, false);                      // 装備変更.
        this.SetCanvasCustomButtonMsg("TrainingBoard/bt_Common", DidTrainingBoard, false);      // 育成ボード    

        View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
    }

    void SetInfo(CardData displayCard)
    {
        // レアリティの設定
		int maxRarity = displayCard.MaxRarity;
        int nowRarity = displayCard.Rarity;
        for(int i = 1; i <= 6; ++i)
        {
            var starObj = this.GetScript<Transform> (string.Format ("StarGrid/Star{0}", i));
            if (maxRarity >= i) {
                starObj.gameObject.SetActive (true);
                var starOn = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOn", i));
                var starOff = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOff", i));
                if (nowRarity >= i) {
                    starOn.gameObject.SetActive (true);
                    starOff.gameObject.SetActive (false);
                } else {
                    starOn.gameObject.SetActive (false);
                    starOff.gameObject.SetActive (true);
                }
            } else {
                starObj.gameObject.SetActive (false);
            }
        }

        // 属性
        this.GetScript<Image>("ElementIcon").sprite = IconLoader.LoadElementIcon(displayCard.Parameter.Element);

        // 国旗
		this.GetScript<Image>("EmblemIcon").sprite = IconLoader.LoadEmblem(displayCard.Card.country);

        // ステータスの設定
		this.GetScript<TextMeshProUGUI> ("UnitInfo/txtp_UnitName").SetText (displayCard.Card.nickname);

		this.GetScript<TextMeshProUGUI> ("txtp_UnitCountry").SetText (displayCard.Card.country.name);

		this.GetScript<TextMeshProUGUI>("txtp_UnitLv").SetText(displayCard.Level);
		this.GetScript<TextMeshProUGUI>("txtp_UnitLvLimit").SetTextFormat("/{0}", displayCard.MaxLevel);
  
		if (displayCard.IsMaxLevel) {
            this.GetScript<TextMeshProUGUI> ("txtp_LvPoint").SetText ("");
			this.GetScript<Image>("txtp_LvPoint/img_LvMax").gameObject.SetActive(true);
			this.GetScript<Image>("txtp_LvPoint/img_Next").gameObject.SetActive(false);
			this.GetScript<Image> ("LvGauge/img_CommonGauge").fillAmount = 1.0f;
        } else {
			this.GetScript<TextMeshProUGUI> ("txtp_LvPoint").SetTextFormat ("{0} / {1}", displayCard.CurrentLevelExp, displayCard.NextLevelExp);
			this.GetScript<Image>("txtp_LvPoint/img_LvMax").gameObject.SetActive(false);
            this.GetScript<Image>("txtp_LvPoint/img_Next").gameObject.SetActive(true);
			this.GetScript<Image>("LvGauge/img_CommonGauge").fillAmount = displayCard.CurrentLevelProgress;;
        }

        this.GetScript<TextMeshProUGUI> ("txtp_UnitHP").SetText (displayCard.Parameter.MaxHp);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitATK").SetText (displayCard.Parameter.Attack);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitDEF").SetText (displayCard.Parameter.Defense);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitSPD").SetText (displayCard.Parameter.Agility);

		// 限界突破
		for (var i = 0; i < 4; ++i){
			var symbolName = string.Format("SelectWeaponSymbol{0}", i+1);
			this.GetScript<Image>(symbolName+"/LimitBreakIconOn").gameObject.SetActive(displayCard.LimitBreakGrade > i);
            this.GetScript<Image>(symbolName+"/LimitBreakIconOff").gameObject.SetActive(displayCard.LimitBreakGrade <= i);
		}

        // 武具の設定
        if (displayCard.EquippedWeaponBagId == 0 || displayCard.Weapon == null) {
			this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(true);
			this.GetScript<RectTransform>("WeaponIconRoot").gameObject.SetActive(false);
        } else {
			this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(false);
			this.GetScript<RectTransform>("WeaponIconRoot").gameObject.SetActive(true);
			var wRoot = this.GetScript<RectTransform>("WeaponIconRoot").gameObject;
			var obj = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", wRoot);
            var c = obj.GetOrAddComponent<ListItem_WeaponIcon>();
			c.Init(displayCard.Weapon, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
        }

        // スキル名の設定
        var actionSkill = displayCard.Parameter.UnitActionSkillList.FirstOrDefault(x => !x.IsNormalAction);
        if (actionSkill != null) {
            this.GetScript<TextMeshProUGUI> ("Skill/txtp_SkillName").SetText (actionSkill.Skill.display_name);
			this.GetScript<TextMeshProUGUI>("Skill/txtp_SkillLv").SetText(actionSkill.Level);
			this.GetScript<CustomButton>("Skill/bt_Info").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("Skill/bt_Info", () => PopupManager.OpenPopupOK(actionSkill.Skill.flavor));
        } else {
			this.GetScript<TextMeshProUGUI> ("Skill/txtp_SkillName").SetText ("なし");
			this.GetScript<TextMeshProUGUI>("Skill/txtp_SkillLv").SetText("-");
        }
		// パッシブスキル名の設定
		var passiveSkill = displayCard.Parameter.UnitPassiveSkillList.FirstOrDefault();
		if (passiveSkill != null) {
			this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillName").SetText(passiveSkill.Skill.display_name);
			this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillLv").SetText(passiveSkill.Level);
			this.GetScript<CustomButton>("Skill2/bt_Info").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("Skill2/bt_Info", () => PopupManager.OpenPopupOK(passiveSkill.Skill.flavor));
        } else {
			this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillName").SetText("なし");
			this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillLv").SetText("-");
        }

        if (displayCard.Parameter.SpecialSkill != null) {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText (displayCard.Parameter.SpecialSkill.Skill.display_name);
			this.GetScript<TextMeshProUGUI>("SP/txtp_SkillLv").SetText(displayCard.Parameter.SpecialSkill.Level);
			this.GetScript<CustomButton>("SP/bt_Info").onClick.RemoveAllListeners();
			this.SetCanvasCustomButtonMsg("SP/bt_Info", () => PopupManager.OpenPopupOK(displayCard.Parameter.SpecialSkill.Skill.flavor));
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText ("なし");
			this.GetScript<TextMeshProUGUI>("SP/txtp_SkillLv").SetText("-");
        }
    }

    void SetInfo(SupporterCardData displayCard)
    {
        // レアリティの設定
        int maxRarity = displayCard.MaxRarity;
        int nowRarity = displayCard.Rarity;
        for(int i = 1; i <= 6; ++i)
        {
            var starObj = this.GetScript<Transform> (string.Format ("StarGrid/Star{0}", i));
            if (maxRarity >= i) {
                starObj.gameObject.SetActive (true);
                var starOn = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOn", i));
                var starOff = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOff", i));
                if (nowRarity >= i) {
                    starOn.gameObject.SetActive (true);
                    starOff.gameObject.SetActive (false);
                } else {
                    starOn.gameObject.SetActive (false);
                    starOff.gameObject.SetActive (true);
                }
            } else {
                starObj.gameObject.SetActive (false);
            }
        }

        // 属性
        this.GetScript<Image>("ElementIcon").sprite = IconLoader.LoadElementIcon(displayCard.Parameter.Element);

        // 国旗
        this.GetScript<Image>("EmblemIcon").sprite = IconLoader.LoadEmblem(displayCard.Card.country);

        // ステータスの設定
        this.GetScript<TextMeshProUGUI> ("UnitInfo/txtp_UnitName").SetText (displayCard.Card.nickname);

        this.GetScript<TextMeshProUGUI> ("txtp_UnitCountry").SetText (displayCard.Card.country.name);

        this.GetScript<TextMeshProUGUI>("txtp_UnitLv").SetText(displayCard.Level);
        this.GetScript<TextMeshProUGUI>("txtp_UnitLvLimit").SetTextFormat("/{0}", displayCard.MaxLevel);

        if (displayCard.IsMaxLevel) {
            this.GetScript<TextMeshProUGUI> ("txtp_LvPoint").SetText ("");
            this.GetScript<Image>("txtp_LvPoint/img_LvMax").gameObject.SetActive(true);
            this.GetScript<Image>("txtp_LvPoint/img_Next").gameObject.SetActive(false);
            this.GetScript<Image> ("LvGauge/img_CommonGauge").fillAmount = 1.0f;
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_LvPoint").SetTextFormat ("{0} / {1}", displayCard.CurrentLevelExp, displayCard.NextLevelExp);
            this.GetScript<Image>("txtp_LvPoint/img_LvMax").gameObject.SetActive(false);
            this.GetScript<Image>("txtp_LvPoint/img_Next").gameObject.SetActive(true);
            this.GetScript<Image>("LvGauge/img_CommonGauge").fillAmount = displayCard.CurrentLevelProgress;;
        }

        this.GetScript<TextMeshProUGUI> ("txtp_UnitHP").SetText (displayCard.Parameter.MaxHp);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitATK").SetText (displayCard.Parameter.Attack);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitDEF").SetText (displayCard.Parameter.Defense);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitSPD").SetText (displayCard.Parameter.Agility);

        // 限界突破
        for (var i = 0; i < 4; ++i){
            var symbolName = string.Format("SelectWeaponSymbol{0}", i+1);
            this.GetScript<Image>(symbolName+"/LimitBreakIconOn").gameObject.SetActive(displayCard.LimitBreakGrade > i);
            this.GetScript<Image>(symbolName+"/LimitBreakIconOff").gameObject.SetActive(displayCard.LimitBreakGrade <= i);
        }

        // 武具の設定
        if (displayCard.EquippedWeaponData == null || displayCard.EquippedWeaponData.BagId == 0) {
            this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(true);
            this.GetScript<RectTransform>("WeaponIconRoot").gameObject.SetActive(false);
        } else {
            this.GetScript<TextMeshProUGUI>("txtp_NoWeapon").gameObject.SetActive(false);
            this.GetScript<RectTransform>("WeaponIconRoot").gameObject.SetActive(true);
            var wRoot = this.GetScript<RectTransform>("WeaponIconRoot").gameObject;
            var obj = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_WeaponIcon", wRoot);
            var c = obj.GetOrAddComponent<ListItem_WeaponIcon>();
            c.Init(displayCard.EquippedWeaponData, ListItem_WeaponIcon.DispStatusType.RarityAndElementOnly);
        }

        // スキル名の設定
        var actionSkill = displayCard.Parameter.UnitActionSkillList.FirstOrDefault(x => !x.IsNormalAction);
        if (actionSkill != null) {
            this.GetScript<TextMeshProUGUI> ("Skill/txtp_SkillName").SetText (actionSkill.Skill.display_name);
            this.GetScript<TextMeshProUGUI>("Skill/txtp_SkillLv").SetText(actionSkill.Level);
			this.GetScript<CustomButton>("Skill/bt_Info").onClick.RemoveAllListeners();
            this.SetCanvasCustomButtonMsg("Skill/bt_Info", () => PopupManager.OpenPopupOK(actionSkill.Skill.flavor));
        } else {
            this.GetScript<TextMeshProUGUI> ("Skill/txtp_SkillName").SetText ("なし");
        }
        // パッシブスキル名の設定
        var passiveSkill = displayCard.Parameter.UnitPassiveSkillList.FirstOrDefault();
        if (passiveSkill != null) {
            this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillName").SetText(passiveSkill.Skill.display_name);
            this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillLv").SetText(passiveSkill.Level);
			this.GetScript<CustomButton>("Skill2/bt_Info").onClick.RemoveAllListeners();
            this.SetCanvasCustomButtonMsg("Skill2/bt_Info", () => PopupManager.OpenPopupOK(passiveSkill.Skill.flavor));
        } else {
            this.GetScript<TextMeshProUGUI>("Skill2/txtp_SkillName").SetText("なし");
        }

        if (displayCard.Parameter.SpecialSkill != null) {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText (displayCard.Parameter.SpecialSkill.Skill.display_name);
            this.GetScript<TextMeshProUGUI>("SP/txtp_SkillLv").SetText(displayCard.Parameter.SpecialSkill.Level);
			this.GetScript<CustomButton>("SP/bt_Info").onClick.RemoveAllListeners();
            this.SetCanvasCustomButtonMsg("SP/bt_Info", () => PopupManager.OpenPopupOK(displayCard.Parameter.SpecialSkill.Skill.flavor));
        } else {
            this.GetScript<TextMeshProUGUI> ("txtp_SPName").SetText ("なし");
        }
    }

    void SetProfile(CardData displayCard, UnitResourceLoader unitResource, WeaponResourceLoader weaponResource)
    {
		// Voice
		UpdateVoiceList(displayCard);

        // Profile
        this.GetScript<TextMeshProUGUI>("UnitName/txtp_UnitName").SetText(displayCard.Card.character.name);
		this.GetScript<TextMeshProUGUI>("txtp_CVName").SetText(displayCard.Card.character.cv);      
        this.GetScript<TextMeshProUGUI>("Country/txtp_Country").SetText(displayCard.Card.character.belonging.name);
		this.GetScript<TextMeshProUGUI>("Affiliation/txtp_Affiliation").SetText(displayCard.Card.post);
		this.GetScript<TextMeshProUGUI>("Favorite/txtp_Favorite").SetText(displayCard.Card.like);
		this.GetScript<TextMeshProUGUI>("Dislike/txtp_Dislike").SetText(displayCard.Card.dislike);
		this.GetScript<TextMeshProUGUI>("Hobby/txtp_Hobby").SetText(displayCard.Card.hobby);
        this.GetScript<TextMeshProUGUI>("Contents1/txtp_Profile").SetText(displayCard.Card.flavor_text);

		this.GetScript<TextMeshProUGUI>("txtp_ProfileTitle2").gameObject.SetActive(displayCard.IsReleaseFlavor2);
		this.GetScript<TextMeshProUGUI>("Contents2/txtp_Profile").gameObject.SetActive(displayCard.IsReleaseFlavor2);
		this.GetScript<TextMeshProUGUI>("txtp_LockedProfileTitle2").gameObject.SetActive(!displayCard.IsReleaseFlavor2);
		this.GetScript<TextMeshProUGUI>("Contents2/txtp_UnlockNotes").gameObject.SetActive(!displayCard.IsReleaseFlavor2);      
		this.GetScript<Image>("img_LockContentsIcon2").gameObject.SetActive(!displayCard.IsReleaseFlavor2);
		if(displayCard.IsReleaseFlavor2){
			this.GetScript<TextMeshProUGUI>("Contents2/txtp_Profile").SetText(displayCard.Card.flavor_text2);
		}else{
			var notes = string.Format("以下の条件を達成すると見ることができます。\n・{0}のシナリオを{1}章までクリアする。", displayCard.Card.release_chapter_flavor2.country.name, displayCard.Card.release_chapter_flavor2.chapter);      
			this.GetScript<TextMeshProUGUI>("Contents2/txtp_UnlockNotes").SetText(notes);
		}      
  
		// ボタン.
		this.GetScript<CustomButton>("Profile/bt_Tab2Off").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("Voice/bt_Tab2Off").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("ChangeView/bt_Change").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("bt_FullScreen").onClick.RemoveAllListeners();
		this.GetScript<CustomButton>("Large/LargeImageBG").onClick.RemoveAllListeners();
		this.SetCanvasCustomButtonMsg("Profile/bt_Tab2Off", DidTapTabProfile);
		this.SetCanvasCustomButtonMsg("Voice/bt_Tab2Off", DidTapTabVoice);      
		this.SetCanvasCustomButtonMsg("ChangeView/bt_Change", DidTapRotationView);
		this.SetCanvasCustomButtonMsg("bt_FullScreen", DidTapFullScreen);
		this.SetCanvasCustomButtonMsg("Large/LargeImageBG", DidTapLageImage);

		// SD生成
		var canvas = this.GetScript<Canvas>("Large");
        var spineObject = Instantiate(unitResource.SpineModel);
		var motType = displayCard.Weapon != null ? displayCard.Weapon.Weapon.motion_type : MasterDataTable.weapon[displayCard.Card.initial_weapon_id].motion_type;
        spineObject.AddComponent<SpineModelController> ().Init (
			motType,
            unitResource, 
            weaponResource != null ? weaponResource.WeaponAtlas : null
        );
        var renderer = spineObject.GetComponent<Renderer> ();
        if (renderer != null) {
            renderer.sortingLayerName = "Default";
			renderer.sortingOrder = 1;
        }
        var sdRoot = this.GetScript<RectTransform> ("SD/SDAnchor").gameObject;
        sdRoot.AddInChild (spineObject);

		spineObject = Instantiate(unitResource.SpineModel);
        spineObject.AddComponent<SpineModelController>().Init(
            motType,
            unitResource,
            weaponResource != null ? weaponResource.WeaponAtlas : null
        );
        renderer = spineObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.sortingLayerName = "Default";
			renderer.sortingOrder = canvas.sortingOrder+1;
        }
        sdRoot = this.GetScript<RectTransform>("LargeSD/SDAnchor").gameObject;
        sdRoot.AddInChild(spineObject);
		// Live2D生成.
		var live2d = Instantiate(unitResource.Live2DModel) as GameObject;
		live2d.transform.SetParent(this.GetScript<Transform>("CharacterAnchor"));
        live2d.transform.localScale = Vector3.one;
        live2d.transform.localPosition = Vector3.zero;
        var cubismRender = live2d.GetComponentsInChildren<CubismRenderController>()[0];
        if (cubismRender != null) {
			var rootCanvas = this.GetScript<Canvas>("CharacterAnchor");
            cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
            cubismRender.SortingLayer = rootCanvas.sortingLayerName;
            cubismRender.SortingOrder = rootCanvas.sortingOrder;
        }

		live2d = Instantiate(unitResource.Live2DModel) as GameObject;
		live2d.transform.SetParent(this.GetScript<Transform>("LargeCharacterAnchor"), false);
        live2d.transform.localScale = Vector3.one;
        live2d.transform.localPosition = Vector3.zero;
        cubismRender = live2d.GetComponentsInChildren<CubismRenderController>()[0];
        if (cubismRender != null) {
			var rootCanvas = this.GetScript<Canvas>("LargeCharacterAnchor");
            cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
            cubismRender.SortingLayer = rootCanvas.sortingLayerName;
            cubismRender.SortingOrder = rootCanvas.sortingOrder;
        }
        // 原画
        var originalButtonRoot = this.GetScript<RectTransform> ("ChangeOriginalImage");
        if (unitResource.OriginalImage != null) {
            this.GetScript<CustomButton> ("ChangeOriginalImage/bt_Sub").onClick.AddListener (DidTapOriginal);
            originalButtonRoot.gameObject.SetActive (true);
			this.GetScript<Image> ("OriginalImageRoot").sprite = unitResource.OriginalImage;
			this.GetScript<Image>("LargeOriginalImage").sprite = unitResource.OriginalImage;
        } else {
            originalButtonRoot.gameObject.SetActive (false);
        }      
        // 拡大カード.         
        var unitCard = GameObjectEx.LoadAndCreateObject ("_Common/View/UnitCard", this.GetScript<RectTransform> ("LargeCardAnchor").gameObject);
        unitCard.GetComponent<UnitCard> ().Init(displayCard, null, unitResource);

		// Newバッジ
		this.UpdateProfileNewBadge();
    }
	private void UpdateVoiceList(CardData displayCard)
	{
		foreach (var setting in MasterDataTable.card_detail_voice_setting.GetEnableList(displayCard)) {
			if (SoundManager.SharedInstance.ContainsVoice(displayCard.Card.voice_sheet_name, setting.voice_cue_id)) { 
				var root = GetScript<GridLayoutGroup>(string.Format("{0}/{0}Grid", setting.category.ToString()));
                root.gameObject.DestroyChildren();
			}
		}
		foreach (var setting in MasterDataTable.card_detail_voice_setting.GetEnableList(displayCard)) {
            if (SoundManager.SharedInstance.ContainsVoice(displayCard.Card.voice_sheet_name, setting.voice_cue_id)) {
                var root = GetScript<GridLayoutGroup>(string.Format("{0}/{0}Grid", setting.category.ToString()));            
                var go = GameObjectEx.LoadAndCreateObject("UnitDetails/ListItem_VoicePlay", root.gameObject);            
                go.GetOrAddComponent<ListItem_VoicePlay>().Init(displayCard, setting, displayCard.Card.voice_sheet_name);
            }
        }
	}

    private void UpdateProfileNewBadge()
	{
		var mod = AwsModule.CardModifiedData.List.Find(d => d.CardId == m_displayCard.CardId);
        this.GetScript<Image>("GoProfile/img_TabNew").gameObject.SetActive(mod.IsAnyNeedSeeReleaseVoice || mod.IsNeedSeeReleaseFlavor2);
        this.GetScript<Image>("Profile/img_TabNew").gameObject.SetActive(mod.IsNeedSeeReleaseFlavor2);
        this.GetScript<Image>("Voice/img_TabNew").gameObject.SetActive(mod.IsAnyNeedSeeReleaseVoice);
		foreach(var item in this.GetComponentsInChildren<ListItem_VoicePlay>(true)){
			item.UpdateNewBadge();
		}
	}

    // 戻るボタン.
    void DidTapBackButon()
    {
		if(m_viewEnhance != null){
			m_viewEnhance.Dispose();
			m_displayCard = CardData.CacheGet(m_displayCard.CardId);
			AwsModule.CardModifiedData.UpdateData(m_displayCard);
			SetInfo(m_displayCard);         
			UpdateProfileNewBadge();
			this.GetScript<CustomButton>("Reinforcement/bt_Common").interactable = !m_displayCard.IsMaxLevel;
			return;
		}
		if(m_viewEvolution != null){
			m_viewEvolution.Dispose();
			return;
		}
        if (m_viewTrainingBoard != null) {
            if (m_viewTrainingBoard.TapBackButton ()) {
                m_viewTrainingBoard.Dispose ();
                m_viewTrainingBoard = null;
            }
            return;
        }

		View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, m_BackSceneCallback);
    }

    void DidTapGoProfile()
    {
        this.GetScript<Transform> ("UnitInfo").gameObject.SetActive (false);
        this.GetScript<Transform> ("UnitProfile").gameObject.SetActive (true);
		ChangeGraphic(CardGraphicMode.Card);
        
		this.GetScript<CustomButton>("Profile/bt_Tab2Off").interactable = false;
        this.GetScript<CustomButton>("Voice/bt_Tab2Off").interactable = true;
        ChangeDispProfile("プロフィール");
    }

    void DidTapGoStatus()
    {
        this.GetScript<Transform> ("UnitInfo").gameObject.SetActive (true);
        this.GetScript<Transform> ("UnitProfile").gameObject.SetActive (false);
		ChangeGraphic(CardGraphicMode.Card);

		if (this.GetScript<CustomButton>("Voice/bt_Tab2Off").ForceHighlight) {
            AwsModule.CardModifiedData.ConfirmedVoiceOnly(m_displayCard);
        } else if (this.GetScript<CustomButton>("Profile/bt_Tab2Off").ForceHighlight) {
            AwsModule.CardModifiedData.ConfirmedFlavorOnly(m_displayCard);
        }
		this.UpdateProfileNewBadge();
    }
 
	void DidTapRotationView()
    {
		NextGraphic();
    }

    void DidTapOriginal()
    {
		var rortationBtnRoot = this.GetScript<Transform>("ChangeView");

		if(!m_bDispOriginal){
            rortationBtnRoot.gameObject.SetActive(false);
			this.ChangeGraphic(CardGraphicMode.Original);
		}else{
			rortationBtnRoot.gameObject.SetActive(true);
			this.ChangeGraphic(CardGraphicMode.Card);
			this.GetScript<HorizontalLayoutGroup>("UnitGraphicChange").gameObject.SetActive(false);
			this.GetScript<HorizontalLayoutGroup>("UnitGraphicChange").gameObject.SetActive(true);
		}      
		m_bDispOriginal = !m_bDispOriginal;
    }
	private bool m_bDispOriginal = false;

    void DidTapFullScreen()
    {
        this.GetScript<RectTransform> ("Large").gameObject.SetActive (true);
    }

    void DidTapLageImage()
    {
        this.GetScript<RectTransform> ("Large").gameObject.SetActive (false);
    }
 
	void DidTapTabProfile()
    {
		AwsModule.CardModifiedData.ConfirmedVoiceOnly(m_displayCard);
		this.UpdateProfileNewBadge();
		this.GetScript<CustomButton>("Profile/bt_Tab2Off").interactable = false;
		this.GetScript<CustomButton>("Voice/bt_Tab2Off").interactable = true;
		ChangeDispProfile("プロフィール");
    }
	void DidTapTabVoice()
    {
		AwsModule.CardModifiedData.ConfirmedFlavorOnly(m_displayCard);
		this.UpdateProfileNewBadge();
		this.GetScript<CustomButton>("Profile/bt_Tab2Off").interactable = true;
        this.GetScript<CustomButton>("Voice/bt_Tab2Off").interactable = false;
		ChangeDispProfile("ボイス");
    }

    enum CardGraphicMode {
        Card,
        SD,
        Live2D,
        Original
    }
    CardGraphicMode m_CurrentMode = CardGraphicMode.Card;
	void NextGraphic()
    {
		var length = System.Enum.GetValues(typeof(CardGraphicMode)).Length;
		var val = (int)m_CurrentMode;

		var mode = (CardGraphicMode)((++val) % length); 
		if (mode == CardGraphicMode.Original) {
			mode = (CardGraphicMode)((++val) % length);
        }
		ChangeGraphic(mode);
    }
	void ChangeGraphic(CardGraphicMode mode)
	{
		m_CurrentMode = mode;
		this.GetScript<RectTransform>("SD").gameObject.SetActive(m_CurrentMode == CardGraphicMode.SD);
		this.GetScript<RectTransform>("LargeSD").gameObject.SetActive(m_CurrentMode == CardGraphicMode.SD);
        this.GetScript<RectTransform>("CardAnchor").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Card);
		this.GetScript<RectTransform>("LargeCardAnchor").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Card);
		this.GetScript<RectTransform>("CharacterAnchor").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Live2D);      
		this.GetScript<RectTransform>("LargeCharacterAnchor").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Live2D);      
		this.GetScript<RectTransform>("OriginalImage").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Original);      
		this.GetScript<RectTransform>("LargeOriginalImage").gameObject.SetActive(m_CurrentMode == CardGraphicMode.Original);      
        this.ChangeDispRotationButton(m_CurrentMode);      
	}

	void ChangeDispRotationButton(CardGraphicMode mode)
	{
		switch(mode){
			case CardGraphicMode.Card:
				this.GetScript<TextMeshProUGUI>("ChangeView/txtp_View").text = "カード";
				break;
			case CardGraphicMode.SD:
				this.GetScript<TextMeshProUGUI>("ChangeView/txtp_View").text = "バトルSD";
				break;
			case CardGraphicMode.Live2D:
				this.GetScript<TextMeshProUGUI>("ChangeView/txtp_View").text = "Live2D";
				break;
		}
		var bActive = this.GetScript<RectTransform>("OriginalImage").gameObject.activeSelf;
        this.GetScript<TextMeshProUGUI>("ChangeOriginalImage/txtp_View").text = bActive ? "閉じる" : "原画";
	}

    void ChangeDispProfile(string tabName)
    {
        var voiceRoot = GetScript<RectTransform> ("ScrollAreaVoice");
        var profileRoot = GetScript<RectTransform> ("ScrollAreaProfile");

        if (tabName == "プロフィール") {
            voiceRoot.gameObject.SetActive (false);
            profileRoot.gameObject.SetActive (true);
			this.GetScript<CustomButton>("Profile/bt_Tab2Off").ForceHighlight = true;
			this.GetScript<CustomButton>("Voice/bt_Tab2Off").ForceHighlight = false;
        } else if (tabName == "ボイス") {
            voiceRoot.gameObject.SetActive (true);
            profileRoot.gameObject.SetActive (false);
			this.GetScript<CustomButton>("Profile/bt_Tab2Off").ForceHighlight = false;
            this.GetScript<CustomButton>("Voice/bt_Tab2Off").ForceHighlight = true;
        }
    }

    // 強化ボタン.
	void DidTapReinforcement()
	{
        if(IsOpenViews()){
			return;
		}

		// 都度素材一覧を取得してから開く.
		m_viewEnhance = View_UnitEnhance.Create(m_displayCard, card => {
			// 強化に応じたボイス解放だけ更新する.
			m_displayCard = card;
			AwsModule.CardModifiedData.UpdateData(m_displayCard);
			UpdateVoiceList(m_displayCard);
			UpdateProfileNewBadge();
		});
	}
    // 進化ボタン.
    void DidTapEvolution()
	{
        if(IsOpenViews()){
			return;
		}
            
		m_viewEvolution = View_UnitEvolution.Create(m_displayCard, card => {
			// 進化後はカードも更新する.
            View_FadePanel.SharedInstance.IsLightLoading = true;
			m_displayCard = card;
			this.GetScript<CustomButton>("Evolution/bt_Common").interactable = !m_displayCard.IsMaxRarity;
			this.GetScript<CustomButton>("Reinforcement/bt_Common").interactable = true;
			AwsModule.CardModifiedData.UpdateData(m_displayCard);
			UpdateVoiceList(m_displayCard);
			UpdateProfileNewBadge();
            SetInfo(m_displayCard);           
            this.GetComponentInChildren<UnitCard>().Init(m_displayCard, () => {
				m_viewEvolution.Dispose();
                View_FadePanel.SharedInstance.IsLightLoading = false;
			});
			// 拡大カード.
			this.GetScript<RectTransform>("LargeCardAnchor").gameObject.DestroyChildren();
            var unitCard = GameObjectEx.LoadAndCreateObject("_Common/View/UnitCard", this.GetScript<RectTransform>("LargeCardAnchor").gameObject);
			unitCard.GetComponent<UnitCard>().Init(m_displayCard, null, m_unitResource);
		}); 
	}

    // 装備ボタン.
    void DidTapEquip()
	{
		// 武器一覧へ.
		View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => {         
            ScreenChanger.SharedInstance.GoToWeapon(m_displayCard, m_DispGlobalMenu, () => {
				// 戻って来る際は現在設定でここに.
				View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, () => { 
					var card = CardData.CacheGet(m_displayCard.CardId);
                    ScreenChanger.SharedInstance.GoToUnitDetails(card, m_BackSceneCallback, m_DispGlobalMenu);
                });
			});
		});      
	}

    // 育成ボードボタン.
    void DidTrainingBoard()
    {
        if(IsOpenViews()){
            return;
        }

        m_viewTrainingBoard = View_TrainingBoard.Create (m_displayCard, m_unitResource,
            (card) => {
                if(m_displayCard != card) {
                    m_displayCard = card;
                    SetInfo(card);
                }
            }
        );
    }

    /// <summary>
    /// なにがしかのViewが開いている
    /// </summary>
    /// <returns><c>true</c> if this instance is open views; otherwise, <c>false</c>.</returns>
    bool IsOpenViews()
    {
        return m_viewTrainingBoard != null || m_viewEvolution != null || m_viewEnhance != null;
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    public override void Dispose ()
    {
		if(this.GetScript<Transform>("UnitProfile").gameObject.activeSelf){
			if(this.GetScript<CustomButton>("Voice/bt_Tab2Off").ForceHighlight){
				AwsModule.CardModifiedData.ConfirmedVoiceOnly(m_displayCard);
			}else if(this.GetScript<CustomButton>("Profile/bt_Tab2Off").ForceHighlight){
				AwsModule.CardModifiedData.ConfirmedFlavorOnly(m_displayCard);
			}
		}
        SoundManager.SharedInstance.StopVoice ();
        AwsModule.CardModifiedData.Sync();      
        base.Dispose ();
    }

    private bool m_DispGlobalMenu;
	private CardData m_displayCard;
	private UnitResourceLoader m_unitResource;
	private View_UnitEnhance m_viewEnhance;
	private View_UnitEvolution m_viewEvolution;
    private View_TrainingBoard m_viewTrainingBoard;
    private System.Action m_BackSceneCallback;
}
