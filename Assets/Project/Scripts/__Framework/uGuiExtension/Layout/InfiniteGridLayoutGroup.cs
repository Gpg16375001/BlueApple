using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InfiniteGridLayoutGroup : GridLayoutGroup
{

    [System.Serializable]
    public class OnUpdateItem : UnityEngine.Events.UnityEvent<int, GameObject> {}
    /// <summary>
    /// アイテムの内容を変更する時に呼び出されるイベント
    /// </summary>
    public OnUpdateItem OnUpdateItemEvent = new OnUpdateItem ();

    [System.Serializable]
    public class OnInitItem : UnityEngine.Events.UnityEvent<GameObject> {}
    /// <summary>
    /// アイテムの生成時に呼び出されるイベント
    /// </summary>
    public OnInitItem OnInitItemEvent = new OnInitItem ();

    /// <summary>
    /// 最大アイテム数の設定
    /// ロジック的に数を増やすことはできるが減らすことはできない。
    /// </summary>
    public int MaxItemCount {
        get {
            return m_MaxItemCount;
        }
        set {
            if (m_MaxItemCount != value) {
                m_MaxItemCount = value;
                SetDirty ();
            }
        }
    }

    /// <summary>
    /// 無限ループするか
    /// </summary>
    public bool IsInfiniteLoop {
        get {
            return m_LoopItem;
        }
    }   

    /// <summary>
    /// 初期化用関数
    /// これの呼び出しをしないと使用できません。
    /// </summary>
    /// <param name="itemPrototype">アイテムの元になるGameobjectを指定</param>
    /// <param name="instantateCount">生成するアイテム数</param>
    /// <param name="maxItemCount">アイテムの最大数</param>
    /// <param name="loopItem">アイテムをループさせるか</param>
    public void Initialize(GameObject itemPrototype, int instantateCount, int maxItemCount, bool loopItem)
    {
		if(!InitShared()){
			return;
		}
		m_InstantateItemCount = instantateCount;
        m_MaxItemCount = maxItemCount;
        m_LoopItem = loopItem;

        // アイテムの生成
        m_ItemList.Clear ();
        gameObject.DestroyChildren ();
        int InstantiateCount = loopItem ? m_InstantateItemCount : Mathf.Min (m_InstantateItemCount, m_MaxItemCount);
        for(int i = 0; i < InstantiateCount; ++i) {
            var item = GameObject.Instantiate(itemPrototype);
            var rectTrans = item.transform as RectTransform;
            rectTrans.SetParent(transform, false);

            item.name = i.ToString ();
            item.SetActive (true);
            m_ItemList.AddLast (rectTrans);

            int index = i % m_MaxItemCount;
            if (index < 0) {
                index = m_MaxItemCount + index;
            }
            OnInitItemEvent.Invoke (item);
            OnUpdateItemEvent.Invoke (index, item);
        }

		// cell個数の計算
		CalculateCellSize();

        m_IsInit = true;
    }
	public void Initialize(int instantateCount, int maxItemCount, bool loopItem, params GameObject[] prefabs)
	{
		if (!InitShared()) {
            return;
        }
		m_InstantateItemCount = instantateCount;
		m_MaxItemCount = maxItemCount;
		m_LoopItem = loopItem;      
  
        m_ItemList.Clear();
        gameObject.DestroyChildren();
		int InstantiateCount = loopItem ? m_InstantateItemCount : Mathf.Min(m_InstantateItemCount, m_MaxItemCount);
		for (var i = 0; i < InstantiateCount; ++i){
			var prefab = prefabs[(i % prefabs.Length)];
			var item = GameObject.Instantiate(prefab);
			var rectTrans = item.transform as RectTransform;
            rectTrans.SetParent(transform, false);

			item.name = string.Format("{0}_{1}", i, prefab.name);
            item.SetActive(true);
            m_ItemList.AddLast(rectTrans);

            int index = i % m_MaxItemCount;
            if (index < 0) {
                index = m_MaxItemCount + index;
            }
            OnInitItemEvent.Invoke (item);
            OnUpdateItemEvent.Invoke(index, item);         
		}
		
        CalculateCellSize();

        m_IsInit = true;
	}

    /// <summary>
    /// すでに生成されている子オブジェクトをベースに無限ループさせる
    /// </summary>
    /// <param name="loopItem">If set to <c>true</c> loop item.</param>
    public void Initialize(bool loopItem)
    {
        if(!InitShared()){
            return;
        }

        // 
        int childCount = transform.childCount;
        m_InstantateItemCount = childCount;
        m_MaxItemCount = childCount;
        m_LoopItem = loopItem;

        // アイテムの登録
        m_ItemList.Clear ();
        for(int i = 0; i < childCount; ++i) {
            m_ItemList.AddLast (transform.GetChild (i).GetComponent<RectTransform> ());
        }

        // cell個数の計算
        CalculateCellSize();

        m_IsInit = true;
    }

    public void UpdateList(GameObject itemPrototype, int maxItemCount)
    {
        //gameObject.DestroyChildren();
        int InstantiateCount = m_LoopItem ? m_InstantateItemCount : Mathf.Min(m_InstantateItemCount, maxItemCount);
        if (InstantiateCount > m_ItemList.Count) {
            for (int i = m_ItemList.Count; i < InstantiateCount; ++i) {
                var item = GameObject.Instantiate (itemPrototype);
                var rectTrans = item.transform as RectTransform;
                rectTrans.SetParent (transform, false);

                item.name = i.ToString ();
                item.SetActive (true);
                m_ItemList.AddLast (rectTrans);

                OnInitItemEvent.Invoke (item);
            }
        } else if(InstantiateCount < m_ItemList.Count){
            for (int i = InstantiateCount; i < m_ItemList.Count; ++i) {
                m_ItemList.ElementAt(i).gameObject.SetActive (false);
            }
        }

        if (m_MaxItemCount != maxItemCount) {
            m_MaxItemCount = maxItemCount;
            CalculateCellSize ();
        }

        ResetScrollPosition ();
        for (int i = 0; i < InstantiateCount; ++i) {
            var item = m_ItemList.ElementAt (i).gameObject;
            int index = i % m_MaxItemCount;
            if (index < 0) {
                index = m_MaxItemCount + index;
            }
            item.SetActive (true);
            OnUpdateItemEvent.Invoke (index, item);
        }
    }

    /// <summary>
	/// 指定個数ごと非同期でリストを生成する.
	/// instantateCountが多い時は1フレームで一挙に生成するとスタックオーバーフローを起こすのでこちらの使用を推奨.
    /// </summary>
	public void InitializeAsync(GameObject itemPrototype, int instantateCount, int maxItemCount, int createdByCapacity, System.Action didInit, bool loopItem)
    {
        if (!InitShared()) {
            return;
        }
        m_InstantateItemCount = instantateCount;
        m_MaxItemCount = maxItemCount;
        m_LoopItem = loopItem;

        // アイテムの生成
        m_ItemList.Clear();
        gameObject.DestroyChildren();
		this.StartCoroutine(this.AsyncCreateList(itemPrototype, createdByCapacity, () => {
			m_IsInit = true;
			this.ResetScrollPosition();
			if(didInit != null){
				didInit();
			}
		}));
    }
    // 非同期リスト生成.
	private IEnumerator AsyncCreateList(GameObject itemPrototype, int createdByCapacity, System.Action didCreated)
	{
		var wait = new WaitForEndOfFrame();
		var createdCnt = 0;
		int InstantiateCount = m_LoopItem ? m_InstantateItemCount : Mathf.Min(m_InstantateItemCount, m_MaxItemCount);
        for (int i = 0; i < InstantiateCount; ++i) {
            var item = GameObject.Instantiate(itemPrototype);
            var rectTrans = item.transform as RectTransform;
            rectTrans.SetParent(transform, false);

            item.name = i.ToString();
            item.SetActive(true);
            m_ItemList.AddLast(rectTrans);

            int index = i % m_MaxItemCount;
            if (index < 0) {
                index = m_MaxItemCount + index;
            }
            OnInitItemEvent.Invoke (item);
            OnUpdateItemEvent.Invoke(index, item);

			++createdCnt;
			if(createdCnt >= createdByCapacity){
				createdCnt = 0;            
				yield return wait;
			}
        }

        // cell個数の計算
        CalculateCellSize();

		if(didCreated != null){
			didCreated();
		}
	}

    /// <summary>
    /// 指定の番号のオブジェクトが存在すればGameObjectを返す
    /// </summary>
    /// <returns>指定の番号のオブジェクト</returns>
    /// <param name="index">リストの番号</param>
    public GameObject GetItem(int index)
    {
        int topIndex = m_CurrentItemNo % m_MaxItemCount;
        if (topIndex < 0) {
            topIndex = m_MaxItemCount + topIndex;
        }

        int listMaxIndex = Mathf.Max(m_InstantateItemCount, m_MaxItemCount);
        int bottomIndex = topIndex + Mathf.Min(m_InstantateItemCount, m_MaxItemCount);

        if (IsInfiniteLoop) {
            if (bottomIndex >= listMaxIndex) {
                if (topIndex <= index && index <= listMaxIndex) {
                    return m_ItemList.ElementAt (index - topIndex).gameObject;
                } else if(index <= 0 && index <= (bottomIndex % listMaxIndex)) {
                    return m_ItemList.ElementAt (listMaxIndex - topIndex + index).gameObject;
                }
            } else {
                if (topIndex <= index && index <= bottomIndex) {
                    return m_ItemList.ElementAt (index - topIndex).gameObject;
                }
            }
        } else {
            if (topIndex <= index && index <= bottomIndex) {
                return m_ItemList.ElementAt (index - topIndex).gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 指定の番号のオブジェクトが存在すればOnUpdateItemEventを呼び出す
    /// </summary>
    /// <param name="index">リストの番号</param>
    public void UpdateItem(int index)
    {
        var obj = GetItem (index);
        if (obj != null) {
            OnUpdateItemEvent.Invoke (index, obj);
        }
    }

    /// <summary>
    /// スクロールポジションを初期のいちに戻す
    /// </summary>
    /// <param name="isItemUpdate"><c>true</c>の時はアイテムの更新も行う</param>
    public void ResetScrollPosition(bool isItemUpdate=false)
    {
        m_CurrentItemNo = 0;
        m_DiffPreFramePosition = 0.0f;
        rectTransform.anchoredPosition = InitPos;

        SetDirty ();

        if (isItemUpdate) {
            // アイテムの状態も元に戻す
            for (int i = 0; i < m_ItemList.Count; ++i) {
                var item = m_ItemList.ElementAt (i).gameObject;
                int index = i % m_MaxItemCount;
                if (index < 0) {
                    index = m_MaxItemCount + index;
                }
                OnUpdateItemEvent.Invoke (index, item);
            }
        }
    }

    // 共通初期化.
	private bool InitShared()
	{
		ScrollRect scrollRect = GetComponentsInParent<ScrollRect>(true).FirstOrDefault();
        if (scrollRect == null) {
            return false;
        }
        this.enabled = true;
        // 両方へのスクロールには対応していないので変更をかけておく。
        if (scrollRect.horizontal && scrollRect.vertical) {
            if (m_Constraint == Constraint.FixedRowCount) {
#if DEFINE_DEVELOP
                Debug.LogWarning("");
#endif
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            } else if (m_Constraint == Constraint.FixedRowCount) {
#if DEFINE_DEVELOP
                Debug.LogWarning("");
#endif
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
            } else {
#if DEFINE_DEVELOP
                Debug.LogError("");
#endif
            }
        }

        if (scrollRect.horizontal) {
            m_ScrollAxis = 0;
        } else if (scrollRect.vertical) {
            m_ScrollAxis = 1;
        }

		return true;
	}

	private void CalculateCellSize()
	{
		// cell個数の計算
        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;
        int cellCountX = 1;
        int cellCountY = 1;
        if (m_Constraint == Constraint.FixedColumnCount) {
            cellCountX = m_ConstraintCount;
            cellCountY = Mathf.CeilToInt(m_MaxItemCount / (float)cellCountX - 0.001f);
        } else if (m_Constraint == Constraint.FixedRowCount) {
            cellCountY = m_ConstraintCount;
            cellCountX = Mathf.CeilToInt(m_MaxItemCount / (float)cellCountY - 0.001f);
        } else {
            if (cellSize.x + spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

            if (cellSize.y + spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        }
        if (startAxis == Axis.Horizontal) {
            m_CellsPerMainAxis = cellCountX;
        } else {
            m_CellsPerMainAxis = cellCountY;
        }
	}

    #region Calculation Layout
    public override void CalculateLayoutInputHorizontal ()
    {
        if (!m_IsInit) {
            #if DEFINE_DEVELOP
            Debug.LogError("Please call the Initialize function");
            #endif
            this.enabled = false;
            return;
        }

        base.CalculateLayoutInputHorizontal();

        int minColumns = 0;
        int preferredColumns = 0;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            minColumns = preferredColumns = m_ConstraintCount;
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            minColumns = preferredColumns = Mathf.CeilToInt(m_MaxItemCount / (float)m_ConstraintCount - 0.001f);
        }
        else
        {
            minColumns = 1;
            preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(m_MaxItemCount));
        }

        SetLayoutInputForAxis(
            padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
            padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
            -1, 0);
    }

    public override void CalculateLayoutInputVertical ()
    {
        if (!m_IsInit) {
            #if DEFINE_DEVELOP
            Debug.LogError("Please call the Initialize function");
            #endif
            this.enabled = false;
            return;
        }

        int minRows = 0;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            minRows = Mathf.CeilToInt(m_MaxItemCount / (float)m_ConstraintCount - 0.001f);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            minRows = m_ConstraintCount;
        }
        else
        {
            float width = rectTransform.rect.size.x;
            int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));
            minRows = Mathf.CeilToInt(m_MaxItemCount / (float)cellCountX);
        }

        float minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
        SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
    }


    public override void SetLayoutHorizontal ()
    {
        if (!m_IsInit) {
            #if DEFINE_DEVELOP
            Debug.LogError("Please call the Initialize function");
            #endif
            this.enabled = false;
            return;
        }

        SetCellsAlongAxis(0);
    }

    public override void SetLayoutVertical ()
    {
        if (!m_IsInit) {
            #if DEFINE_DEVELOP
            Debug.LogError("Please call the Initialize function");
            #endif
            this.enabled = false;
            return;
        }

        SetCellsAlongAxis(1);
    }

    private void SetCellsAlongAxis(int axis)
    {
        // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
        // and only vertical values when invoked for the vertical axis.
        // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
        // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
        // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

        if (axis == 0)
        {
            // Only set the sizes when invoked for horizontal axis, not the positions.
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform rect = rectChildren[i];

                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = cellSize;
            }
            return;
        }

        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;

        int cellCountX = 1;
        int cellCountY = 1;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            cellCountX = m_ConstraintCount;
            cellCountY = Mathf.CeilToInt(m_MaxItemCount / (float)cellCountX - 0.001f);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            cellCountY = m_ConstraintCount;
            cellCountX = Mathf.CeilToInt(m_MaxItemCount / (float)cellCountY - 0.001f);
        }
        else
        {
            if (cellSize.x + spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

            if (cellSize.y + spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        }

        int cornerX = (int)startCorner % 2;
        int cornerY = (int)startCorner / 2;

        int actualCellCountX, actualCellCountY;
        if (startAxis == Axis.Horizontal)
        {
            m_CellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, m_MaxItemCount);
            actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(m_MaxItemCount / (float)m_CellsPerMainAxis));
        }
        else
        {
            m_CellsPerMainAxis = cellCountY;
            actualCellCountY = Mathf.Clamp(cellCountY, 1, m_MaxItemCount);
            actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(m_MaxItemCount / (float)m_CellsPerMainAxis));
        }

        Vector2 requiredSpace = new Vector2(
            actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
            actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
        );
        Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
        );

        var item = m_ItemList.First;
        for (int i = 0; i < m_ItemList.Count; i++)
        {
            int positionX;
            int positionY;
            int index = m_CurrentItemNo + i;
            if (startAxis == Axis.Horizontal)
            {
                positionX = index % m_CellsPerMainAxis;
                positionY = index / m_CellsPerMainAxis;
            }
            else
            {
                positionX = index / m_CellsPerMainAxis;
                positionY = index % m_CellsPerMainAxis;
            }

            if (cornerX == 1)
                positionX = actualCellCountX - 1 - positionX;
            if (cornerY == 1)
                positionY = actualCellCountY - 1 - positionY;

            SetChildAlongAxis(item.Value, 0, startOffset.x + (cellSize[0] + spacing[0]) * positionX, cellSize[0]);
            SetChildAlongAxis(item.Value, 1, startOffset.y + (cellSize[1] + spacing[1]) * positionY, cellSize[1]);

            item = item.Next;
        }
    }
    #endregion

    void LateUpdate()
    {
        bool isChanged = false;
        // スクロール位置からアイテムの更新をかける。
        while(anchoredPosition - m_DiffPreFramePosition  < -itemSize * 2) {
            if (m_LoopItem || m_MaxItemCount - m_CurrentItemNo > m_InstantateItemCount) {
                m_DiffPreFramePosition -= itemSize;
                // セル個数分処理する
                for (int i = 0; i < m_CellsPerMainAxis; ++i) {
                    isChanged = true;

                    var item = m_ItemList.First.Value;
                    m_ItemList.RemoveFirst ();
                    m_ItemList.AddLast (item);

                    int index = (m_CurrentItemNo + m_InstantateItemCount) % m_MaxItemCount;
                    if (index < 0) {
                        index = m_MaxItemCount + index;
                    }
                    OnUpdateItemEvent.Invoke (index, item.gameObject);
                    m_CurrentItemNo++;
                    if (!m_LoopItem && m_MaxItemCount - m_CurrentItemNo <= m_InstantateItemCount) {
                        break;
                    }
                }
            } else {
                break;
            }
        }

        while(anchoredPosition - m_DiffPreFramePosition > 0) {
            if (m_LoopItem || m_CurrentItemNo > 0) {
                m_DiffPreFramePosition += itemSize;

                // セル個数分処理する
                int count = m_CurrentItemNo % m_CellsPerMainAxis;
                count = count == 0 ? m_CellsPerMainAxis : count;
                for (int i = 0; i < count; ++i) {
                    isChanged = true;

                    var item = m_ItemList.Last.Value;
                    m_ItemList.RemoveLast ();
                    m_ItemList.AddFirst (item);

                    m_CurrentItemNo--;

                    int index = m_CurrentItemNo % m_MaxItemCount;
                    if (index < 0) {
                        index = m_MaxItemCount + index;
                    }
                    OnUpdateItemEvent.Invoke (index, item.gameObject);
                    if (!m_LoopItem && m_CurrentItemNo < 0) {
                        break;
                    }
                }
            } else {
                break;
            }
        }

        // アイテムの位置変更があった時にレイアウトの計算をし直す。
        if (isChanged) {
            SetDirty ();
        }
    }

    protected override void Awake ()
    {
        base.Awake ();
        InitPos = rectTransform.anchoredPosition;
    }

    private float anchoredPosition
    {
        get {
            return m_ScrollAxis == 0 ? rectTransform.anchoredPosition.x : -rectTransform.anchoredPosition.y;
        }
    }

    private float itemSize
    {
        get {
            return m_ScrollAxis == 0 ? cellSize.x + spacing.x : cellSize.y + spacing.y;
        }
    }

    public bool IsInit
    { 
        get {
            return m_IsInit;
        }
    }

    private Vector2 InitPos;

    private bool m_IsInit;
    private int m_InstantateItemCount;
    private int m_MaxItemCount;
    private bool m_LoopItem = false;

    private float m_DiffPreFramePosition;
    protected int m_CurrentItemNo = 0;
    private int m_ScrollAxis = 0;
    private int m_CellsPerMainAxis;

    private LinkedList<RectTransform> m_ItemList = new LinkedList<RectTransform> ();
}
