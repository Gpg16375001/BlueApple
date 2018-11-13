// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;

namespace Utage
{

	/// <summary>
	/// 自作コマンド：赤いフラッシュ演出   develop by ryo yoshida. 2018/08/29
	/// </summary>
	internal class AdvCommandFlashRed : AdvCommand
	{
		public AdvCommandFlashRed(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			// アニメーション本体.
			if (!dataManager.OtherObjectSetting.Dictionary.ContainsKey("eff_AdvFlashRed01")){
				Debug.LogError(ToErrorString("eff_AdvFlashRed01 is not contained in file setting"));
			}
			flashAnimGraphic = dataManager.OtherObjectSetting.LabelToGraphic("eff_AdvFlashRed01");
			AddLoadGraphic(flashAnimGraphic);
                     
			this.layerName = ParseCellOptional<string>(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName)) {
                Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
            }
   
			//グラフィック表示処理を作成
			this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, flashAnimGraphic, 0);
		}

		public override void DoCommand(AdvEngine engine)
		{
			string layer = layerName;
			if (string.IsNullOrEmpty(layer)){
				layer = engine.GraphicManager.BetweenCharaAndBgSprite.DefaultLayer.name;    //レイヤー名指定なしなら背景とキャラの間のデフォルトレイヤー
			}         

			//表示する
			var prefab = flashAnimGraphic.File.UnityObject as GameObject;
			if(prefab == null){
				Debug.LogError("[AdvCommandRunShake] DoCommand Error!! : object not setting.");
				return;
			}
			UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(layer, engine));
		}
		IEnumerator PlayAndWaitAnimation(string layer, AdvEngine engine)
		{
			engine.GraphicManager.DrawObject(layer, "eff_AdvFlashRed01", this.graphicOperaitonArg);
			var obj = engine.GraphicManager.BetweenCharaAndBgSprite.FindObject("eff_AdvFlashRed01");
			while(obj == null){
				yield return null;
				obj = engine.GraphicManager.BetweenCharaAndBgSprite.FindObject("eff_AdvFlashRed01");
			}
			var vb = obj.gameObject.GetOrAddComponent<ViewBase>();
			var anim = vb.GetScript<Animation>("AnimPart");
			anim.Play();
			do {
				yield return null;
			} while (anim.isPlaying);
			Debug.Log("Anim End");
			if(obj != null){
				obj.Clear();
			}         
		}

		protected AdvGraphicInfo flashAnimGraphic;  // 斬撃アニメーション再生用オブジェクト
		protected string layerName;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}