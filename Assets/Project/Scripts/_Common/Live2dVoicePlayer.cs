using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using Live2D.Cubism.Framework.MouthMovement;


/// <summary>
/// Live2Dモデルにつけてボイス再生させるコンポーネント.
/// </summary>
[RequireComponent(typeof(CubismMouthController))]
[RequireComponent(typeof(CubismAudioMouthInputFromCri))]
[RequireComponent(typeof(CubismAutoMouthInput))]
public class Live2dVoicePlayer : MonoBehaviour
{   
	public bool IsPlayingVoice
	{ 
		get {
			var ctrl = SoundManager.SharedInstance.GetSoundControll<CriSoundControll>();
			return ctrl.GetPlayerStatusFromIndex(m_audioIndex) != CriAtomExPlayer.Status.PlayEnd;
		}
	}   

    /// <summary>
    /// ボイス再生.
    /// </summary>
	public void Play(string fileName, SoundVoiceCueEnum cueEnum)
	{
		this.GetComponent<CubismMouthController>().enabled = true;   
		this.GetComponent<CubismAutoMouthInput>().enabled = false;
		audio.enabled = true;
		SoundManager.SharedInstance.StopVoice();    

		if(m_coPlayWait != null){
			Stop();
			this.StopCoroutine(m_coPlayWait);
		}
        m_coPlayWait = this.StartCoroutine(this.PlayAndWaitEnd(fileName, cueEnum.ToString()));
	}
    public void Play(string fileName, string cueName)
    {
        this.GetComponent<CubismMouthController>().enabled = true;   
        this.GetComponent<CubismAutoMouthInput>().enabled = false;
        audio.enabled = true;
        SoundManager.SharedInstance.StopVoice();    

        if(m_coPlayWait != null){
            Stop();
            this.StopCoroutine(m_coPlayWait);
        }
        m_coPlayWait = this.StartCoroutine(this.PlayAndWaitEnd(fileName, cueName));
    }
	IEnumerator PlayAndWaitEnd(string fileName, string cueName)
	{      
		var ctrl = SoundManager.SharedInstance.GetSoundControll<CriSoundControll>();      
		if (m_audioIndex >= 0) {
			Stop();
        }
		var acb = ctrl.LoadACB(fileName);
        m_audioIndex = ctrl.SoundPrepare(acb, cueName, false, true);
		ctrl.StartFromIndex(m_audioIndex);
        do {
			audio.Analyzer = ctrl.GetAnalyzerFromIndex(m_audioIndex);
			yield return null;
		} while (ctrl.GetPlayerStatusFromIndex(m_audioIndex) != CriAtomExPlayer.Status.PlayEnd);
		this.GetComponent<CubismMouthController>().enabled = false;
		audio.enabled = false;
	}

    /// <summary>
    /// 停止.
    /// </summary>
    public void Stop()
	{
        if (m_audioIndex < 0) {
            return;
        }
		var ctrl = SoundManager.SharedInstance.GetSoundControll<CriSoundControll>();
        if (ctrl.GetPlayerStatusFromIndex (m_audioIndex) != CriAtomExPlayer.Status.PlayEnd) {
            ctrl.StopFromIndex (m_audioIndex, true);
        }
		ctrl.RemoveAnalyzerFromIndex(m_audioIndex);
		m_audioIndex = -1;
	}

	private Coroutine m_coPlayWait;
	private int m_audioIndex = -1;
       
	CubismAudioMouthInputFromCri audio { get { return this.gameObject.GetComponent<CubismAudioMouthInputFromCri>(); } }
}
