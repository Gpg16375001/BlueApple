using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;
using SmileLab.UI;
using SmileLab.Net;
using SmileLab.Net.API;
using TMPro;
using System.Text.RegularExpressions;

/// <summary>
/// View : アイテムの詳細
/// </summary>
public class View_ItemListDetailsPop : PopupViewBase 
{
    /// <summary>
    /// 生成
    /// </summary>
    public static View_ItemListDetailsPop Create(object itemData, Action didClose = null)
    {
        var go = GameObjectEx.LoadAndCreateObject ("ItemList/View_ItemListDetailsPop");
        var c = go.GetOrAddComponent<View_ItemListDetailsPop> ();
        c.InitInternal (itemData, didClose);
        return c;
    }
    private void InitInternal(object itemData, Action didClose)
    {
        m_didClose = didClose;

        this.SetCanvasCustomButtonMsg ("bt_Close", DidTapClose);

        Sprite _itemIcon = null;    // アイコン画像
        string _itemName = "";      // アイテム名
        int _itemRarity = 0;        // レアリティ
        string _flavorText = "";    // 説明

        m_RarityObjectList = new Dictionary<int, GameObject> ();
        foreach (Transform obj in this.GetScript<Transform>("StarGrid")) {
            // 数字のみを文字列から取り出しパース
            int index = int.Parse(Regex.Replace (obj.name, @"[^0-9]", ""));
            m_RarityObjectList.Add (index, obj.gameObject);
        }

        if (itemData.GetType () == typeof(MaterialData)) {
            // アイテムデータがマテリアル
            var _data = (MaterialData)itemData;
            _itemName = _data.CharaMaterialInfo.name;
            _itemRarity = _data.CharaMaterialInfo.rarity;
            if (_data.CharaMaterialInfo.IsSelectTypeMaterial (CharaMaterialTypeEnum.card_based_raw_material, CharaMaterialTypeEnum.card_based_material)) {
                _itemRarity = 0;
            }
            _flavorText = _data.CharaMaterialInfo.flavor_text;

            IconLoader.LoadMaterial (
                _data.MaterialId,
                (loadinfo, sprite) => {
                    if (loadinfo.type == ItemTypeEnum.material && loadinfo.id == _data.MaterialId) {
                        _itemIcon = sprite;
                    }
                }
            );
        } else if (itemData.GetType () == typeof(MagikiteData)) {
            // アイテムデータがマギカイト
            var _data = (MagikiteData)itemData;
            _itemName = _data.Magikite.name;
            _itemRarity = _data.Rarity;
            if (_data.Magikite.skill_id.HasValue) {
                _flavorText = MasterDataTable.skill.DataList.Find(x => x.id == _data.Magikite.skill_id.Value).flavor;
            }

            IconLoader.LoadMagikite (
                _data.MagikiteId,
                (loadinfo, sprite) => {
                    if (loadinfo.type == ItemTypeEnum.magikite && loadinfo.id == _data.MagikiteId) {
                        _itemIcon = sprite;
                    }
                }
            );
        } else if (itemData.GetType () == typeof(ConsumerData)) {
            // アイテムデータがその他
            var _data = (ConsumerData)itemData;
            _itemName = MasterDataTable.consumer_item[_data.ConsumerId].name;
            _flavorText = MasterDataTable.consumer_item[_data.ConsumerId].flavor_text;

            IconLoader.LoadConsumer (
                _data.ConsumerId,
                (loadInfo, sprite) => {
                    if(loadInfo.type == ItemTypeEnum.consumer && loadInfo.id == _data.ConsumerId) {
                        _itemIcon = sprite;
                    }
                }
            );
        } else if (itemData.GetType () == typeof(ItemListCurrencyData)) {
            try {
                var _data = (ItemListCurrencyData)itemData;
                _itemName = _data.itemData.name;
                _flavorText = _data.itemData.flavor_text;
                var info = _data.itemTypeEnum.GetIconInfo (0);
                _itemIcon = info.IconSprite;
            }
            catch {
            }
        }

        this.GetScript<TextMeshProUGUI> ("txtp_Name").SetText (_itemName);
        this.GetScript<TextMeshProUGUI> ("txtp_ItemDetail").SetText (_flavorText);
        this.GetScript<Image> ("Icon").overrideSprite = _itemIcon;

        this.GetScript<Transform> ("WhitePanel").gameObject.SetActive (_itemRarity > 0);
        this.GetScript<Transform>("StarGrid").gameObject.SetActive(_itemRarity > 0);
        if (_itemRarity > 0) {
            foreach (var item in m_RarityObjectList) {
                item.Value.SetActive (_itemRarity >= item.Key);
            }
        }
    }

    void DidTapClose()
    {
        this.PlayOpenCloseAnimation (
            false,
            () => {
                if (m_didClose != null) {
                    m_didClose ();
                }
                base.Dispose();
            }
        );
    }

    Action m_didClose;
    Dictionary<int, GameObject> m_RarityObjectList;
}