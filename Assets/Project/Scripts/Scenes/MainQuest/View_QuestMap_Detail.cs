using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;


/// <summary>
/// View : 詳細クエストマップ.
/// </summary>
public class View_QuestMap_Detail : ViewBase
{
	public override void Dispose()
	{
		base.Dispose();
		CameraHelper.SharedInstance.CameraQuestMap.transform.localPosition = m_baseCamPos;
	}

    /// <summary>
    /// 初期化.
    /// </summary>
	public void Init(MainQuest questData, bool bReleaseRoot = false)
	{
		m_quest = questData;
        m_baseCamPos = CameraHelper.SharedInstance.CameraQuestMap.transform.localPosition;

		this.ClearView();
		this.DrawRoot(bReleaseRoot);

        // 初回フォーカス.終わったらクエスト情報に合わせて現在の解放状況に合わせたルートを表示する.
        var target = this.GetScript<Transform>("CameraPosAnchor" + m_quest.camera_forcus_index.ToString("d4"));
        CameraHelper.SharedInstance.CameraQuestMap.transform.SetPositionAndRotation(new Vector3(target.localPosition.x, target.localPosition.y, CameraHelper.SharedInstance.CameraQuestMap.transform.localPosition.z), Quaternion.identity);
	}

    /// <summary>
    /// マーカー位置の変更.
    /// </summary>
	public void ChangeMarkerPostion(MainQuest questData)
	{
		var list = this.gameObject.GetComponentsInChildren<View_QuestLandPoint>();
		var item = list.FirstOrDefault(p => p.PointID == questData.map_land_point.id);
		if(item != null && item.gameObject.activeSelf){
			list.Select(p => p.IsEnableMarker = p.PointID == questData.map_land_point.id).ToList();
		}      
	}

    /// <summary>
	/// カメラフォーカス処理.
    /// </summary>
	public void ForcusCamera(MainQuest questData, bool bReleaseRoot, Action didForcus = null)
	{
		if(m_bForcusing){
			return;
		}
		m_bForcusing = true;
		m_quest = questData;
		this.ForcusCamera(questData.camera_forcus_index, bReleaseRoot, didForcus);
	}
    // カメラフォーカス処理.
	private void ForcusCamera(int camAnchorIdx, bool bReleaseRoot,  Action didForcus)
	{
		Debug.Log("ForcusCamera CameraPosAnchor"+camAnchorIdx.ToString("d4")+" bReleaseRoot="+bReleaseRoot);
		var target = this.GetScript<Transform>("CameraPosAnchor"+camAnchorIdx.ToString("d4"));
		var moveHash = new Hashtable();
		moveHash.Add("x", target.localPosition.x);
		moveHash.Add("y", target.localPosition.y);
		moveHash.Add("isLocal", true);
        moveHash.Add("time", 0.6f);
		moveHash.Add("easetype", iTween.EaseType.linear);
		iTween.MoveTo(CameraHelper.SharedInstance.CameraQuestMap.gameObject, moveHash);
		this.StartCoroutine(this.WaitiTweenMoveToEnd(CameraHelper.SharedInstance.CameraQuestMap.gameObject, () => { 
			if (bReleaseRoot){
				this.TryPlayReleaseRootAnimation(didForcus);
				return;
			}
			if(didForcus != null){
				didForcus();
			}
		} ));
	}
    // oncompleteがoncompletetargetを指定してもどうにも反応しないのでこうする.
	IEnumerator WaitiTweenMoveToEnd(GameObject moveObj, Action didEnd)
	{
		var itween = moveObj.GetOrAddComponent<iTween>();
		while(itween == null){
			yield return null;
			itween = moveObj.GetOrAddComponent<iTween>();
		}
		var bExistsTween = moveObj.GetComponentsInChildren<iTween>(true).Length > 0;
		while(bExistsTween){
			yield return null;
			bExistsTween = moveObj.GetComponentsInChildren<iTween>(true).Length > 0;
		}
		if(didEnd != null){
			didEnd();
		}
		m_bForcusing = false;
	}
	private bool m_bForcusing = false;
    
    // 地点間の道程を描画する.演出は別.
	private void DrawRoot(bool bReleaseRoot)
	{
		var bLatestQuest = AwsModule.ProgressData.GetPlayableLatestMainQuest(m_quest.Country);

		// 章内のクエストに絞ってリスト化.
		var qList = MasterDataTable.quest_main.GetQuestList(m_quest.Country, m_quest.ChapterNum);
		var latest = AwsModule.ProgressData.GetPlayableLatestMainQuest(m_quest.Country);
		qList.Sort((x, y) => x.ID - y.ID);
		int prevLandID = -1;
		foreach(var q in qList){
			var bRelease = AwsModule.ProgressData.IsReleaseMainQuest(m_quest.Country, q.ChapterNum, q.StageNum, q.quest);
			if(!bReleaseRoot){
				bRelease |= AwsModule.ProgressData.IsLatestMainQuest(q.id); // ルート解放演出しないのであれば最新クエストまで一気に表示.
			}         
			if(!bRelease){
				break;
			}
			Debug.Log("QuestMapDrawRoot =>" + q.id + " : bRelease=" + (bRelease.ToString()) + "\nlatest=" + latest.id);

			// 地点設定.
			var anchor = this.GetScript<Transform>("LandPointAnchor"+q.map_land_point.id);
			var com = this.CreateOrGetLandPoint(q.map_land_point, anchor);
			com.IsEnablePoint = true;
			// 地点移動しているようであれば地点間.
			if(prevLandID >= 0){
				if(prevLandID != q.map_land_point.id){
					// すでに解放済みルート.逆からのルートが存在している可能性もある.
					var lineName = "img_QuestLandLine"+prevLandID+"to"+q.map_land_point.id;
                    if (!this.Exist<Transform>(lineName)) {
						lineName = "img_QuestLandLine"+q.map_land_point.id+"to"+prevLandID;
                    }
                    this.GetScript<Transform>(lineName).gameObject.SetActive(true);
				}
			}

			prevLandID = q.map_land_point.id;
		}
  
        // マーカー設定.
		if(!bReleaseRoot){
			this.ChangeMarkerPostion(m_quest);
		}
	}   

	// ルート解放演出再生試み.
	private void TryPlayReleaseRootAnimation(Action didPlayEnd = null)
	{
		var qList = MasterDataTable.quest_main.GetQuestList(m_quest.Country, m_quest.ChapterNum); 
		var latest = qList.Find(q => AwsModule.ProgressData.IsLatestMainQuest(q.id));
		if(latest == null){
			if (didPlayEnd != null) {
                didPlayEnd();
            }
            return;
		}
  
		var prev = AwsModule.ProgressData.GetLatestClearMainQuest(m_quest.Country);
		if(prev == null){
			var anchor = this.GetScript<Transform>("LandPointAnchor"+m_quest.map_land_point.id);
            var point = this.CreateOrGetLandPoint(m_quest.map_land_point, anchor);
			point.PlayLandOpen();
            anchor.gameObject.SetActive(true);
            return;
		}

		this.StartCoroutine(this.CoTryPlayReleaseRootAnimation(prev.map_land_point, latest.map_land_point, didPlayEnd));
	}
	IEnumerator CoTryPlayReleaseRootAnimation(MainQuestMapLandPoint prevPoint, MainQuestMapLandPoint landPoint, Action didPlayEnd)
	{
		Debug.Log(prevPoint.land_name_main+" to "+landPoint.land_name_main);
		var anchor = this.GetScript<Transform>("LandPointAnchor"+landPoint.id);
		var point = anchor.childCount <= 0 ? this.CreateOrGetLandPoint(landPoint, anchor): anchor.GetComponentInChildren<View_QuestLandPoint>();
		anchor.gameObject.SetActive(true);      
              

		// ルート表示      
		var lineName = "img_QuestLandLine"+prevPoint.id+"to"+landPoint.id;
		if(!this.Exist<Animation>(lineName)){
			lineName = "img_QuestLandLine"+landPoint.id+"to"+prevPoint.id;
        }
		// ルートがない=地点移動がない or 章始めの地点.すでに解放済みルートの場合も考慮.
		Debug.Log("ルート解放チェック : lineName=" + lineName + " already active?=" + (this.Exist<Animation>(lineName) ? this.GetScript<Transform>(lineName).gameObject.activeSelf.ToString(): "ラインが存在しない"));
		if(this.Exist<Animation>(lineName) && !this.GetScript<Transform>(lineName).gameObject.activeSelf){
			// ルート解放.
			var anim = this.GetScript<Animation>(lineName);
            anim.gameObject.SetActive(true);
            anim.Play();
            do {
                yield return null;
            } while (anim.isPlaying);  
			point.PlayLandOpen();
		}else{
			// 同じ地点をさしていなければ地点のみ解放演出.章始まりなど.
			if(prevPoint.id != landPoint.id){
				point.PlayLandOpen();
			}else{
				this.ChangeMarkerPostion(m_quest);
			}
		}

		if(didPlayEnd != null){
			didPlayEnd();
		}
	}

    // 表示クリア.
    private void ClearView()
	{
		foreach(Transform p in this.GetScript<Transform>("QuestPointRoot")){
			p.gameObject.SetActive(false);
		}
	}

    // 地点作成.すでに生成済みなら取得する.
	private View_QuestLandPoint CreateOrGetLandPoint(MainQuestMapLandPoint landPoint, Transform anchor)
	{
		if(anchor.childCount > 0){
			return this.GetComponentInChildren<View_QuestLandPoint>();
		}
		var landObj = GameObjectEx.LoadAndCreateObject("MainQuest/QuestLandPoint", anchor.gameObject);
        var com = landObj.GetOrAddComponent<View_QuestLandPoint>();
		com.Init(landPoint);
		anchor.gameObject.SetActive(true);
		return com;
	}

	private MainQuest m_quest;
	private Vector3 m_baseCamPos;
}
