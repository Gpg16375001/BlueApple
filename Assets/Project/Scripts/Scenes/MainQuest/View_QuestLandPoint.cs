using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using SmileLab;


/// <summary>
/// クエストで使用するのマップ地点.
/// </summary>
public class View_QuestLandPoint : ViewBase
{
	/// <summary>
    /// ID.
    /// </summary>
	public int PointID { get { return m_point.id; } }

    /// <summary>
    /// 現在地であることを示すマーカーのON/OFF.
    /// </summary>
	public bool IsEnableMarker 
	{ 
		get { return this.GetScript<Transform>("Marker").gameObject.activeSelf; }
		set { this.GetScript<Transform>("Marker").gameObject.SetActive(value); }
	}

    /// <summary>
    /// 地点ポイントはすでに解放済みの場所は手動でOnにする.
    /// </summary>
    public bool IsEnablePoint
	{
		get { return this.GetScript<Transform>("Point").gameObject.activeSelf; }
		set { 
			this.GetScript<Transform>("Point").gameObject.SetActive(value);
			this.GetScript<Transform>("Name").gameObject.SetActive(value);
		}
	}


    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MainQuestMapLandPoint landPoint)
	{
		m_point = landPoint;
		this.GetScript<TextMeshProUGUI>("txtp_QuestLandNameSub").text = m_point.land_name_sub;
		this.GetScript<TextMeshProUGUI>("txtp_QuestLandNameMain").text = m_point.land_name_main;
		this.IsEnablePoint = false;
	}

    /// <summary>
    /// 地点解放演出再生.
    /// </summary>
    public void PlayLandOpen(Action didOpen = null)
	{
		this.StartCoroutine(this.CoPlayOpenAnimation(didOpen));
	}
	IEnumerator CoPlayOpenAnimation(Action didOpen)
	{
		var anim = this.GetComponent<Animation>();
		anim.Play();
		do {
			yield return null;
		} while (anim.isPlaying);
		if(didOpen != null){
			didOpen();
		}
	}

    /// <summary>
    /// 地点移動演出.
    /// </summary>
    public void PlayMarkerInOut(bool bIn, Action didEnd = null)
	{
		this.StartCoroutine(this.CoPlayMarkerAnimation(bIn, didEnd));
	}
	IEnumerator CoPlayMarkerAnimation(bool bIn, Action didEnd)
    {
		var anim = this.GetScript<Animation>("Marker");
		anim.Play(bIn ? "MainQuestMarkerIn": "MainQuestMarkerOut");
        do {
            yield return null;
        } while (anim.isPlaying);
		if (didEnd != null) {
			didEnd();
        }
    }

	private MainQuestMapLandPoint m_point;
}
