using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

using SmileLab;

#if UNITY_ANDROID

/// <summary>
/// auMarketの課金処理
/// </summary>
public class AuMarketControll : IPurchaseControll
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
	bool billingSupported = true;   // AuMarket対象ユーザーか   

	/// <summary>
    /// 初期化
    /// </summary>
    public void Init(PurchaseManager manager)
	{
		var receiver = manager.gameObject.GetOrAddComponent<AuMarketHelperReceiver>();
		receiver.DidBind += CallbackBind;
		receiver.DidConfirmReceipt += CallbackConfirmReceipt;
		receiver.DidIssueReceipt += CallbackIssueReceipt;
		receiver.DidInvalidateItem += CallbackInvalidateItem;

		m_helper = manager.gameObject.GetOrAddComponent<AuMarketHelper>();
		m_helper.Init();
	}

    /// <summary>
    /// 破棄.
    /// </summary>
	public void Dispose()
	{
		if(m_helper != null){
			this.Unbind();
			m_helper = null;
		}
	}

	/// <summary>
	/// 課金アイテム情報取得
	/// </summary>
	/// <param name="productIDs">課金アイテムのID一覧</param>
	/// <param name="callback">課金アイテム取得時呼び出しコールバック</param>
	public void GetSkuItems(string[] productIDs, Action<List<SkuItem>> callback)
	{ 
		if(m_helper == null){
			return;
		}
		if (!billingSupported) {
			this.CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return;
        }
  
		// TODO : ConfirmReceiptを空文字指定することで全アイテム情報を得られるが取ってくる必要性があるかどうか
		var list = new List<SkuItem>();
		foreach(var productId in productIDs){
			var setting = MasterDataTable.gem_store_setting.DataList.Find(i => i.store_product_id == productId);
			var data = MasterDataTable.gem_product[setting.app_product_id];
			var sku = new SkuItem { productID = setting.store_product_id, currencyCode = "", description = data.description, formattedPrice = "￥"+data.price, price = data.price, title = data.description };
			list.Add(sku);
		}
  
		this.Bind(() => {
			Debug.Log("Call CheckValidate.");
			CoroutineAgent.Execute(this.CoCheckValidate(list, callback));
		});      
	}
	IEnumerator CoCheckValidate(List<SkuItem> list, Action<List<SkuItem>> callback)
	{
		Debug.Log("CoCheckValidate start.");
		m_bCheckNotValidate = true;
        foreach (var item in list) {
			Debug.Log(item.productID+" confirm receipt....");
			m_bCheckValidateWait = true;
            m_currentBuyItem = item;
            m_helper.ConfirmReceipt(item.productID);
			yield return new WaitUntil(() => !m_bCheckValidateWait);
			Debug.Log(">>>>>>>>>> "+item.productID + " confirm receipt end.");
        }
		m_currentBuyItem = null;

		if(callback != null){
            callback(list);
        }
	}   

	/// <summary>
    /// 課金処理
    /// </summary>
    /// <param name="item">Item.</param>
    public void Buy(SkuItem item)
	{
		if (m_helper == null) {
            return;
        }
        if (!billingSupported) {
			CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return;
        }
		if(m_currentBuyItem.HasValue){
			Debug.Log("auMarket 購入中のため購入できず.");
			return; // 購入中.
		}
		m_currentBuyItem = item;
		m_bCheckNotValidate = false;
		this.Bind(() => {
			CoroutineAgent.Execute(this.CoBuy());
		});      
	}
	IEnumerator CoBuy()
	{
		Debug.Log("CoBuy");
		// 購入開始前のイベント実行
		// resultで一個でもfalseが帰ってきたら課金処理を中断
		bool errorStop = false;
        string errorString = string.Empty;
		if (this.BeforeEvent != null) {
			foreach (var before in this.BeforeEvent.GetInvocationList()) {
				var del = before as PurchaseManager.OnBefore;
				if (del != null) {
					for (var e = del(m_currentBuyItem.Value, (error) => {
                        errorString = error;
                        errorStop = true;
                    }); e.MoveNext();) {
                        yield return e.Current;
                    }
				}

				// エラー停止
                if (errorStop) {
					this.CallError(PurchaseManager.PurchaseError.BEFORECHECK_ERROR, errorString, m_currentBuyItem);
					m_currentBuyItem = null;
                    yield break;
                }
			}
		}
		m_helper.ConfirmReceipt(m_currentBuyItem.Value.productID);  // レシート確認
	}

	/// <summary>
    /// Validateの処理が終わっていないが課金情報があるか
    /// </summary>
	public bool ExistNotValidatedPurchase()
	{
		if (m_helper == null) {
            return false;
        }
        if (!billingSupported) {
			this.CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return false;
        }
		
		return nonConsumeItemList != null && nonConsumeItemList.Count > 0;
	}

	/// <summary>
    /// Validateが終わっていない課金情報をValidateの処理をする。
    /// </summary>
    /// <param name="didValidate">Validateの処理終了時呼び出し関数</param>
    public void ValidateNotValidatedPurchase(Action didValidate)
	{
		if (m_helper == null) {
            return;
        }
        if (!billingSupported) {
			this.CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "", null);
            return;
        }
  
		if (ExistNotValidatedPurchase()) {
			this.Bind(() => {
				CoroutineAgent.Execute(this.CoValidateNotValidatedPurchase(didValidate));
			});
		}
	}
	IEnumerator CoValidateNotValidatedPurchase(Action didValidate)
	{ 
		foreach(var item in nonConsumeItemList){
			yield return CoroutineAgent.Execute(this.CoVerify(item.item, item.receipt, item.signature));
		}
		nonConsumeItemList.Clear();
		if(didValidate != null){
			didValidate();
			didValidate = null;
		}      
	}

	#region Callbacks

	// コールバック : バインド処理
	void CallbackBind(long resCode)
	{      
		// 成功.
		if(resCode == 0){
			if(m_didBind != null){
				Debug.Log("Call didBeBind.");
				m_didBind();
			}
			m_bBind = true;
		}else{
			// AuMarketアプリが未インストール.
			if(resCode == -1){
				// TODO : au Marketアプリがインストールされていないと通るかどうかわからないので一旦無視.
				//CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "au Marketアプリがインストールされていません。");
				if (m_didBind != null) {
					m_didBind();
					m_didBind = null;
                }
			}
			// パーミッション未定義.
			else if(resCode == -2){
				this.CallError(PurchaseManager.PurchaseError.NOTSUPPORTED_ERROR, "au Marketアプリ利用のパーミッションが未定義です。");            
			} 
            // その他のエラー.
			else if(resCode == -99){
				this.CallError(PurchaseManager.PurchaseError.UNKNOWN_ERROR, "au Marketアプリ\n不明なエラー。");
			}
		}
	}

	// コールバック : レシート確認
	void CallbackConfirmReceipt(long resCode, string signature, string receipt)
	{      
        // 成功 = 前回の購入時に失敗がありアイテムの付与漏れがある.
		if(resCode == 0){
			if (m_bCheckNotValidate) {
                nonConsumeItemList.Add(new TransactionInfo { item = m_currentBuyItem.Value, receipt = receipt, signature = signature });
			}else{
				CoroutineAgent.Execute(this.CoVerify(m_currentBuyItem, receipt, signature));
			}         
		}
		// 指定されたアイテムが登録されていない = 前回正常に購入手続きが済んでいた.
		else if (resCode == -24) {
			if (!m_bCheckNotValidate) { 
				m_helper.IssueReceipt(m_currentBuyItem.Value.productID);
			}         
		}else{
			var msg = "";
			// 通信エラー.
			if(resCode == -1){
				msg = "通信エラー";
			}
            // AuMarketサーバーエラー
			else if(resCode == -2){
				msg = "AuMarketサーバーエラー";
			}
            // AuMarketサーバーメンテナンス.
			else if(resCode == -3){
				msg = "AuMarketサーバーメンテナンス";
			}
            // ユーザー認証エラー.
			else if(resCode == -4){
				msg = "ユーザー認証エラー";
			}
			// AuMarketアプリのバージョンアップ中.
            else if (resCode == -5) {
				msg = "AuMarketアプリが更新処理中です。";
            }
			// バージョンアップ未実施.
            else if (resCode == -6) {
				msg = "AuMarketアプリに新しいバージョンがあります。";
            }
			// 入力値エラー.
            else if (resCode == -8) {
				msg = "入力値エラー";
            }
			// ユーザー認証連続失敗に伴うパスワードロック中.
            else if (resCode == -9) {
				msg = "パスワードがロックされています。";
            }         
			// au oneトークン未設定.
            else if (resCode == -40) {
				msg = "au oneトークンが未設定です。";
            }
			// aST無効エラー.
            else if (resCode == -93) {
				msg = "aST無効エラー";
            }
			// AuMarket接続エラー.
            else if (resCode == -98) {
				msg = "AuMarket接続エラー";
            }
			// 予期せぬエラー.
            else if (resCode == -99) {
				msg = "不明なエラー";
            }
			this.CallError(PurchaseManager.PurchaseError.GETRECEIPT_ERROR, msg, m_currentBuyItem);
		}
		if (m_bCheckNotValidate) { 
			m_bCheckValidateWait = false;
		}
	}

	// コールバック : レシート発行.
	void CallbackIssueReceipt(long resCode, string signature, string receipt)
    {      
        // 正常終了.
		if (resCode == 0) {
			CoroutineAgent.Execute(this.CoVerify(m_currentBuyItem, receipt, signature));
		}
		// 購入キャンセル.
		else if(resCode == -20){
			this.CallError(PurchaseManager.PurchaseError.CANCELED_ERROR, "購入をキャンセルしました。", m_currentBuyItem);
			m_currentBuyItem = null;
        }
        // エラー.
		else{
			this.CallError(PurchaseManager.PurchaseError.GETRECEIPT_ERROR, "", m_currentBuyItem);
        }
    }

	// コールバック : レシート無効設定.
	public void CallbackInvalidateItem(long resCode)
    {      
		if (resCode == 0) {
			if (this.SucceedEvent != null && m_currentBuyItem.HasValue) {
				this.SucceedEvent(m_currentBuyItem.Value);
            }
			m_currentBuyItem = null;
        } else {
			this.CallError(PurchaseManager.PurchaseError.PURCHASED_ERROR, "", m_currentBuyItem);
        }
    }
	#endregion

	// ヘルパーのバインド.
    private void Bind(Action didBind)
    {
        if (m_helper == null) {
            return;
        }
        if (m_bBind) {
			if(didBind != null){
				didBind();            
			}
            return;
        }
		Debug.Log("Be bind!!");
		m_didBind = didBind;
        
		try{
			m_helper.Bind();
		}catch (Exception e){
			if(e is AndroidJavaException){
				this.Bind(didBind);
			}
		}
    }

    // ヘルパーのバインド解除.
    public void Unbind()
    {
        if (m_helper == null) {
            return;
        }
        if (!m_bBind) {
            return;
        }
		Debug.Log("Be unbind!!");
		m_didBind = null;
        m_helper.Unbind();
        m_bBind = false;
    }
    private bool m_bBind = false;

	// 検証時処理
	IEnumerator CoVerify(SkuItem? item, string purchaseData, string signature)
	{ 
		if (!item.HasValue) {
            yield break;
        }

		bool isError = false;
		if (this.ValidateEvent != null) { 
			foreach (var validate in this.ValidateEvent.GetInvocationList()) {
                var del = validate as PurchaseManager.OnValidate;
                if (del != null) {
					for (var e = del(item.Value.productID, purchaseData, signature, item.Value,
                        (errorCode) => {
                            isError = true;
                            // エラー処理
                            switch (errorCode) {
                            case PurchaseManager.ValidateError.CONNECTION_ERROR:
								this.CallError(PurchaseManager.PurchaseError.CONNECTION_ERROR, string.Empty, item);
                                break;

                            case PurchaseManager.ValidateError.UNKNOWN_ERROR:
								this.CallError(PurchaseManager.PurchaseError.UNKNOWN_ERROR, string.Empty, item);
                                break;

                            case PurchaseManager.ValidateError.DUPLICATION_ERROR:
                                    // 追加済みなので特にエラーとしては扱わない
                                    isError = false;
                                break;

                            case PurchaseManager.ValidateError.VERIFY_ERROR:
								this.CallError(PurchaseManager.PurchaseError.VERIFY_ERROR, string.Empty, item);
                                break;
                            }
                        }); e.MoveNext();) {
                        yield return e.Current;
                    }

                    // どれかでエラーが発生した場合はそこで終了
                    if (isError) {
                        yield break;
                    }
                }
            }
		}
		m_helper.InvalidateItem(item.Value.productID);
	}

	// 共通エラー時処理
    private void CallError(PurchaseManager.PurchaseError errorCode, string error, SkuItem? item = null)
    {
		Debug.Log("CallError : errorCode="+errorCode.ToString());
		if (this.ErrorEvent != null) {
			this.ErrorEvent(errorCode, error, item);
		}
		m_currentBuyItem = null;
    }

	private AuMarketHelper m_helper;
	private Action m_didBind = null;
	private SkuItem? m_currentBuyItem = null;   // 現在課金を行おうとしている課金アイテム情報
	private bool m_bCheckNotValidate = false;
	private bool m_bCheckValidateWait = false;   
	private List<TransactionInfo> nonConsumeItemList = new List<TransactionInfo>();   // Consumeが終わっていない課金アイテムの一覧


    // 購入情報.
    private class TransactionInfo
	{
		public SkuItem item;
		public string receipt;
		public string signature;      
	}


	/// private class : auMarketHelperからアクセスしているネイティブコードのコールバック受容体.
	private class AuMarketHelperReceiver : MonoBehaviour
	{
		/// event : バインド.
		public event Action<long/*resCode*/> DidBind;      
		/// event : レシート確認.
		public event Action<long/*resultCode*/, string/*signature*/, string/*receipt*/> DidConfirmReceipt;      
		/// event : レシート発行.
		public event Action<long/*resultCode*/, string/*signature*/, string/*receipt*/> DidIssueReceipt;
		/// event : レシート無効処理.
		public event Action<long/*resCode*/> DidInvalidateItem;
        
		#region Callbacks

		// コールバック : バインド処理
		public void CallbackBind(string message)
		{
			Debug.Log("CallbackBind : \n" + message);
			var res = Json.Deserialize(message) as Dictionary<string, object>;
            var resCode = (Int64)res["resultCode"];
   
			if (DidBind != null) {
                DidBind(resCode);
            }
		}

		// コールバック : レシート確認
		public void CallbackConfirmReceipt(string message)
		{
			Debug.Log("CallbackConfirmReceipt : \n" + message);
			var res = Json.Deserialize(message) as Dictionary<string, object>;
            var resCode = (Int64)res["resultCode"];
            var signature = (string)res["signature"];
            var receipt = (string)res["receipt"];

			if(DidConfirmReceipt != null){
				DidConfirmReceipt(resCode, signature, receipt);
			}
		}

		// コールバック : レシート発行.
		public void CallbackIssueReceipt(string message)
		{
			Debug.Log("CallbackIssueReceipt : \n" + message);
			var res = Json.Deserialize(message) as Dictionary<string, object>;
			var resCode = (Int64)res["resultCode"];
			var signature = (string)res["signature"];
            var receipt = (string)res["receipt"];

			if(DidIssueReceipt != null){
				DidIssueReceipt(resCode, signature, receipt);
			}
		}

		// コールバック : レシート無効設定.
		public void CallbackInvalidateItem(string message)
		{
			Debug.Log("CallbackInvalidateItem : \n" + message);
			var res = Json.Deserialize(message) as Dictionary<string, object>;
			var resCode = (Int64)res["resultCode"];

			if(DidInvalidateItem != null) {
				DidInvalidateItem(resCode);
			}
		}

        #endregion
	}
}

#endif