// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;

namespace Utage
{

	/// <summary>
	/// 自作コマンド：アイテムポップアップ表示   develop by ryo yoshida. 2018/08/07
	/// </summary>
	internal class AdvCommandItemPop : AdvCommand
	{
		public AdvCommandItemPop(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			// アニメーション本体.
			if (!dataManager.OtherObjectSetting.Dictionary.ContainsKey("AdvItemPop")){
				Debug.LogError(ToErrorString("AdvItemPop is not contained in file setting"));
			}
			itemPopGraphic = dataManager.OtherObjectSetting.LabelToGraphic("AdvItemPop");
			AddLoadGraphic(itemPopGraphic);
            // 表示したいアイテム.
			string label = ParseCell<string>(AdvColumnName.Arg1);
            if (!dataManager.TextureSetting.ContainsLabel(label)) {
                Debug.LogError(ToErrorString(label + " is not contained in file setting"));
            }         
			this.itemGraphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(itemGraphic);

			this.layerName = ParseCellOptional<string>(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName)) {
                Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
            }
   
			//グラフィック表示処理を作成
			this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, itemPopGraphic, 0);
		}

		public override void DoCommand(AdvEngine engine)
		{
			string layer = layerName;
			if (string.IsNullOrEmpty(layer)){
				layer = engine.GraphicManager.SpriteManager.DefaultLayer.name;    //レイヤー名指定なしならSpriteレイヤー
			}         

			//表示する
			var prefab = itemPopGraphic.File.UnityObject as GameObject;
			if(prefab == null){
				Debug.LogError("[AdvCommandItemPop] DoCommand Error!! : object not setting.");
				return;
			}
			UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(layer, engine));
		}
		IEnumerator PlayAndWaitAnimation(string layer, AdvEngine engine)
        {
			engine.GraphicManager.DrawObject(layer, "AdvItemPop", this.graphicOperaitonArg);
			var obj = engine.GraphicManager.SpriteManager.FindObject("AdvItemPop");
            while (obj == null) {
                yield return null;
				obj = engine.GraphicManager.SpriteManager.FindObject("AdvItemPop");
            }
            var vb = obj.gameObject.GetOrAddComponent<ViewBase>();
			var anim = vb.GetScript<Animation>("Contents");
			vb.GetScript<Image>("Item").sprite = Sprite.Create(itemGraphic.Main.File.Texture, new Rect(0, 0, itemGraphic.Main.File.Texture.width, itemGraphic.Main.File.Texture.height), Vector2.zero);
			anim.Play("AdvItemPopOpen");
            do {
                yield return null;
            } while (anim.isPlaying);
        }

		protected AdvGraphicInfo itemPopGraphic;    // 走るアニメーション再生用オブジェクト
		protected AdvGraphicInfoList itemGraphic;   // 走り始め地点の背景
		protected string layerName;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}