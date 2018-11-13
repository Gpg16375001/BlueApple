using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// uGUIのページスクロールビュー.デフォルトではない様子.
/// </summary>
public class uGUIPageScrollRect : ScrollRect
{
    /// <summary>
    /// 自動回転時間.0で無回転.
    /// </summary>
    public float RotationInterval
    {
        set {
            if(m_autoRotateRoutine != null) {
                this.StopCoroutine(m_autoRotateRoutine);
            }
            if(value > 0){
                m_valueAutoRotate = value;
                m_autoRotateRoutine = this.StartCoroutine(this.AutoRotation());
            }
        }
    }

    private Coroutine m_autoRotateRoutine = null;
    private float m_valueAutoRotate = 0f;

    /// <summary>スクロールの結果センタリングした際のコールバックイベント.</summary>
	public event Action<GameObject> onCenter;

    protected override void Awake()
    {
        base.Awake();

        Init ();
    }

    /// <summary>
    /// 外部から初期化を行えるようにする
    /// </summary>
    public void Init()
    {
        UpdatePageInfo();
        // LEDランプのグリッド選択.
        var roots = this.GetComponentsInChildren<GridLayoutGroup>();
        gridLed = Array.Find(roots, g => { return g.name == "BannerLedGrid";});
        currentPageIndex = 0;
        SwitchLed();        
    }

    /// <summary>
    /// 現在センタリングしているオブジェクトを取得.※生成直後に行うと正しく取得できないので注意.
    /// </summary>
	public GameObject GetCurrentCenterObject()
	{
		Debug.Log("[uGUIPageScrollRect] GetCurrentCenterObject : currentPageIndex="+currentPageIndex+ "%"+ "maxPage="+maxPage+" => "+Math.Abs(currentPageIndex % maxPage));
		var idx = Math.Abs(currentPageIndex % maxPage);   // 無限スクロール対策
		return content.transform.GetChild(idx).gameObject;
	}

    /// <summary>
    /// 指定GameObjectをセンタリングする.
    /// </summary>
	public void CenterOn(GameObject obj)
	{
		if(obj.transform.parent.GetInstanceID() != content.transform.GetInstanceID()){
			return;
		}
  
		currentPageIndex = Mathf.FloorToInt(obj.transform.localPosition.x / pageWidth)*-1;
		Debug.Log("CenterOn : page "+currentPageIndex);
		PagingProc();
	}

    /// <summary>
    /// 指定ページ数送る.
    /// </summary>
    public void Paging(int plusPageNum)
	{
        if (m_autoRotateRoutine != null) {
            StopCoroutine (m_autoRotateRoutine);
            m_autoRotateRoutine = null;
        }
        currentPageIndex = currentPageIndex + plusPageNum;
        if (!isInfinite) {
            currentPageIndex = Mathf.Clamp (currentPageIndex, -(maxPage - 1), 0);
		}
        PagingProc();

        m_autoRotateRoutine = this.StartCoroutine(this.AutoRotation());
	}

    public void SetInfinit(bool isInf, int pageNumber)
    {
        isInfinite = isInf;
        maxPage = pageNumber;

        UpdatePageInfo ();
    }

    /// <summary>
	/// ページ情報更新.
    /// </summary>
    public void UpdatePageInfo()
	{
		GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        // 1ページの幅を取得.
        pageWidth = grid.cellSize.x + grid.spacing.x;
        if (!isInfinite) {
            maxPage = grid.transform.childCount;
        }
	}

    // 自動回転処理.
    private IEnumerator AutoRotation()
    {
        var waitSec = new WaitForSeconds(m_valueAutoRotate);
        while(m_valueAutoRotate > 0f && !this.IsDestroyed()){
            yield return waitSec;
            --currentPageIndex;
            if(!isInfinite && currentPageIndex < -(maxPage-1)){
                currentPageIndex = 0;
            }
            PagingProc();
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if(m_autoRotateRoutine != null) {
            this.StopCoroutine(m_autoRotateRoutine);
        }
        m_autoRotateRoutine = this.StartCoroutine(this.AutoRotation());
    }

    // ドラッグを開始したとき.
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if(m_autoRotateRoutine != null) {
            this.StopCoroutine(m_autoRotateRoutine);
        }
        base.OnBeginDrag(eventData);
    }

    // ドラッグを終了したとき.
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // ドラッグを終了したとき、スクロールをとめます.
        // スナップさせるページが決まった後も慣性が効いてしまうので.
        StopMovement();

        // スナップさせるページを決定する.
        // スナップさせるページのインデックスを決定する.
        currentPageIndex = Mathf.RoundToInt(content.anchoredPosition.x / pageWidth);      
        // ページが変わっていない且つ、素早くドラッグした場合.
        // ドラッグ量の具合は適宜調整してください.
        if (currentPageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 5)
        {
            currentPageIndex += (int)Mathf.Sign(eventData.delta.x);
        }
		Debug.Log("OnEndDrag : currentPageIndex=" + currentPageIndex);
        // ページング.
        PagingProc();
        m_autoRotateRoutine = this.StartCoroutine(this.AutoRotation());
    }

    private void PagingProc()
    {
		if(bProcessing){
			return;
		}
		bProcessing = true;

        SwitchLed();
        // Contentをスクロール位置を決定する.
        // 必ずページにスナップさせるような位置になるところがポイント.
        iTween.ValueTo(this.gameObject, iTween.Hash(
                "from", content.anchoredPosition.x,
                "to", currentPageIndex * pageWidth,
                "delay", 0,
                "time", 0.3f,
                "easeType", iTween.EaseType.easeInOutSine,
                "onupdatetarget", this.gameObject,
                "onupdate", "OnUpdatePos", 
			    "oncompletetarget", this.gameObject, 
			    "oncomplete", "OnCompletePos")
            );

		// 「ページが変わっていない」の判定を行うため、前回スナップされていたページを記憶しておく.
        prevPageIndex = currentPageIndex;
    }
    void OnUpdatePos(float pos)
    {
        content.anchoredPosition = new Vector2(pos, content.anchoredPosition.y);
    }
    void OnCompletePos()
	{
		if(onCenter != null){
			onCenter(GetCurrentCenterObject());
		}
		bProcessing = false;
	}

    // LEDランプ切り替え.
    private void SwitchLed()
    {
        if(gridLed == null){
            return;
        }

        if (maxPage > 0) {
            int selectIndex = (-currentPageIndex % maxPage);
            if (selectIndex < 0) {
                selectIndex = maxPage + selectIndex;
            }
            foreach (var spt in gridLed.GetComponentsInChildren<uGUISprite>()) {
                var idx = spt.transform.GetSiblingIndex ();
                spt.ChangeSprite (idx != selectIndex ? "bannerled_off" : "bannerled_on");
            }
        }
    }

	private bool bProcessing = false;
    // 1ページの幅.
    private float pageWidth;
    // 前回のページIndex. 最も左を0とする.
    private int prevPageIndex = 0;
    // 現在のページIndex.
    private int currentPageIndex = 0;
    // ページ量.
    private int maxPage = 0;
    private bool isInfinite = false;

    // ページランプ用のgrid.
    private GridLayoutGroup gridLed;
}