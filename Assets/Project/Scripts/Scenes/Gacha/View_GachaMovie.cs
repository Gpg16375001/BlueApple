using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net.API;
using TMPro;
using Live2D.Cubism.Rendering;
using UnityEngine.UI;


/// <summary>
/// View : ガチャ演出.
/// </summary>
public class View_GachaMovie : ViewBase
{
	/// <summary>
	/// 生成処理.オブジェクト表示.
    /// </summary>
	public static View_GachaMovie Create(ReceiveGachaPurchaseProduct data, List<KeyValuePair<int, GameObject>> objList, Action didEnd = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaMovie");
		var c = go.GetOrAddComponent<View_GachaMovie>();
		c.InitInternal(data, objList, didEnd);
		return c;
	}
	private void InitInternal(ReceiveGachaPurchaseProduct data, List<KeyValuePair<int, GameObject>> objList, Action didEnd = null)
    {
        m_didEnd = didEnd;
        m_objList = objList;
		m_sptList = null;
		m_service = data.RarestCardGachaItemData;
		m_remainItemList = new Queue<AcquiredGachaItemData>(data.AcquiredGachaItemDataList);
		this.CommonInit();
    }   

	/// <summary>
    /// 生成処理.スプライト表示.
    /// </summary>
	public static View_GachaMovie Create(ReceiveGachaPurchaseProduct data, List<KeyValuePair<int, Sprite>> sptList, Action didEnd = null)
    {
        var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaMovie");
        var c = go.GetOrAddComponent<View_GachaMovie>();
        c.InitInternal(data, sptList, didEnd);
        return c;
    }
    private void InitInternal(ReceiveGachaPurchaseProduct data, List<KeyValuePair<int, Sprite>> sptList, Action didEnd = null)
    {
        m_didEnd = didEnd;
		m_objList = null;
		m_sptList = sptList;
        m_service = data.RarestCardGachaItemData;
		m_remainItemList = new Queue<AcquiredGachaItemData>(data.AcquiredGachaItemDataList);
		this.CommonInit();
    }   

    /// <summary>
    /// チュートリアルから生成.
    /// </summary>
	public static View_GachaMovie Create(ReceiveGachaExecuteTutorialGacha data, List<KeyValuePair<int, GameObject>> objList, Action didEnd = null)
	{ 
		var go = GameObjectEx.LoadAndCreateObject("Gacha/View_GachaMovie");
        var c = go.GetOrAddComponent<View_GachaMovie>();
        c.InitInternal(data, objList, didEnd);
        return c;
	}
    private void InitInternal(ReceiveGachaExecuteTutorialGacha data, List<KeyValuePair<int, GameObject>> objList, Action didEnd = null)
    {
        m_didEnd = didEnd;
        m_objList = objList;
		m_sptList = null;
        m_remainItemList = new Queue<AcquiredGachaItemData>(data.AcquiredGachaItemDataList);      
		this.CommonInit();
    }
    
    private void CommonInit()
	{
		// モーション管理システム初期化.
        m_motionModule = this.gameObject.GetOrAddComponent<GachaMovieMotionModule>();
        m_motionModule.DidEnterStateIn += CallbackEnterStateIn;
		m_motionModule.DidExitServiceCheck += CallbackExitServiceCheck;
        m_motionModule.Init();
        m_motionModule.RegistStateChangeStream();

		// 天井かどうか.
		m_motionModule.Animator.SetInteger("ServiceRarity", m_service != null ? 1 : 0);

        // 一番初めの予兆演出設定.
        var maxRarity = m_remainItemList.Select(item => GetNextRarity(item)).Max();
        this.SettingEntryOmenRarity(maxRarity);
        // 最初の1枚目は手動で設定.
		m_current = m_remainItemList.Dequeue();
        this.SettingNextEffect(m_current);
        this.SettingView();

        // ボタン.
        var trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>();
		m_screenTriggerDisposable = trigger.OnPointerDownAsObservable()
		                                   .Subscribe(pointer => DidTapStart());
        this.SetCanvasButtonMsg("Button", DidTapSkip);
	}

    /// <summary>
    /// オブジェクトリストも破棄する.
    /// </summary>
	public override void Dispose()
	{
		if(m_objList != null && m_objList.Count > 0){
			foreach(var o in m_objList){
				GameObject.Destroy(o.Value);
			}
			m_objList.Clear();
		}
		base.Dispose();
	}

	#region ButtonDelegate

	// 開始.
	void DidTapStart()
	{
		if(m_motionModule.Animator.GetBool("IsStart")){         
			return;
		}
		if(m_bTapStart){
			return;
		}
		m_bTapStart = true;
		Debug.Log("DidTapStart");
		m_motionModule.Animator.SetBool("IsStart", true);      
		if (m_service == null) {
			this.StartCoroutine(this.WaitMovieEnd());
		}

		if(m_screenTriggerDisposable != null){
			m_screenTriggerDisposable.Dispose();
		}      
	}
	private IDisposable m_screenTriggerDisposable;

    // 天井ガチャ開始.
    void DidTapStartRarity4()
	{
        if (m_service == null || m_motionModule.Animator.GetBool("IsStartServiceRarity")) {
            return;
        }
		if (m_bTapStart) {
            return;
        }
        m_bTapStart = true;
		Debug.Log("DidTapRarity4Start");
        m_motionModule.Animator.SetBool("IsStartServiceRarity", true);
        this.StartCoroutine(this.WaitMovieEnd());
	}

    // スキップ.
    void DidTapSkip()
	{
		this.StopAllCoroutines();
		this.StartCoroutine(this.WaitMovieEnd());
		m_motionModule.Animator.SetInteger("ServiceRarity", 0);
		m_motionModule.Animator.Play("coda_out", 0, 0.0f);
	}

    #endregion

    // 演出終了待ち.
	private IEnumerator WaitMovieEnd()
	{
		while(!m_motionModule.IsEnd){
			yield return null;         
		}
		Dispose();
		if(m_didEnd != null){
			m_didEnd();
		}      
	}

	// コールバック : 演出開始時.
    void CallbackEnterStateIn()
	{
        // ステートの作り上初回設定のみ例外的に対応しなければならない.
		if(m_bFirstSkip){
			m_bFirstSkip = false;
			m_bTapStart = false;
			return;
		}

		// 終了.      
		if (m_remainItemList.Count <= 0) { 
			// 最後のカード表示.
			this.SettingView();
			m_motionModule.Animator.SetBool("IsOmenGreaterRarity3", false);
			m_motionModule.Animator.SetInteger("Rarity", 0);
            return;
		}
		// カード表示と更新.
		var next = m_remainItemList.Dequeue();
        this.SettingNextEffect(next);
        this.SettingView();
        // かぶり石獲得SE
        if (!m_current.IsNew) {
            SoundManager.SharedInstance.PlaySE(SoundClipName.se151);
        }
        m_current = next;     
	}

	// コールバック : 星４確定演出チェック終了.
    void CallbackExitServiceCheck()
	{      
		// 天井対応.
		if(m_motionModule.Animator.GetInteger("Rarity") <= 0){

			var trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>();
            trigger.OnPointerDownAsObservable()
			       .Subscribe(pointer => DidTapStartRarity4());

            if (m_service != null) {            
                if ((ItemTypeEnum)m_service.ItemType == ItemTypeEnum.card) {
                    this.SettingCard(m_service);
                }
                // TODO : カード以外でもあれば.
            }
		}

	}
	private bool m_bFirstSkip = true;   

	private void SettingView()
	{
		this.GetScript<RectTransform>("CharaInfor").gameObject.SetActive((ItemTypeEnum)m_current.ItemType == ItemTypeEnum.card);
		this.GetScript<RectTransform>("WeaponInfor").gameObject.SetActive((ItemTypeEnum)m_current.ItemType == ItemTypeEnum.weapon);
		this.GetScript<RectTransform>("Weapon").gameObject.SetActive((ItemTypeEnum)m_current.ItemType == ItemTypeEnum.weapon);
		
		switch ((ItemTypeEnum)m_current.ItemType) {
            case ItemTypeEnum.card: {
					SettingCard(m_current);
                }
                break;
			case ItemTypeEnum.weapon: {
					SettingWeapon(m_current);               
				}            
                break;
            case ItemTypeEnum.money:
                break;
            case ItemTypeEnum.free_gem:
                break;
        }
	}   

    // 排出がカードだった場合の設定
	private void SettingCard(AcquiredGachaItemData gachaItemData)
	{
		if((ItemTypeEnum)gachaItemData.ItemType != ItemTypeEnum.card){
			return;
		}

		// ユニットLive2Dの表示.
		if(m_model != null){
			m_model.gameObject.SetActive(false);
		}
		var cardData = new CardData(MasterDataTable.card[gachaItemData.ItemId]);
        var unitRoot = this.GetScript<RectTransform>("CharacterAnchor");
		m_model = m_objList.Find(kvp => kvp.Key == cardData.CardId).Value;
        m_model.transform.SetParent(unitRoot);
        m_model.transform.localScale = Vector3.one;
        m_model.transform.localPosition = Vector3.zero;
        m_model.transform.localRotation = Quaternion.Euler(0, 0, 0);
        m_model.GetComponent<CubismRenderController>().SortingOrder = this.GetScript<Canvas>("Movie_SRT/Chara").sortingOrder;
		m_model.SetActive(true);
        // 情報.
		this.GetScript<TextMeshProUGUI>("Charaname_Text").text = cardData.Card.nickname;
		this.GetScript<TextMeshProUGUI>("Charaalias_Text").text = cardData.Card.alias;
        // 被ってるかどうか.
		this.GetScript<RectTransform>("Layout_main/NewInfor_SRT").gameObject.SetActive(gachaItemData.IsNew);
		this.GetScript<RectTransform>("Layout_main/CharaMultipleInfor_SRT").gameObject.SetActive(!gachaItemData.IsNew);
        // レアリティ.
		Debug.Log(cardData.Card.nickname + "  " + cardData.Rarity + " : " + cardData.MaxRarity);
        for (var i = 1; i <= 4; ++i) {
            var emptyStarName = "StarEmpty" + i.ToString();
            var rarity = i + cardData.Rarity;
            this.GetScript<RectTransform>("Rarity_SRT/" + emptyStarName).gameObject.SetActive(rarity <= cardData.MaxRarity);
        }
	}
	private GameObject m_model;

	// 排出が武器だった場合の設定
	private void SettingWeapon(AcquiredGachaItemData gachaItemData)
	{
		if ((ItemTypeEnum)gachaItemData.ItemType != ItemTypeEnum.weapon) {
            return;
        }

		var weapon = MasterDataTable.weapon[m_current.ItemId];
		var sprite = m_sptList.Find(kvp => kvp.Key == weapon.id).Value;
        // 名前.
		this.GetScript<TextMeshProUGUI>("Weaponname_Text").text = weapon.name;
		// レアリティの設定
		var children = this.GetScript<RectTransform>("WeaponRarity_SRT").gameObject.GetChildren();
		for (var i = 0; i < children.Length; ++i){
			var rarity = (i+1);
			var objName = string.Format("WeaponRarity_SRT/Star{0}", rarity);
			this.GetScript<RectTransform>(objName).gameObject.SetActive(rarity < weapon.rarity.rarity);
		}
		// 画像
		this.GetScript<Image>("WeaponImage").sprite = sprite;
	}

	private int GetNextRarity(AcquiredGachaItemData next)
	{
		switch ((ItemTypeEnum)next.ItemType) {
            case ItemTypeEnum.card:
                return MasterDataTable.card[next.ItemId].rarity;
            case ItemTypeEnum.weapon:
				return MasterDataTable.weapon[next.ItemId].rarity.rarity;
            case ItemTypeEnum.money:
                return 2;    // TODO : しょぼいアイテムらしいのでとりあえず星2扱い.
            case ItemTypeEnum.free_gem:
				return 3;    // TODO : いいアイテムなのでとりあえず星3扱い.
        }
		return 0;
	}   
	private void SettingNextEffect(AcquiredGachaItemData next)
    {
		var val = UnityEngine.Random.Range(0, 100);
        var bOmen = val < 20;   // 20%の確率とのこと.      
		var rarity = GetNextRarity(next);
		m_motionModule.Animator.SetBool("IsOmenGreaterRarity3", rarity >= 3 && bOmen);
		m_motionModule.Animator.SetInteger("Rarity", rarity);
    }

    // 一番初めの予兆レアリティ.
	private void SettingEntryOmenRarity(int rarity)
	{
		var val = UnityEngine.Random.Range(0, 100);
		var bOmen = val < 20;   // 20%の確率とのこと.
		if(rarity >= 4 && bOmen){
			m_motionModule.Animator.SetInteger("EntryOmenRarity", 4);
			return;
		}
		m_motionModule.Animator.SetInteger("EntryOmenRarity", 0);
	}

	void Awake()
	{
		var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
	}

	private AcquiredGachaItemData m_current;
	private AcquiredGachaItemData m_service;
	private Queue<AcquiredGachaItemData> m_remainItemList = new Queue<AcquiredGachaItemData>();
	private Action m_didEnd;
	private List<KeyValuePair<int, GameObject>>  m_objList;
	private List<KeyValuePair<int, Sprite>> m_sptList;
	private GachaMovieMotionModule m_motionModule;

	private bool m_bTapStart = false;
}
