// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimurausing System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// コマンド：サウンドのボリュームを変更
	/// </summary>
	internal class AdvCommandChangeSoundVolume : AdvCommand
	{
		public AdvCommandChangeSoundVolume(StringGridRow row)
			:base(row)
		{
			this.groups = ParseCellArray<string>(AdvColumnName.Arg1 );
			this.volume = ParseCell<float>(AdvColumnName.Arg2);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (groups.Length == 1 && groups[0] == "All")
			{
				engine.SoundManager.SetGroupVolume(SoundManager.IdBgm, volume);
				engine.SoundManager.SetGroupVolume(SoundManager.IdAmbience, volume);
				engine.SoundManager.SetGroupVolume(SoundManager.IdSe, volume);
				engine.SoundManager.SetGroupVolume(SoundManager.IdVoice, volume);
			}
			else
			{
				foreach (var group in groups)
				{
					engine.SoundManager.SetGroupVolume(group, volume);
				}
			}
		}

		string[] groups;
		float volume;
	}
}