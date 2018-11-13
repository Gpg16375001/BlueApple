using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace SmileLab
{
    /// <summary>
    /// uGUIを使用したボタンコンポーネント.Canvas管理外のものに適用する.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class uGUIButtonBehaviour : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private UnityEvent clickHandler;

        [SerializeField]
        private EventTriggerDelegate longPressHandler;
        [SerializeField]
        private EventTriggerDelegate releaseHandler;

        // ボタン : 押下した直後.
        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_pressRoutine != null) {
                this.StopCoroutine(m_pressRoutine);
            }
            m_bPressing = true;
            m_pressRoutine = this.StartCoroutine(CheckWaitLongPress(eventData));
        }
        IEnumerator CheckWaitLongPress(PointerEventData data)
        {
            m_pressSec = 0f;
            while (m_bPressing) {
                m_pressSec += Time.unscaledDeltaTime;
                if (!m_bCallLongPress && m_pressSec >= THRESHOLD_LONG_PRESS) {
                    Debug.Log("OnLongPress called.");
                    m_bCallLongPress = true;
                    if(longPressHandler != null){
                        longPressHandler(data);
                    }
                    yield break;
                }
                yield return null;
            }
        }

        public void AddLongPressHandler(EventTriggerDelegate handler)
        {
            longPressHandler = handler;
        }

        // ボタン : クリック判定.離した時.
        public void OnPointerClick(PointerEventData eventData)
        {
            if(m_bCallLongPress){
                return;
            }
            Debug.Log("OnPointerClick called.");
            this.clickHandler.Invoke();
        }
        public void AddClickHandler(UnityAction handler)
        {
            this.clickHandler.AddListener(handler);
        }

        // ボタン : ボタンを離した時.
        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp called.");

            if(releaseHandler != null){
                releaseHandler(eventData);
            }
            // OnPointerClickの判定にロングタップしたかどうかの判定を考慮したいので少し遅れて解放処理を行う.
            Invoke("ReleaseButton", 0.01f);
        }
        public void AddReleaseHandler(EventTriggerDelegate handler)
        {
            this.releaseHandler = handler;
        }

        public Vector3 GetColliderSize()
        {
            var collider = GetComponent<BoxCollider> ();
            return collider.size;
        }

        public void SetColliderSize(Vector3 size)
        {
            var collider = GetComponent<BoxCollider> ();
            collider.size = size;
        }

        // ボタンを離した時の処理.
        private void ReleaseButton()
        {
            m_bPressing = false;
            m_bCallLongPress = false;
            m_pressSec = 0f;
        }

        private Coroutine m_pressRoutine;
        private bool m_bPressing = false;
        private bool m_bCallLongPress = false;
        private float m_pressSec = 0f;
        private const float THRESHOLD_LONG_PRESS = 1f;  // 長押し判定とみなす押下継続時間の閾値.
    }
}
