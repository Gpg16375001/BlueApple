using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SmileLab
{
    /// <summary>
    /// Androidのバックボタン処理.
    /// </summary>
    public class AndroidBackButton : ViewBase
    {
        /// <summary>
        /// 使用可能？
        /// </summary>
        public static bool IsReady { get; private set; }


        /// <summary>
        /// イベント追加.イベントはスタック形式になっていて、登録された新しいものから呼び出される.シーンを跨ぐ事でクリアされる.
        /// </summary>
        /// <returns>登録ID.外部から削除する際に使用する.</returns>
        /// <param name="didTap">タップイベント.</param>
        /// <param name="bDestroy">呼び出し時に削除するかどうか.デフォではtrue.</param>
        public static string SetEventInThisScene(Action didTap, bool bDestroy = true)
        {
            TapEventInfo info;
            info.ID = Guid.NewGuid().ToString();
            info.DidTap = didTap;
            info.IsDestroy = bDestroy;
            m_listDidTap.Add(info);
            return info.ID;
        }

        /// <summary>
        /// イベント削除.
        /// </summary>
        /// <param name="guid">SetEvent時に生成されたGUID.</param>
        public static void DeleteEvent(string guid)
        {
            m_listDidTap.RemoveAll(i => i.ID == guid);
        }

        /// <summary>
        /// 破棄メソッド.
        /// </summary>
        public override void Dispose()
        {
            SceneManager.sceneLoaded -= CallbackChangeScene;
            base.Dispose();
        }

        void Update()
        {
            #if !UNITY_EDITOR
            if(Application.platform != RuntimePlatform.Android){
                return;
            }
            #endif
            if(m_listDidTap.Count <= 0){
                return;
            }
            if(LockInputManager.SharedInstance.IsLock){
                return;
            }

            if(Input.GetKeyDown(KeyCode.Escape)){
                // スタック形式に新しく追加されたものから消化.
                var info = m_listDidTap[m_listDidTap.Count-1];
                if(info.DidTap != null){
                    info.DidTap();
                }
                if(info.IsDestroy){
                    m_listDidTap.Remove(info);
                }
            }
        }

        private void Awake()
        {
            SceneManager.sceneLoaded += CallbackChangeScene;
            IsReady = true;
        }
        // コールバック:シーン切り替え.
        void CallbackChangeScene(Scene scene, LoadSceneMode mode)
        {
            m_listDidTap.Clear();
        }

        private static List<TapEventInfo> m_listDidTap = new List<TapEventInfo>();


        // private struct : 戻る押下時のイベント情報
        private struct TapEventInfo
        {
            public string ID;       // 識別ID.
            public Action DidTap;   // タップイベント.
            public bool IsDestroy;  // 呼び出し時に削除するかどうか.
        }
    }
}