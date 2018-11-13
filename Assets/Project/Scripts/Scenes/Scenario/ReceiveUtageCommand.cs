using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;
using SmileLab;


/// <summary>
/// 宴側のコマンドから送られてくる処理レシーバークラス.
/// </summary>
public class ReceiveUtageCommand : MonoBehaviour
{   
	// チュートリアルよりユーザー名入力.
	void InputUserNameWithTutorial(AdvCommandSendMessageByName command)
	{
		Utage.SoundManager.GetInstance().System.StopGroup(Utage.SoundManager.IdVoice, 0);
		View_RegistUserPop.Create(() => {
            UtageModule.SharedInstance.IsCustomInput = true;
        });
	}
	// チュートリアルより最初のカード(リーンハルト取得).
	void GetFirstCardWithTutorial(AdvCommandSendMessageByName command)
	{
		UtageModule.SharedInstance.SetActiveCore(false);
        // キャラ解放演出削除.
		View_MainQuestUnlock.DeleteAsync(() => {
			UtageModule.SharedInstance.SetActiveCore(true);
            UtageModule.SharedInstance.IsCustomInput = true;
		});
	}

    // 選択肢ID受信処理.
    void RegistSelectionID(AdvCommandSendMessageByName command)
    {
        var id = command.ParseCell<int>("Arg3");
        Debug.Log(id+"の選択肢を選択した。");
        AwsModule.ProgressData.CurrentScenarioSelectIdList.Add(id);
    }
}
