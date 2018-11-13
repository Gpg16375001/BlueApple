using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Prime31;

using SmileLab;
using SmileLab.Net;

#if UNITY_IOS || UNITY_TVOS

public class Prime31StoreKitControll : IPurchaseControll {
    /// <summary>
    /// 課金開始前イベント
    /// </summary>
    public event PurchaseManager.OnBefore BeforeEvent;

    /// <summary>
    /// 課金成功時イベント
    /// </summary>
    public event PurchaseManager.OnSucceed SucceedEvent;

    /// <summary>
    /// 課金内容検証イベント
    /// </summary>
    public event PurchaseManager.OnValidate ValidateEvent;

    /// <summary>
    /// 課金エラー時イベント
    /// </summary>
    public event PurchaseManager.OnError ErrorEvent;

    /// <summary>
    /// 課金可能な状態か
    /// </summary>
    public bool EnablePurchase {
        get {
            return CanMakePayment;
        }
    }

    /// <summary>
    /// 課金アイテム取得時コールバック
    /// </summary>
    Action<List<SkuItem>> ProductListReceivedCallback;

    /// <summary>
    /// レシート更新時コールバック
    /// </summary>
    Action<bool, string> RefreshReceiptDone;

    /// <summary>
    /// 課金処理できるか
    /// </summary>
    bool CanMakePayment = true;

    /// <summary>
    /// 課金処理中か
    /// </summary>
    bool IsBuying {
        get {
            return CurrentBuyItem.HasValue;
        }
    }
    /// <summary>
    /// 現在課金を行おうとしている課金アイテム情報
    /// </summary>
    SkuItem? CurrentBuyItem = null;

    /// <summary>
    /// レシート取得に失敗した時の最大リトライ回数
    /// </summary>
    const int MAX_RECIEPTRETRY = 5;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="manager">Manager.</param>
    public void Init (PurchaseManager manager)
    {
        StoreKitManager.refreshReceiptSucceededEvent += RefreshReceiptSucceeded;
        StoreKitManager.refreshReceiptFailedEvent += RefreshReceiptFailed;
        StoreKitManager.productListReceivedEvent += ProductListReceived;
        StoreKitManager.productListRequestFailedEvent += ProductListRequestFailed;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent += ProductPurchaseAwaitingConfirmation;
        StoreKitManager.purchaseSuccessfulEvent += PurchaseSuccessful;
        StoreKitManager.purchaseFailedEvent += PurchaseFailed;
        StoreKitManager.purchaseCancelledEvent += PurchaseCancelled;

        // 自動でFinishTransactionしないようにする。
        // Finishする前にレシート検証とアプリサーバーへのデータ反映をするため
        StoreKitManager.autoConfirmTransactions = false;

        CanMakePayment = StoreKitBinding.canMakePayments();
    }

    /// <summary>
    /// Releases all resource used by the <see cref="Prime31StoreKitControll"/> object.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Prime31StoreKitControll"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="Prime31StoreKitControll"/> in an unusable state. After
    /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Prime31StoreKitControll"/> so
    /// the garbage collector can reclaim the memory that the <see cref="Prime31StoreKitControll"/> was occupying.</remarks>
    public void Dispose()
    {
        StoreKitManager.refreshReceiptSucceededEvent -= RefreshReceiptSucceeded;
        StoreKitManager.refreshReceiptFailedEvent -= RefreshReceiptFailed;
        StoreKitManager.productListReceivedEvent -= ProductListReceived;
        StoreKitManager.productListRequestFailedEvent -= ProductListRequestFailed;
        StoreKitManager.productPurchaseAwaitingConfirmationEvent -= ProductPurchaseAwaitingConfirmation;
        StoreKitManager.purchaseSuccessfulEvent -= PurchaseSuccessful;
        StoreKitManager.purchaseFailedEvent -= PurchaseFailed;
        StoreKitManager.purchaseCancelledEvent -= PurchaseCancelled;
    }

    /// <summary>
    /// 課金アイテム情報取得
    /// </summary>
    /// <param name="callback">Callback.</param>
    public void GetSkuItems(string[] productIDs, Action<List<SkuItem>> callback)
    {
        if (!CanMakePayment) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, string.Empty);
            return;
        }
        ProductListReceivedCallback = callback;
        StoreKitBinding.requestProductData (productIDs);
    }

    /// <summary>
    /// 課金処理
    /// </summary>
    /// <param name="item">購入したいアイテム情報</param>
    public void Buy (SkuItem item)
    {
        if (!CanMakePayment) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, string.Empty);
            return;
        }

        // 重複起動しないようにする。
        if (IsBuying) {
            return;
        }

        CurrentBuyItem = item;
        CoroutineAgent.Execute (CoBuy());
    }

    /// <summary>
    /// Validateの処理が終わっていないTransactionがあるか
    /// </summary>
    public bool ExistNotValidatedPurchase()
    {
        if (!CanMakePayment) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, string.Empty);
            return false;
        }

        // Purchasedになっているものが残っていたらtrue
        return StoreKitBinding.getAllCurrentTransactions ().Any(x => x.transactionState == StoreKitTransactionState.Purchased);
    }

    /// <summary>
    /// Validateが終わっていないものをValidate処理し直す。
    /// </summary>
    /// <param name="didValidate">Did validate.</param>
    public void ValidateNotValidatedPurchase(Action didValidate)
    {
        if (!CanMakePayment) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, string.Empty);
            return;
        }

        // 課金処理中はできないようにしておく
        if (IsBuying) {
            return;
        }

        if (ExistNotValidatedPurchase ()) {
            CoroutineAgent.Execute (CoValidateNotValidatedPurchase (didValidate));
        }
    }

#region Prime31用コールバックの実装
    void RefreshReceiptSucceeded()
    {
        // 
        if (RefreshReceiptDone != null) {
            RefreshReceiptDone (true, string.Empty);
        }
    }
    void RefreshReceiptFailed(string error)
    {
        // 
        if (RefreshReceiptDone != null) {
            RefreshReceiptDone (false, error);
        }
    }

    void ProductListReceived(List<StoreKitProduct> productList)
    {
        if (ProductListReceivedCallback != null) {
            List<SkuItem> skuItems = new List<SkuItem> ();
            foreach (var product in productList) {
                skuItems.Add (CreateSkuItem (product));
            }
            ProductListReceivedCallback (skuItems);
        }
    }
    void ProductListRequestFailed(string error)
    {
        CanMakePayment = false;
        CallError (PurchaseManager.PurchaseError.GETSKUITEM_ERROR, error, null);
    }

    void ProductPurchaseAwaitingConfirmation(StoreKitTransaction transaction)
    {
        Debug.Log (string.Format("ProductPurchaseAwaitingConfirmation: {0}", transaction.productIdentifier));
        CoroutineAgent.Execute(CoVerify(transaction));
    }

    void PurchaseSuccessful(StoreKitTransaction transaction)
    {
        var skuItem = PurchaseManager.SharedInstance.GetSkuItem (transaction.productIdentifier);
        // トランザクションの終了まで終わったら成功とする。
        if (SucceedEvent != null && skuItem.HasValue) {
            SucceedEvent (skuItem.Value);
        }
        CurrentBuyItem = null;
    }
    void PurchaseFailed(string error)
    {
        CallError (PurchaseManager.PurchaseError.PURCHASED_ERROR, error, CurrentBuyItem);

        CurrentBuyItem = null;
    }
    void PurchaseCancelled(string error)
    {
        CallError (PurchaseManager.PurchaseError.CANCELED_ERROR, error, CurrentBuyItem);

        CurrentBuyItem = null;
    }
#endregion
    // 課金アイテム情報の作成
    SkuItem CreateSkuItem(StoreKitProduct product)
    {
        var ret = new SkuItem ();
        ret.productID = product.productIdentifier;
        ret.title = product.title;
        ret.description = product.description;
        ret.formattedPrice = product.formattedPrice;
        ret.currencyCode = product.currencyCode;
        double.TryParse(product.price, out ret.price);

        return ret;
    }

    // 購入時処理
    IEnumerator CoBuy()
    {
        bool errorStop = false;
        string errorString = string.Empty;
        // 購入開始前のイベント実行
        // resultで一個でもfalseが帰ってきたら課金処理を中断
        if (BeforeEvent != null) {
            foreach (var before in BeforeEvent.GetInvocationList ()) {
                var del = before as PurchaseManager.OnBefore;
                if (del != null) {
                    for (var e = del (CurrentBuyItem.Value, (error) => {
                        errorString = error;
                        errorStop = true;
                    }); e.MoveNext ();) {
                        yield return e.Current;
                    }

                    // エラー停止
                    if (errorStop) {
                        CallError (PurchaseManager.PurchaseError.BEFORECHECK_ERROR, errorString, CurrentBuyItem);
                        CurrentBuyItem = null;
                        yield break;
                    }
                }
            }
        }
        StoreKitBinding.purchaseProduct (CurrentBuyItem.Value.productID, 1);
    }

    // 検証時処理
    IEnumerator CoVerify(StoreKitTransaction transaction)
    {
        SkuItem? skuItem = CurrentBuyItem.HasValue ? CurrentBuyItem : PurchaseManager.SharedInstance.GetSkuItem (transaction.productIdentifier);

        if (!skuItem.HasValue) {
            // 初期化がまだ終わっていないのであとで処理するようにする。
            yield break;
        }
        int retryCount = 0;
        bool isGetReceiptError = false;
        string receipt = string.Empty;
        string errorStr = string.Empty;

        do {
            bool isRefreashReceiptError = false;
            if(isGetReceiptError) {
                bool isDone = false;

                // レシートの更新処理の終了時コールバック
                RefreshReceiptDone = (succeed, error) => {
                    isDone = true;
                    isRefreashReceiptError = !succeed;
                    errorStr = error;
                };

                // レシートの取得に失敗していたらレシートの更新を要求する。
                StoreKitBinding.refreshReceipt ();

                while(!isDone) {
                    yield return null;
                }
            }

            if(!isRefreashReceiptError) {
                bool isWait = true;
                // レシートのダウンロード
                NetRequestManager.Download (StoreKitBinding.getAppStoreReceiptLocation (),
                    (data) => {
                        isWait = false;
                        isGetReceiptError = false;
Debug.Log(string.Format("Prime31StoreKitControll CoVerify receipt: {0}", receipt));
                        receipt = System.Convert.ToBase64String(data);
                    },
                    (ex) => {
                        isWait = false;
                        isGetReceiptError = true;
                        errorStr = ex.Message;
                    }
                );
                while (isWait) {
                    yield return null;
                }
            }
        } while(isGetReceiptError && string.IsNullOrEmpty(receipt) && retryCount++ < MAX_RECIEPTRETRY);

        if (isGetReceiptError && string.IsNullOrEmpty (receipt)) {
            // レシートの取得に失敗
            CallError(PurchaseManager.PurchaseError.GETRECEIPT_ERROR, errorStr, skuItem);
            yield break;
        }

        if (skuItem.HasValue) {
            bool isError = false;
            if (ValidateEvent != null) {
                foreach (var validate in ValidateEvent.GetInvocationList ()) {
                    var del = validate as PurchaseManager.OnValidate;
                    if (del != null) {
                        for (var e = del (transaction.transactionIdentifier, receipt, string.Empty, skuItem.Value,
                            (errorCode) => {
                                isError = true;
                                // エラー処理
                                switch (errorCode) {
                                case PurchaseManager.ValidateError.CONNECTION_ERROR:
                                    CallError (PurchaseManager.PurchaseError.CONNECTION_ERROR, string.Empty, skuItem);
                                    break;

                                case PurchaseManager.ValidateError.UNKNOWN_ERROR:
                                    CallError (PurchaseManager.PurchaseError.UNKNOWN_ERROR, string.Empty, skuItem);
                                    break;

                                case PurchaseManager.ValidateError.DUPLICATION_ERROR:
                                    // 追加済みなので特にエラーとしては扱わない
                                    isError = false;
                                    break;

                                case PurchaseManager.ValidateError.VERIFY_ERROR:
                                    CallError (PurchaseManager.PurchaseError.VERIFY_ERROR, string.Empty, skuItem);
                                    break;
                                }
                            }); e.MoveNext ();) {
                            yield return e.Current;
                        }

                        // どれかでエラーが発生した場合はそこで終了
                        if (isError) {
                            yield break;
                        }
                    }
                }
            }

            // どれもエラーが発生しなかったらトランザクションを終了する
            StoreKitBinding.finishPendingTransaction(transaction.transactionIdentifier);
        }
    }

    // 検証処理が終わっていないトランザクションの検証処理
    // 検証が終わるとfinishTransactionされる。
    IEnumerator CoValidateNotValidatedPurchase(Action didValidate)
    {
        foreach (var transaction in StoreKitBinding.getAllCurrentTransactions ().Where(x => x.transactionState == StoreKitTransactionState.Purchased)) {
            yield return CoVerify (transaction);
        }
        didValidate ();
    }

    // エラー時処理
    void CallError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item=null)
    {
        if (ErrorEvent != null) {
            ErrorEvent (errorCode, error, item);
        }
    }
}

#endif
