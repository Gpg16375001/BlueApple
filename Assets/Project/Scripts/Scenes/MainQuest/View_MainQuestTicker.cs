using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using TMPro;


/// <summary>
/// View : メインクエストのティッカー表示対応.
/// </summary>
public class View_MainQuestTicker : ViewBase
{

    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init()
	{
		m_txtBase = this.GetScript<TextMeshProUGUI>("txtp_Ticker");
		m_barWidth = m_txtBase.rectTransform.sizeDelta.x+BUFFA_WIDTH;

		var dataList = MasterDataTable.quest_main_common_info.DataList;
		var list = dataList.Where(d => !d.is_disabled && d.start_date <= GameTime.SharedInstance.Now && d.end_date >= GameTime.SharedInstance.Now)
		                   .Select(d => d.ticker_text).ToList();
		if(list.Count <= 0){
			this.gameObject.SetActive(false);   // 表示するものがなければルートごと消す.
			return;
		}
		foreach(var msg in list){
			var go = GameObject.Instantiate(m_txtBase.gameObject) as GameObject;
			go.transform.SetParent(this.GetScript<Transform>("Mask"));
			go.transform.SetPositionAndRotation(m_txtBase.transform.position, m_txtBase.transform.rotation);
			go.transform.localScale = m_txtBase.transform.localScale;
			go.transform.localPosition = new Vector3(go.transform.localPosition.x+m_barWidth, go.transform.localPosition.y, go.transform.localPosition.z);
			var tm = go.GetComponent<TextMeshProUGUI>();
			tm.text = msg;
			m_txtQue.Enqueue(tm);         
		}

		m_txtBase.gameObject.SetActive(false);
		this.StartMoveText();
	}

	#region MoveTransition.

	// 文字移動開始.
	private void StartMoveText()
	{
		m_currentMoveText = m_txtQue.Dequeue();
        var moveHash = new Hashtable();
		moveHash.Add("x", m_txtBase.transform.localPosition.x);
        moveHash.Add("isLocal", true);
        moveHash.Add("time", 1.5f);
		moveHash.Add("easetype", iTween.EaseType.easeOutQuad);
		moveHash.Add("oncomplete", "EndMoveText");
		moveHash.Add("oncompletetarget", this.gameObject);
		iTween.MoveTo(m_currentMoveText.gameObject, moveHash);      
	}   
    // 所定位置まで移動完了.少しの間そこに表示し続ける.
    private void EndMoveText()
	{
		this.StartCoroutine(WaitDisp(WAIT_DISP_SEC, LeaveText));
	}

    // 文字履け開始.履けた後次の文字表示までインターバルあり.
    private void LeaveText()
	{
        var moveHash = new Hashtable();
		moveHash.Add("x", (m_txtBase.transform.localPosition.x - (m_currentMoveText.preferredWidth+BUFFA_WIDTH)));
        moveHash.Add("isLocal", true);
        moveHash.Add("time", 3f);
		moveHash.Add("easetype", iTween.EaseType.linear);
		moveHash.Add("oncomplete", "EndLeaveText");
        moveHash.Add("oncompletetarget", this.gameObject);
        iTween.MoveTo(m_currentMoveText.gameObject, moveHash);
	}   
	// 履けた後の次の文字表示までインターバル.
    private void EndLeaveText()
	{
		var dataList = MasterDataTable.quest_main_common_info.DataList;
        var list = dataList.Where(d => !d.is_disabled && d.start_date <= GameTime.SharedInstance.Now && d.end_date >= GameTime.SharedInstance.Now)
                           .Select(d => d.ticker_text).ToList();
        if (list.Count <= 0) {
            this.gameObject.SetActive(false);   // 表示するものがなければルートごと消す.
            return;
        }      
        // 追加.
		foreach (var msg in list) {
			if(m_txtQue.Any(t => t.text == msg) || m_currentMoveText.text == msg){
				continue;
			}
            var go = GameObject.Instantiate(m_txtBase.gameObject) as GameObject;
            go.transform.SetParent(this.GetScript<Transform>("Mask"));
            go.transform.SetPositionAndRotation(m_txtBase.transform.position, m_txtBase.transform.rotation);
            go.transform.localScale = m_txtBase.transform.localScale;
            go.transform.localPosition = new Vector3(go.transform.localPosition.x + m_barWidth, go.transform.localPosition.y, go.transform.localPosition.z);
            var tm = go.GetComponent<TextMeshProUGUI>();
            tm.text = msg;
            m_txtQue.Enqueue(tm);
        }
		// テキストがまだあれば継続して再生.なくなっていたら破棄.
        if (list.Contains(m_currentMoveText.text)) {
            m_currentMoveText.transform.localPosition = new Vector3(m_txtBase.transform.localPosition.x + m_barWidth, m_txtBase.transform.localPosition.y, m_txtBase.transform.localPosition.z);
            m_txtQue.Enqueue(m_currentMoveText);
        } else {
            if (m_txtQue.Count <= 0) {
                this.gameObject.SetActive(false);
                return;
            }
        }
		this.StartCoroutine(WaitDisp(WAIT_RESTART_SEC, StartMoveText));
	}

	IEnumerator WaitDisp(float waitTime, Action didWait)
    {
        var start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start <= waitTime) {
            yield return null;
        }
        didWait();
    }

    #endregion   
    
	private TextMeshProUGUI m_txtBase;
	private float m_barWidth;
	private TextMeshProUGUI m_currentMoveText;
	private Queue<TextMeshProUGUI> m_txtQue = new Queue<TextMeshProUGUI>();

	const float BUFFA_WIDTH = 50f;        // 幅のバッファ値.幅だけだとギリギリ見切れるぐらいなので.
	const float WAIT_DISP_SEC = 2f;       // 表示し続ける時間.
	const float WAIT_RESTART_SEC = 1.5f;  // 再表示するまでの時間.
}
