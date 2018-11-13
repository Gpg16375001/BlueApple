using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// 複数のトグル式ボタン管理クラス(uGUI).
/// </summary>
public class MultiToggleButtonGroup : ViewBase
{
    // トグル化したいボタンリスト.
    [SerializeField]
    private List<Button> buttonList;

    /// <summary>トグルアクティブ時イベント.Initごとにリセットされる.</summary>
    public event Action<int/*index*/> DidActiveToggleEvent;


    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(int activeIdx = 0)
    {
        DidActiveToggleEvent = null;
        m_tglList.Clear();

        var index = 0;
        foreach(var btn in buttonList){
            var tgl = btn.gameObject.GetOrAddComponent<ToggleButton>();
            tgl.Init(index, DidTapToggleButton);
            m_tglList.Add(tgl);
            ++index;
        }
        DidTapToggleButton(activeIdx);
    }

    /// <summary>
    /// 手動でアクティブ化させる.
    /// </summary>
    public void ManualActivate(int index)
    {
        DidTapToggleButton(index);
    }

    // ボタン : トグル化させたボタン押下.
    void DidTapToggleButton(int index)
    {
        foreach(var tgl in m_tglList){
            tgl.IsOn = tgl.Index == index;
        }
        if(DidActiveToggleEvent!= null){
            DidActiveToggleEvent(index);
        }
    }

    private List<ToggleButton> m_tglList = new List<ToggleButton>(); 


    // private class : トグルボタンコンポーネント
    [RequireComponent(typeof(Button))]
    private class ToggleButton : ViewBase
    {
        /// <summary>インデックス.</summary>
        public int Index { get; private set; }

        /// <summary>On状態？</summary>
        public bool IsOn 
        { 
            get{
                return m_bOn;
            }
            set {
                var btn = this.GetComponent<Button>();
                btn.image.sprite = value ? m_sptHighlight: m_sptNormal;
                m_bOn = value;
            }
        }
        private bool m_bOn;

        /// <summary>初期化.</summary>
        public void Init(int index, Action<int> didTap)
        {
            this.Index = index;
            m_didTap = didTap;

            var btn = this.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            if(m_sptNormal == null || m_sptHighlight == null){
                m_sptNormal = btn.image.sprite;
                m_sptHighlight = btn.spriteState.highlightedSprite;
            }
            btn.onClick.AddListener(DidTapButton);
        }

        // ボタン : タップ.
        void DidTapButton()
        {
            if(m_didTap != null){
                m_didTap(this.Index);
            }
        }

        private Action<int> m_didTap;
        private Sprite m_sptNormal;
        private Sprite m_sptHighlight;
    }
}
