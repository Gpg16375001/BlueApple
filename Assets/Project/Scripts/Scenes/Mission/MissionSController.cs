using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.Net.API;


/// <summary>
/// ScreenController : ミッション.
/// </summary>
public class MissionSController : ScreenControllerBase
{
	public override void Init(Action<bool> didConnectEnd)
	{
		SendAPI.MissionsGetAchievement((bSuccess, res) => {
			if(!bSuccess || res == null){
				Debug.LogError("MissionSController Error!! : response error. bSuccess="+bSuccess);
				didConnectEnd(false);
				return;
			}
			m_missionAchievements = res.MissionAchievementList;
			didConnectEnd(true);
		});      
	}

	public override void CreateBootScreen()
	{
		var go = GameObjectEx.LoadAndCreateObject("Mission/Screen_Mission", this.gameObject);
		var c = go.GetOrAddComponent<Screen_Mission>();
		c.Init(m_missionAchievements);
	}

	private MissionAchievement[] m_missionAchievements;
}
