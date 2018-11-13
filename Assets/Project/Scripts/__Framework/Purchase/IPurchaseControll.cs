using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SmileLab
{
    /// <summary>
    /// I purchase controll.
    /// </summary>
    public interface IPurchaseControll : IDisposable {

        /// <summary>
        /// 課金開始前イベント
        /// </summary>
        event PurchaseManager.OnBefore BeforeEvent;

        /// <summary>
        /// 課金成功時イベント
        /// </summary>
        event PurchaseManager.OnSucceed SucceedEvent;

        /// <summary>
        /// 課金内容検証イベント
        /// </summary>
        event PurchaseManager.OnValidate ValidateEvent;

        /// <summary>
        /// 課金エラー時イベント
        /// </summary>
        event PurchaseManager.OnError ErrorEvent;

        /// <summary>
        /// 課金可能な状態か
        /// </summary>
        bool EnablePurchase {
            get;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="manager">Manager.</param>
        void Init (PurchaseManager manager);

        /// <summary>
        /// 課金アイテム情報取得
        /// </summary>
        /// <param name="callback">Callback.</param>
        void GetSkuItems(string[] productIDs, Action<List<SkuItem>> callback);

        /// <summary>
        /// 課金処理
        /// </summary>
        /// <param name="item">Item.</param>
        void Buy (SkuItem item);


        /// <summary>
        /// Validateの処理が終わっていないが課金情報があるか
        /// </summary>
        bool ExistNotValidatedPurchase ();

        /// <summary>
        /// Validateが終わっていない課金情報をValidateの処理をする。
        /// </summary>
        /// <param name="didValidate">Validateの処理終了時呼び出し関数</param>
        void ValidateNotValidatedPurchase (Action didValidate);
    }
}
