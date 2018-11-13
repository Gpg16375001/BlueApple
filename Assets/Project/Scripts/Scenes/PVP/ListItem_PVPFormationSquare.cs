using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_PVPFormationSquare : ViewBase {
    public void Init(Formation formation, int position, PvpCardData card)
    {
        var NoneGo = this.GetScript<Transform> ("None").gameObject;
        var ActiveGo = this.GetScript<Transform> ("ActiveSquare").gameObject;
        // indexが５より大きいかつformationがnullなら強制でNone状態に
        if (formation == null || position <= 0 || position > 5) {
            NoneGo.SetActive (true);
            ActiveGo.SetActive (false);
            return;
        }
        NoneGo.SetActive (false);
        ActiveGo.SetActive (true);

        var UnitIconRoot = this.GetScript<Transform> ("UnitIcon").gameObject;
        UnitIconRoot.DestroyChildren ();

        if (card == null || card.CardId == 0) {
            return;
        }
        GameObjectEx.LoadAndCreateObject ("_Common/View/ListItem_UnitIcon", UnitIconRoot).GetOrAddComponent<ListItem_UnitIcon> ().Init (card.ConvertCardData());
    }
}
