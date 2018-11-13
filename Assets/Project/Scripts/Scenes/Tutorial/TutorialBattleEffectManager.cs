using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using TMPro;


/// <summary>
/// バトルWAVE演出管理.
/// </summary>
public class TutorialBattleWaveEffectManager
{   
    /// <summary>
    /// 開始演出アニメーション再生.
    /// </summary>
    public void PlayStart(Action didEnd)
	{
        m_battleStartEffect.Play(didEnd);
	}

    /// <summary>
    /// 終了演出アニメーション再生.
    /// </summary>
	public void PlayFinish(Action didEnd)
	{
        m_battleWaveFinishEffect.Play(didEnd);
	}

    /// <summary>
    /// 通常再生.
    /// </summary>
    public void Play(Action didEnd, int wave = -1, int maxWave = -1)
	{
        m_battleWaveCountEffect.gameObject.GetOrAddComponent<View_BattleWaveCountUp> ().SetWaveCount (AwsModule.BattleData.WaveCount);
        m_battleWaveCountEffect.Play(didEnd);
	}

	public TutorialBattleWaveEffectManager()
	{
        m_battleStartEffect = View_TutorialBattleEffect.Create("Battle/BattleStart");
        m_battleStartEffect.IsVisible = false;
        m_battleWaveCountEffect = View_TutorialBattleEffect.Create("Battle/BattleWave_countup");
        m_battleWaveCountEffect.IsVisible = false;
        m_battleWaveFinishEffect = View_TutorialBattleEffect.Create("Battle/BattleWave_finish");
        m_battleWaveFinishEffect.IsVisible = false;
	}

    private View_TutorialBattleEffect m_battleStartEffect;
    private View_TutorialBattleEffect m_battleWaveCountEffect;
    private View_TutorialBattleEffect m_battleWaveFinishEffect;
}
