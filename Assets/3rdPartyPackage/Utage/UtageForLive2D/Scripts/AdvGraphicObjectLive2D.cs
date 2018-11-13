using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

using Live2D.Cubism.Rendering;

namespace Utage
{
    [AddComponentMenu("Utage/ADV/Internal/GraphicObject/Live2D")]
    internal class AdvGraphicObjectLive2D : MonoBehaviour, IAdvGraphicObjectCustom
    {
        /// <summary>ADVエンジン</summary>
        public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
        [SerializeField]
        AdvEngine engine;

        AdvGraphicObjectCustom AdvObj
        {
            get {
                if (advObj == null) {
                    advObj = this.GetComponentInParent<AdvGraphicObjectCustom>();
                }
                return advObj;
            }
        }
        AdvGraphicObjectCustom advObj;

        CubismRenderController RenderController
        {
            get { return this.GetComponentCache<CubismRenderController>(ref renderController); }
        }
        CubismRenderController renderController;

        // 2018/2/7 LipSyncForCriへの対応. yoshida.
        #region LipSynchs.
        // 使用するLipsynch.
        LipSynchBase UseLipSynch
        {
            get {
                var normal = this.GetComponentCache<Live2DLipSynch>(ref lipSynch);
                if (normal != null) {
                    return normal;
                } else {
                    var forCri = this.GetComponentCache<Live2DLipSynchForCri>(ref lipSynchForCri);
                    if (forCri != null) {
                        return forCri;
                    }
                }
                return null;
            }
        }
        Live2DLipSynch lipSynch;
        Live2DLipSynchForCri lipSynchForCri;
        /*
        Live2DLipSynchForCri LipSynchForCri 
        {
            get { return this.GetComponentCache<Live2DLipSynchForCri>(ref lipSynchForCri); }
        }
        Live2DLipSynchForCri lipSynchForCri;

        Live2DLipSynch LipSynch
		{
            get { return this.GetComponentCache<Live2DLipSynch>(ref lipSynch); }
		}
		Live2DLipSynch lipSynch;
        */
        #endregion

        //描画時のリソース変更
        public void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
		{
			//描画順の設定
			RenderController.SortingOrder = AdvObj.Layer.SettingData.Order;
			//リップシンクありならそれを設定
            if (UseLipSynch != null)
			{
                UseLipSynch.Play();
			}
		}

		//エフェクト用の色が変化したとき
		public void OnEffectColorsChange(AdvEffectColor color)
		{
			// TODO : αの設定を行うとTextureが複数構成になっているため不自然な透け感になる.むしろα以外の色を繁栄させる方法に修正.ただしDrawbleの数によっては負荷が心配. fix by yoshida. 2018/08/03
			//this.RenderController.Opacity = color.MulColor.a;
			foreach(var r in this.GetComponentsInChildren<CubismRenderer>()){
				r.Color = new Color(color.MulColor.r, color.MulColor.g, color.MulColor.b);
			}
		}

        void Start()
		{
            if (UseLipSynch != null && Engine!=null)
			{
				AdvGraphicObjectCustom advObj = this.GetComponentInParent<AdvGraphicObjectCustom>();
				if (advObj != null)
				{
					//リップシンクのキャラクターラベルを設定
                    UseLipSynch.CharacterLabel = advObj.ParentObject.gameObject.name;
                    UseLipSynch.OnCheckTextLipSync.AddListener(
							(x) =>
							{
								x.EnableTextLipSync = (x.CharacterLabel == Engine.Page.CharacterLabel && Engine.Page.IsSendChar);
							});
				}
			}
		}
	}
}
