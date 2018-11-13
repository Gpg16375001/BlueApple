using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.Net.API;
using TMPro;

public class MiniView_BattleMissionRewardPop : ViewBase, IBattleResultPage
{
    /// <summary>
    /// リザルト内のページインデックス.
    /// </summary>
    public int Index { get { return m_index; } }
    private int m_index = 0;

    /// <summary>
    /// 演出中？
    /// </summary>
    public bool IsEffecting { get { return m_bEffecting; } }
    private bool m_bEffecting = false;

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(int index, ReceiveQuestsCloseQuest questResult, Action didOpen)
    {
        DidOpen = didOpen;
        m_index = index;
        var dropItem = questResult.MissionRewardItemList [0];
        var itemType = (ItemTypeEnum)dropItem.ItemType;
        var iconInfo = itemType.GetIconInfo (dropItem.ItemId,
            itemType != ItemTypeEnum.card && itemType != ItemTypeEnum.weapon);

        if (iconInfo.IsEnableSprite) {
            GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);

            var uGuiSprite = GetScript<uGUISprite> ("ItemAnchor");
            uGuiSprite.gameObject.SetActive (true);
            uGuiSprite.LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
        } else if (iconInfo.IconObject != null) {
            Transform root;
            if (itemType != ItemTypeEnum.card && itemType != ItemTypeEnum.weapon) {
                GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                var image = GetScript<Image> ("ItemAnchor");
                image.enabled = false;
                image.gameObject.SetActive (true);
                root = image.transform;
            } else {
                GetScript<Image> ("ItemAnchor").gameObject.SetActive(false);
                root = GetScript<RectTransform> ("UnitWeaponRoot");
                root.gameObject.SetActive (true);
            }

            iconInfo.IconObject.transform.SetParent (root);
            iconInfo.IconObject.transform.localPosition = Vector3.zero;
            iconInfo.IconObject.transform.localScale = Vector3.one;
            iconInfo.IconObject.transform.localRotation = Quaternion.identity;
        }
        GetScript<TextMeshProUGUI> ("txtp_MissionRewardName").SetText (itemType.GetName (dropItem.ItemId));
        GetScript<TextMeshProUGUI> ("txtp_ItemName").SetText (itemType.GetName (dropItem.ItemId));
        GetScript<TextMeshProUGUI> ("txtp_RewardNum").SetText (dropItem.Quantity);
    }

    /// <summary>開く.</summary>
    public void Open()
    {
        if (!this.gameObject.activeSelf) {
            this.gameObject.SetActive(true);
        }

        if(DidOpen != null) {
            DidOpen();
        }
        m_bEffecting = true;
        StartCoroutine(PlayOpenClose (true,
            () => {

                m_bEffecting = false;
            }
        ));
    }

    /// <summary>閉じる.</summary>
    public void Close(Action didEnd)
    {
        StartCoroutine(PlayOpenClose (false, didEnd));
    }

    private IEnumerator PlayOpenClose(bool bOpen, Action didEnd = null)
    {
        var anim = GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
        do {
            yield return null;
        } while (anim.isPlaying);
        if (didEnd != null) {
            didEnd();
        }
    }
    /// <summary>アニメーションを強制的に即時終了させる.</summary>
    public void ForceImmediateEndAnimation()
    {
        //m_bEffecting = false;
    }

    Action DidOpen;
}
