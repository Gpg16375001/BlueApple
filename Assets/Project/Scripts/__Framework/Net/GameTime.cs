using System;
using System.Collections;
using UnityEngine;

namespace SmileLab
{
    /// <summary>
    /// 時間管理クラス.ゲーム中の時間はローカル時間をそのまま使わずにこのクラスを介して取得すること.
    /// </summary>
    public class GameTime : ViewBase
    {

        /// <summary>
        /// 共通インスタンス.
        /// </summary>
        public static GameTime SharedInstance { get; private set; }

        /// <summary>
        /// 現在時刻のDateTime.
        /// </summary>
        public DateTime Now { get; private set; }

        /// <summary>
        /// 本日を表すDateTime.今日の0時0分0秒.
        /// </summary>
        /// <value>To day.</value>
        public DateTime Today { get { return new DateTime(Now.Year, Now.Month, Now.Day); } }


        /// <summary>
        /// 引数のDateTimeと時間を同期する.
        /// </summary>
        public void SyncTime(DateTime now)
        {
            if(m_updateRoutine != null) {
                this.StopCoroutine(m_updateRoutine);
                m_updateRoutine = null;
            }
            this.Now = now;
            m_updateRoutine = this.StartCoroutine("UpdateTime");
        }

		/// <summary>
		/// 指定DateタイムがGameTimeから見て期間内かどうかを判定する.
		/// </summary>
		public bool IsWithinPeriod(DateTime start, DateTime end)
		{
			return this.Now >= start && this.Now <= end;
		}

        // 日付はこのクラスが自動更新していく.
        IEnumerator UpdateTime()
        {
            var time = this.Now;
            var startTime = Time.realtimeSinceStartup;
            var waitTime = new WaitForSeconds(0.8f); // 精度を上げる為気持ち早めに更新.
            while(true) {
                var dif = Time.realtimeSinceStartup - startTime;
                this.Now = time.AddSeconds(dif);
                yield return waitTime;
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if(m_updateRoutine == null) {
                return;
            }
            if(focus){
                this.SyncTime(DateTime.Now);    // オフラインを考慮して端末時間でSync.
            }
        }

        void Awake()
        {
            if(SharedInstance != null) {
                SharedInstance.Dispose();
            }
            SharedInstance = this;
            this.SyncTime(DateTime.Now);
        }

        private Coroutine m_updateRoutine;

    }
}
