using UnityEngine;
using UnityEngine.UI;
using System;

namespace SmileLab
{
    /// <summary>
    /// 各スクリーンコントローラのベースクラス
    /// </summary>
    public abstract class ScreenControllerBase : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// スクリーン生成.ルートにする場合はtrueを渡す.
        /// </summary>
        public static T Create<T>(bool bWithRoot = false) where T : ScreenControllerBase
        {
            var go = new GameObject("ScreenController");
            if (bWithRoot) {
#if USE_NGUI
    	        var root = go.AddComponent<UIRoot>();
    	        root.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
    	        root.manualWidth = 1136;
    	        root.manualHeight = 640;
    	        root.fitWidth = root.fitHeight = true;
#else
                go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D);
#endif
            }
            return go.AddComponent<T>();
        }

        /// <summary>
        /// アンマネージド系のリソースを使うことを考慮
        /// </summary>
        public virtual void Dispose()
        {
            /* 少し待って破棄しないとCanvasの内部再初期化時にクラッシュすることがあるっぽい.
             * Unity2017.3.0f3時点. UnityEditor, iOSで頻発するのを確認.
             * Destroyされてる状態のCanvasにWorldSpaceなどでカメラをアタッチしてるとNull参照になる...？ */
            Destroy(this.gameObject, 0.02f);
        }

        /// <summary>
        /// 初期化.スクリーン展開前の通信処理がある場合はここで.
        /// </summary>
        /// <param name="didConnectEnd">通信終了時の処理</param>
        public virtual void Init(Action<bool/*bSuccess*/> didConnectEnd)
        {
            didConnectEnd(true);
        }

        /// <summary>
        /// コントローラが最初に管理するスクリーンの生成
        /// </summary>
        public abstract void CreateBootScreen();

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
