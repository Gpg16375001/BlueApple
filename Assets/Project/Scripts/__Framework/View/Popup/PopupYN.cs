using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

namespace SmileLab 
{
    /// <summary>
    /// ポップアップ：Yes/No選択
    /// </summary>
    public class PopupYN : PopupViewBase
    {
        /// <summary>
        /// 破棄メソッド.
        /// </summary>
        public override void Dispose()
        {
            if(m_didDispose != null){
                m_didDispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// 初期化.
        /// </summary>
		public void Init(string msg, Camera camera, Action didTapYes, Action didTapNo = null, Action didDispose = null)
    	{
    		m_didTapYes	= didTapYes;
    		m_didTapNo	= didTapNo;
            m_didDispose = didDispose;

            gameObject.AttachUguiRootComponent(camera, 0f);
            this.GetScript<TextMeshProUGUI>("txtp_Popup").SetText(msg);

            this.SetCanvasButtonMsg("Yes/bt_Common", DidTapYes);
            this.SetCanvasButtonMsg("No/bt_Common", DidTapNo);
            SetBackButton ();

            PlayOpenCloseAnimation (true);
    	}

        protected override void DidBackButton ()
        {
            DidTapNo ();
        }

        // ボタン: Yes選択
    	void DidTapYes()
    	{
            if (IsClosed) {
                return;
            }
            PlayOpenCloseAnimation (false, () => {
                if(m_didTapYes != null) {
                    m_didTapYes();
                }
                Dispose();
            });
    	}
        // ボタン: No選択
        void DidTapNo()
    	{
            if (IsClosed) {
                return;
            }
            PlayOpenCloseAnimation (false, () => {
                if(m_didTapNo != null) {
                    m_didTapNo();
                }
                Dispose();
            });
    	}


    	private Action	m_didTapYes;
    	private Action	m_didTapNo;
        private Action  m_didDispose;
        private IgnoreTimeScaleAnimation m_anim;
    }
}
