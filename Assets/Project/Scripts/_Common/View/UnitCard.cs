using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class UnitCard : ViewBase {
    UnitResourceLoader loader;
    public void Init(CardData card, System.Action didLoad, UnitResourceLoader resource = null)
    {
        m_Card = card;
        m_Loaded = didLoad;
        if (resource == null) {
            loader = new UnitResourceLoader (card.CardId);
            loader.IsLoadCardBg = true;
            loader.IsLoadPortrait = true;
            loader.IsLoadAnimationClip = false;
            loader.IsLoadLive2DModel = false;
            loader.IsLoadSpineModel = false;
            loader.IsLoadTimeLine = false;

            loader.LoadResource (LoadedResource, this);
        } else {
            LoadedResource (resource);
        }

        this.GetScript<Image> ("EmblemIcon").sprite = IconLoader.LoadEmblem (m_Card.Card.character.belonging);

        this.GetScript<TextMeshProUGUI> ("txtp_UnitNameSub").SetText (m_Card.Card.alias);
        this.GetScript<TextMeshProUGUI> ("txtp_UnitNameJa").SetText (m_Card.Card.nickname);

        // レアリティの設定
		int maxRarity = card.MaxRarity;
        int nowRarity = card.Rarity;
        for(int i = 1; i <= 6; ++i)
        {
            var starObj = this.GetScript<Transform> (string.Format ("StarGrid/Star{0}", i));
            if (maxRarity >= i) {
                starObj.gameObject.SetActive (true);
                var starOn = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOn", i));
                var starOff = this.GetScript<Transform> (string.Format ("Star{0}/RarityStarOff", i));
                if (nowRarity >= i) {
                    starOn.gameObject.SetActive (true);
                    starOff.gameObject.SetActive (false);
                } else {
                    starOn.gameObject.SetActive (false);
                    starOff.gameObject.SetActive (true);
                }
            } else {
                starObj.gameObject.SetActive (false);
            }
        }
    }


    private void LoadedResource(UnitResourceLoader loader)
    {
        var portraitRoot = this.GetScript<RectTransform> ("UnitAnchor");      
        var imageGo = new GameObject ("image");
		var image = imageGo.GetOrAddComponent<Image>();
		portraitRoot.DetachChildren();
        portraitRoot.gameObject.AddInChild (imageGo);
        image.sprite = loader.GetPortrait (m_Card.Rarity);
        image.SetNativeSize ();
        image.raycastTarget = false;
        imageGo.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 1.0f);
        imageGo.GetComponent<RectTransform> ().anchorMin = new  Vector2 (0.5f, 1.0f);
        imageGo.GetComponent<RectTransform> ().anchorMax = new  Vector2 (0.5f, 1.0f);
        imageGo.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

        // SpCardがある場合はPortraitの表示を削除してCardBgのみの表示とする
        var spCard = loader.GetSpCard (m_Card.Rarity);
        if (spCard == null) {
            this.GetScript<Image> ("CardBg").sprite = loader.GetCardBg (m_Card.Rarity);
            this.GetScript<Image> ("CardFrame").sprite = loader.GetCardFrame (m_Card.Rarity);
            this.GetScript<RectTransform> ("Emblem").gameObject.SetActive (true);
            image.gameObject.SetActive (true);
        } else {
            this.GetScript<Image> ("CardBg").sprite = spCard;
            this.GetScript<RectTransform> ("Emblem").gameObject.SetActive (false);
            image.gameObject.SetActive (false);
        }
        if (m_Loaded != null) {
            m_Loaded ();
        }
    }

    void OnDestory()
    {
        if (loader != null) {
            loader.Dispose ();
        }
        loader = null;
    }

    private CardData m_Card;
    private System.Action m_Loaded;
}
