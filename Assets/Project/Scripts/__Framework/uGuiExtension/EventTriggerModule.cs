using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace SmileLab
{
    /// <summary>EventTriggerModuleで操作イベントを登録する際のデリゲート.これで統一する.</summary>
    public delegate void EventTriggerDelegate(PointerEventData data);

    /// <summary>
    /// EventTrigger操作用モジュール.EventTriggerは直接スクリプトから操作すると登録が面倒なのでこれを使う.
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class EventTriggerModule : ViewBase
    {
        /// <summary>EventTriggerModule : 押下判定.ボタンから指が離れた際に検知.ロンクタップ時はスキップされる.</summary>
        public EventTriggerDelegate addPress
        {
            set {
                var trigger = this.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(data => OnPress((PointerEventData)data, value));
                trigger.triggers.Add(entry);
            }
        }
        /// <summary>EventTriggerModule : 長押し.長押し判定時addPressの処理はスキップされる.</summary>
        public EventTriggerDelegate addLongPress 
        {
            set {
                var trigger = this.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(data => OnLongPress((PointerEventData)data, value));
                trigger.triggers.Add(entry);
            }
        }
        /// <summary>EventTriggerModule : 押下解除.</summary>
        public EventTriggerDelegate addRelease
        {
            set {
                var trigger = this.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(data => OnRelease((PointerEventData)data, value));
                trigger.triggers.Add(entry);
            }
        }

        #region Internal Events.

        // ボタン : 押下.
        void OnPress(PointerEventData data, EventTriggerDelegate didPress = null)
        {
            if (m_bCallLongPress) {
                return;
            }

            Debug.Log("OnPress called.");
            if (didPress != null) {
                didPress(data);
            }
            this.ReleaseButton();
        }

        // ボタン : 長押し.
        void OnLongPress(PointerEventData data, EventTriggerDelegate didLongPress)
        {
            if (m_pressRoutine != null) {
                this.StopCoroutine(m_pressRoutine);
            }
            m_bPressing = true;
            m_pressRoutine = this.StartCoroutine(CheckWaitLongPress(data, didLongPress));
        }
        IEnumerator CheckWaitLongPress(PointerEventData data, EventTriggerDelegate didLongPress)
        {
            m_pressSec = 0f;
            while (m_bPressing) {
                m_pressSec += Time.unscaledDeltaTime;
                if (!m_bCallLongPress && m_pressSec >= THRESHOLD_LONG_PRESS) {
                    Debug.Log("OnLongPress called.");
                    m_bCallLongPress = true;
                    if (didLongPress != null) {
                        didLongPress(data);
                    }
                    yield break;
                }
                yield return null;
            }
        }
        Coroutine m_pressRoutine;

        // ボタン : ボタンを離した.
        void OnRelease(PointerEventData data, EventTriggerDelegate didRelease)
        {
            Debug.Log("OnRelease called.");
            if(didRelease != null){
                didRelease(data);
            }
            this.ReleaseButton();
        }

        #endregion

        // ボタン押下解除時の処理.
        private void ReleaseButton()
        {
            m_bPressing = false;
            m_bCallLongPress = false;
            m_pressSec = 0f;
        }

        private const float THRESHOLD_LONG_PRESS = 1f;  // 長押し判定とみなす押下継続時間の閾値.

        private bool m_bPressing = false;
        private bool m_bCallLongPress = false;
        private float m_pressSec = 0f;
    }

}