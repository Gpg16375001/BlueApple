using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

using Live2D.Cubism.Framework.MouthMovement;

namespace Utage
{
	[RequireComponent(typeof(CubismMouthController), typeof(CubismAudioMouthInput), typeof(CubismAutoMouthInput))]
	[AddComponentMenu("Utage/Live2D/LipSynch")]
	public class Live2DLipSynch : LipSynchBase
	{
		CubismMouthController Controller { get { return this.gameObject.GetComponentCache<CubismMouthController>(ref controller); } }
		CubismMouthController controller;

		CubismAudioMouthInput Audio { get { return this.gameObject.GetComponentCache<CubismAudioMouthInput>(ref _audio); } }
		CubismAudioMouthInput _audio;

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
				Audio.AudioInput = SoundManager.GetInstance().System.GetAudioSource(SoundManager.IdVoice,this.CharacterLabel);
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
