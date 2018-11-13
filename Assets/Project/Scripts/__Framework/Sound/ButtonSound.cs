using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using SmileLab.UI;

namespace SmileLab
{
    /// <summary>
    /// クラス：ボタン押した時のSE再生用スクリプト.
    /// </summary>
    [DisallowMultipleComponent]
    public class ButtonSound : MonoBehaviour
    {

        [SerializeField]
        private ReceiveType receiveType = ReceiveType.Button;
    
        [SerializeField]
        private SoundClipName soundClip;

        void Awake ()
        {
            this.SetReceiver ();
        }
    
        // レシーバーに設定.
        private void SetReceiver ()
        {
            switch (receiveType) {
            case ReceiveType.Button:
            // ボタンタップイベント
                Button button = this.GetComponent<Button> ();
                button.onClick.AddListener (() => {
					if (button.interactable) {
#if DEFINE_DEVELOP
                        Debug.Log ("play se " + soundClip.ToString () + " " + name);
#endif
                        SoundManager.SharedInstance.PlaySE (soundClip);
                    }
                });
                break;
            case ReceiveType.Toggle:
            // トグルタップイベント
                Toggle toggle = this.GetComponent<Toggle> ();
                toggle.onValueChanged.AddListener ((bChange) => {
					if (toggle.interactable) {
                        // なんらかのグループに所属しているトグルだった場合はアクティブ時にのみ音を再生させる.
                        if (toggle.group != null) {
                            if (!bChange) {
                                SoundManager.SharedInstance.PlaySE (soundClip);
                            }
                        }
                    // それ以外は値が切り替わるごとに再生
                    else {
                            SoundManager.SharedInstance.PlaySE (soundClip);
                        }
                    }
                });
                break;
            }
        }
    
        public SoundClipName GetSoundClip()
        {
            return soundClip;
        }

        // サウンドを鳴らす対象タイプ.
        private enum ReceiveType
        {
            Button,
            Toggle,
        }
    }
}
