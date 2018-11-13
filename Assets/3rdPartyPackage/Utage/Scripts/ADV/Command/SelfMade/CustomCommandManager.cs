using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

    /// <summary>
    /// ゲーム中で使用する独自コマンドはここでまとめて処理する.
    /// </summary>
	public class CustomCommandManager : AdvCustomCommandManager
    {
		public override void OnBootInit()
        {
            AdvCommandParser.OnCreateCustomCommandFromID += CreateCustomCommand;
        }

		//AdvEnginのクリア処理のときに呼ばれる
        public override void OnClear()
        {
        }

		//カスタムコマンドの作成用コールバック
		public void CreateCustomCommand(string commandName, StringGridRow row, AdvSettingDataManager dataManager, ref AdvCommand command)
		{ 
			switch(commandName){
				case "ItemPop":         // アイテムポップ表示(小)
					command = new AdvCommandItemPop(row, dataManager);
					break;
				case "ItemPopClose":    // アイテムポップ表示(小)閉じる
					command = new AdvCommandItemPopClose(row, dataManager);
                    break;
				case "LargeItemPop":    // アイテムポップ表示(大)
                    command = new AdvCommandLargeItemPop(row, dataManager);
                    break;
                case "LargeItemPopClose":    // アイテムポップ表示(大)閉じる
                    command = new AdvCommandLargeItemPopClose(row, dataManager);
                    break;
				case "Effect_Run":      // 走る演出
					command = new AdvCommandRunShake(row, dataManager);
					break;
				case "Effect_Slash":    // 斬撃
					command = new AdvCommandSlash(row, dataManager);
					break;
				case "Effect_Flash_Red":    // 赤いフラッシュ
					command = new AdvCommandFlashRed(row, dataManager);
                    break;
				case "Effect_IntensiveLine":    // 集中線
					command = new AdvCommandIntensiveLine(row, dataManager);
                    break;
				case "Effect_Magic":    // 魔術
                    command = new AdvCommandMagic(row, dataManager);
                    break;
                case "Effect_Gun":  // 銃撃演出
                    command = new AdvCommandGunEffect(row, dataManager);
                    break;
                case "Effect_Claw": // 爪撃演出
                    command = new AdvCommandClaw(row, dataManager);
                    break;
                    // TODO : 適宜カスタムコマンドを使用したい場合は追記.
			}
		}
    }

}