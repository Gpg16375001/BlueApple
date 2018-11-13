using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;


/// <summary>
/// View : 武器フィルター専用ポップ.
/// </summary>
public class View_WeaponFilterPop : PopupViewBase
{
	/// <summary>
    /// アクティブかどうか.
    /// </summary>
	public bool IsEnable 
	{
		set { 
			if(value){
				m_currentSetting.LoadData();
				this.UpdateView();
			}         
			this.gameObject.SetActive(value);
            this.PlayOpenCloseAnimation(value); 
		} 
	}

	/// <summary>
    /// 生成.
    /// </summary>
	public static View_WeaponFilterPop Create(Action<WeaponFilterSetting.Data> didClose)
	{
		if(instance != null){
			instance.Dispose();
		}
		var go = GameObjectEx.LoadAndCreateObject("Weapon/View_WeaponFilterPop");
        instance = go.GetOrAddComponent<View_WeaponFilterPop>();
		instance.InitInternal(didClose);
		return instance;
	}
	private void InitInternal(Action<WeaponFilterSetting.Data> didClose)
	{
		m_didClose = didClose;
		m_currentSetting = new WeaponFilterSetting();

		// 初回表示更新.
		this.UpdateView();

		// ボタン.      
		this.GetScript<CustomButton>("bt_Close").onClick.AddListener(DidTapCancel);
		this.GetScript<CustomButton>("Cancel/bt_Common").onClick.AddListener(DidTapCancel);
		this.GetScript<CustomButton>("OK/bt_Common").onClick.AddListener(DidTapOK);
		this.GetScript<CustomButton>("Reset/bt_TopLineGray").onClick.AddListener(DidTapReset);
        // レアリティ.
        var maxRarity = MasterDataTable.weapon_rarity.DataList.Select(r => r.rarity).Max();
        for (var i = 1; i <= maxRarity; ++i){
            var rootName = "Rarity_"+i.ToString("d2");
			var rarity = i+0;
			this.GetScript<CustomButton>(rootName+"/bt_Tab").onClick.AddListener(() => DidTapRarity(rarity));
        }
        // 武器種.
		var maxWeaponId = MasterDataTable.weapon_type.DataList.Where(w => !w.name.Contains("特殊") &&  !w.name.Contains("素材")).Select(t => t.index).Max();
        for (var i = 1; i <= maxWeaponId; ++i) {
            var rootName = "Weapon_"+i.ToString("d2");
			var index = i + 0;
			this.GetScript<CustomButton>(rootName + "/bt_Tab").onClick.AddListener(() => DidTapWeaponType(index));
        }
		// ロック中.
		this.GetScript<CustomButton>("LockOn/bt_Tab").onClick.AddListener(DidTapLock);
		// TODO : ロック中以外.
		this.GetScript<CustomButton>("LockOff/bt_Tab").onClick.AddListener(DidTapWithoutLock);

		// 素材.
		this.GetScript<CustomButton>("EnhanceMaterial/bt_Tab").onClick.AddListener(DidTapMaterial);

        SetBackButton ();

		this.PlayOpenCloseAnimation(bOpen: true);
	}

    // ボタン類など表示更新.
    private void UpdateView()
	{
		if(m_currentSetting == null || m_currentSetting.CurrentData == null){
			return;
		}
		// レアリティ.
        var maxRarity = MasterDataTable.weapon_rarity.DataList.Select(r => r.rarity).Max();
        for (var i = 1; i <= maxRarity; ++i) {
            var rootName = "Rarity_" + i.ToString("d2");
            this.GetScript<CustomButton>(rootName + "/bt_Tab").ForceHighlight = m_currentSetting.CurrentData.IsHaveContents ? m_currentSetting.RarityList.Exists(r => r == i): false;
        }
        // 武器種.
		var maxWeaponId = MasterDataTable.weapon_type.DataList.Where(w => !w.name.Contains("特殊") && !w.name.Contains("素材")).Select(t => t.index).Max();
        for (var i = 1; i <= maxWeaponId; ++i) {
            var rootName = "Weapon_" + i.ToString("d2");
            this.GetScript<CustomButton>(rootName + "/bt_Tab").ForceHighlight = m_currentSetting.CurrentData.IsHaveContents ? m_currentSetting.WeaponTypeIndexList.Exists(r => r == i): false;
        }
        // ロック中.
        this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight = m_currentSetting.CurrentData.IsHaveContents && m_currentSetting.IsVisibleLock;
        this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight = m_currentSetting.CurrentData.IsHaveContents && m_currentSetting.IsVisibleWithoutLock;            
        // 素材.
        this.GetScript<CustomButton>("EnhanceMaterial/bt_Tab").ForceHighlight = m_currentSetting.CurrentData.IsHaveContents && m_currentSetting.IsVisibleMaterial;  
	}

    /// <summary>
    /// アニメーション付随で破棄する.
	/// </summary>
	public override void Dispose()
	{
        m_currentSetting = new WeaponFilterSetting();
        this.UpdateView();
        m_currentSetting.SaveData();
		if (m_didClose != null) {
            m_didClose(m_currentSetting.CurrentData);
        }
        base.Dispose();
	}

    protected override void DidBackButton ()
    {
        DidTapCancel ();
    }

	#region ButtonDelegate.

	// ボタン : キャンセル.
	void DidTapCancel()
	{
        if (IsClosed) {
            return;
        }

		this.PlayOpenCloseAnimation(false, () => {
			this.gameObject.SetActive(false);
            if (m_didClose != null){
				m_didClose(m_currentSetting.CurrentData);
			}
		});
	}
	// ボタン : OK.
    void DidTapOK()
    {
        if (IsClosed) {
            return;
        }

		m_currentSetting.SaveData();
        this.PlayOpenCloseAnimation(false, () => {
			this.gameObject.SetActive(false);
            if (m_didClose != null) {
                m_didClose(m_currentSetting.CurrentData);
            }
        });
    }
	// ボタン : リセット.
    void DidTapReset()
    {
        if (IsClosed) {
            return;
        }

		m_currentSetting = new WeaponFilterSetting();
		this.UpdateView();
    }

	// ボタン : レアリティ.
    void DidTapRarity(int rarity)
	{
        if (IsClosed) {
            return;
        }

		var rootName = "Rarity_"+rarity.ToString("d2");
		this.GetScript<CustomButton>(rootName+"/bt_Tab").ForceHighlight = !this.GetScript<CustomButton>(rootName+"/bt_Tab").ForceHighlight;

		if(m_currentSetting.RarityList.Exists(r => r == rarity)){
			m_currentSetting.RarityList.Remove(rarity);
		}else{
			m_currentSetting.RarityList.Add(rarity);
		}
	}

	// ボタン : 武器種.
    void DidTapWeaponType(int index)
    {
        if (IsClosed) {
            return;
        }

		var rootName = "Weapon_"+index.ToString("d2");
		this.GetScript<CustomButton>(rootName+"/bt_Tab").ForceHighlight = !this.GetScript<CustomButton>(rootName+"/bt_Tab").ForceHighlight;

		if (m_currentSetting.WeaponTypeIndexList.Exists(idx => idx == index)) {
			m_currentSetting.WeaponTypeIndexList.Remove(index);
        } else {
			m_currentSetting.WeaponTypeIndexList.Add(index);
        }
    }

	// ボタン : ロック中.
    void DidTapLock()
	{
        if (IsClosed) {
            return;
        }

		this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight = !this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight;
		// ロック中がonでロック中以外もonだったらロック中以外をoffにする.
		if(this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight && this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight){
			this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight = false;
			m_currentSetting.IsVisibleWithoutLock = false;
		}
		m_currentSetting.IsVisibleLock = !m_currentSetting.IsVisibleLock;
	}   
	// ボタン : ロック中以外.
    void DidTapWithoutLock()
    {
        if (IsClosed) {
            return;
        }

		this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight = !this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight;
        if (this.GetScript<CustomButton>("LockOff/bt_Tab").ForceHighlight && this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight) {
            this.GetScript<CustomButton>("LockOn/bt_Tab").ForceHighlight = false;
			m_currentSetting.IsVisibleLock = false;
        }
		m_currentSetting.IsVisibleWithoutLock = !m_currentSetting.IsVisibleWithoutLock;
    }

	// ボタン : 素材.
    void DidTapMaterial()
	{
        if (IsClosed) {
            return;
        }

		this.GetScript<CustomButton>("EnhanceMaterial/bt_Tab").ForceHighlight = !this.GetScript<CustomButton>("EnhanceMaterial/bt_Tab").ForceHighlight;
		m_currentSetting.IsVisibleMaterial = !m_currentSetting.IsVisibleMaterial;
	}

    #endregion   

	private WeaponFilterSetting m_currentSetting;
	private Action<WeaponFilterSetting.Data> m_didClose;

	private static View_WeaponFilterPop instance;
}

/// <summary>
/// 武器フィルター設定情報クラス.恒久的なローカル保存はしないとのこと.
/// </summary>
public class WeaponFilterSetting
{
	/// <summary>現在の設定情報.</summary>
    public Data CurrentData { get; private set; }   

	/// <summary>レアリティ設定.フィルター設定しているもののリスト.</summary>
	public List<int> RarityList { get; set; }
	/// <summary>武器種index設定.フィルター設定しているもののリスト.</summary>
	public List<int> WeaponTypeIndexList { get; set; }
    /// <summary>ロック中を表示するか.</summary>
	public bool IsVisibleLock { get; set; }
	/// <summary>ロック中以外を表示するか.</summary>
	public bool IsVisibleWithoutLock { get; set; }
	/// <summary>素材武器を表示するか.</summary>
    public bool IsVisibleMaterial { get; set; }


    /// <summary>現在の設定でデータを保存する.</summary>
    public void SaveData()
	{
		CurrentData.Apply(this);
	}
	/// <summary>前回情報を復元する.</summary>
    public void LoadData()
	{
		RarityList = CurrentData.RarityList != null ? new List<int>(CurrentData.RarityList): new List<int>();
		WeaponTypeIndexList = CurrentData.WeaponTypeIndexList != null ? new List<int>(CurrentData.WeaponTypeIndexList): new List<int>();
		IsVisibleLock = CurrentData.IsVisibleLock;
		IsVisibleWithoutLock = CurrentData.IsVisibleWithoutLock;
		IsVisibleMaterial = CurrentData.IsVisibleMaterial;
	}

	public WeaponFilterSetting()
	{
		RarityList = new List<int>();
		WeaponTypeIndexList = new List<int>();
		CurrentData = new Data();
	}


	/// <summary>class : WeaponFilterSettingデータ.</summary>
	public class Data
	{
		/// <summary>内容を持っているか？</summary>
		public bool IsHaveContents { get; private set; }
		
		/// <summary>レアリティ設定.フィルター設定しているもののリスト.</summary>
        public int[] RarityList { get; private set; }
        /// <summary>武器種index設定.フィルター設定しているもののリスト.</summary>
		public int[] WeaponTypeIndexList { get; private set; }
        /// <summary>ロック中を表示するか.</summary>
        public bool IsVisibleLock { get; private set; }
		/// <summary>ロック中以外を表示するか.</summary>
        public bool IsVisibleWithoutLock { get; private set; }
        /// <summary>素材武器を表示するか.</summary>
        public bool IsVisibleMaterial { get; private set; }

        /// <summary>データ反映.</summary>
		public void Apply(WeaponFilterSetting setting)
		{
			RarityList = setting.RarityList.ToArray();
			WeaponTypeIndexList = setting.WeaponTypeIndexList.ToArray();
			IsVisibleLock = setting.IsVisibleLock;
			IsVisibleWithoutLock = setting.IsVisibleWithoutLock;
			IsVisibleMaterial = setting.IsVisibleMaterial;
			IsHaveContents = (RarityList != null && RarityList.Length > 0) ||
				             (WeaponTypeIndexList != null && WeaponTypeIndexList.Length > 0) || 
				             IsVisibleLock || IsVisibleWithoutLock || IsVisibleMaterial;
		}
  
		public Data()
		{
			RarityList = null;
			WeaponTypeIndexList = null;
			IsVisibleLock = false;
			IsVisibleWithoutLock = false;
			IsVisibleMaterial = false;
			IsHaveContents = false;
		}
	}
}