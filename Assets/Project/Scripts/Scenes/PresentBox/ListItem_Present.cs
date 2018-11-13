using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class ListItem_Present : ViewBase
{
    public void Init(PresentData data = null, Action<PresentData[]> didReceiveItem = null) {
        m_Data = data;
        m_DidReceiveItem = didReceiveItem;
        if (m_Data != null) {
            if (m_IsInitFirst) {
                SetCanvasCustomButtonMsg ("bt_Common", DidTapPresentGet);
                m_IsInitFirst = false;
            }
            ItemTypeEnum type = (ItemTypeEnum)m_Data.ItemType;

            // icon設定
            var iconInfo = type.GetIconInfo(m_Data.ItemId, data.ItemType != (int)ItemTypeEnum.card && data.ItemType != (int)ItemTypeEnum.weapon);
            if (iconInfo.IsEnableSprite) {
                GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive(false);
                GetScript<RectTransform> ("OtherIconRoot").gameObject.SetActive(false);
                GetScript<RectTransform> ("Item").gameObject.SetActive(true);
                GetScript<uGUISprite> ("ItemIcon").gameObject.SetActive (true);
                GetScript<uGUISprite> ("ItemIcon").LoadAtlasFromResources (iconInfo.AtlasName, iconInfo.SpriteName);
            } else if (iconInfo.IconObject != null) {
                GetScript<uGUISprite> ("ItemIcon").gameObject.SetActive (false);

                GameObject iconRoot = null;
                if (data.ItemType != (int)ItemTypeEnum.card && data.ItemType != (int)ItemTypeEnum.weapon) {
                    GetScript<RectTransform> ("UnitWeaponRoot").gameObject.SetActive (false);
                    GetScript<RectTransform> ("Item").gameObject.SetActive (true);
                    GetScript<uGUISprite> ("ItemIcon").gameObject.SetActive (false);
                    iconRoot = GetScript<RectTransform> ("OtherIconRoot").gameObject;
                } else {
                    GetScript<RectTransform> ("Item").gameObject.SetActive (false);
                    iconRoot = GetScript<RectTransform> ("UnitWeaponRoot").gameObject;
                }

                iconRoot.DestroyChildren ();
                iconRoot.gameObject.SetActive (true);

                iconInfo.IconObject.transform.SetParent (iconRoot.transform);
                iconInfo.IconObject.transform.localScale = Vector3.one;
                iconInfo.IconObject.transform.localPosition = Vector3.zero;
                iconInfo.IconObject.transform.localRotation = Quaternion.identity;
            }

            var txtpPresentName = GetScript<TextMeshProUGUI> ("txtp_PresentName");
            txtpPresentName.SetText (type.GetName (m_Data.ItemId));

            var txtpNum = GetScript<TextMeshProUGUI> ("txtp_Num");
            txtpNum.SetText (m_Data.Quantity.ToString());

            var txtpPresentDetail = GetScript<TextMeshProUGUI> ("txtp_PresentDetails");
            txtpPresentDetail.SetText (m_Data.Description);

            var boxObj = GetScript<RectTransform> ("Box").gameObject;
            var historyObj = GetScript<RectTransform> ("History").gameObject;

            boxObj.SetActive (!m_Data.IsReceived);
            historyObj.SetActive (m_Data.IsReceived);
            if (!m_Data.IsReceived) {
                DateTime createDate;
                // 受け取り期限の計算
                // TODO: DateTimeの生成などは共通関数にすべきかな？
                var str = m_Data.CreationDate.Replace ("+0000", "");
                createDate = System.DateTime.Parse (str, CultureInfo.CurrentCulture);
                // TODO: 受け取り期限の定数をどこかで持たないと。。。
                var limitDate = createDate.AddDays (30);
                var limit_delta = limitDate - GameTime.SharedInstance.Now;

                var txtpTimeLimit = GetScript<TextMeshProUGUI> ("txtp_TimeLimit");
                if (limit_delta.Days >= 1) {
                    txtpTimeLimit.color = new Color(0.93f, 0.81f, 0.61f);
                    txtpTimeLimit.SetText (string.Format ("あと{0}日", limit_delta.Days));
                } else {
                    txtpTimeLimit.color = new Color(1.0f, 0.99f, 0.0f);
                    txtpTimeLimit.SetText ("今日まで");
                }
            } else {
                DateTime recieveDate;
                var str = m_Data.ModificationDate.Replace ("+0000", "");
                recieveDate = System.DateTime.Parse (str, CultureInfo.CurrentCulture);
                var txtp_Date = GetScript<TextMeshProUGUI> ("txtp_Date");
                txtp_Date.SetText(recieveDate.ToString ("yyyy年MM月dd日"));
            }
        }
    }

    void DidTapPresentGet()
    {
        if (m_Data != null) {
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.IsLightLoading = true;
            SendAPI.PresentboxReceiveItem (new int[1] { m_Data.PresentId },
                (success, response) => {
                    LockInputManager.SharedInstance.IsLock = false;
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    if (success) {
                        AwsModule.UserData.UserData = response.UserData;
                        if(response.CardDataList != null) {
                            response.CardDataList.CacheSet();
                        }
                        if(response.MagikiteDataList != null) {
                            response.MagikiteDataList.CacheSet();
                        }
                        if(response.MaterialDataList != null) {
                            response.MaterialDataList.CacheSet();
                        }
                        if(response.WeaponDataList != null) {
                            response.WeaponDataList.CacheSet();
                        }
                        if(response.ConsumerDataList != null) {
                            response.ConsumerDataList.CacheSet();
                        }
                        if (m_DidReceiveItem != null) {
                            m_DidReceiveItem (response.PresentDataList);
                        }
                    }
                }
            );
        }
    }

    PresentData m_Data;
    Action<PresentData[]> m_DidReceiveItem;
    private bool m_IsInitFirst = true;
}
