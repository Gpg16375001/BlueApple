using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using BattleLogic;

public class View_BattleUnitNotes : ViewBase {
    public static View_BattleUnitNotes Create()
    {
        var go = GameObjectEx.LoadAndCreateObject ("Battle/View_BattleUnitNotes");
        var script = go.GetOrAddComponent<View_BattleUnitNotes> ();
        script.Init ();
        return script;
    }

    public bool IsOpen {
        get;
        private set;
    }

    private void Init()
    {
        this.GetScript<RectTransform>("AnimParts").gameObject.SetActive(false);
    }

    public void Open(GameObject parent, Parameter parameter)
    {
        GetScript<Image> ("img_UnitElement").overrideSprite = IconLoader.LoadElementIcon (parameter.Element);
        GetScript<TextMeshProUGUI> ("txtp_UnitName").SetText (parameter.Name);
        if (parameter.IsCard) {
            GetScript<RectTransform> ("UnitLv").gameObject.SetActive(true);
            GetScript<TextMeshProUGUI> ("txtp_UnitLv").SetText (parameter.Level);
        } else {
            GetScript<RectTransform> ("UnitLv").gameObject.SetActive(false);
        }

        var buffStatus = GetScript<RectTransform> ("GridBuffStatus").gameObject;
        buffStatus.DestroyChildren ();

        if (parameter.Conditions.HasCondition) {
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            foreach (var c in parameter.Conditions.ConditionDataList()) {
                if (c.condition_type == ConditionTypeEnum.AbnormalState) {
                    builder.Append (c.name);
                    builder.Append (' ');
                } else {
                    var go = GameObjectEx.LoadAndCreateObject ("Battle/ListItem_BuffStatusIcon", buffStatus);
                    go.GetOrAddComponent<ListItem_BuffStatusIcon> ().Init (c);
                }
            }
            GetScript<TextMeshProUGUI> ("txtp_debuff").SetText (builder.ToString ());
        } else {
            GetScript<TextMeshProUGUI> ("txtp_debuff").SetText (string.Empty);
        }

        IsOpen = true;
        parent.AddInChild (gameObject);
        this.GetScript<RectTransform>("AnimParts").gameObject.SetActive(true);
        PlayOpenCloseAnimation (true);
    }

    public void Close()
    {
        IsOpen = false;
        PlayOpenCloseAnimation (false, () => {
            this.GetScript<RectTransform>("AnimParts").gameObject.SetActive(false);
        });
    }

    // 開閉アニメーション処理.
    private void PlayOpenCloseAnimation(bool bOpen, System.Action didEnd = null)
    {
        this.StopAllCoroutines ();

        this.StartCoroutine(CoPlayOpenClose(bOpen, didEnd));
    }
    IEnumerator CoPlayOpenClose(bool bOpen, System.Action didEnd)
    {
        var anim = this.GetScript<Animation>("AnimParts");
        anim.gameObject.SetActive (true);
        anim.Play(bOpen ? "BattlePopOpen" : "BattlePopClose");
        do{
            yield return null;
        }while(anim.isPlaying);
        if(didEnd != null){
            didEnd();
        }
    }
}
