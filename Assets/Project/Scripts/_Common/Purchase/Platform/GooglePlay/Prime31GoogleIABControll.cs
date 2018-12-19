using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmileLab;
using Prime31;
using System;
using System.Linq;

#if UNITY_ANDROID

public class Prime31GoogleIABControll : IPurchaseControll
{
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
	public bool EnablePurchase
	{
		get {
			return billingSupported;
		}
	}
	/// <summary>
	/// アイテム情報の取得イベント
	/// </summary>
	Action<List<SkuItem>> QueryInventorySucceededCallback;

	/// <summary>
	/// 課金処理中か
	/// </summary>
	bool IsBuying
	{
		get {
			return CurrentBuyItem.HasValue;
		}
	}
	/// <summary>
	/// 現在課金を行おうとしている課金アイテム情報
	/// </summary>
	SkuItem? CurrentBuyItem = null;

	/// <summary>
	/// IABをサポートしているか
	/// </summary>
	bool billingSupported = true;

	bool isInited = false;

	/// <summary>
	/// Consumeが終わっていない課金情報の一覧
	/// </summary>
	List<GooglePurchase> nonConsumePurchaseList;

	/// <summary>
	/// ユーザーキャンセル判定用の定義
	/// </summary>
	const int BILLING_RESPONSE_RESULT_USER_CANCELED = 1;

	PurchaseManager _manager;
	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="manager">Manager.</param>
	public void Init(PurchaseManager manager)
	{
		// イベント登録
		GoogleIABManager.billingSupportedEvent += BillingSupported;
		GoogleIABManager.billingNotSupportedEvent += BillingNotSupported;
		GoogleIABManager.queryInventorySucceededEvent += QueryInventorySucceeded;
		GoogleIABManager.queryInventoryFailedEvent += QueryInventoryFailed;
		GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += PurchaseCompleteAwaitingVerification;
		GoogleIABManager.purchaseSucceededEvent += PurchaseSucceeded;
		GoogleIABManager.purchaseFailedEvent += PurchaseFailed;
		GoogleIABManager.consumePurchaseSucceededEvent += ConsumePurchaseSucceeded;
		GoogleIABManager.consumePurchaseFailedEvent += ConsumePurchaseFailed;

		_manager = manager;

		isInited = false;
		// GoogleIAB初期化
		// TODO: DevSeven用のキーになっているのであとで差し替えが必要


#if DEFINE_DEVELOP || DEFINE_BETA
		var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqpd90H1nF6rk9UwbhDH12YFpik5cMRWt9DciuvpPzuo8qHkXaboDsbRrIEKdBXKUWF1UsjBHjI2LO3um7iJ6+RPxNs97PdrxRJCBB6gqHUn9R1dWq79VDoGyhkbYYoed8mQeOK/DkapyK+5433h8cB5YBONgYmwwVQM7N+SjXU4UuNXI64btJjt+eGdgm0gQsvQISFanTYNp3Y8XXAwRsBznTdRjwkGOHjSjZVT++8DokTloieArZ7EBKGAVCOE3UmOXzGM0uqGK8l0WFw/ouLkvEuk0ZXqVJdJN8sZIx8qNcJCI5QCx8HXxgjYtAH7vOwkn2KkbWn78KQaujbljGwIDAQAB";
#else
        var key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApmKMbO9ssu1DnqNBn6DnKz0wYs10fifk7LiaAdbG0Fk9tU+ZiozUuswrOvUXA6Zrhrz6+egeHANRDlJHYLaPbeEWn16P19jgYwJW85gRHcDRNxOQU6VbuC0RkgcFNS7uOHXXIPPfFfPZdtL7Tt1BUDnmCsXFX//Hk5x6yY9ouXlWJ2hIBrNLrZ7YW4g4liOQIhpktbkanQDrEJz2s6AZy25lPnvPEzqRybKvSr8WMuye+iMMpP6stuV25tTZjJV6p/SACDca7RFArPcXVlSGTWgkSdINV1lA2EYKS1Hz6O8DV6aVZawQoHVceSMFuZSan9V+2C2UfD+7M9l+xOuEHwIDAQAB";
#endif
		GoogleIAB.init( key );

        // レシートの検証を自前で行う設定に変更
        GoogleIAB.setAutoVerifySignatures (false);

        // debugビルド時はログを許可する
        GoogleIAB.enableLogging (Debug.isDebugBuild);
    }

    /// <summary>
    /// Releases all resource used by the <see cref="Prime31GoogleIABControll"/> object.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Prime31GoogleIABControll"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="Prime31GoogleIABControll"/> in an unusable state. After
    /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Prime31GoogleIABControll"/> so
    /// the garbage collector can reclaim the memory that the <see cref="Prime31GoogleIABControll"/> was occupying.</remarks>
    public void Dispose()
    {
        // イベント削除
        GoogleIABManager.billingSupportedEvent -= BillingSupported;
        GoogleIABManager.billingNotSupportedEvent -= BillingNotSupported;
        GoogleIABManager.queryInventorySucceededEvent -= QueryInventorySucceeded;
        GoogleIABManager.queryInventoryFailedEvent -= QueryInventoryFailed;
        GoogleIABManager.purchaseCompleteAwaitingVerificationEvent -= PurchaseCompleteAwaitingVerification;
        GoogleIABManager.purchaseSucceededEvent -= PurchaseSucceeded;
        GoogleIABManager.purchaseFailedEvent -= PurchaseFailed;
        GoogleIABManager.consumePurchaseSucceededEvent -= ConsumePurchaseSucceeded;
        GoogleIABManager.consumePurchaseFailedEvent -= ConsumePurchaseFailed;

        // サービスの終了
        GoogleIAB.unbindService ();
    }

    private IEnumerator WaitInit(Action didEnd)
    {
        yield return new WaitUntil (() => isInited);

        if (didEnd != null) {
            didEnd ();
        }
    }

    /// <summary>
    /// 課金アイテム情報取得
    /// </summary>
    /// <param name="productIDs">課金アイテムのID一覧</param>
    /// <param name="callback">課金アイテム取得時呼び出しコールバック</param>
    public void GetSkuItems(string[] productIDs, Action<List<SkuItem>> callback)
    {
        if (!isInited) {
            _manager.StartCoroutine (WaitInit(
                () => {
                    GetSkuItems(productIDs, callback);
                }
            ));
            return;
        }
        if (!billingSupported) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return;
        }
        // コールバックの設定
        QueryInventorySucceededCallback = callback;

        // 商品リストを取得のためのリクエストを行う
        GoogleIAB.queryInventory (productIDs);
    }

    /// <summary>
    /// 課金処理
    /// </summary>
    /// <param name="item">Item.</param>
    public void Buy (SkuItem item)
    {
        if (!isInited) {
            _manager.StartCoroutine (WaitInit(
                () => {
                    Buy(item);
                }
            ));
            return;
        }
        if (!billingSupported) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
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
    public bool ExistNotValidatedPurchase ()
    {
        if (!isInited) {
            return false;
        }
        if (!billingSupported) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return false;
        }

        return nonConsumePurchaseList != null && nonConsumePurchaseList.Count(x => x.purchaseState == GooglePurchase.GooglePurchaseState.Purchased) > 0;
    }

    /// <summary>
    /// Validateが終わっていないものをValidate処理し直す。
    /// </summary>
    /// <param name="didValidate">Did validate.</param>
    public void ValidateNotValidatedPurchase (Action didValidate)
    {
        if (!isInited) {
            _manager.StartCoroutine (WaitInit(
                () => {
                    ValidateNotValidatedPurchase(didValidate);
                }
            ));
            return;
        }
        if (!billingSupported) {
            CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return;
        }

        // 重複起動しないようにする。
        if (IsBuying) {
            return;
        }

        if (ExistNotValidatedPurchase ()) {
            CoroutineAgent.Execute (CoValidateNotValidatedPurchase (didValidate));
        }
    }
#region Prime31用コールバックの実装
    void BillingSupported()
    {
        isInited = true;
        // 特に何もしない
        billingSupported = true;
    }
    void BillingNotSupported(string error)
    {
        isInited = true;

        billingSupported = false;
        CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, error, null);
    }

    void QueryInventorySucceeded(List<GooglePurchase> purchaseList, List<GoogleSkuInfo> skuInfoList)
    {
        if (QueryInventorySucceededCallback != null) {
            List<SkuItem> items = new List<SkuItem> ();
            foreach(var skuInfo in skuInfoList) {
                items.Add(CreateSkuItem (skuInfo));
            }
            QueryInventorySucceededCallback (items);
        }
        nonConsumePurchaseList = purchaseList;
    }
    void QueryInventoryFailed(string error)
    {
        billingSupported = false;
        CallError (PurchaseManager.PurchaseError.GETSKUITEM_ERROR, error, null);
    }

    void PurchaseCompleteAwaitingVerification(string purchaseData, string signature)
    {
        CoroutineAgent.Execute(CoVerify(purchaseData, signature));
    }
    void PurchaseSucceeded(GooglePurchase purchase)
    {
        //GoogleIAB.consumeProduct (purchase.productId);
    }
    void PurchaseFailed(string result, int response)
    {
        if (result.Contains ("User canceled.")) {
            CallError (PurchaseManager.PurchaseError.CANCELED_ERROR, result, CurrentBuyItem, response);
        } else {
            CallError (PurchaseManager.PurchaseError.PURCHASED_ERROR, result, CurrentBuyItem, response);
        }
        CurrentBuyItem = null;
    }

    void ConsumePurchaseSucceeded(GooglePurchase purchase)
    {
        var skuItem = PurchaseManager.SharedInstance.GetSkuItem (purchase.productId);
        // 課金アイテムの消費まで終わったら成功とする。
        if (SucceedEvent != null && skuItem.HasValue) {
            SucceedEvent (skuItem.Value);
        }
        CurrentBuyItem = null;
    }
    void ConsumePurchaseFailed(string error)
    {
        CallError (PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, error, CurrentBuyItem);
        CurrentBuyItem = null;
    }
#endregion
    // 課金アイテム情報を変換
    SkuItem CreateSkuItem(GoogleSkuInfo info)
    {
        SkuItem ret = new SkuItem ();
        ret.productID = info.productId;
        ret.title = info.title;
        ret.description = info.description;
        ret.price = info.priceAmountMicros / 1000000.0;
        ret.formattedPrice = info.price;
        ret.currencyCode = info.priceCurrencyCode;

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
        GoogleIAB.purchaseProduct (CurrentBuyItem.Value.productID);
    }

    // 検証時処理
    IEnumerator CoVerify(string purchaseData, string signature)
    {
        bool isError = false;
        var purchase = new GooglePurchase(purchaseData.dictionaryFromJson());
        SkuItem? skuItem = CurrentBuyItem.HasValue ? CurrentBuyItem : PurchaseManager.SharedInstance.GetSkuItem (purchase.productId);

        if (!skuItem.HasValue) {
            // 初期化がまだ終わっていないのであとで処理するようにする。
            yield break;
        }
        if (skuItem.HasValue) {
            if (ValidateEvent != null) {
                foreach (var validate in ValidateEvent.GetInvocationList ()) {
                    var del = validate as PurchaseManager.OnValidate;
                    if (del != null) {
                        for (var e = del (purchase.orderId, purchaseData, signature, skuItem.Value,
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

            // どれもエラーが発生しなかったらそのまま消化する
            GoogleIAB.consumeProduct (purchase.productId);
        }
    }

    // 検証処理が終わっていないトランザクションの検証処理
    // 検証が終わるとconsumePurchaseされる。
    IEnumerator CoValidateNotValidatedPurchase(Action didValidate)
    {
        foreach (var purchase in nonConsumePurchaseList) {
            // 購入済みのものだけやる。
            if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased) {
                yield return CoVerify (purchase.originalJson, purchase.signature);
            }
        }
        nonConsumePurchaseList.Clear ();
        didValidate ();
    }

    // エラー時処理
    void CallError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item=null, int purchaseErrorResponse=0)
    {
        if (ErrorEvent != null) {
            if (errorCode == PurchaseManager.PurchaseError.PURCHASED_ERROR && purchaseErrorResponse == BILLING_RESPONSE_RESULT_USER_CANCELED) {
                // この場合はキャンセルとしてエラーを発行
                errorCode = PurchaseManager.PurchaseError.CANCELED_ERROR;
            }
            ErrorEvent (errorCode, error, item);
        }
    }
}

#endif
