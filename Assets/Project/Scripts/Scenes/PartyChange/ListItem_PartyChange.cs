using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;

public class ListItem_PartyChange : ViewBase
{
    public void Init(int index, Action<int> didTap)
    {
        var party = AwsModule.PartyData [index];
        var isSelect = AwsModule.PartyData.CurrentTeamIndex == index;

        m_PartyIndex = index;
        m_DidTap = didTap;

        this.GetScript<Text> ("txt_PartyName").text = party.Name;

        this.SetCanvasCustomButtonMsg("bt_SmallList", DidTap);
        if (isSelect) {
            this.GetScript<CustomButton> ("bt_SmallList").ForceHighlight = true;
        }

        this.GetScript<RectTransform> ("img_PVPPartyMark").gameObject.SetActive (party.isPvP);
        if (party.IsEmpty) {
            this.GetScript<Transform> ("txtp_Empty").gameObject.SetActive (true);
            this.GetScript<Transform> ("UnitIconSet").gameObject.SetActive (false);
            this.GetScript<Transform> ("FormationIconRoot").gameObject.SetActive (false);
            return;
        }

        this.GetScript<Transform> ("txtp_Empty").gameObject.SetActive (false);

        var UnitIconSetRoot = this.GetScript<Transform> ("UnitIconSet").gameObject;
        UnitIconSetRoot.SetActive (true);

        for (int i = 1; i <= Party.PartyCardMax; ++i) {
            var cardData = party [i];
            if (cardData == null) {
                continue;
            }
            var go = GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", UnitIconSetRoot);
            var script = go.GetOrAddComponent<ListItem_UnitIcon> ();
            script.Init (cardData);
        }

        var FormationIconRoot = this.GetScript<Transform> ("FormationIconRoot").gameObject;
        FormationIconRoot.SetActive (true);
        var formationIconGo = GameObjectEx.LoadAndCreateObject ("PartyEdit/FormationIcon", FormationIconRoot);
        var formationIcon = formationIconGo.GetOrAddComponent<FormationIcon> ();
        formationIcon.Init (party.FormationData.Formation);
    }

    void DidTap()
    {
        if (m_DidTap != null) {
            m_DidTap (m_PartyIndex);
        }
    }

    private int m_PartyIndex;
    private Action<int> m_DidTap;
}
