using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SmileLab
{    


    /// <summary>
    /// ピンチやスワイプなどのカメラ操作を行う用のモジュール.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class Camera2DTouchModule : ViewBase
    {
        /// <summary>
        /// 有効/無効.
        /// </summary>
        public bool IsEnable = true;


        // PinchInOut時の2Dカメラサイズ変更の上限値.
        [SerializeField,Range(1, 10),HeaderAttribute("ピンチ操作の際のカメラサイズ変更上限値")]
        private float pinchLimitValue = 5f;


        void Update()
        {
            if(!IsEnable){
                return;
            }
            if(Input.touchCount <= 0){
                return; // タッチが一切ないので無視.
            }

            // ピンチ.
            if(Input.touchCount >= 2){
                PintchInOut(Input.GetTouch(0), Input.GetTouch(1));
            }
        }

        #region Internal proc.

        // ピンチイン/アウト処理
        private void PintchInOut(Touch touch1, Touch touch2)
        {
            // 2点タッチを検知.開始時の距離を記憶.
            if (touch2.phase == TouchPhase.Began) {
                m_prevDistance = Vector2.Distance(touch1.position, touch2.position);
                return;
            }
            // 押してるけど動かしてないのでピンチ判定無し.
            if (touch1.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved) {
                return;
            }

            // タッチ位置の移動後、長さを再測し、前回の距離からの相対値を取る。
            float dist = Vector2.Distance(touch1.position, touch2.position);
            var val = (dist-m_prevDistance) / WEIGHT_PINCH;
            // カメラサイズに反映.
            m_cam.orthographicSize += val;
            if(m_cam.orthographicSize > m_baseCameraSize+pinchLimitValue){
                m_cam.orthographicSize = m_baseCameraSize+pinchLimitValue;
            }
            if(m_cam.orthographicSize < m_baseCameraSize-pinchLimitValue){
                m_cam.orthographicSize = m_baseCameraSize-pinchLimitValue;
            }
        }

        #endregion

        void Awake()
        {
            m_cam = this.GetComponent<Camera>();
            m_cam.orthographic = true;
            m_baseCameraSize = m_cam.orthographicSize;
        }

        private Camera m_cam;
        private float m_baseCameraSize; // 最初のorthographicSize.
        private float m_prevDistance;   // 前回ピンチした時の距離.
        private float m_pinchVal;       // Pinch値.
        private static readonly float WEIGHT_PINCH = 100f;  // この値でピンチ操作時の2点感のタッチ距離を割る.
    }

}
