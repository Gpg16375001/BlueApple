using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;

namespace Utage
{
	/// <summary>
    /// 自作コマンド：魔術演出   develop by ryo yoshida. 2018/08/29
    /// </summary>
	public class AdvCommandMagic : AdvCommand
    {
		public AdvCommandMagic(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
			// 表示したい魔術.
            string label = ParseCell<string>(AdvColumnName.Arg1);
			if(label == "Type01") {
				objName = "eff_AdvMagic01";         
			}else if(label == "Type02"){
				objName = "eff_AdvMagic02";
			}else{
				Debug.LogError(ToErrorString(label + " is not contained in magic type."));
				return;
			}
			if (!dataManager.OtherObjectSetting.Dictionary.ContainsKey(objName)) {
				Debug.LogError(ToErrorString(objName+" is not contained in file setting"));
            }
			magicAnimGraphic = dataManager.OtherObjectSetting.LabelToGraphic(objName);
            AddLoadGraphic(magicAnimGraphic);

            this.layerName = ParseCellOptional<string>(AdvColumnName.Arg3, "");
            if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName)) {
                Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
            }

            //グラフィック表示処理を作成
            this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, magicAnimGraphic, 0);
        }

        public override void DoCommand(AdvEngine engine)
        {
            string layer = layerName;
            if (string.IsNullOrEmpty(layer)) {
				layer = engine.GraphicManager.SpriteManager.DefaultLayer.name;    //レイヤー名指定なしなら背景とキャラの間のデフォルトレイヤー
            }

            //表示する
            var prefab = magicAnimGraphic.File.UnityObject as GameObject;
            if (prefab == null) {
                Debug.LogError("[AdvCommandRunShake] DoCommand Error!! : object not setting.");
                return;
            }
            UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(layer, engine));
        }
        IEnumerator PlayAndWaitAnimation(string layer, AdvEngine engine)
        {
			engine.GraphicManager.DrawObject(layer, objName, this.graphicOperaitonArg);
			var obj = engine.GraphicManager.SpriteManager.FindObject(objName);
            while (obj == null) {
                yield return null;
				obj = engine.GraphicManager.SpriteManager.FindObject(objName);
            }
            var vb = obj.gameObject.GetOrAddComponent<ViewBase>();
            var anim = vb.GetScript<Animation>("AnimPart");
            anim.Play();
            do {
                yield return null;
            } while (anim.isPlaying);
            Debug.Log("Anim End");
            if (obj != null) {
                obj.Clear();
            }
        }

		private string objName;
		protected AdvGraphicInfo magicAnimGraphic;  // 魔法アニメーション再生用オブジェクト
        protected string layerName; 
        protected AdvGraphicOperaitonArg graphicOperaitonArg;
    }	
}


