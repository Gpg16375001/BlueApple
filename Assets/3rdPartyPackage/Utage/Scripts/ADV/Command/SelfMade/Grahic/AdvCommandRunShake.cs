// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;

namespace Utage
{

	/// <summary>
	/// 自作コマンド：走る演出(画面振動)   develop by ryo yoshida. 2018/07/31
	/// </summary>
	internal class AdvCommandRunShake : AdvCommand
	{
		public AdvCommandRunShake(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			// アニメーション本体.
			if (!dataManager.OtherObjectSetting.Dictionary.ContainsKey("eff_AdvRun01")){
				Debug.LogError(ToErrorString("eff_AdvRun01 is not contained in file setting"));
			}
			runAnimGraphic = dataManager.OtherObjectSetting.LabelToGraphic("eff_AdvRun01");
			AddLoadGraphic(runAnimGraphic);
            // 移動開始地点絵.
			string label = ParseCell<string>(AdvColumnName.Arg1);
            if (!dataManager.TextureSetting.ContainsLabel(label)) {
                Debug.LogError(ToErrorString(label + " is not contained in file setting"));
            }         
			this.fromGraphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(fromGraphic);
            // 移動終了地点絵.
			label = ParseCell<string>(AdvColumnName.Arg2);
            if (!dataManager.TextureSetting.ContainsLabel(label)) {
                Debug.LogError(ToErrorString(label + " is not contained in file setting"));
            }         
			this.toGraphic = dataManager.TextureSetting.LabelToGraphic(label);         
			AddLoadGraphic(toGraphic);

			this.layerName = ParseCellOptional<string>(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName)) {
                Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
            }
   
			//グラフィック表示処理を作成
			this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, runAnimGraphic, 0);
		}

		public override void DoCommand(AdvEngine engine)
		{
			string layer = layerName;
			if (string.IsNullOrEmpty(layer)){
				layer = engine.GraphicManager.BetweenCharaAndBgSprite.DefaultLayer.name;    //レイヤー名指定なしなら背景とキャラの間のデフォルトレイヤー
			}         

			//表示する
			var prefab = runAnimGraphic.File.UnityObject as GameObject;
			if(prefab == null){
				Debug.LogError("[AdvCommandRunShake] DoCommand Error!! : object not setting.");
				return;
			}
			UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(layer, engine));
            
			// 背景表示切り替え
            var bgGraphicArg = new AdvGraphicOperaitonArg(this, toGraphic.Main, 0);
            engine.GraphicManager.IsEventMode = false;
			engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, bgGraphicArg);
		}
		IEnumerator PlayAndWaitAnimation(string layer, AdvEngine engine)
		{
			engine.GraphicManager.DrawObject(layer, "eff_AdvRun01", this.graphicOperaitonArg);
			var obj = engine.GraphicManager.BetweenCharaAndBgSprite.FindObject("eff_AdvRun01");
			while(obj == null){
				yield return null;
				obj = engine.GraphicManager.BetweenCharaAndBgSprite.FindObject("eff_AdvRun01");
			}
			var vb = obj.gameObject.GetOrAddComponent<ViewBase>();
			var anim = vb.GetScript<Animation>("AnimPart");
			vb.GetScript<Image>("BG01").sprite = Sprite.Create(fromGraphic.Main.File.Texture, new Rect(0, 0, fromGraphic.Main.File.Texture.width, fromGraphic.Main.File.Texture.height), Vector2.zero);
            vb.GetScript<Image>("BG02").sprite = Sprite.Create(toGraphic.Main.File.Texture, new Rect(0, 0, toGraphic.Main.File.Texture.width, toGraphic.Main.File.Texture.height), Vector2.zero);         
			anim.Play();
			do {
				yield return null;
			} while (anim.isPlaying);
			Debug.Log("Anim End");
			if(obj != null){
				obj.Clear();
			}         
		}

		protected AdvGraphicInfo runAnimGraphic;    // 走るアニメーション再生用オブジェクト
		protected AdvGraphicInfoList fromGraphic;   // 走り始め地点の背景
		protected AdvGraphicInfoList toGraphic;     // 走り終わり地点の背景
		protected string layerName;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}