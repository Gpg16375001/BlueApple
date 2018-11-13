using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace SmileLab
{
    /// <summary>
    /// Purchase manager.
    /// </summary>
    public class PurchaseManager : ViewBase
    {
        /// <summary>
        /// アプリ側課金開始前処理
        /// resultは課金処理に進んでいいか設定を行う関数
        /// 進んでいい時はtrueを進めてはいけない時はfalseを設定する。
        /// </summary>
        public delegate IEnumerator OnBefore (SkuItem item, Action<string> error);

        /// <summary>
        /// アプリ側課金成功時処理
        /// </summary>
        public delegate void OnSucceed (SkuItem item);

        /// <summary>
        /// アプリ側検証処理
        /// </summary>
        public delegate IEnumerator OnValidate (string transactionID, string receipt, string signature, SkuItem item, Action<ValidateError> validateError);

        /// <summary>
        /// アプリ側エラー時処理
        /// </summary>
        public delegate void OnError (PurchaseError errorCode, string error, SkuItem? item);

        /// <summary>
        /// 検証時エラー定義
        /// アプリ側で検証時にエラーが出た時にフレームワーク側へのエラー通知
        /// </summary>
        public enum ValidateError
        {
            // 通信エラー
            CONNECTION_ERROR,
            // すでに処理済みのレシートであるのでトランザクションを終了させる。扱いは成功となる。
            DUPLICATION_ERROR,
            // 検証に失敗
            VERIFY_ERROR,
            // 原因不明
            UNKNOWN_ERROR
        }

        /// <summary>
        /// 課金時エラー定義
        /// フレームワーク側からアプリ側へのエラー通知用
        /// </summary>
        public enum PurchaseError
        {
            // 課金システムを対応していない
            NOTSUPPORTED_ERROR,
            // 課金アイテム情報の取得時にエラー
            GETSKUITEM_ERROR,
            // Beforeコールバック内でエラーが発生。
            BEFORECHECK_ERROR,
            // 通信エラー
            CONNECTION_ERROR,
            // 検証エラー
            VERIFY_ERROR,
            // ユーザーが課金処理を中断した場合
            CANCELED_ERROR,
            // 課金処理中に何らかの原因でエラーがでた
            PURCHASED_ERROR,
            // レシートの取得に失敗した(iOSのみ)
            GETRECEIPT_ERROR,
            // 原因不明
            UNKNOWN_ERROR,
        };

        /// <summary>
        /// Gets the shared instance.
        /// </summary>
        /// <value>The shared instance.</value>
        public static PurchaseManager SharedInstance { get; private set; }

        /// <summary>
        /// 課金アイテム情報
        /// </summary>
        public List<SkuItem> SkuItems { get; private set; }

        /// <summary>
        /// 課金開始前イベント
        /// </summary>
        public event PurchaseManager.OnBefore BeforeEvent {
            add {
                PurchaseController.BeforeEvent += value;
            }
            remove {
                PurchaseController.BeforeEvent -= value;
            }
        }

        /// <summary>
        /// 課金成功時イベント
        /// </summary>
        public event PurchaseManager.OnSucceed SucceedEvent {
            add {
                PurchaseController.SucceedEvent += value;
            }
            remove {
                PurchaseController.SucceedEvent -= value;
            }
        }

        /// <summary>
        /// 課金確認時イベント
        /// </summary>
        public event PurchaseManager.OnValidate ValidateEvent {
            add {
                PurchaseController.ValidateEvent += value;
            }
            remove {
                PurchaseController.ValidateEvent -= value;
            }
        }

        /// <summary>
        /// エラー時イベント
        /// </summary>
        public event PurchaseManager.OnError ErrorEvent {
            add {
                PurchaseController.ErrorEvent += value;
            }
            remove {
                PurchaseController.ErrorEvent -= value;
            }
        }

        /// <summary>
        /// 課金実処理コントローラー
        /// </summary>
        private IPurchaseControll PurchaseController;

        /// <summary>
        /// 初期化済みか
        /// </summary>
        private bool IsInitialized {
            get {
                return PurchaseController != null && !_WaitGetSkuItem;
            }
        }

        /// <summary>
        /// 課金処理を行うことができるか
        /// </summary>
        public bool EnablePurchase {
            get {
                return SkuItems != null && SkuItems.Count > 0 && PurchaseController.EnablePurchase;
            }
        }

        private bool _WaitGetSkuItem = false;
        public bool WaitGetSkuItem {
            get {
                return _WaitGetSkuItem;
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="productIDs">課金アイテムのproductIDの一覧</param>
        /// <typeparam name="T">IPurchaseControllを継承したクラスを指定</typeparam>
        public void Initialize <T> (string[] productIDs) where T : IPurchaseControll
        {
            PurchaseController = (IPurchaseControll)System.Activator.CreateInstance (typeof(T));
            PurchaseController.Init (this);
            _WaitGetSkuItem = true;
            PurchaseController.GetSkuItems (productIDs, (skuList) => {
                _WaitGetSkuItem = false;
                SkuItems = new List<SkuItem>(skuList);
            });
        }

        /// <summary>
        /// 商品情報を取得
        /// </summary>
        /// <returns>商品情報
        /// nullableで返すのでnullの場合はproductIDが見つからなかった</returns>
        /// <param name="productID">欲しい商品情報のproductID</param>
        public SkuItem? GetSkuItem (string productID)
        {
            if (SkuItems != null && SkuItems.Count > 0) {
                return SkuItems.FirstOrDefault (x => x.productID.Equals (productID));
            }
            return null;
        }

        public void BuyItem (SkuItem item)
        {
            if (!IsInitialized) {
                return;
            }
            PurchaseController.Buy (item);
        }

        public bool ExistNonValidateTransaction ()
        {
            if (!IsInitialized) {
                return false;
            }
            return PurchaseController.ExistNotValidatedPurchase ();
        }

        public void ValidateForNonValdateTransaction (Action didValidate)
        {
            if (!IsInitialized) {
                return;
            }
            PurchaseController.ValidateNotValidatedPurchase (didValidate);
        }

        /// <summary>
        /// 使用後は必ず呼び出すこと！内部でGameObjectも破棄してる.アンマネージド系のリソースを使うことを考慮.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="SmileLab.PurchaseManager"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="SmileLab.PurchaseManager"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="SmileLab.PurchaseManager"/>
        /// so the garbage collector can reclaim the memory that the <see cref="SmileLab.PurchaseManager"/> was occupying.</remarks>
        public override void Dispose ()
        {
            if (PurchaseController != null) {
                PurchaseController.Dispose ();
            }
            base.Dispose ();
        }

        void Awake ()
        {
            if (SharedInstance != null) {
                SharedInstance.Dispose ();
            }
            SharedInstance = this;
        }
    }
}
