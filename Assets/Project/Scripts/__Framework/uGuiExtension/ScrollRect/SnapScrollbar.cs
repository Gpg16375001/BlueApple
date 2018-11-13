using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// SnapScrollRectでスクロールバーを使えるようにするための拡張クラス
/// ドラッグ情報を引き渡せるようにしている。
/// </summary>
public class SnapScrollbar : Scrollbar, IEndDragHandler
{
    [System.Serializable]
    public class OnDragging : UnityEngine.Events.UnityEvent<bool> {}
    public OnDragging onDraggingEvent = new OnDragging ();

    public override void OnBeginDrag (PointerEventData eventData)
    {
        base.OnBeginDrag (eventData);
        onDraggingEvent.Invoke (true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDraggingEvent.Invoke (false);
    }

    public override void OnPointerDown (PointerEventData eventData)
    {
        base.OnPointerDown (eventData);
        onDraggingEvent.Invoke (true);
    }

    public override void OnPointerUp (PointerEventData eventData)
    {
        base.OnPointerUp (eventData);
        onDraggingEvent.Invoke (false);
    }
}
