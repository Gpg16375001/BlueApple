using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using Live2D.Cubism.Rendering;

public class View_TrainingBoard : ViewBase {
    enum DispInfoType {
        SelectUnit = 0,
        RequiredTrainingMaterial,
        TrainingDone,
        TrainingBundle,
        RequiredLimitBreakMaterial,

        Max
    }

    /// <summary>
    /// 生成.
    /// </summary>
    public static View_TrainingBoard Create(CardData card, UnitResourceLoader unitResource, Action<CardData> didClose)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_TrainingBoard");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_TrainingBoard>();
        c.InitInternal(card, unitResource, didClose);
        return c;
    }

    public override void Dispose ()
    {
        m_DidClose (m_Card);
        base.Dispose ();
    }

    public bool TapBackButton()
    {
        if(m_MagikiteList != null) {
            if (m_MagikiteList.TapBackButton ()) {
                m_MagikiteList = null;
            }
            return false;
        }
        return true;
    }
    private void InitInternal(CardData card, UnitResourceLoader unitResource, Action<CardData> didClose)
    {
        m_Card = card;
        m_DidClose = didClose;

        var growthBoardSetting = MasterDataTable.card_growth_board_setting [card.CardId];
        if (growthBoardSetting == null) {
            Debug.LogError ("Growth Board is not set. Please set it.");
            return ;
        }
            
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

        // ボード情報の取得
        m_BoardPattern = MasterDataTable.material_growth_board_pattern [growthBoardSetting.growth_board_pattern_id];
        for (int i = 1; i <= m_BoardPattern.total_board_number; ++i) {
            var boardDefinition = MasterDataTable.material_growth_board_definition [m_BoardPattern.id, i];
            if (boardDefinition == null) {
                continue;
            }
            m_BoardDefinitions.Add (boardDefinition, new List<MaterialGrowthBoardSlot>());
            for (int slot = 1; slot <= boardDefinition.total_slot_number; ++slot) {
                var boardSlot = MasterDataTable.material_growth_board_slot [m_BoardPattern.id, boardDefinition.board_index, slot];
                if (boardSlot == null) {
                    return;
                }
                m_BoardDefinitions [boardDefinition].Add (boardSlot);
            }
        }

        m_DispInfoObj = new GameObject[(int)DispInfoType.Max];
        foreach (DispInfoType type in Enum.GetValues(typeof(DispInfoType))) {
            if (type == DispInfoType.Max) {
                break;
            }
            m_DispInfoObj[(int)type] = GetScript<Transform>(string.Format("Info/{0}", type.ToString())).gameObject;
        }

        GetScript<PinchContorollModule>("SlotGroup").PinchCallbackEvent += BoardScale;

        // 育成ボードの設定
        SetGrowthBoard(card);

        // ユニット情報の設定
        SetInfoSelectUnit(card);

        SetCanvasCustomButtonMsg ("GemTrainingBundleMenu/bt_Common02", DidTapTrainingAllWithGem);
        SetCanvasCustomButtonMsg("TrainingBundleMenu/bt_TrainingBundleMenu", DidTapTrainingAll);
        SetCanvasCustomButtonMsg ("GemTraining/bt_Common02", DidTapTrainingWithGem);
        SetCanvasCustomButtonMsg ("Training/bt_Common", DidTapTraining);
        SetCanvasCustomButtonMsg ("TrainingBundle/bt_Common", DidTapTraining);
        SetCanvasCustomButtonMsg ("Deselect/bt_CommonS", DidTapDeselect);
        SetCanvasCustomButtonMsg ("LimitBreak/bt_Common", DidTapLimitBreak);
        SetCanvasCustomButtonMsg ("bt_BoardEquipMagi", DidTapBoardEquipMagi);
        //SetCanvasCustomButtonMsg ("GemLimitBreak/bt_Common02", DidTapLimitBreakWithGem);
        SetDispInfo (DispInfoType.SelectUnit);
    }

    private void SetGrowthBoard(CardData card)
    {
        var slotRoot = GetScript<Transform> ("SlotRoot").gameObject;
        m_BoardOffsetSetting = GetScript<TrainingBoardOffsetSetting> ("SlotRoot");
        var boardDataList = card.BoardDataList;

        // ボードを作成
        // 描画順の関係でボード番号が大きいものから順に作成する。
        foreach (var boardDefinition in m_BoardDefinitions.OrderByDescending(x => x.Key.board_index)) {
            var boardData = boardDataList.FirstOrDefault (x => x.Index == boardDefinition.Key.board_index);
            var nextBoardData = boardDataList.FirstOrDefault (x => x.Index == boardDefinition.Key.board_index + 1);
            if (boardData != null) {
                var prefabName = boardDefinition.Key.board_prefab_name;
                var go = GameObjectEx.LoadAndCreateObject (string.Format ("UnitDetails/{0}", prefabName), slotRoot);
                var script = go.GetOrAddComponent<View_TrainingBoardSlot> ();
                bool isLimitBreaked = nextBoardData != null && nextBoardData.IsAvailable;
                script.Init (card, boardData, isLimitBreaked, boardDefinition.Key, boardDefinition.Value, DidTapSlot, DidTapLimitBreak);
                var rectTrans = script.gameObject.GetComponent<RectTransform> ();
                rectTrans.anchoredPosition = new Vector2 (m_BoardOffsetSetting.Offset.x * boardDefinition.Key.board_index, m_BoardOffsetSetting.Offset.y * boardDefinition.Key.board_index);
                script.gameObject.SetActive (boardData.IsAvailable);
                var canvasGroup = script.gameObject.GetOrAddComponent<CanvasGroup> ();
                canvasGroup.interactable = false;
                canvasGroup.alpha = m_BoardOffsetSetting.Alpha;
                m_viewTrainingBoardSlots.Insert(0, script);
            }
        }
        m_NowDispPage = -1;
        m_MaxPage = boardDataList.Count(x => x.IsAvailable);
        SetCanvasCustomButtonMsg("bt_ArrowPage_1", DidTapArrowRight);
        SetCanvasCustomButtonMsg("bt_ArrowPage_2", DidTapArrowLeft);

        SetMaxPage ();

        // ページ設定
        SetDispBoardSlot(0);
    }

    private void SetMagikite(int bordNumber, MagikiteData magikite)
    {
        // 装備なし
        m_MagikiteData = magikite;

        var data = m_MagikiteData != null ? m_MagikiteData.Magikite : null;
        GetScript<TextMeshProUGUI>("txtp_BoardEquipMagiNo").SetText(bordNumber + 1);
        if (m_MagikiteData == null || data == null) {
            GetScript<Image> ("MagiIcon").gameObject.SetActive (false);
            GetScript<TextMeshProUGUI> ("txtp_EquipMagiName").SetText (string.Empty);
            GetScript<RectTransform> ("txtp_Empty").gameObject.SetActive (true);
            return;
        }

        var magikiteIcon = GetScript<Image> ("MagiIcon");
        GetScript<RectTransform> ("txtp_Empty").gameObject.SetActive (false);
        magikiteIcon.gameObject.SetActive (true);
        magikiteIcon.overrideSprite = null;
        IconLoader.LoadMagikite(data.id,
            (loadData, spt) => {
                if(loadData.id == m_MagikiteData.MagikiteId) {
                    magikiteIcon.overrideSprite = spt;
                }
            }
        );
        GetScript<TextMeshProUGUI> ("txtp_EquipMagiName").SetText (data.name);
    }

    private void BoardScale(float scale)
    {
        if (m_viewTrainingBoardSlots == null || m_viewTrainingBoardSlots.Count <= m_NowDispPage) {
            return;
        }
        var vec3Scale = new Vector3 (scale, scale, 1.0f);
        var board = m_viewTrainingBoardSlots [m_NowDispPage];
        var rect = board.gameObject.GetComponent<RectTransform> ();
        rect.localScale = vec3Scale;
    }

    private void SetDispBoardSlot(int next)
    {
        if (m_NowDispPage == next) {
            return;
        }

        //
        MagikiteData equipMagikite = null;
        if (m_Card.EquippedMagikiteBagIdList.Length > next) {
            if (m_Card.EquippedMagikiteBagIdList [next] > 0) {
                equipMagikite = MagikiteData.CacheGet (m_Card.EquippedMagikiteBagIdList [next]);
            }
        }
        SetMagikite (next, equipMagikite);

        // 
        if (m_NowDispPage >= 0) {
            m_viewTrainingBoardSlots [m_NowDispPage].Deselect ();

            SetDispInfo (DispInfoType.SelectUnit);
        }

        var slotRoot = GetScript<Transform> ("SlotRoot").gameObject;

        m_NowDispPage = next;

        for (int i = 0; i < m_MaxPage; ++i) {
            m_viewTrainingBoardSlots [i].gameObject.SetActive (m_NowDispPage <= i);
            var canvasGroup = m_viewTrainingBoardSlots [i].gameObject.GetOrAddComponent<CanvasGroup> ();
            canvasGroup.interactable = m_NowDispPage == i;
            canvasGroup.alpha = m_NowDispPage == i ? 1.0f: m_BoardOffsetSetting.Alpha;
        }

        var rectTras = slotRoot.GetComponent<RectTransform>();
        var pos = rectTras.anchoredPosition;
        pos.x = -1.0f * m_viewTrainingBoardSlots [m_NowDispPage].gameObject.GetComponent<RectTransform>().anchoredPosition.x; 
        pos.y = -1.0f * m_viewTrainingBoardSlots [m_NowDispPage].gameObject.GetComponent<RectTransform>().anchoredPosition.y;
        rectTras.anchoredPosition = pos;

        GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(m_NowDispPage < m_MaxPage - 1);
        GetScript<CustomButton> ("bt_ArrowPage_2").gameObject.SetActive(m_NowDispPage > 0);
    }

    private void SetDispInfo(DispInfoType type)
    {
        Array.ForEach (m_DispInfoObj, x => x.SetActive (false));
        m_DispInfoObj [(int)type].SetActive (true);
    }

    private void SetInfoSelectUnit(CardData card)
    {
        // アイコンの対応
        var iconRoot = GetScript<Transform> ("SelectUnit/UnitIcon");
        var icon = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", iconRoot.gameObject);
        icon.GetOrAddComponent<ListItem_UnitIcon> ().Init (card);

        // ユニット名
        GetScript<TextMeshProUGUI>("SelectUnit/txtp_UnitName").SetText(card.Card.nickname);

        // ステータス
        GetScript<TextMeshProUGUI>("txtp_UnitLv").SetText(card.Level);
        GetScript<TextMeshProUGUI>("txtp_UnitHP").SetText(card.Parameter.Hp);
        GetScript<TextMeshProUGUI>("txtp_UnitATK").SetText(card.Parameter.Attack);
        GetScript<TextMeshProUGUI>("txtp_UnitDEF").SetText(card.Parameter.Defense);
        GetScript<TextMeshProUGUI>("txtp_UnitSPD").SetText(card.Parameter.Agility);

        // 限界突破数
        int maxLimitBreak = m_BoardPattern.total_board_number - m_BoardPattern.initial_board_number;
        for (int i = 1; i <= 5; ++i) {

            if (maxLimitBreak < i) {
                GetScript<Transform> (string.Format ("SelectUnitSymbol{0}", i)).gameObject.SetActive(false);
            } else {
                GetScript<Transform> (string.Format ("SelectUnitSymbol{0}", i)).gameObject.SetActive(true);

                var limitBreakSymbolOn = GetScript<Transform> (string.Format ("SelectUnitSymbol{0}/LimitBreakIconOn", i));
                var limitBreakSymbolOff = GetScript<Transform> (string.Format ("SelectUnitSymbol{0}/LimitBreakIconOff", i));

                if (card.LimitBreakGrade >= i) {
                    limitBreakSymbolOff.gameObject.SetActive (false);
                    limitBreakSymbolOn.gameObject.SetActive (true);
                } else {
                    limitBreakSymbolOn.gameObject.SetActive (false);
                    limitBreakSymbolOff.gameObject.SetActive (true);
                }
            }
        }

        // スキル
        var actionSkill = card.Parameter.UnitActionSkillList.FirstOrDefault (x => !x.IsNormalAction);
        if (actionSkill == null) {
            GetScript<Transform> ("Skill").gameObject.SetActive (false);
        } else {
            GetScript<Transform> ("Skill").gameObject.SetActive (true);
            GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (actionSkill.Skill.display_name);
        }
        // 必殺技
        if (card.Parameter.SpecialSkill == null) {
            GetScript<Transform> ("SP").gameObject.SetActive (false);
        } else {
            GetScript<Transform> ("SP").gameObject.SetActive (true);
            GetScript<TextMeshProUGUI> ("txtp_SPName").SetText (card.Parameter.SpecialSkill.Skill.display_name);
        }
    }

    private void SetRequiredTrainingMaterial(MaterialGrowthBoardSlot slot, bool meetCondition, bool isRelease)
    {
        var statusValueObj = GetScript<Transform> ("SelectStatusValue").gameObject;
        var freeTextObj = GetScript<Transform> ("SelectFreeText").gameObject;
        if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.action_skill_level ||
            slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.special_skill_level ||
            slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.passive_skill_level) {
            statusValueObj.SetActive (false);
            freeTextObj.SetActive (true);
            var txtpText = GetScript<TextMeshProUGUI> ("SelectFreeText/txtp_Text");
            BattleLogic.SkillParameter displaySkill = null;
            if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.action_skill_level) {
                displaySkill = m_Card.Parameter.UnitActionSkillList.FirstOrDefault (x => !x.IsNormalAction);
            } else if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.special_skill_level) {
                displaySkill = m_Card.Parameter.SpecialSkill;
            } else if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.passive_skill_level) {
                displaySkill = m_Card.Parameter.UnitPassiveSkillList.FirstOrDefault();
            }
            if (displaySkill != null) {
                txtpText.SetText (displaySkill.Skill.display_name);
            } else {
                txtpText.SetText (slot.parameter_type.GetSlotDisplay ());
            }
        } else {
            freeTextObj.SetActive (false);
            statusValueObj.SetActive (true);
            var txtpTitle = GetScript<TextMeshProUGUI> ("SelectStatusValue/txtp_Title");
            var txtpNum = GetScript<TextMeshProUGUI> ("SelectStatusValue/txtp_Num");
            txtpTitle.SetText (slot.parameter_type.GetSlotDisplay ());
            txtpNum.SetTextFormat ("+{0}", slot.parameter_value);
        }

        // 素材設定
        var materialGridObj = GetScript<Transform>("RequiredTrainingMaterial/MaterialGrid").gameObject;
        materialGridObj.DestroyChildren ();
        foreach (var needMaterial in slot.item_combination.GetNeedMaterials(m_Card.CardId, m_Card.Parameter.Element.Enum, m_Card.Parameter.belonging.Enum, m_Card.Card.role)) {
            var go =GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_TrainingMaterial", materialGridObj);
            go.GetOrAddComponent<ListItem_TrainingMaterial> ().Init (needMaterial.Key, needMaterial.Value);
        }
        GetScript<TextMeshProUGUI> ("Cost/txtp_Coin").SetTextFormat("{0:#,0}", slot.item_combination.cost);

        GetScript<RectTransform> ("txtp_Alert").gameObject.SetActive (!meetCondition);
        GetScript<CustomButton> ("Training/bt_Common").interactable = meetCondition && isRelease;
        GetScript<CustomButton> ("GemTraining/bt_Common02").interactable = meetCondition;
        SetDispInfo (DispInfoType.RequiredTrainingMaterial);
    }

    private void SetTrainingBundle(int totalCost)
    {
        GetScript<TextMeshProUGUI> ("TrainingBundleCost/txtp_Coin").SetTextFormat("{0:#,0}", totalCost);
        SetDispInfo (DispInfoType.TrainingBundle);
    }

    private void SetRequiredLimitBreakMaterial(MaterialGrowthBoardItemCombination item_combination)
    {
        m_SelectLimitBreakItemCombination = item_combination;
        // 素材設定
        var materialGridObj = GetScript<Transform>("RequiredLimitBreakMaterial/MaterialGrid").gameObject;
        materialGridObj.DestroyChildren ();
        foreach (var needMaterial in item_combination.GetNeedMaterials(m_Card.CardId, m_Card.Parameter.Element.Enum, m_Card.Parameter.belonging.Enum, m_Card.Card.role)) {
            var go =GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_TrainingMaterial", materialGridObj);
            go.GetOrAddComponent<ListItem_TrainingMaterial> ().Init (needMaterial.Key, needMaterial.Value);
        }

        GetScript<CustomButton> ("LimitBreak/bt_Common").interactable = m_SelectLimitBreakItemCombination.IsRelease (m_Card.CardId, m_Card.Parameter.Element.Enum, m_Card.Parameter.belonging.Enum, m_Card.Card.role);
        GetScript<TextMeshProUGUI> ("LimitBreakCost/txtp_Coin").SetTextFormat("{0:#,0}", m_SelectLimitBreakItemCombination.cost);
        SetDispInfo (DispInfoType.RequiredLimitBreakMaterial);
    }

    private void SetTrainingDone(MaterialGrowthBoardSlot slot)
    {
        SetDispInfo (DispInfoType.TrainingDone);

        var statusValueObj = GetScript<Transform> ("DoneSelectStatusValue").gameObject;
        var freeTextObj = GetScript<Transform> ("DoneSelectFreeText").gameObject;
        if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.action_skill_level ||
            slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.special_skill_level ||
            slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.passive_skill_level) {
            statusValueObj.SetActive (false);
            freeTextObj.SetActive (true);
            var txtpText = GetScript<TextMeshProUGUI> ("DoneSelectFreeText/txtp_Text");
            BattleLogic.SkillParameter displaySkill = null;
            if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.action_skill_level) {
                displaySkill = m_Card.Parameter.UnitActionSkillList.FirstOrDefault (x => !x.IsNormalAction);
            } else if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.special_skill_level) {
                displaySkill = m_Card.Parameter.SpecialSkill;
            } else if (slot.parameter_type == MaterialGrowthBoardParameterTypeEnum.passive_skill_level) {
                displaySkill = m_Card.Parameter.UnitPassiveSkillList.FirstOrDefault();
            }
            if (displaySkill != null) {
                txtpText.SetText (displaySkill.Skill.display_name);
            } else {
                txtpText.SetText (slot.parameter_type.GetSlotDisplay ());
            }
        } else {
            freeTextObj.SetActive (false);
            statusValueObj.SetActive (true);
            var txtpTitle = GetScript<TextMeshProUGUI> ("DoneSelectStatusValue/txtp_Title");
            var txtpNum = GetScript<TextMeshProUGUI> ("DoneSelectStatusValue/txtp_Num");
            txtpTitle.SetText (slot.parameter_type.GetSlotDisplay ());
            txtpNum.SetTextFormat ("+{0}", slot.parameter_value);
        }
    }

    private void SetMaxPage()
    {
        if (m_NowDispPage >= 0) {
            GetScript<CustomButton>("bt_ArrowPage_1").gameObject.SetActive(m_NowDispPage < m_MaxPage - 1);
            GetScript<CustomButton> ("bt_ArrowPage_2").gameObject.SetActive(m_NowDispPage > 0);
        }
    }

    void DidTapArrowLeft()
    {
        if (m_NowDispPage <= 0) {
            return;
        }

        SetDispBoardSlot (m_NowDispPage - 1);
    }

    void DidTapArrowRight()
    {
        if (m_NowDispPage >= m_MaxPage - 1) {
            return;
        }

        SetDispBoardSlot (m_NowDispPage + 1);        
    }

    void ResetData(CardData card, bool playVoice)
    {
        if (playVoice) {
            // パネル解放時ボイス再生
            SoundManager.SharedInstance.PlayVoice (m_Card.Card.voice_sheet_name, SoundVoiceCueEnum.panel_release);
        }
        m_viewTrainingBoardSlots [m_NowDispPage].Deselect ();
        SetDispInfo (DispInfoType.SelectUnit);

        m_Card = card;
        SetInfoSelectUnit (m_Card);
        UpdateBoard();
    }

    void DidTapTrainingAllWithGem()
    {
        var slotDataList = m_viewTrainingBoardSlots [m_NowDispPage].SlotDataList;
        var boardData = m_Card.BoardDataList.First (x => x.Index == m_viewTrainingBoardSlots [m_NowDispPage].BoardIndex);
        var lockSlots = slotDataList.Where(x => !boardData.UnlockedSlotList.Contains(x.slot_index)).ToArray();

        if (lockSlots.Length <= 0) {
            PopupManager.OpenPopupOK ("すべてのスロットが開放済みです。");
            return;
        }

        var needGem = lockSlots.Sum (x => x.item_combination.gem_quantity);
        if (needGem > AwsModule.UserData.UserData.GemCount) {
            PopupManager.OpenPopupYN (string.Format("ジェムが足りません。\nジェムが{0}個必要です。\nジェムを購入しますか？", needGem),
                () => {
                    View_GemShop.Create();
                }
            );
            return;
        }

        View_GemTrainingBundleOKPop.Create (
            m_Card, lockSlots, true,
            () => {
                View_FadePanel.SharedInstance.IsLightLoading = true;
                LockInputManager.SharedInstance.IsLock = true;
                SendAPI.CardsUnlockAllBoardSlotsWithGem (
                    m_Card.CardId,
                    m_viewTrainingBoardSlots [m_NowDispPage].BoardIndex,
                    (result, response) => {
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        LockInputManager.SharedInstance.IsLock = false;
                        if(!result || response == null) {
                            return;
                        }

                        // データの更新
                        response.CardData.CacheSet();
                        AwsModule.UserData.UserData = response.UserData;

                        ResetData(response.CardData, true);
                    }
                );
            }
        );

    }

    void DidTapTrainingAll()
    {
        var slotDataList = m_viewTrainingBoardSlots [m_NowDispPage].SlotDataList;
        var boardData = m_Card.BoardDataList.First (x => x.Index == m_viewTrainingBoardSlots [m_NowDispPage].BoardIndex);
        var lockSlots = slotDataList.Where(x => !boardData.UnlockedSlotList.Contains(x.slot_index)).ToArray();

        if (lockSlots.Length <= 0) {
            PopupManager.OpenPopupOK ("すべてのスロットが開放済みです。");
            return;
        }

        View_TrainingBundleOKPop.Create (
            m_Card, boardData, lockSlots,
            (slots) => {
                SendCardUnlockSlot(slots);
            }
        );
    }

    void DidTapTrainingWithGem()
    {
        if (m_SelectSlotInfo == null) {
            return;
        }

        var needGem = m_SelectSlotInfo.item_combination.gem_quantity;
        if (needGem > AwsModule.UserData.UserData.GemCount) {
            PopupManager.OpenPopupYN (string.Format("ジェムが足りません。\nジェムが{0}個必要です。\nジェムを購入しますか？", needGem),
                () => {
                    View_GemShop.Create();
                }
            );
            return;
        }

        View_GemTrainingBundleOKPop.Create (
            m_Card, new MaterialGrowthBoardSlot[1] { m_SelectSlotInfo }, false,
            () => {
                View_FadePanel.SharedInstance.IsLightLoading = true;
                LockInputManager.SharedInstance.IsLock = true;

                SendAPI.CardsUnlockBoardSlotWithGem (
                    m_Card.CardId,
                    m_viewTrainingBoardSlots [m_NowDispPage].BoardIndex,
                    new int[1] {m_SelectSlotInfo.slot_index},
                    (result, response) => {
                        View_FadePanel.SharedInstance.IsLightLoading = false;
                        LockInputManager.SharedInstance.IsLock = false;
                        if (!result || response == null) {
                            return;
                        }
                        // データの更新
                        response.CardData.CacheSet ();
                        AwsModule.UserData.UserData = response.UserData;

                        ResetData (response.CardData, true);
                    }
                );
            }
        );
    }

    void DidTapTraining()
    {
        if (m_SelectSlotInfo == null) {
            return;
        }
            
        View_TrainingOKPop.Create (
            m_Card, m_SelectSlotInfo,
            () => {
                SendCardUnlockSlot(new MaterialGrowthBoardSlot[1] {m_SelectSlotInfo});
            }
        );
    }

    private void SendCardUnlockSlot(MaterialGrowthBoardSlot[] releaseSlots)
    {
        View_FadePanel.SharedInstance.IsLightLoading = true;
        LockInputManager.SharedInstance.IsLock = true;
        SendAPI.CardsUnlockBoardSlot (
            m_Card.CardId,
            m_viewTrainingBoardSlots [m_NowDispPage].BoardIndex,
            releaseSlots.Select(x => x.slot_index).ToArray(),
            (result, response) => {
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
                if (!result || response == null) {
                    return;
                }
                // データの更新
                response.CardData.CacheSet ();
                AwsModule.UserData.UserData = response.UserData;

                var selectedMaterials = new Dictionary<int, int> ();
                foreach (var slot in releaseSlots) {
                    var need = slot.item_combination.GetNeedMaterials (m_Card.CardId, m_Card.Parameter.Element.Enum, m_Card.Parameter.belonging.Enum, m_Card.Card.role);
                    foreach (var kv in need) {
                        if (selectedMaterials.ContainsKey (kv.Key)) {
                            selectedMaterials [kv.Key] += kv.Value;
                        } else {
                            selectedMaterials [kv.Key] = kv.Value;
                        }
                    }
                }
                List<MaterialData> spendData = new List<MaterialData>();
                foreach(var kv in selectedMaterials) {
                    var meterailData = MaterialData.CacheGet(kv.Key);
                    meterailData.Count -= kv.Value;
                    spendData.Add(meterailData);
                }
                spendData.CacheSet();

                ResetData (response.CardData, true);
            }
        );
    }

    void UpdateBoard()
    {
        m_MaxPage = m_Card.BoardDataList.Count(x => x.IsAvailable);
        SetMaxPage ();

        // maxPageの更新のため表示ボードも更新する。
        for (int i = 0; i < m_MaxPage; ++i) {
            m_viewTrainingBoardSlots [i].gameObject.SetActive (m_NowDispPage <= i);
            var canvasGroup = m_viewTrainingBoardSlots [i].gameObject.GetOrAddComponent<CanvasGroup> ();
            canvasGroup.interactable = m_NowDispPage == i;
            canvasGroup.alpha = m_NowDispPage == i ? 1.0f: m_BoardOffsetSetting.Alpha;
        }

        m_viewTrainingBoardSlots.ForEach (x => {
            var boardData = m_Card.BoardDataList.FirstOrDefault(board => board.Index == x.BoardIndex);
            var nextBoardData = m_Card.BoardDataList.FirstOrDefault(board => board.Index == x.BoardIndex + 1);
            bool isLimitBreaked = nextBoardData != null && nextBoardData.IsAvailable;
            x.UpdateSlot (m_Card, boardData, isLimitBreaked);
        }
        );
    }

    void DidTapDeselect()
    {
        m_viewTrainingBoardSlots [m_NowDispPage].Deselect ();

        SetDispInfo (DispInfoType.SelectUnit);
    }

    void DidTapLimitBreak()
    {
        if (m_SelectLimitBreakItemCombination == null) {
            return;
        }

        View_TrainingLimitBreakOKPop.Create (() => {
            View_FadePanel.SharedInstance.IsLightLoading = true;
            LockInputManager.SharedInstance.IsLock = true;
            SendAPI.CardsLimitBreak (m_Card.CardId,
                (result, response) => {
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    LockInputManager.SharedInstance.IsLock = false;
                    if (!result || response == null) {
                        return;
                    }
                    // データの更新
                    response.CardData.CacheSet ();
                    AwsModule.UserData.UserData = response.UserData;
                    m_SelectLimitBreakItemCombination.Spending(m_Card.CardId, m_Card.Parameter.Element.Enum, m_Card.Parameter.belonging.Enum, m_Card.Card.role);

                    View_UnitLimitBreakMovie.Create(response.CardData, response.CardData.LimitBreakGrade, 
                        () => {
                            ResetData (response.CardData, false);
                            // 開けたSlotに変化する。
                            SetDispBoardSlot(m_NowDispPage + 1);
                        }
                    );
                }
            );
        });
    }

    void DidTapSlot(MaterialGrowthBoardSlot slotInfo, bool selected, bool released, bool meetCondition)
    {
        Dictionary<int, int> selectedMaterials = null;
        int totalCost = 0;
        bool isRelease = slotInfo.item_combination.IsRelease (m_Card.CardId, m_Card.Card.element.Enum, m_Card.Card.character.belonging.Enum, m_Card.Card.role);
        if (selected) {
            // 開放済み
            if (released) {
                SetTrainingDone(slotInfo);
                m_SelectSlotInfo = null;
                return;
            }
            // 解放できない
            if (!meetCondition) {
                m_SelectSlotInfo = null;
            } else {
                m_SelectSlotInfo = slotInfo;
            }
            totalCost += slotInfo.item_combination.cost;
            SetRequiredTrainingMaterial(slotInfo, meetCondition, isRelease);
        } else {
            m_SelectSlotInfo = null;
            SetDispInfo (DispInfoType.SelectUnit);
        }
        m_viewTrainingBoardSlots [m_NowDispPage].UpdateSlotInteractable (m_Card, selectedMaterials, totalCost);
    }

    void DidTapLimitBreak(MaterialGrowthBoardItemCombination itemCmbination)
    {
        m_viewTrainingBoardSlots [m_NowDispPage].Deselect ();

        SetRequiredLimitBreakMaterial (itemCmbination);
    }

    void DidTapBoardEquipMagi()
    {
        long? equipedBagId = null;
        if (m_MagikiteData != null) {
            equipedBagId = m_MagikiteData.BagId;
        }
        m_MagikiteList = View_MagiList.Create(m_Card, equipedBagId, SelectMagikite);
    }

    void SelectMagikite(MagikiteData magikite)
    {
        EquippedMagikite param = new EquippedMagikite ();
        param.CardId = m_Card.CardId;
        param.MagikiteBagIdList = m_Card.EquippedMagikiteBagIdList.ToArray();
        if (param.MagikiteBagIdList.Length <= m_NowDispPage) {
            Array.Resize(ref param.MagikiteBagIdList, m_NowDispPage + 1);
        }

        bool isRemove = false;
        if (param.MagikiteBagIdList [m_NowDispPage] == magikite.BagId) {
            // 外す
            isRemove = true;
            param.MagikiteBagIdList [m_NowDispPage] = 0;
        } else {
            // 設定
            // 同じものを自分が装備していた時はそっちを外す。
            int equipedIndex = Array.FindIndex (param.MagikiteBagIdList, x => x == magikite.BagId);
            if (equipedIndex >= 0) {
                param.MagikiteBagIdList [equipedIndex] = 0;
            }
            param.MagikiteBagIdList [m_NowDispPage] = magikite.BagId;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.CardsSetMagikite (new EquippedMagikite[1] {param},
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(!success) {
                    return;
                }
                response.AffectedCardDataList.CacheSet();
                response.AffectedMagikiteDataList.CacheSet();

                m_Card = CardData.CacheGet(m_Card.CardId);
                if(!isRemove) {
                    SetMagikite(m_NowDispPage, magikite);
                } else {
                    SetMagikite(m_NowDispPage, null);
                }

                m_MagikiteList.Dispose();
                m_MagikiteList = null;
            }
        );
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    int m_NowDispPage = 0;
    int m_MaxPage = 0;

    CardData m_Card;

    GameObject[] m_DispInfoObj;

    MaterialGrowthBoardPattern m_BoardPattern;
    Dictionary<MaterialGrowthBoardDefinition, List<MaterialGrowthBoardSlot>> m_BoardDefinitions = new Dictionary<MaterialGrowthBoardDefinition, List<MaterialGrowthBoardSlot>> ();
    List<View_TrainingBoardSlot> m_viewTrainingBoardSlots = new List<View_TrainingBoardSlot>();

    TrainingBoardOffsetSetting m_BoardOffsetSetting;

    MaterialGrowthBoardSlot m_SelectSlotInfo = null;

    MaterialGrowthBoardItemCombination m_SelectLimitBreakItemCombination;
    System.Action<CardData> m_DidClose;

    MagikiteData m_MagikiteData;

    View_MagiList m_MagikiteList;
}
