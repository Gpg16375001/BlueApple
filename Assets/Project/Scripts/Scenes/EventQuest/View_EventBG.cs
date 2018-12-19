using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_EventBG : ViewBase {
    // Live2Dの作成

	public void Init(int? top_display_card_1, int? top_display_card_2)
    {
        // 位置のずれ対応
        var rect = gameObject.GetComponent<RectTransform> ();
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        if (top_display_card_1.HasValue) {
            m_UnitResource1 = new UnitResourceLoader (top_display_card_1.Value);
            m_UnitResource1.LoadFlagReset ();
            m_UnitResource1.IsLoadLive2DModel = true;
            m_UnitResource1.LoadResource (LoadUnitResource1);
        }

        if (top_display_card_2.HasValue) {
            m_UnitResource2 = new UnitResourceLoader (top_display_card_2.Value);
            m_UnitResource2.LoadFlagReset ();
            m_UnitResource2.IsLoadLive2DModel = true;
            m_UnitResource2.LoadResource (LoadUnitResource2);
        }

        m_Animation = gameObject.GetComponent<Animation> ();
    }

    public override void Dispose ()
    {
        if (m_UnitResource1 != null) {
            m_UnitResource1.Dispose ();
        }
        if (m_UnitResource2 != null) {
            m_UnitResource2.Dispose ();
        }
        base.Dispose ();
    }

    public void InSelect()
    {
        m_Animation.Play ("TopIn");
    }

    public void InShop()
    {
        m_Animation.Play ("ShopIn");
    }

    public void GotoSelect()
    {
        m_Animation.Play ("ShopToTop");
    }

    public void GotoShop()
    {
        m_Animation.Play ("TopToShop");
    }

    private void LoadUnitResource1(UnitResourceLoader resource)
    {
        var go = Instantiate(resource.Live2DModel) as GameObject;
        var characterAnchorCanvas = GetScript<Canvas> ("CharacterAnchor01");
        go.transform.SetParent(characterAnchorCanvas.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        var renderCntl = go.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>();
        renderCntl.SortingLayer = characterAnchorCanvas.sortingLayerName;
        renderCntl.SortingOrder = characterAnchorCanvas.sortingOrder;
    }

    private void LoadUnitResource2(UnitResourceLoader resource)
    {
        var go = Instantiate(resource.Live2DModel) as GameObject;
        var characterAnchorCanvas = GetScript<Canvas> ("CharacterAnchor02");
        go.transform.SetParent(characterAnchorCanvas.transform);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        var renderCntl = go.GetComponent<Live2D.Cubism.Rendering.CubismRenderController>();
        renderCntl.SortingLayer = characterAnchorCanvas.sortingLayerName;
        renderCntl.SortingOrder = characterAnchorCanvas.sortingOrder;
    }

    UnitResourceLoader m_UnitResource1;
    UnitResourceLoader m_UnitResource2;

    Animation m_Animation;
}
