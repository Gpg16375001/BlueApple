using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class View_MagiEquip : ViewBase {
    /// <summary>
    /// 生成.
    /// </summary>
    public static View_MagiEquip Create(CardData cardData, long? EquipedBagId, MagikiteData magikite, Action<MagikiteData> didEquip)
    {
        var go = GameObjectEx.LoadAndCreateObject("UnitDetails/View_MagiEquip");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        var c = go.GetOrAddComponent<View_MagiEquip>();
        c.InitInternal(cardData, EquipedBagId, magikite, didEquip);
        return c;
    }


    private void InitInternal(CardData cardData, long? EquipedBagId, MagikiteData magikite, Action<MagikiteData> didEquip)
    {
        // アイコンの作成
        var IconObject = GameObjectEx.LoadAndCreateObject("_Common/View/ListItem_UnitIcon", GetScript<RectTransform>("UnitIcon").gameObject);
        IconObject.GetOrAddComponent<ListItem_UnitIcon>().Init(cardData);

        m_DidEquip = didEquip;
        m_BeforeMagikite = null;
        if (EquipedBagId.HasValue) {
            m_BeforeMagikite = MagikiteData.CacheGet (EquipedBagId.Value);
        }
        m_AfterMagikite = magikite;
        if (EquipedBagId.HasValue && EquipedBagId.Value == magikite.BagId) {
            // 外す処理
            GetScript<TextMeshProUGUI>("txtp_Equip").SetText("外す");
            if (m_BeforeMagikite != null) {
                var magiMaster = m_BeforeMagikite.Magikite;
                // マギカイトアイコンの表示
                var magikiteRoot = GetScript<RectTransform> ("Before/MagiIcon").gameObject;
                magikiteRoot.SetActive (true);
                var go = GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_MagiIcon", magikiteRoot);         
                var c = go.GetOrAddComponent<ListItem_MagiIcon> ();
                c.UpdateItem (m_BeforeMagikite);

                GetScript<TextMeshProUGUI> ("BeforeMagiName/txtp_MagiName").SetText (magiMaster.name);
                if (magiMaster.skill_id.HasValue && MasterDataTable.skill[magiMaster.skill_id.Value] != null) {
                    GetScript<TextMeshProUGUI> ("txtp_SkillBefore").SetText (MasterDataTable.skill[magiMaster.skill_id.Value].flavor);
                } else {
                    GetScript<TextMeshProUGUI> ("txtp_SkillBefore").SetText ("-");
                }
            }

            // 外す場合は何も表示しない
            GetScript<RectTransform> ("After/MagiIcon").gameObject.SetActive(false);
            GetScript<TextMeshProUGUI> ("AfterMagiName/txtp_MagiName").SetText ("未装備");
            GetScript<TextMeshProUGUI> ("txtp_SkillAfter").SetText ("-");

        } else {
            GetScript<TextMeshProUGUI>("txtp_Equip").SetText("装備する");
            if (m_BeforeMagikite != null) {
                var magiMaster = m_BeforeMagikite.Magikite;
                // マギカイトアイコンの表示
                var magikiteRoot = GetScript<RectTransform> ("Before/MagiIcon").gameObject;
                magikiteRoot.SetActive (true);
                var go = GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_MagiIcon", magikiteRoot);         
                var c = go.GetOrAddComponent<ListItem_MagiIcon> ();
                c.UpdateItem (m_BeforeMagikite);

                GetScript<TextMeshProUGUI> ("BeforeMagiName/txtp_MagiName").SetText (magiMaster.name);
                if (magiMaster.skill_id.HasValue && MasterDataTable.skill[magiMaster.skill_id.Value] != null) {
                    GetScript<TextMeshProUGUI> ("txtp_SkillBefore").SetText (MasterDataTable.skill[magiMaster.skill_id.Value].flavor);
                } else {
                    GetScript<TextMeshProUGUI> ("txtp_SkillBefore").SetText ("-");
                }
            } else {
                GetScript<RectTransform> ("Before/MagiIcon").gameObject.SetActive(false);
                GetScript<TextMeshProUGUI> ("BeforeMagiName/txtp_MagiName").SetText ("未装備");
                GetScript<TextMeshProUGUI> ("txtp_SkillBefore").SetText ("-");
            }

            // 装備するもの
            {
                var magiMaster = m_AfterMagikite.Magikite;
                // マギカイトアイコンの表示
                var magikiteRoot = GetScript<RectTransform> ("After/MagiIcon").gameObject;
                magikiteRoot.SetActive (true);
                var go = GameObjectEx.LoadAndCreateObject ("UnitDetails/ListItem_MagiIcon", magikiteRoot);         
                var c = go.GetOrAddComponent<ListItem_MagiIcon> ();
                c.UpdateItem (m_AfterMagikite);

                GetScript<TextMeshProUGUI> ("AfterMagiName/txtp_MagiName").SetText (magiMaster.name);
                if (magiMaster.skill_id.HasValue && MasterDataTable.skill[magiMaster.skill_id.Value] != null) {
                    GetScript<TextMeshProUGUI> ("txtp_SkillAfter").SetText (MasterDataTable.skill[magiMaster.skill_id.Value].flavor);
                } else {
                    GetScript<TextMeshProUGUI> ("txtp_SkillAfter").SetText ("-");
                }
            }
        }

        SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);
        SetCanvasCustomButtonMsg ("Cancel/bt_CommonS01", DidTapClose);
        SetCanvasCustomButtonMsg ("Equip/bt_CommonS02", DidTapEquip);
    }

    public void Close()
    {
        PlayOpenCloseAnimation (false, Dispose);
    }

    void DidTapEquip()
    {
        if (m_DidEquip != null) {
            m_DidEquip (m_AfterMagikite);
        }
    }

    void DidTapClose()
    {
        PlayOpenCloseAnimation (false, Dispose);
    }

    // 開閉アニメーション処理.
    private void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.Play(bOpen ? "CommonPopOpen" : "CommonPopClose");
        do{
            yield return null;
        }while(anim.isPlaying);
        if(didEnd != null){
            didEnd();
        }
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    MagikiteData m_BeforeMagikite;
    MagikiteData m_AfterMagikite;
    Action<MagikiteData> m_DidEquip;
}
