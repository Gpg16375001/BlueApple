// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;

namespace Utage
{

	/// <summary>
	/// 自作コマンド：アイテムポップアップ閉   develop by ryo yoshida. 2018/08/07
	/// </summary>
	internal class AdvCommandItemPopClose : AdvCommand
	{
		public AdvCommandItemPopClose(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{         
			// すでにあるものを閉じる.
			UniRx.MainThreadDispatcher.StartCoroutine(this.PlayAndWaitAnimation(engine));
		}
		IEnumerator PlayAndWaitAnimation(AdvEngine engine)
        {
			var obj = engine.GraphicManager.SpriteManager.FindObject("AdvItemPop");
            while (obj == null) {
				yield break;
            }
            var vb = obj.gameObject.GetOrAddComponent<ViewBase>();
			var anim = vb.GetScript<Animation>("Contents");
			anim.Play("AdvItemPopClose");
            do {
                yield return null;
            } while (anim.isPlaying);
			obj.Clear();
        }

		protected AdvGraphicInfo itemPopGraphic;    // 走るアニメーション再生用オブジェクト
		protected AdvGraphicInfoList itemGraphic;   // 走り始め地点の背景
		protected string layerName;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}