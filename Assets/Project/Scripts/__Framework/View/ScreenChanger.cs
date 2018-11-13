using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
//using JsonDataParser;

namespace SmileLab
{
    /// <summary>
    /// スクリーン切り替えロジック
    /// </summary>
    public partial class ScreenChanger : ViewBase
    {
        /// シーン切り替え前にやりたい処理
        public static event Action<string/*nextSceneName*/> WillChangeScene;
        /// シーン切り替え後にやりたい処理
        public static event Action<string/*nextSceneName*/> DidEndChangeScene;

        /// <summary>
        /// 共通インスタンス
        /// </summary>
        public static ScreenChanger SharedInstance { get; private set; }

        /// <summary>
        /// 直前のシーン名.
        /// </summary>
        public string PrevSceneName { get; private set; }

        public ScreenControllerBase CurrentController {
            get {
                return m_currentCtrl;
            }
        }

        #region リブート処理.

        /// <summary>
        /// アプリリブート.
        /// </summary>
        public void Reboot(bool bDestroyCache = false)
        {
            this.StartCoroutine(this.RebootScene(bDestroyCache));
        }
        /// <summary>
        /// リブート時のEditorでの無限ループ回避用処理
        /// </summary>
        /// <returns>The scene.</returns>
        private IEnumerator RebootScene(bool bDestroyCache)
        {
            // ここで1フレーム待つと無限ループを回避できる。
            // 他のシーンでやるのは冗長的すぎるのでRebootでだけする。
            yield return null;

			// 宴は強制的にキャッシュをクリアしておかないと多重ロードでエラーになってしまう.
			if(UtageModule.SharedInstance != null){
				UtageModule.SharedInstance.ClearCache();
                UtageModule.SharedInstance.DestroyCore();
			}         

            if(m_currentCtrl != null) {
                m_currentCtrl.Dispose();
                m_currentCtrl = null;
            }
            if(bDestroyCache) {
                GameSystem.ClearChache(GameSystem.DownloadDirectoryPath);   // ダウンロード周りのキャッシュは削除する.
                Caching.ClearCache();   // アセット類のキャッシュもクリア.
            }

            yield return null;

            ChangeScene ("boot", null);
        }

        #endregion

        #region internal proc.

        // シーン切り替え実行
        private void Exec(string nextSceneName, ScreenControllerBase ctrl, Action didProcEnd = null)
        {
            if(m_currentCtrl != null) {
                m_currentCtrl.Dispose();
                m_currentCtrl = null;
            }
            m_currentCtrl = ctrl;

            this.ChangeScene(nextSceneName, delegate () {
                m_currentCtrl.Init(delegate (bool bSuccess) {
                    // 通信エラー時はスクリーンを展開せずにそのまま
                    if(!bSuccess) {
                        Debug.LogWarning("[ScreenChanger - Exec] Connection error.");
                        return;
                    }
                    // シーン名更新.
                    if (!string.IsNullOrEmpty(m_currentSceneName)) {
                        PrevSceneName = m_currentSceneName;
                    }
                    m_currentSceneName = nextSceneName;
                    // 新しいシーン起動.
                    m_currentCtrl.CreateBootScreen();
                    if(didProcEnd != null) {
                        didProcEnd();
                    }
                });
            });
        }
        private void ChangeScene(string nextSceneName, Action didProcEnd = null)
        {
            this.StartCoroutine(this.ChangeSceneproc(nextSceneName, didProcEnd));
        }
        private IEnumerator ChangeSceneproc(string nextSceneName, Action didProcEnd)
        {
            if(WillChangeScene != null) {
                WillChangeScene(nextSceneName);
            }

            SceneManager.LoadScene(nextSceneName);

            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            if(DidEndChangeScene != null) {
                DidEndChangeScene(nextSceneName);
            }

            if(didProcEnd != null) {
                didProcEnd();
            }
        }

        void Awake()
        {
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
        }

        private ScreenControllerBase m_currentCtrl;
        private string m_currentSceneName;

        #endregion

    }
}
