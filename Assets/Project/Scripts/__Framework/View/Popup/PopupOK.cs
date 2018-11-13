using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

namespace SmileLab 
{
    /// <summary>
    /// ポップアップ：閉じるのみ
    /// </summary>
    public class PopupOK : PopupViewBase
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
		public void Init(string msg, Camera camera, Action didTapOk = null, Action didDispose = null)
    	{
    		m_didTapOk	= didTapOk;
            m_didDispose = didDispose;

            gameObject.AttachUguiRootComponent(camera, 0f);
            this.GetScript<TextMeshProUGUI>("txtp_Popup").SetText(msg);
            this.SetCanvasCustomButtonMsg("bt_Close", DidTapOk);

            SetBackButton ();

            PlayOpenCloseAnimation (true);
    	}
    	
        protected override void DidBackButton ()
        {
            DidTapOk ();
        }

        // ボタン: OKタップ.
        void DidTapOk()
        {
            if (IsClosed) {
                return;
            }
            PlayOpenCloseAnimation (false, () => {
                if(m_didTapOk != null) {
                    m_didTapOk();
                }
                Dispose();
            });
        }

    	private Action	m_didTapOk;
        private Action  m_didDispose;
        private IgnoreTimeScaleAnimation m_anim;
    }
}