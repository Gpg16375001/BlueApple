using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SmileLab
{
	/// <summary>
    /// ポップアップ：スクロール式の汎用ポップ.Yes/No選択とOK単独両方いける.
    /// </summary>
	public class PopupScroll : PopupViewBase
    {
		/// <summary>
        /// OKのみとして初期化.
        /// </summary>
		public void InitOK(string msg, Camera camera, Action didTapOk, Action didDispose = null, string title = null)
        {
			m_didTapOk = didTapOk;
            m_didDispose = didDispose;

            gameObject.AttachUguiRootComponent(camera, 0f);
			this.GetScript<RectTransform>("L").gameObject.SetActive(false);
			this.GetScript<RectTransform>("R").gameObject.SetActive(true);
            this.GetScript<TextMeshProUGUI>("txtp_Popup").SetText(msg);
			this.SetCanvasCustomButtonMsg("R/bt_Common", DidTapOk);
			this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").gameObject.SetActive(!string.IsNullOrEmpty(title));
			if(!string.IsNullOrEmpty(title)){
				this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").text = title;
			}
            SetBackButton ();

            PlayOpenCloseAnimation (true);
        }

		/// <summary>
		/// Yes/Noとして初期化.
		/// </summary>
		public void InitYN(string msg, Camera camera, Action didTapYes, Action didTapNo = null, Action didDispose = null, string title = null)
		{ 
			m_didTapYes = didTapYes;
            m_didTapNo = didTapNo;
            m_didDispose = didDispose;

            gameObject.AttachUguiRootComponent(camera, 0f);
			this.GetScript<RectTransform>("L").gameObject.SetActive(true);
            this.GetScript<RectTransform>("R").gameObject.SetActive(true);
            this.GetScript<TextMeshProUGUI>("txtp_Popup").SetText(msg);
			this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").gameObject.SetActive(!string.IsNullOrEmpty(title));
            if (!string.IsNullOrEmpty(title)) {
                this.GetScript<TextMeshProUGUI>("txtp_PopupTitle").text = title;
            }

			this.SetCanvasCustomButtonMsg("R/bt_Common", DidTapYes);
			this.SetCanvasCustomButtonMsg("L/bt_Common", DidTapNo);
            SetBackButton ();

            PlayOpenCloseAnimation (true);
		}

		/// <summary>
        /// 閉じる処理.
        /// </summary>
        protected override void DidBackButton ()
        {
            if (m_didTapOk != null) {
                DidTapOk ();
            } else {
                DidTapNo ();
            }
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

        public override void Dispose ()
        {
            if (m_didDispose != null) {
                m_didDispose ();
            }
            base.Dispose ();
        }

		private Action m_didTapOk;
		private Action m_didTapYes;
		private Action m_didTapNo;
		private Action m_didDispose;
		private IgnoreTimeScaleAnimation m_anim;
    }   
}