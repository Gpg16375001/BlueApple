using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using BattleLogic;

public class View_BattleCommandNotes : ViewBase {
    public static View_BattleCommandNotes Create(Transform parent)
    {
        var go = GameObjectEx.LoadAndCreateObject ("Battle/View_BattleCommandNotes", parent.gameObject);
        var script = go.GetOrAddComponent<View_BattleCommandNotes> ();
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

    public void Open(SkillParameter skill) {
        GetScript<TextMeshProUGUI> ("txtp_SkillName").SetText (skill.Skill.display_name);
        GetScript<TextMeshProUGUI> ("txtp_SkillDescription").SetText (skill.Skill.flavor);

        IsOpen = true;
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
