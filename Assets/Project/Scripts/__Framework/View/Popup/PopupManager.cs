using UnityEngine;

namespace SmileLab 
{
    
    /// <summary>
    /// ポップアップ生成クラス.
    /// </summary>
    public class PopupManager : ViewBase
    {
    	/*
    	/// OKポップアップ使用例
    		PopupManager.SharedInstance.Create<PopupOK>("PopupOK").Init(
    		"ポップアップ文言",
    		//Okボタンがタップされた時の処理
    		delegate(){
    		});
    		
    	/// YNポップアップ使用例
    		PopupManager.SharedInstance.Create<PopupYN>("PopupYN").Init(
    		"ポップアップ文言",
    		//Yesボタンがタップされた時の処理
    		delegate(){
    		},
    		//Noボタンがタップされた時の処理
    		delegate(){
    		});
    	*/

        public static void OpenPopupOK(string msg, System.Action didTapOk = null)
        {
			SharedInstance.Create<PopupOK> ("PopupOK").Init (msg, CameraHelper.SharedInstance.Camera2D, didTapOk);
        }
		public static void OpenPopupSystemOK(string msg, System.Action didTapOk = null)
        {
			var pop = SharedInstance.Create<PopupOK>("PopupOK");
			pop.gameObject.SetLayerRecursively((int)DefineLayerNo.FADE);
			pop.Init(msg, CameraHelper.SharedInstance.FadeCamera, didTapOk);
        }

        public static void OpenPopupYN(string msg, System.Action didTapYes = null, System.Action didTapNo = null)
        {
            SharedInstance.Create<PopupYN>("Popup_YesNo").Init(msg, CameraHelper.SharedInstance.Camera2D, didTapYes, didTapNo);
        }
        public static void OpenPopupSystemYN(string msg, System.Action didTapYes = null, System.Action didTapNo = null)
        {
			var pop = SharedInstance.Create<PopupYN>("Popup_YesNo");
			pop.gameObject.SetLayerRecursively((int)DefineLayerNo.FADE);
			pop.Init (msg, CameraHelper.SharedInstance.FadeCamera, didTapYes, didTapNo);
        }
        
        /// <summary>スクロール付きOKポップ.</summary>
		public static void OpenPopupOkWithScroll(string msg, System.Action didTapOk = null, string title = null)
		{
			SharedInstance.Create<PopupScroll>("Popup_Scroll").InitOK(msg, CameraHelper.SharedInstance.Camera2D, didTapOk, null, title);
		}
		public static void OpenPopupSystemOkWithScroll(string msg, System.Action didTapOk = null, string title = null)
        {
			var pop = SharedInstance.Create<PopupScroll>("Popup_Scroll");
			pop.gameObject.SetLayerRecursively((int)DefineLayerNo.FADE);
			pop.InitOK(msg, CameraHelper.SharedInstance.FadeCamera, didTapOk, null, title);
        }

		/// <summary>スクロール付きY/Nポップ.</summary>
		public static void OpenPopupYesNoWithScroll(string msg, System.Action didTapYes = null, System.Action didTapNo = null, string title = null)
        {
			SharedInstance.Create<PopupScroll>("Popup_Scroll").InitYN(msg, CameraHelper.SharedInstance.Camera2D, didTapYes, didTapNo, null, title);
        }
		public static void OpenPopupSystemYesNoWithScroll(string msg, System.Action didTapYes = null, System.Action didTapNo = null, string title = null)
        {
            var pop = SharedInstance.Create<PopupScroll>("Popup_Scroll");
            pop.gameObject.SetLayerRecursively((int)DefineLayerNo.FADE);
			pop.InitYN(msg, CameraHelper.SharedInstance.FadeCamera, didTapYes, didTapNo, null, title);
        }


    	/// <summary>
    	/// 共通インスタンス
    	/// </summary>
    	public static PopupManager SharedInstance { get; private set; }
        

        /// <summary>
        /// 作りたいPopの名前を指定して生成.
        /// </summary>
    	public T Create<T>(string name) where T : MonoBehaviour
    	{
            var obj = GameObjectEx.LoadAndCreateObject("_Common/Popup/View_" + name, this.gameObject);
            return obj.GetOrAddComponent<T>();
    	}

    	void Awake()
    	{
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
    	}
    }
}