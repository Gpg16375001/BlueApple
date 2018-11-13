using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class TMP_DropDownEx : TMP_Dropdown
{
    private ScrollRect sr;

    protected override GameObject CreateDropdownList (GameObject template)
    {
        var go = base.CreateDropdownList (template);
        sr = go.GetComponent<ScrollRect> ();
        return go;
    }
    public override void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData)
    {
        Show ();
        SetPosition ();
    }

    public override void OnSubmit (UnityEngine.EventSystems.BaseEventData eventData)
    {
        Show ();
        SetPosition ();
    }

    private void SetPosition()
    {
        if (options.Count <= 1) {
            return;
        }

        if (sr != null) {
            if (sr.horizontal && sr.vertical) {
            } else if (sr.horizontal) {
                sr.horizontalNormalizedPosition = 1.0f - ((float)value / (float)(options.Count - 1));
                sr.SetLayoutHorizontal ();
            } else if (sr.vertical) {
                sr.verticalNormalizedPosition = 1.0f - ((float)value / (float)(options.Count - 1));
                sr.SetLayoutVertical ();
            }
        }
    }
}
