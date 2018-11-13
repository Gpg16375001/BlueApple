using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;
using Live2D.Cubism.Rendering;

public class BattleCutin : ViewBase
{
    public static void StartCutinEffect(int cardId, ElementEnum element, GameObject standingImagePrefab, string skillName, string voiceFileName, System.Action didEnd)
	{
		var go = GameObjectEx.LoadAndCreateObject("Battle/BattleEffect/BattleCutin");
        go.AttachUguiRootComponent(CameraHelper.SharedInstance.Camera2D, 0f);
        go.GetOrAddComponent<BattleCutin>().InitInternal(cardId, element, standingImagePrefab, skillName, voiceFileName, didEnd);
	}

    private void InitInternal(int cardId, ElementEnum element, GameObject standingImage, string skillName, string voiceFileName, System.Action didEnd)
	{
		this.GetScript<TextMeshProUGUI>("txtp_SPName_1").SetText(skillName);
		this.GetScript<TextMeshProUGUI>("txtp_SPName_2").SetText(skillName);

		// Live2DのPrefabがない場合はスキップ
		if (standingImage != null) {
			var modelAnchor = this.GetScript<Transform>("ModelAnchor");
			standingImage.SetActive(true);
			standingImage.transform.SetParent(modelAnchor);
			standingImage.transform.localScale = Vector3.one;
			standingImage.transform.localPosition = Vector3.zero;
			standingImage.transform.localRotation = Quaternion.identity;
            MasterDataTable.card_live2d_anchor_setting.FixPostionFaceCenter (cardId, standingImage, 0.001f);
			var cubismRender = standingImage.GetComponentsInChildren<CubismRenderController>()[0];
			if(cubismRender != null){
				var rootCanvas = this.GetScript<Canvas>("ModelAnchor");
				cubismRender.gameObject.SetLayerRecursively(rootCanvas.gameObject.layer);
				cubismRender.SortingLayer = rootCanvas.sortingLayerName;
				cubismRender.SortingOrder = rootCanvas.sortingOrder;
			}
		}
        StartCoroutine(CoPlayAnime(string.Format("BattleCutin{0:D2}", (int)element), voiceFileName, didEnd));
	}

	private IEnumerator CoPlayAnime(string animeName, string voiceFileName, System.Action didEnd)
	{
		var animation = this.GetScript<Animation>("AnimParts");

        if (animation.GetClip (animeName) != null) {
            animation.Play (animeName);
        } else {
            animation.Play ();
        }

		SoundManager.SharedInstance.PlayVoice(voiceFileName, SoundVoiceCueEnum.use_spattack);

        yield return new WaitUntil(() => !animation.isPlaying);

		this.Dispose();
		if (didEnd != null) {
			didEnd();
		}
	}

	void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
			canvas.worldCamera = CameraHelper.SharedInstance.BattleCamera;
        }
    }
}
