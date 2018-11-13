using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

using Live2D.Cubism.Framework.MouthMovement;

namespace Utage
{
	[RequireComponent(typeof(CubismMouthController), typeof(CubismAudioMouthInputFromCri), typeof(CubismAutoMouthInput))]
	[AddComponentMenu("Utage/Live2D/LipSynchForCri")]
	public class Live2DLipSynchForCri : LipSynchBase
	{
		CubismMouthController Controller { get { return this.gameObject.GetComponentCache<CubismMouthController>(ref controller); } }
		CubismMouthController controller;

		CubismAudioMouthInputFromCri Audio { get { return this.gameObject.GetComponentCache<CubismAudioMouthInputFromCri>(ref _audio); } }
		CubismAudioMouthInputFromCri _audio;

		CubismAutoMouthInput Auto{ get { return this.gameObject.GetComponentCache<CubismAutoMouthInput>(ref auto); } }
		CubismAutoMouthInput auto;

		void Awake()
		{
			Controller.enabled = false;
			Audio.enabled = false;
			Auto.enabled = false;
		}

		protected override void OnStartLipSync()
		{
			Controller.enabled = true;
			switch(this.LipSynchMode)
			{
			case LipSynchMode.Voice:
				Audio.enabled = true;
				Auto.enabled = false;
				break;
			default:
				Audio.enabled = false;
				Auto.enabled = true;
				break;
			}
		}

		protected override void OnUpdateLipSync()
		{
			if (this.CheckVoiceLipSync())
			{
				if(!Controller.enabled) Controller.enabled = true;
				var atomSoundManagerSystem = SoundManager.GetInstance ().System as AtomSoundManagerSystem;
				if (atomSoundManagerSystem != null) {
					Audio.Analyzer = atomSoundManagerSystem.GetAudioAnalyzer (SoundManager.IdVoice, this.CharacterLabel);
				}
			}
		}

		protected override void OnStopLipSync()
		{
			Controller.enabled = false;
			Audio.enabled = false;
			Auto.enabled = false;
		}
	}
}
