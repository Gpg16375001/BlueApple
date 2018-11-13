using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using SmileLab;

public class View_PartyNameChangePop : ViewBase
{
    /// <summary>
    /// 生成メソッド.
    /// </summary>
    public static View_PartyNameChangePop Create(string initialText, Action<string> didChange)
    {
        var go = GameObjectEx.LoadAndCreateObject("PartyEdit/View_PartyNameChangePop");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
        var instance = go.GetOrAddComponent<View_PartyNameChangePop>();
        instance.InitInternal(initialText, didChange);
        instance.Open ();
        return instance;
    }

    private void InitInternal(string initialText, Action<string> didChange)
    {
        var inputArea = this.GetScript<InputField> ("bt_InputArea");
        inputArea.text = initialText;

        m_didChange = didChange;

        this.SetCanvasButtonMsg ("Cancel/bt_Common", DidTapCancel);
        this.SetCanvasButtonMsg ("bt_Close", DidTapCancel);
        this.SetCanvasButtonMsg ("Change/bt_Common", DidTapChange);
    }

    void DidTapCancel()
    {
        Close (false);
    }

    void DidTapChange()
    {
        Close (true);
    }

    private void Open()
    {
        if (m_bOpen && gameObject.activeSelf) {
            return;
        }
        this.StartCoroutine( this.PlayOpenClose() );
    }

    private void Close(bool isChange)
    {
        if (!m_bOpen && !gameObject.activeSelf) {
            return;
        }
        this.StartCoroutine( this.PlayOpenClose(isChange) );
    }

    // アニメーション開閉処理.
    private IEnumerator PlayOpenClose(bool isChange = false)
    {
        LockInputManager.SharedInstance.IsLock = true;

        if(m_Anime == null){
            m_Anime = this.GetScript<Animation>("AnimParts");
        }
        if(m_bOpen){
            m_Anime.Play("CommonPopClose");
        }else{
            m_Anime.Play("CommonPopOpen");
        }
        yield return null;

        m_bOpen = !m_bOpen;

        // アニメーション終了待ち
        while (m_Anime.isPlaying) {
            yield return null;
        }
        if (!m_bOpen) {
            if (isChange) {
                // クローズ時設定されている文字列を返す
                m_didChange(this.GetScript<InputField>("bt_InputArea").text);
            }
            Dispose ();
        }
        LockInputManager.SharedInstance.IsLock = false;
    }

    private bool m_bOpen = true;        // 開閉状態.
    private Action<string> m_didChange;
    private Animation m_Anime;
}
