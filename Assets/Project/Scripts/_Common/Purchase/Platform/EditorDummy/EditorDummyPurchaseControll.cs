using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using SmileLab;
using SmileLab.Net;

#if UNITY_EDITOR
public class EditorDummyPurchaseControll : IPurchaseControll {

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
            return true;
        }
    }

    public void Dispose()
    {
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="manager">Manager.</param>
    public void Init (PurchaseManager manager)
    {
        // 特に何もしない。
    }

    /// <summary>
    /// 課金アイテム情報取得
    /// </summary>
    /// <param name="callback">Callback.</param>
    public void GetSkuItems(string[] productIDs, Action<List<SkuItem>> callback)
    {
        var products = MasterDataTable.gem_store_setting.DataList.Where(x => productIDs.Contains(x.store_product_id));

        List<SkuItem> ret = new List<SkuItem> ();

        foreach (var product in products) {
            SkuItem item = new SkuItem ();
            item.description = "Dummy";
            item.productID = product.store_product_id;
            item.price = 0.0;
            item.formattedPrice = "Dummy";
            item.currencyCode = "JPY";

            ret.Add (item);
        }

        if (callback != null) {
            callback (ret);
        }
    }

    /// <summary>
    /// 課金処理
    /// </summary>
    /// <param name="item">Item.</param>
    public void Buy (SkuItem item)
    {
        CoroutineAgent.Execute(CoVerify(item));
    }

    private IEnumerator CoVerify(SkuItem item)
    {
        bool isError = false;
        if (ValidateEvent != null) {
            foreach (var validate in ValidateEvent.GetInvocationList ()) {
                var del = validate as PurchaseManager.OnValidate;
                if (del != null) {
                    for (var e = del ("Dummy", "Dummy", string.Empty, item,
                        (errorCode) => {
                            isError = true;
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

        SucceedEvent (item);
    }


    /// <summary>
    /// Validateの処理が終わっていないが課金情報があるか
    /// </summary>
    public bool ExistNotValidatedPurchase ()
    {
        return false;
    }

    /// <summary>
    /// Validateが終わっていない課金情報をValidateの処理をする。
    /// </summary>
    /// <param name="didValidate">Validateの処理終了時呼び出し関数</param>
    public void ValidateNotValidatedPurchase (Action didValidate)
    {
        if (didValidate != null) {
            didValidate ();
        }
    }

}
#endif