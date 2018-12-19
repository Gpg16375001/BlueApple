using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;
using SmileLab.Net.API;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using SmileLab.UI;
using System;

public class ListItem_ItemIcon : ViewBase
{
    private CustomButton m_CustomButton;                    // カスタムボタン
    private Action<ListItem_ItemIcon> m_OnClick;            // OnClickイベント
    private Image m_IconImage;                              // アイコン
    private TextMeshProUGUI m_ItemCount;                    // 所持アイテム数
    private Dictionary<int, GameObject> m_RarityObjectList; // レアリティのオブジェクトリスト
    public object ItemData { get; private set; }            // アイコンデータ

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="itemData">アイテムデータ</param>
    /// <param name="onClick">クリックイベント</param>
    public void Init(object itemData, Action<ListItem_ItemIcon> onClick)
    {
        this.ItemData = itemData;

        m_CustomButton = this.GetScript<CustomButton> ("ListIconBg");
        m_OnClick = onClick;
        this.SetCanvasCustomButtonMsg ("ListIconBg", OnClickIcon, true, true);

        m_IconImage = this.GetScript<Image> ("Icon");
        if (m_IconImage != null) {
            m_IconImage.overrideSprite = null;
        }

        m_ItemCount = this.GetScript<TextMeshProUGUI> ("txtp_ItemNum");
        m_ItemCount.gameObject.SetActive (true);

        m_RarityObjectList = new Dictionary<int, GameObject> ();
        foreach (Transform obj in this.GetScript<Transform>("StarGrid")) {
            // 数字のみを文字列から取り出しパース
            int index = int.Parse(Regex.Replace (obj.name, @"[^0-9]", ""));
            m_RarityObjectList.Add (index, obj.gameObject);
        }

        if (itemData.GetType () == typeof(MaterialData)) {
            InitAtMaterialData ((MaterialData)itemData);
        } else if (itemData.GetType () == typeof(MagikiteData)) {
            InitAtMagikiteData ((MagikiteData)itemData);
        } else if (itemData.GetType () == typeof(ConsumerData)) {
            InitAtConsumerData ((ConsumerData)itemData);
        } else if (itemData.GetType () == typeof(ItemListCurrencyData)) {
            InitAtCurrencyData ((ItemListCurrencyData)itemData);
        }
    }

    void InitAtMaterialData(MaterialData data)
    {
        SetCount (data.Count);
        int rarity = data.CharaMaterialInfo.rarity;
        if (data.CharaMaterialInfo.IsSelectTypeMaterial (CharaMaterialTypeEnum.card_based_raw_material, CharaMaterialTypeEnum.card_based_material)) {
            rarity = 0;
        }
        SetRarity (rarity);

        IconLoader.LoadMaterial (
            data.MaterialId,
            (loadInfo, sprite) => {
                if(loadInfo.type == ItemTypeEnum.material && loadInfo.id == data.MaterialId) {
                    m_IconImage.overrideSprite = sprite;
                }
            }
        );
    }

    void InitAtMagikiteData(MagikiteData data)
    {
        SetCount ();
        SetRarity (data.Rarity);

        IconLoader.LoadMagikite (
            data.MagikiteId,
            (loadInfo, sprite) => {
                if(loadInfo.type == ItemTypeEnum.magikite && loadInfo.id == data.MagikiteId) {
                    m_IconImage.overrideSprite = sprite;
                }
            }
        );
    }

    void InitAtConsumerData(ConsumerData data)
    {
        SetCount (data.Count);
        SetRarity ();

        IconLoader.LoadConsumer (
            data.ConsumerId,
            (loadInfo, sprite) => {
                if(loadInfo.type == ItemTypeEnum.consumer && loadInfo.id == data.ConsumerId) {
                    m_IconImage.overrideSprite = sprite;
                }
            }
        );
    }

    void InitAtCurrencyData(ItemListCurrencyData data)
    {
        SetCount (data.itemCount);
        SetRarity ();

        var info = data.itemTypeEnum.GetIconInfo (0);
        m_IconImage.overrideSprite = info.IconSprite;
    }

    /// <summary>
    /// 所持数の設定
    /// </summary>
    void SetCount(int count = 0)
    {
        m_ItemCount.gameObject.SetActive (count > 0);
        if (m_ItemCount.gameObject.activeSelf) {
            m_ItemCount.SetText (count);
        }
    }

    /// <summary>
    /// レアリティの設定
    /// </summary>
    void SetRarity(int rarity = 0)
    {
        var root = this.GetScript<Transform> ("StarGrid").gameObject;
        var bg = this.GetScript<Transform> ("WhitePanel").gameObject;

        if (rarity == 0) {
            bg.SetActive (false);
            root.SetActive (false);
            return;
        }

        bg.SetActive (true);
        root.SetActive (true);
        foreach (var item in m_RarityObjectList) {
            item.Value.SetActive (rarity >= item.Key);
        }
    }

    void OnClickIcon()
    {
        if (m_OnClick != null) {
            m_OnClick (this);
        }
    }
}
