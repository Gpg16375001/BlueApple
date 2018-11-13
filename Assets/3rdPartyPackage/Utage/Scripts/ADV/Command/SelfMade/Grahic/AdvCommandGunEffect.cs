using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;

namespace Utage
{
	/// <summary>
    /// 自作コマンド：銃撃演出表示   develop by ryo yoshida. 2018/08/07
    /// </summary>
	public class AdvCommandGunEffect : AdvCommand
    {
		public AdvCommandGunEffect(StringGridRow row, AdvSettingDataManager dataManager)
            : base(row)
        {
            // アニメーション本体.
            if (!dataManager.OtherObjectSetting.Dictionary.ContainsKey("eff_AdvMachinegun01")) {
				Debug.LogError(ToErrorString("eff_AdvMachinegun01 is not contained in file setting"));
            }
			gunAnimGraphic = dataManager.OtherObjectSetting.LabelToGraphic("eff_AdvMachinegun01");
            AddLoadGraphic(gunAnimGraphic);

            this.layerName = ParseCellOptional<string>(AdvColumnName.Arg3, "");
            if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName)) {
                Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
            }

            //グラフィック表示処理を作成
            this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, gunAnimGraphic, 0);
        }

        public override void DoCommand(AdvEngine engine)
        {
            string layer = layerName;
            if (string.IsNullOrEmpty(layer)) {
				layer = engine.GraphicManager.SpriteManager.DefaultLayer.name;    //レイヤー名指定なしなら背景とキャラの間のデフォルトレイヤー
            }

            //表示する
            var prefab = gunAnimGraphic.File.UnityObject as GameObject;
            if (prefab == null) {
                Debug.LogError("[AdvCommandRunShake] DoCommand Error!! : object not setting.");
                return;
            }
            UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(layer, engine));
        }
        IEnumerator PlayAndWaitAnimation(string layer, AdvEngine engine)
        {
			engine.GraphicManager.DrawObject(layer, "eff_AdvMachinegun01", this.graphicOperaitonArg);
			var obj = engine.GraphicManager.SpriteManager.FindObject("eff_AdvMachinegun01");
            while (obj == null) {
                yield return null;
				obj = engine.GraphicManager.SpriteManager.FindObject("eff_AdvMachinegun01");
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

        protected AdvGraphicInfo gunAnimGraphic;  // 斬撃アニメーション再生用オブジェクト
        protected string layerName;
        protected AdvGraphicOperaitonArg graphicOperaitonArg;
    }

}