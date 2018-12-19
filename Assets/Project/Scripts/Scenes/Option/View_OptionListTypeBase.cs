using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SmileLab;


/// <summary>
/// View : 設定画面リストタイプ基底View.
/// </summary>
public abstract class View_OptionListTypeBase : ViewBase, IViewOption
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public virtual void Init(OptionBootMenu boot)
    {
        m_boot = boot;

        this.CreateList();

        // グローバルメニュー関連の設定.
        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
		View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;
    }

    // リスト作成.
    private void CreateList()
    {
		var rootObj = this.GetScript<VerticalLayoutGroup>("SideMenuList").gameObject;
        rootObj.DestroyChildren();
        rootObj.transform.DetachChildren();
        var menuList = MasterDataTable.option_menu.DataList.FindAll(d => d.BootMenuType == m_boot.Enum);
        foreach (var menu in menuList) {
            var go = GameObjectEx.LoadAndCreateObject("Option/ListItem_Option", rootObj);
            var c = go.GetOrAddComponent<ListItem_Option>();
            c.Init(menu, DidTapMenu);
        }

        // 先頭オブジェクトを選択状態とする.
        var first = rootObj.GetComponentsInChildren<ListItem_Option>()[0];
        DidTapMenu(first.Menu);
    }

    /// Viewモード切り替え 
    protected void SwitchViewMode(ViewMode mode)
    {
        this.GetScript<RectTransform>("GridView").gameObject.SetActive(mode == ViewMode.Grid);
        this.GetScript<RectTransform>("GridTabView").gameObject.SetActive(mode == ViewMode.GridTab);
        this.GetScript<RectTransform>("SoundSettingView").gameObject.SetActive(mode == ViewMode.SoundSetting);
        this.GetScript<RectTransform>("PushSettingView").gameObject.SetActive(mode == ViewMode.PushSetting);
        this.GetScript<RectTransform>("DataBackupView").gameObject.SetActive(mode == ViewMode.Inherit);
        this.GetScript<RectTransform>("TextOnlyView").gameObject.SetActive(mode == ViewMode.TextOnly);
        this.GetScript<RectTransform>("FriendView").gameObject.SetActive(mode == ViewMode.Friend);
		this.GetScript<RectTransform>("DictionaryView").gameObject.SetActive(mode == ViewMode.Dictionary);
		this.GetScript<RectTransform>("ContactView").gameObject.SetActive(mode == ViewMode.Contact);
    }

    #region ButtonDelegate.

    // ボタン : 戻る.
    void DidTapBack()
    {
        // オプションメニューを開いた状態で戻る.現状マイページからしから設定にいけないのでこれで.
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim(View_FadePanel.FadeColor.Black, () => {
            if (m_currentMiniView != null) {
                m_currentMiniView.AsyncDetach(() => {
                    View_OptionSupport.DataClear();
                    ScreenChanger.SharedInstance.GoToMyPage(() => View_OptionTopPop.Create());
                });
                return;
            }
            ScreenChanger.SharedInstance.GoToMyPage(() => View_OptionTopPop.Create());
        });
    }

    // ボタン : メニュー選択時.コールバックで設定.
    protected virtual void DidTapMenu(OptionMenu menu)
    {
        if(m_currentMenu != null && m_currentMenu.Enum == menu.Enum){
            return; // 重複タップは禁止.
        }

        Debug.Log("DidTap Menu : menu=" + menu.Enum + "/" + menu.name);
        if (m_currentMiniView != null) {
            m_currentMiniView.AsyncDetach(() => ChangeView(menu));
            return;
        }
        ChangeView(menu);
    }
    /// View切り替えの処理.
    protected virtual void ChangeView(OptionMenu menu)
    {
        if (menu.Enum != OptionMenuEnum.BackTitle) {
            this.SelectThisMenu(menu.Enum);
            m_currentMenu = menu;
        }
    }

    #endregion

    /// 指定オプションメニューを選択.
    protected void SelectThisMenu(OptionMenuEnum selected)
    {
		var rootObj = this.GetScript<VerticalLayoutGroup>("SideMenuList").gameObject;
        foreach(var menu in rootObj.GetComponentsInChildren<ListItem_Option>()){
            menu.IsSelected = menu.Menu.Enum == selected;
        }
    }
    /// 指定属性タブ項目を選択.
    protected void SelectThisElement(ListItem_HorizontalElementTab selected)
    {
        var rootObj = this.GetScript<GridLayoutGroup>("TabMenu").gameObject;
        foreach (var tab in rootObj.GetComponentsInChildren<ListItem_HorizontalElementTab>()){
			tab.IsSelected = tab.CategoryName == selected.CategoryName;
        }
    }
	/// 指定武器タブ項目を選択.
	protected void SelectThisWeapon(ListItem_HorizontalWeaponTab selected)
    {
        var rootObj = this.GetScript<GridLayoutGroup>("TabMenu").gameObject;
		foreach (var tab in rootObj.GetComponentsInChildren<ListItem_HorizontalWeaponTab>()) {
			tab.IsSelected = tab.WeaponID == selected.WeaponID;
        }
    }

    void Awake()
    {
        // 独立したUIなのでCanvas設定.
        this.gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
    }

    private OptionBootMenu m_boot;
    protected OptionMenu m_currentMenu;
    protected OptionMiniViewBase m_currentMiniView;

    // enum : 表示モード
    protected enum ViewMode
    {
        Grid,
        GridTab,
        SoundSetting,
        PushSetting,
        Inherit,
        TextOnly,   // 単調にテキストがずらっと並ぶ表示モード.
        Friend,
        Dictionary, // 項目がならび、タップでその詳細が見られる表示モード.
        Contact,    // お問い合わせ
    }
}
