using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TMPro;

using SmileLab;
using SmileLab.Net.API;

public class Screen_FormationSelect : ViewBase
{
    public void Init(bool isPvp, Action backScene)
    {
        m_IsPvp = isPvp;
        m_BackScene = backScene;

        View_GlobalMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_GlobalMenu.DidTapButton += GlobalMenuDidTapButton;
        View_PlayerMenu.DidSetEnableButton += bActive => this.IsEnableButton = bActive;
        View_PlayerMenu.DidTapBackButton += DidTapBackButton;

        m_Party = m_IsPvp ?  AwsModule.PartyData.PvPTeam : AwsModule.PartyData.CurrentTeam;

        // パーティー名の設定
        this.GetScript<Text> ("txt_TeamName").text = m_Party.Name;

        CreateFormationList ();

        // 情報表示用のグリッドを作成
        var FormationGridGo = this.GetScript<Transform>("FormationGrid").gameObject;
        for (int i = 0; i < 9; ++i) {
            var go = GameObjectEx.LoadAndCreateObject ("FormationSelect/ListItem_FormationSquare", FormationGridGo);
            m_FormationSquares.Add (go.GetComponent<ListItem_FormationSquare> ());
        }
        // 設定済みデータを初期データとして表示
        SetFormationData();

        // ボタンの設定
        this.SetCanvasCustomButtonMsg ("FormationSet/bt_Common", DidTapSet);

        GetScript<ScreenBackground> ("BG").CallbackLoaded (() => {
            View_FadePanel.SharedInstance.FadeIn (View_FadePanel.FadeColor.Black);
        });
    }
     
    void CreateFormationList()
    {
        // 並び替え
        var formationDataList = FormationData.CacheGetAll ().OrderBy(x => x.FormationId == m_Party.FormationID ? 0 : 1).ThenBy(x => x.FormationId).ToList();

        // リストの初期化
        m_ListItems.Clear();
        var scrollRect = this.GetScript<ScrollRect>("ScrollAreaFormationList");
        scrollRect.content.gameObject.DestroyChildren ();
        foreach (var formationData in formationDataList) {
            var go = GameObjectEx.LoadAndCreateObject ("FormationSelect/ListItem_FormationName", scrollRect.content.gameObject);
            var formationNameScript = go.GetOrAddComponent<ListItem_FormationName> ();
            bool isUse = m_Party.FormationID == formationData.FormationId;
            formationNameScript.Init (formationData, isUse, DidTapListItem);
            m_ListItems.Add (formationNameScript);
            if (isUse) {
                m_SelectItem = formationNameScript;
                m_SelectItem.SetSelect (true);
                m_SelectFormationData = formationData;
            }
        }

    }
    void SetFormationData()
    {
        if (m_SelectFormationData == null) {
            this.GetScript<TextMeshProUGUI> ("txtp_ActiveFormationName").SetText("未設定");
            this.GetScript<TextMeshProUGUI> ("txtp_ActiveFormationLv").SetText(string.Empty);

            // 未設定の時はNoneのままにする。
            m_FormationSquares.ForEach (x => x.Init (null, -1));
            return;
        }
        this.GetScript<TextMeshProUGUI> ("txtp_ActiveFormationName").SetText(m_SelectFormationData.Formation.name);
        this.GetScript<TextMeshProUGUI> ("txtp_ActiveFormationLv").SetTextFormat("Lv.{0}", m_SelectFormationData.FormationLevel);
        this.GetScript<TextMeshProUGUI> ("txtp_FormationText").SetText(m_SelectFormationData.Formation.explain);

        var formation = m_SelectFormationData.Formation;
        for (int i = 1; i <= 9; ++i) {
            var col = formation.GetPostionColumn (i);
            var row = formation.GetPostionRow (i);

            var script = GetFormationSquare (row, col);
            script.Init (formation, i);
        }
    }

    private ListItem_FormationSquare GetFormationSquare(int row, int column)
    {
        int index = row * 3 + column;
        return m_FormationSquares[index];
    }

    // リストアイテムの押下時呼び出し
    public void DidTapListItem(ListItem_FormationName item, FormationData formation)
    {
        // Select状態の更新
        m_ListItems.ForEach(x => x.SetSelect(false));
        m_SelectItem = item;
        m_SelectItem.SetSelect (true);

        m_SelectFormationData = formation;


        // 押されたリストの情報を表示
        SetFormationData ();
    }

    // 設定ボタン
    void DidTapSet()
    {
        m_ListItems.ForEach(x => x.SetUse(false));
        m_SelectItem.SetUse (true);
        var party = m_IsPvp ? AwsModule.PartyData.PvPTeam : AwsModule.PartyData.CurrentTeam;
        party.FormationData = m_SelectFormationData;

        // 変更があった場合はリストも並び替える
        CreateFormationList ();
    }

    // バックボタン
    void DidTapBackButton()
    {
        if (m_BackScene != null) {
            m_BackScene ();
        }
    }

    void GlobalMenuDidTapButton(System.Action exec)
    {
        if (m_Party.IsEmpty) {
            PopupManager.OpenPopupYN("ユニットが一体も設定されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        // 変更が保存されていない場合
        if (AwsModule.PartyData.IsModify) {
            PopupManager.OpenPopupYN("変更が保存されていません。編集前の状態に戻していいでしょうか？",
                () => {
                    AwsModule.PartyData.Reset();
                    exec ();
                },
                () => {
                }
            );
            return;
        }

        exec ();
    }

    void Awake()
    {
        var canvas = this.gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null) {
            canvas.worldCamera = CameraHelper.SharedInstance.Camera2D;
        }
    }


    private List<ListItem_FormationName> m_ListItems = new List<ListItem_FormationName> ();
    private ListItem_FormationName m_SelectItem;

    private List<ListItem_FormationSquare> m_FormationSquares = new List<ListItem_FormationSquare> ();

    private FormationData m_SelectFormationData;
    private bool m_IsPvp;
    private Action m_BackScene;
    private Party m_Party;
}
