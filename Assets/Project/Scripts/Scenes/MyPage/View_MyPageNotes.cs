using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net;


/// <summary>
/// View : マイページに表示するお知らせ.
/// </summary>
public class View_MyPageNotes : PopupViewBase
{
	/// <summary>
	/// 生成.
	/// </summary>
	public static View_MyPageNotes Create(Dictionary<string, Sprite> bannerDict=null, Action didClose = null)
	{
		var go = GameObjectEx.LoadAndCreateObject("MyPage/View_MyPageNotes");
		var c = go.GetOrAddComponent<View_MyPageNotes>();
		c.InitInternal(bannerDict, didClose);
		return c;
	}
	private void InitInternal(Dictionary<string, Sprite> bannerDict, Action didClose)
	{
		m_myPageBannerDict = bannerDict;
		m_myPageBannerCount = 0;
		m_didClose = didClose;

		this.UpdateNewBadge();

		// バナーリストを読み込み
		if (bannerDict == null) {
			DLCManager.StartBannerDownload(MasterDataTable.banner_setting.EnableData.Select(x => x.image_path).ToArray(), UpdateBanner);
		}

		// リスト.
		DidTapTab(CommonNoticeCategoryEnum.Note);

		// ボタン.
		this.SetCanvasCustomButtonMsg("bt_Close", DidTapClose);
		this.SetCanvasCustomButtonMsg("TabNews/bt_Tab2Off", () => DidTapTab(CommonNoticeCategoryEnum.Note));
		this.SetCanvasCustomButtonMsg("TabUpdate/bt_Tab2Off", () => DidTapTab(CommonNoticeCategoryEnum.Update));
		this.SetCanvasCustomButtonMsg("TabMaintenance/bt_Tab2Off", () => DidTapTab(CommonNoticeCategoryEnum.Bug));

		SetBackButton();
	}

	/// <summary>
	/// 外部からバナーを設定.
	/// </summary>
	public void UpdateBanner(string imageName, Sprite sprite)
	{
		var dat = MasterDataTable.banner_setting.EnableData.FirstOrDefault(d => d.image_path == imageName);
		if (dat == null) {
			return;
		}
		var root = this.GetScript<ScrollRect>("ScrollAreaInfoBanner").content;
		var go = GameObjectEx.LoadAndCreateObject("MyPage/ListItem_Banner", root.gameObject);
		var c = go.GetOrAddComponent<ListItem_Banner>();
		c.UpdateItem(dat, sprite, DidTapClose);
	}

	// 情報リスト更新.
	private void UpdateInfoList(CommonNoticeCategoryEnum category)
	{
		LockInputManager.SharedInstance.IsLock = true;
		View_FadePanel.SharedInstance.IsLightLoading = true;

		var root = this.GetScript<ScrollRect>("ScrollAreaInfoTabView").content;
        root.gameObject.DestroyChildren();

        // 専用にあげているアセットを優先で参照しなければローカルに上がっているであろうマスターを参照する.
		DLCManager.NoticeFromDownloadOrCache(table => {
			var infoList = MasterDataTable.notice.GetListThisPlatform(category);
			if(table != null){
				infoList = table.GetListThisPlatform(category);
			}         
			foreach (var info in infoList) {
				var go = GameObjectEx.LoadAndCreateObject("MyPage/ListItem_MyPageNotes", root.gameObject);
				var c = go.GetOrAddComponent<ListItem_MyPageNotes>();
				c.Init(info);
			}
			this.GetScript<RectTransform>("NoItem").gameObject.SetActive(root.childCount <= 0);

			LockInputManager.SharedInstance.IsLock = false;
            View_FadePanel.SharedInstance.IsLightLoading = false;
		});
    }

    // Newバッジの更新.
	private void UpdateNewBadge()
	{
		AwsModule.NotesModifiedData.UpdateDataAll();      
		this.GetScript<Image>("TabNews/img_TabNew").gameObject.SetActive(AwsModule.NotesModifiedData.IsNew(CommonNoticeCategoryEnum.Note));
		this.GetScript<Image>("TabUpdate/img_TabNew").gameObject.SetActive(AwsModule.NotesModifiedData.IsNew(CommonNoticeCategoryEnum.Update));
		this.GetScript<Image>("TabMaintenance/img_TabNew").gameObject.SetActive(AwsModule.NotesModifiedData.IsNew(CommonNoticeCategoryEnum.Bug));
	}

    protected override void DidBackButton ()
    {
        DidTapClose();
    }

#region ButtonDelegate.

	// ボタン : 閉じる.
	void DidTapClose()
	{
		this.PlayOpenCloseAnimation(false, () => {
            if (m_didClose != null){
                m_didClose();
            }
            base.Dispose();
        });
    }

	// ボタン : タブタップ
	void DidTapTab(CommonNoticeCategoryEnum category)
	{
		this.GetScript<CustomButton>("TabNews/bt_Tab2Off").ForceHighlight = category == CommonNoticeCategoryEnum.Note;
		this.GetScript<CustomButton>("TabUpdate/bt_Tab2Off").ForceHighlight = category == CommonNoticeCategoryEnum.Update;
		this.GetScript<CustomButton>("TabMaintenance/bt_Tab2Off").ForceHighlight = category == CommonNoticeCategoryEnum.Bug;

		this.UpdateInfoList(category);
	}
#endregion

	private Dictionary<string, Sprite> m_myPageBannerDict;
	private int m_myPageBannerCount;

	void Update() {
		//m_myPageBannerDictの監視
		if( m_myPageBannerDict != null ) {
			while( m_myPageBannerCount < m_myPageBannerDict.Count ) {
				var key = m_myPageBannerDict.Keys.ToList()[ m_myPageBannerCount++ ];
				UpdateBanner( key, m_myPageBannerDict[key] );
			}
		}
	}

	private Dictionary<int, ListItem_Banner> m_bannerDict = new Dictionary<int, ListItem_Banner>();
	private Action m_didClose;
}
