using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// スナップするScrollRectです。
/// 通常のScrollbarは使用できませんので、Scrollbarを使う場合はSnapScrollbarを使用してください。
/// </summary>
public class SnapScrollRect : ScrollRect
{
    /// <summary>
    /// Snap終了時呼び出しイベント
    /// </summary>
    [System.Serializable]
    public class OnSnapped : UnityEngine.Events.UnityEvent<int, int> {}
    public OnSnapped SnappedEvent = new OnSnapped ();

    /// <summary>
    /// アイテム位置変更時呼び出しイベント
    /// </summary>
    [System.Serializable]
    public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, int> {}
    public OnItemPositionChange ItemPositionChangeEvent = new OnItemPositionChange ();

    public float smoothness = 10f;
    [SerializeField, Range(0.0f, 1.0f)]
    public float scrollWeight = 0.1f;

    public float BeginSnapMomentVerocity = 0;

    private Vector2 _anchoredPosition;
    public Vector2 anchoredPosition {
        get {
            return content.anchoredPosition;
        }
        set {
            content.anchoredPosition = value;
        }
    }

    public float ItemSpaceWidth {
        get {
            return (itemWidth + spacing_x);
        }
    }

    public float ItemSpaceHeight {
        get {
            return (itemHeight + spacing_y);
        }
    }

    /// <summary>
    /// 子の追加が終わったタイミングで呼び出す。
    /// レイアウトの情報を取得してパラメータの初期化を行う。
    /// </summary>
    public void RecalcParameter()
    {
        if (content == null) {
            return;
        }
            
        var childCount = content.childCount;
        if (childCount <= 0) {
            return;
        }

        // セル情報の取得
        var layoutGroup = content.GetComponent<LayoutGroup> ();
        layoutGroup.CalculateLayoutInputHorizontal ();
        layoutGroup.CalculateLayoutInputVertical ();
        vItemMaxCount = childCount;
        hItemMaxCount = childCount;

        var child = content.GetChild (0) as RectTransform;
        itemWidth = child.rect.width;
        itemHeight = child.rect.height;

        float width = content.rect.width;
        float height = content.rect.height;
        if (layoutGroup != null) {
            width = layoutGroup.preferredWidth;
            height = layoutGroup.preferredHeight;

            hItemMinCount = 0;
            vItemMinCount = 0;
            vItemMaxCount = (int)layoutGroup.flexibleHeight;
            hItemMaxCount = (int)layoutGroup.flexibleWidth;

            if (layoutGroup is GridLayoutGroup) {
                GridLayoutGroup gridLayout = layoutGroup as GridLayoutGroup;
                spacing_x = gridLayout.spacing.x;
                spacing_y = gridLayout.spacing.y;

                itemWidth = gridLayout.cellSize.x;
                itemHeight = gridLayout.cellSize.y;
                InfiniteGridLayoutGroup infiniteGridLayout = layoutGroup as InfiniteGridLayoutGroup;
                if(infiniteGridLayout != null && infiniteGridLayout.IsInfiniteLoop) {
                    if (horizontal) {
                        hItemMinCount = int.MinValue;
                        hItemMaxCount = int.MaxValue;
                    }
                    if (vertical) {
                        vItemMinCount = int.MinValue;
                        vItemMaxCount = int.MaxValue;
                    }

                    if (movementType != MovementType.Unrestricted) {
#if DEBUG
                        Debug.LogError(string.Format("infiniteGridLayout isInfiniteLoop Unsupported MovementType {0} change Unrestricted", movementType));
#endif
                        movementType = MovementType.Unrestricted;
                    }
                } else if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount) {
                    hItemMaxCount = gridLayout.constraintCount;
                    vItemMaxCount = Mathf.Max (1, Mathf.FloorToInt ((gridLayout.minHeight - gridLayout.padding.vertical + gridLayout.spacing.y + 0.001f) / (itemHeight + spacing_y)));
                } else if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount) {
                    vItemMaxCount = gridLayout.constraintCount;
                    hItemMaxCount = Mathf.Max (1, Mathf.FloorToInt ((gridLayout.minWidth - gridLayout.padding.horizontal + gridLayout.spacing.x + 0.001f) / (itemWidth + spacing_x)));
                } else {
#if DEBUG
                    Debug.LogError (string.Format ("Unsupported GridLayoutGroup Constraint {0} ", gridLayout.constraint));
#endif
                    vItemMaxCount = Mathf.Max (1, Mathf.FloorToInt ((gridLayout.minHeight - gridLayout.padding.vertical + gridLayout.spacing.y + 0.001f) / (itemHeight + spacing_y)));
                    hItemMaxCount = Mathf.Max (1, Mathf.FloorToInt ((gridLayout.minWidth - gridLayout.padding.horizontal + gridLayout.spacing.x + 0.001f) / (itemWidth + spacing_x)));
                }
            } else if (layoutGroup is VerticalLayoutGroup) {
                VerticalLayoutGroup verticalLayout = layoutGroup as VerticalLayoutGroup;
                spacing_y = verticalLayout.spacing;
                if (vItemMaxCount == 0) {
                    vItemMaxCount = childCount;
                    hItemMaxCount = 1;
                }
            } else if (layoutGroup is HorizontalLayoutGroup) {
                HorizontalLayoutGroup horizontalLayout = layoutGroup as HorizontalLayoutGroup;
                spacing_x = horizontalLayout.spacing;
                if (hItemMaxCount == 0) {
                    hItemMaxCount = childCount;
                    vItemMaxCount = 1;
                }
            }
        }

        m_IsInitParameter = true;
    }

    public void ScrollTo(int x, int y, bool immediate = false)
    {
        if (!m_IsInitParameter) {
            return;
        }

        hTargetIndex = x;     
        vTargetIndex = y;
        targetPosition = GetSnapPosition();

        if (immediate) {
            hIndex = x;     
            vIndex = y;
            SetContentAnchoredPosition (targetPosition);
        }
    }

    public override void OnBeginDrag (PointerEventData eventData)
    {
        base.OnBeginDrag (eventData);
        SetDragging(true);
    }

    public override void OnEndDrag (PointerEventData eventData)
    {
        base.OnEndDrag (eventData);
        SetDragging(false);
    }

    protected override void Start ()
    {
        base.Start ();
        RecalcParameter ();
    }

    protected override void OnEnable ()
    {
        base.OnEnable ();

        if (horizontalScrollbar != null) {
            if ((horizontalScrollbar is SnapScrollbar) && horizontal) {
                var scrollbar = horizontalScrollbar as SnapScrollbar;
                scrollbar.onDraggingEvent.AddListener (SetDragging);
            }
        }
        if (verticalScrollbar != null) {
            if ((verticalScrollbar is SnapScrollbar) && vertical) {
                var scrollbar = verticalScrollbar as SnapScrollbar;
                scrollbar.onDraggingEvent.AddListener (SetDragging);
            }
        }
    }

    protected override void OnDisable ()
    {
        base.OnDisable ();

        // scrollbarの設定
        if (horizontalScrollbar != null) {
            if ((horizontalScrollbar is SnapScrollbar) && horizontal) {
                var scrollbar = horizontalScrollbar as SnapScrollbar;
                scrollbar.onDraggingEvent.RemoveListener (SetDragging);
            }
        }
        if (verticalScrollbar != null) {
            if ((verticalScrollbar is SnapScrollbar) && vertical) {
                var scrollbar = verticalScrollbar as SnapScrollbar;
                scrollbar.onDraggingEvent.RemoveListener (SetDragging);
            }
        }
    }

    protected override void SetNormalizedPosition (float value, int axis)
    {
        base.SetNormalizedPosition (value, axis);
    }

    protected override void LateUpdate ()
    {
        if (content == null) {
            return;
        }

        base.LateUpdate ();

        if (!m_IsInitParameter) {
            return;
        }

        // 現在位置検出処理
        UpdateIndex ();

        if (!dragging) {
            if (velocity.sqrMagnitude <= BeginSnapMomentVerocity * BeginSnapMomentVerocity) {
                if (prevVelocity != Vector2.zero) {
                    StopMovement ();
                    CalcSnapIndex (prevVelocity);
                    targetPosition = GetSnapPosition ();
                }
                // snap処理
                if (targetPosition != anchoredPosition) {
                    var position = Vector2.Lerp (anchoredPosition, targetPosition, smoothness * Time.deltaTime);
                    if (Vector2.Distance (anchoredPosition, targetPosition) < 1.0f) {
                        position = targetPosition;
                        SnappedEvent.Invoke (hIndex, vIndex);
                    }
                    SetContentAnchoredPosition (position);
                }
            }
        }

        prevVelocity = velocity;
    }

    private void CalcSnapIndex(Vector2 velocity)
    {
        if (!m_IsInitParameter) {
            return;
        }

        float xPage = -1.0f;
        float yPage = -1.0f;

        hTargetIndex = hIndex;
        vTargetIndex = vIndex;

        if (horizontal) {
            xPage = (-1.0f * anchoredPosition.x) / ItemSpaceWidth;
            if (xPage >= 0) {
                float diff = xPage - (float)hIndex;
                if (prevVelocity.x > 0.0f) {
                    diff = 1.0f - diff;
                    if (diff <= scrollWeight) {
                        hTargetIndex++;
                    }
                } else {
                    if (diff >= scrollWeight) {
                        hTargetIndex++;
                    }
                }
            } else {
                float diff = Mathf.Abs (xPage - (float)hIndex);
                if (prevVelocity.x > 0.0f) {
                    diff = 1.0f - diff;
                    if (diff <= scrollWeight) {
                        hTargetIndex--;
                    }
                } else {
                    if (diff >= scrollWeight) {
                        hTargetIndex--;
                    }
                }
            }
            hTargetIndex = Mathf.Clamp (hTargetIndex, hItemMinCount, hItemMaxCount - 1);
        }

        if (vertical) {
            yPage = anchoredPosition.y / ItemSpaceHeight;

            if (yPage >= 0) {
                float diff = yPage - (float)vIndex;
                if (prevVelocity.y < 0.0f) {
                    diff = 1.0f - diff;
                    if (diff <= scrollWeight) {
                        vTargetIndex++;
                    }
                } else {
                    if (diff >= scrollWeight) {
                        vTargetIndex++;
                    }
                }
            } else {
                float diff = Mathf.Abs (yPage - (float)vIndex);
                if (prevVelocity.y < 0.0f) {
                    diff = 1.0f - diff;
                    if (diff <= scrollWeight) {
                        vTargetIndex--;
                    }
                } else {
                    if (diff >= scrollWeight) {
                        vTargetIndex--;
                    }
                }
            }
            vTargetIndex = Mathf.Clamp (vTargetIndex, vItemMinCount, vItemMaxCount - 1);
        }
    }

    private bool IsHorizontalSnap(Bounds viewBounds)
    {
        Debug.Log (m_ContentBounds.min);
        Debug.Log (m_ContentBounds.max);
        return !(movementType != MovementType.Unrestricted &&
            (m_ContentBounds.min.x >= viewBounds.min.x || m_ContentBounds.max.x <= viewBounds.max.x));
    }

    private bool IsVerticalSnap(Bounds viewBounds)
    {
        return !(movementType != MovementType.Unrestricted &&
            (m_ContentBounds.min.y >= viewBounds.min.y || m_ContentBounds.max.y <= viewBounds.max.y));
    }

    private void UpdateIndex()
    {
        if (!m_IsInitParameter) {
            return;
        }

        int oldHIndex = hIndex;
        int oldVIndex = vIndex;

        if (horizontal) {
            hIndex = Mathf.Clamp ((int)((-1.0f * anchoredPosition.x ) / ItemSpaceWidth), hItemMinCount, hItemMaxCount - 1);
        }

        if (vertical) {
            vIndex = Mathf.Clamp ((int)(anchoredPosition.y / ItemSpaceHeight), vItemMinCount, vItemMaxCount - 1);
        }
            
        // 位置変更時通知イベント
        if (oldHIndex != hIndex || oldVIndex != vIndex) {
            ItemPositionChangeEvent.Invoke (hIndex, vIndex);
        }
    }

    private Vector2 GetSnapPosition()
    {
        if (!m_IsInitParameter) {
            return Vector2.zero;
        }

        Bounds viewBounds = new Bounds (viewRect.rect.center, viewRect.rect.size);
        float x = anchoredPosition.x;
        float y = anchoredPosition.y;
        if (horizontal && !(movementType != MovementType.Unrestricted &&
            (
                Mathf.Approximately(viewBounds.max.x, m_ContentBounds.max.x) ||
                viewBounds.max.x > m_ContentBounds.max.x ||
                Mathf.Approximately(viewBounds.min.x, m_ContentBounds.min.x) || 
                viewBounds.min.x < m_ContentBounds.min.x)
        )) {
            x = -1.0f * (itemWidth + spacing_x) * hTargetIndex;
        }

        if (vertical && !(movementType != MovementType.Unrestricted && 
            (
                Mathf.Approximately(viewBounds.max.y, m_ContentBounds.max.y) ||
                viewBounds.max.y > m_ContentBounds.max.y ||
                Mathf.Approximately(viewBounds.min.y, m_ContentBounds.min.y) || 
                viewBounds.min.y < m_ContentBounds.min.y)
        )) {
            y = (itemHeight + spacing_y) * vTargetIndex;
        }
        return new Vector2(x, y);
    }

    private void SetDragging(bool dragging)
    {
        this.dragging = dragging;
        if (!dragging) {
            CalcSnapIndex (prevVelocity);
            targetPosition = GetSnapPosition ();
        }
    }

    private bool m_IsInitParameter = false;
    private bool dragging = false;
    private Vector2 targetPosition;

    private float spacing_x = 0.0f;
    private float spacing_y = 0.0f;

    private float itemWidth;
    private float itemHeight;

    private int hItemMinCount = 0;
    private int vItemMinCount = 0;
    private int hItemMaxCount = 0;
    private int vItemMaxCount = 0;

    private int hIndex = 0;
    private int vIndex = 0;

    private int hTargetIndex = 0;
    private int vTargetIndex = 0;

    private Vector2 prevVelocity = Vector2.zero;
}
