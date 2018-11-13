using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// ListItem : グローバルメニューの各種メニューリストアイテム
/// </summary>
public class ListItem_GlobalMenu : ViewBase
{
    /// <summary>
    /// メニュータイプ.
    /// </summary>
    public GlobalMenuType Type { get; private set; }


    /// <summary>
    /// 初期化.
    /// </summary>
    public void Init(GlobalMenuType type)
    {
        this.Type = type;

        // ボタン設定.
        this.GetComponent<Button>().onClick.AddListener(DidTapItem);
    }

    #region ButtonDelegate

    // ボタン：タップした.
    void DidTapItem()
    {
        switch(this.Type){
        case GlobalMenuType.CharaEdit:  // キャラ編成.
            this.DidTapCharaEdit();
            break;
        case GlobalMenuType.Equip:      // 装備.
            this.DidTapEquip();
            break;
        case GlobalMenuType.Gacha:      // ガチャ.
            this.DidTapGacha();
            break;
        case GlobalMenuType.Shop:       // ショップ.
            this.DidTapShop();
            break;
        case GlobalMenuType.Friend:     // フレンド.
            this.DidTapFriend();
            break;
        }
    }

    #endregion

    // キャラ編成タップ時の処理
    void DidTapCharaEdit()
    {
        Debug.Log("DidTap CharaEdit");
        PopupManager.OpenPopupOK("キャラ編成画面は未実装です。");
    }
    // 装備タップ時の処理
    void DidTapEquip()
    {
        Debug.Log("DidTap Equip");
        PopupManager.OpenPopupOK("装備画面は未実装です。");
    }
    // ガチャタップ時の処理
    void DidTapGacha()
    {
        Debug.Log("DidTap Gacha");
        PopupManager.OpenPopupOK("ガチャ画面は未実装です。");
    }
    // ショップタップ時の処理
    void DidTapShop()
    {
        Debug.Log("DidTap Shop");
        PopupManager.OpenPopupOK("ショップ画面は未実装です。");
    }
    // フレンドタップ時の処理
    void DidTapFriend()
    {
        Debug.Log("DidTap Friend");
        PopupManager.OpenPopupOK("フレンド画面は未実装です。");
    }
}
