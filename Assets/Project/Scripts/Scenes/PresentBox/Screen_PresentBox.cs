using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class Screen_PresentBox : ViewBase
{
    public void Init(int receivablePresentCount, PresentData[] presentDataList, int historyCount, PresentData[] historyDataList)
    {
        // scroll周りの初期化
        presentScrollArea = GetScript<ScrollRect> ("Box/ScrollArea");
        presentLayoutGroup = GetScript<InfiniteGridLayoutGroup> ("ViewportBox/Content");
        presentLayoutGroup.OnUpdateItemEvent.AddListener (UpdatePresentItem);


        dropdown = GetScript<TMP_Dropdown> ("Sort/bt_PullDown");
        dropdown.onValueChanged.AddListener (DropdownValueChange);

        SetCanvasCustomButtonMsg ("GetAll/bt_Common", DidTapAllReceive);

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBack;


        m_PresentTotalCount = receivablePresentCount;
        m_PresentDataList = null;
        if (presentDataList != null) {
            m_PresentDataList = presentDataList.ToList ();
        }
        CreatePresentList();

        // フェードを開ける.
        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }


    private enum Mode
    {
        PresentBox,
        History
    }

    private void CreatePresentList()
    {
        var listItem = Resources.Load("PresentBox/ListItem_Present") as GameObject;

        // present list
        this.GetScript<TextMeshProUGUI>("txtp_PresentNum").SetText(string.Format("{0, 3}", m_PresentTotalCount));
        var noItemObj = GetScript<RectTransform>("txtp_NoItem").gameObject;
        presentScrollArea.content.gameObject.DestroyChildren ();
        if (m_PresentDataList != null && m_PresentDataList.Count > 0) {
            presentLayoutGroup.ResetScrollPosition ();
            presentLayoutGroup.Initialize (listItem, 5, m_PresentDataList.Count, false);
            noItemObj.SetActive (false);
        } else {
            noItemObj.SetActive (true);
            // 受け取りボタンを押せなくする。
            var btGetAll = GetScript<SmileLab.UI.CustomButton>("GetAll/bt_Common");
            btGetAll.interactable = false;
        }
    }

    void OnDestroy()
    {
        if (dropdown != null) {
            dropdown.onValueChanged.RemoveListener (DropdownValueChange);
        }
    }

    void DidTapBack()
    {
        if (!gameObject.activeSelf) {
            return;
        }
        View_FadePanel.SharedInstance.FadeOutWithLoadingAnim (View_FadePanel.FadeColor.Black, () => { 
            ScreenChanger.SharedInstance.GoToMyPage ();
        });
    }

    void DidTapAllReceive()
    {
        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        int receiveCount = Mathf.Min (m_PresentDataList.Count, PresentBoxSController.ONE_REQUEST_COUNT);
        int[] receivePresentIds = new int[receiveCount];
        for (int i = 0; i < receiveCount; ++i) {
            receivePresentIds [i] = m_PresentDataList [i].PresentId;
        }

        if (receivePresentIds.Length <= 0) {
            return;
        }

        LockInputManager.SharedInstance.IsLock = true;
        View_FadePanel.SharedInstance.IsLightLoading = true;
        SendAPI.PresentboxReceiveItem (receivePresentIds,
            (success, response) => {
                LockInputManager.SharedInstance.IsLock = false;
                View_FadePanel.SharedInstance.IsLightLoading = false;
                if(success) {
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

                    DidReceivePresents(response.PresentDataList);
                }
                View_FadePanel.SharedInstance.IsLightLoading = false;
                LockInputManager.SharedInstance.IsLock = false;
            }
        );
    }

    void DidReceivePresents(PresentData[] receivePresents)
    {
        View_PresentBoxRecievePop.Create(receivePresents, () => {
            var removePresets = m_PresentDataList.Where(x => receivePresents.Any(y => x.PresentId == y.PresentId)).ToList();
            foreach(var removePreset in removePresets) {
                m_PresentDataList.Remove(removePreset);
            }

            m_PresentTotalCount -= removePresets.Count;
            if(m_PresentDataList.Count < PresentBoxSController.ONE_REQUEST_COUNT && m_PresentTotalCount > m_PresentDataList.Count) {
                presentScrollArea.content.gameObject.DestroyChildren ();
                View_FadePanel.SharedInstance.IsLightLoading = true;
                SendAPI.PresentboxGetReceivableList (m_PresentDataList.Count, PresentBoxSController.ONE_REQUEST_COUNT, m_SortType, (success, response) => {
                    if(success) {
                        m_PresentDataList.AddRange(response.PresentDataList);
                        CreatePresentList();
                    }
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                });
            } else {
                CreatePresentList();
            }
        });
    }

    void UpdatePresentItem(int i, GameObject obj)
    {
        var script = obj.GetComponent<ListItem_Present> ();
        script.Init (m_PresentDataList[i], DidReceivePresents);

        if (i >= m_PresentDataList.Count - 1 && m_PresentDataList.Count < m_PresentTotalCount) {
            SendAPI.PresentboxGetReceivableList (m_PresentDataList.Count, PresentBoxSController.ONE_REQUEST_COUNT, m_SortType, (success, response) => {
                if(success) {
                    m_PresentDataList.AddRange(response.PresentDataList);
                    presentLayoutGroup.MaxItemCount = m_PresentDataList.Count;
                }
            });
        }
    }

    void DropdownValueChange(int index)
    {
        m_SortType = index;

        // 総数は変わらないので総数0の時は何もしない
        if (m_PresentTotalCount > 0) {
            LockInputManager.SharedInstance.IsLock = true;
            View_FadePanel.SharedInstance.IsLightLoading = true;
            // 並び替え情報を元に情報を取得し直す。
            SendAPI.PresentboxGetReceivableList (0, PresentBoxSController.ONE_REQUEST_COUNT, m_SortType,
                (success, response) => {
                    if (success) {
                        m_PresentDataList = new List<PresentData> (response.PresentDataList);
                        CreatePresentList ();
                    }
                    View_FadePanel.SharedInstance.IsLightLoading = false;
                    LockInputManager.SharedInstance.IsLock = false;
                }
            );
        }
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }

    int m_PresentTotalCount;
    int m_SortType;
    List<PresentData> m_PresentDataList;
    private ScrollRect presentScrollArea;
    private InfiniteGridLayoutGroup presentLayoutGroup;

    private TMP_Dropdown dropdown;
}
