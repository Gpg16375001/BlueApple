// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utage;

/// <summary>
/// CGギャラリー画面のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/CgGalleryViewer")]
public class UtageUguiCgGalleryViewer : UguiView, IPointerClickHandler, IDragHandler, IPointerDownHandler
{
	/// <summary>
	/// ギャラリー選択画面
	/// </summary>
	public UtageUguiGallery gallery;

	/// <summary>
	/// CG表示画面
	/// </summary>
	public AdvUguiLoadGraphicFile texture;
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	/// <summary>スクロール対応</summary>
	public ScrollRect ScrollRect
	{
		get
		{
			if (scrollRect == null)
			{
				scrollRect = GetComponent<ScrollRect>();
				if (scrollRect == null)
				{
					scrollRect = this.gameObject.AddComponent<ScrollRect>();
					scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
				}
				if(scrollRect.content == null)
				{
					scrollRect.content = texture.transform as RectTransform;
				}
			}
			return scrollRect;
		}
	}
	[SerializeField]
	ScrollRect scrollRect;

	Vector3 startContentPosition;
	bool isEnableClick;
	bool isLoadEnd;

	AdvCgGalleryData data;
	int currentIndex = 0;

	void Awake()
	{
		texture.OnLoadEnd.AddListener(OnLoadEnd);
	}
	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	public void Open(AdvCgGalleryData data)
	{
		gallery.Sleep();
		this.Open();
		this.data = data;
		this.currentIndex = 0;
		this.startContentPosition = ScrollRect.content.localPosition;
		LoadCurrentTexture();
	}

	/// <summary>
	/// クローズしたときに呼ばれる
	/// </summary>
	void OnClose()
	{
		ScrollRect.content.localPosition = this.startContentPosition;
		texture.ClearFile();
		gallery.WakeUp();
	}

	void Update()
	{
		//右クリックで戻る
		if (InputUtil.IsMouseRightButtonDown())
		{
			Back();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if(isLoadEnd) isEnableClick = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!isEnableClick) return;

		++currentIndex;
		if (currentIndex >= data.NumOpen)
		{
			Back();
			return;
		}
		else
		{
			LoadCurrentTexture();
		}
	}


	public void OnDrag(PointerEventData eventData)
	{
		isEnableClick = false;
	}

	void LoadCurrentTexture()
	{
		isLoadEnd = false;
		isEnableClick = false;
		ScrollRect.enabled = false;
		ScrollRect.content.localPosition = this.startContentPosition;
		AdvTextureSettingData textureData = data.GetDataOpened(currentIndex);
		texture.LoadFile(Engine.DataManager.SettingDataManager.TextureSetting.LabelToGraphic(textureData.Key).Main);
	}

	void OnLoadEnd()
	{
		isLoadEnd = true;
		isEnableClick = false;
		ScrollRect.enabled = true;
	}
}
