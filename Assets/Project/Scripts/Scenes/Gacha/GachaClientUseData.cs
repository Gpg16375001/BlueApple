using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using SmileLab.Net.API;
using SmileLab;


/// <summary>
/// クライアントで使う用のガチャデータ.
/// </summary>
public class GachaClientUseData
{
	/// <summary>
    /// 全てのコンテンツ.
    /// </summary>
	public List<ContentsForView> ContentsAllList { get; private set; }
 
    /// <summary>
    /// キャラクターガチャコンテンツリスト
    /// </summary>
	public List<ContentsForView> CharacterGachaContents { get { return ContentsAllList.Where(c => c.Type.Enum == GachaTypeEnum.character_gacha).ToList(); } }

    /// <summary>
    /// 武器ガチャコンテンツ.一つしかない想定.
    /// </summary>
	public ContentsForView WeaponContent { get { return ContentsAllList.Find(c => c.Type.Enum == GachaTypeEnum.weapon_gacha); } }

    /// <summary>
	/// キャラクターガチャ内のカテゴリーリスト.
    /// </summary>
	public List<Gacha> CategoryListInCharacter { get; private set; }
 

    /// <summary>
    /// サーバーレスポンスから情報更新.
    /// </summary>
	public void UpdateInfo(ReceiveGachaPurchaseProduct res)
	{
		foreach(var c in CharacterGachaContents){
			c.UpdateMissCount(res.RarestCardMissCount);
		}
	}

	/// <summary>
    /// 初期化.
    /// </summary>
	public GachaClientUseData(ReceiveGachaGetProductList data)
	{
		this.ContentsAllList = new List<ContentsForView>();
		foreach(var d in data.GachaProductDataList){
			var item = this.ContentsAllList.FirstOrDefault(c => c.ID == d.GachaId && c.Type.index == d.GachaType);
			if (item != null) {
				item.AddContents(d);
				continue;
            }
			var master = MasterDataTable.gacha[d.GachaId];
#if UNITY_EDITOR
			if( master == null ) {
				PopupManager.OpenPopupSystemOK( string.Format("MasterDataTable.gacha から GachaId={0}が見つかりません",d.GachaId), () => ScreenChanger.SharedInstance.Reboot());
			}else{
				this.ContentsAllList.Add(new ContentsForView(d, data.RarestCardMissCount, data.RarestCardMissGachaTriggerCount, master.priority_view));
			}
#else
			this.ContentsAllList.Add(new ContentsForView(d, data.RarestCardMissCount, data.RarestCardMissGachaTriggerCount, master.priority_view));
#endif
		}
		this.ContentsAllList.Sort((x, y) => x.PriorityView - y.PriorityView);

		this.CategoryListInCharacter = data.GachaProductDataList.Select(p => p.GachaId)
							             					    .Distinct()
			                                                    .Select(id => MasterDataTable.gacha[id])
			                                                    .Where(g => g.type.Enum == GachaTypeEnum.character_gacha)
			                                                    .ToList();
		this.CategoryListInCharacter.Sort((x, y) => x.priority_view - y.priority_view);
	}
    

    /// <summary>
    /// 種類毎のView用ガチャデータ.
    /// </summary>
	public class ContentsForView
	{
		/// <summary>ガチャID.</summary>
		public int ID { get; private set; }

        /// <summary>なんのガチャなのか情報.</summary>
		public Gacha Gacha { get; private set; }
        /// <summary>ガチャタイプ.</summary>
		public GachaType Type { get; private set; }

		/// <summary>単発ガチャチケット使用のデータ.</summary>
		public RowData DataUseTicket { get; private set; }
        /// <summary>単発ガチャのデータ.</summary>
		public RowData Data { get; private set; }
		/// <summary>単発無料ガチャのデータ.</summary>
        public RowData DataFree { get; private set; }
		/// <summary>10連ガチャのデータ.</summary>
		public RowData Data10th { get; private set; }
		/// <summary>無料10連ガチャのデータ.</summary>
        public RowData DataFree10th { get; private set; }
		/// <summary>1日1回制限有償割引単発ガチャのデータ.</summary>
		public RowData DataOncePerDayDiscount { get; private set; }

        /// <summary>ガチャ天井のリミット数.分母.</summary>
		public int MissTriggerCount { get; private set; }
        /// <summary>ガチャ天井カウント.</summary>
		public int MissCount { get; private set; }

        /// <summary>表示優先度.</summary>
		public int PriorityView { get; private set; }
  
        /// <summary>このガチャの開始時間.</summary>
		public DateTime StartDate { get; private set; }
		/// <summary>このガチャの終了時間.</summary>
        public DateTime EndDate { get; private set; }


        /// <summary>
        /// 同ガチャとしてコンテンツ追加.単発、10連、回数制限有償割引単発の3種類を判別し適宜データ更新を行う.
        /// </summary>
		public void AddContents(GachaProductData data)
		{
			if (data.DrawCount >= 10) {
				var bFree = data.ExchangeQuantity <= 0;
				if(bFree){
					DataFree10th = new RowData(data);
				}else{
					Data10th = new RowData(data);
				}
            } else {
				// 有償ガチャmp1日1回制限のみ割引する仕様がありボタンが別途存在しているので設定する.
                var bPaid = (ItemTypeEnum)data.ExchangeItemType == ItemTypeEnum.paid_gem;
				var bOncePerDay = (PurchaseLimitationEnum)data.PurchaseLimitation == PurchaseLimitationEnum.once_per_day;
				var bUseTicket = (ItemTypeEnum)data.ExchangeItemType == ItemTypeEnum.consumer && 
                                 MasterDataTable.consumer_item[data.ExchangeItemId] != null && 
                                 MasterDataTable.consumer_item[data.ExchangeItemId].sub_type == "ガチャチケット";
				if (data.ExchangeQuantity <= 0) {
                    DataFree = new RowData(data);
				}else if(bPaid && bOncePerDay) {
					DataOncePerDayDiscount = new RowData(data);
				}else if(bUseTicket){
					DataUseTicket = new RowData(data);
				}else{
					Data = new RowData(data);
				}
            }
		}
        
		public ContentsForView(GachaProductData data, int missCnt, int missTriggerCnt, int priority_view)
        {
            ID = data.GachaId;
			Gacha = MasterDataTable.gacha[data.GachaId];
			Type = MasterDataTable.gacha_type[data.GachaType];
			PriorityView = priority_view;
			StartDate = DateTime.Parse(data.StartDate, null, DateTimeStyles.RoundtripKind);
			EndDate = DateTime.Parse(data.EndDate, null, DateTimeStyles.RoundtripKind);

			Debug.Log(Gacha.name+" : start="+StartDate+" ~ end="+EndDate);

			this.MissCount = missCnt;
			this.MissTriggerCount = missTriggerCnt;

            AddContents(data);
        }

        // 天井カウント更新
		public void UpdateMissCount(int newValue)
		{
			this.MissCount = newValue;
		}

		private Action<GachaType> m_didUpdateDrawCount;


		// class : データ本体.
		public class RowData
        {
            /// <summary>ガチャID.</summary>
            public int ID { get { return m_data.GachaId; } }

            /// <summary>商品ID.購入時に使用.</summary>
			public int ProductID { get { return m_data.GachaProductId; } }
            
			/// <summary>ガチャタイプ.使用通貨名を取得したい場合はUseCurrencyNameを使用する.</summary>
            public GachaType Type { get { return MasterDataTable.gacha_type[m_data.GachaType]; } }

			/// <summary>このガチャのタイトル.</summary>
			public string Title { get { return MasterDataTable.gacha[ID].name; } }
			/// <summary>コンテンツ名.</summary>
            public string ContentsName { get { return MasterDataTable.gacha_product[ProductID].product_name; } }
            /// <summary>詳細説明URL.</summary>
			public string DescriptionURL { get { return MasterDataTable.gacha_product[ProductID].url_description; } }

            /// <summary>このガチャの引く回数.</summary>
			public int DrawCount { get { return m_data.DrawCount; } }

            /// <summary>使用通貨名として表示したいときはこれを使う.</summary>
			public string UseCurrencyName { get { return MasterDataTable.item_type[m_data.ExchangeItemType].display_name; } }
            
            /// <summary>使用通貨のアイコン名.</summary>
			public string UseCurrencyIconSptName 
			{
				get {
					switch(CurrencyType){
						case GachaUseCurrencyType.FreeGem:
						case GachaUseCurrencyType.PaidGem:
							return "IconGem";
						case GachaUseCurrencyType.Money:
							return "IconCoin";
						case GachaUseCurrencyType.TicketOne:
						case GachaUseCurrencyType.TicketOneR4:
							return "IconGachaTicket";
					}
					// TODO : 未設定のものはとりあえずgemとする.
					return "IconGem";
				}
			}
            
            /// <summary>購入制限種別.</summary>
			public PurchaseLimitationEnum DrawLimitaionType { get { return (PurchaseLimitationEnum)m_data.PurchaseLimitation; } }
			/// <summary>このガチャの開始期間.</summary>
			public DateTime StartDate { get; private set; }
			/// <summary>このガチャの終了期間.</summary>
			public DateTime EndDate { get; private set; }

            /// <summary>交換情報：通貨タイプ.</summary>
            public GachaUseCurrencyType CurrencyType
            {
                get {
                    switch ((ItemTypeEnum)m_data.ExchangeItemType) {
                        case ItemTypeEnum.paid_gem:
                            return GachaUseCurrencyType.PaidGem;
                        case ItemTypeEnum.free_gem:
                            return GachaUseCurrencyType.FreeGem;
						case ItemTypeEnum.money:
							return GachaUseCurrencyType.Money;
                        case ItemTypeEnum.event_point:
                            return GachaUseCurrencyType.EventPoint;
						case ItemTypeEnum.consumer:
							// TODO : IDで直接見るのは何とかしたい.
							if(m_data.ExchangeItemId == 30001){
								return GachaUseCurrencyType.TicketOne;
							}else if(m_data.ExchangeItemId == 30002){
								return GachaUseCurrencyType.TicketOneR4;
							}
							break;
                    }
					Debug.LogError("[GachaClientUseData.Contents] CurrencyType Error!! : unknown type. "+(ItemTypeEnum)m_data.ExchangeItemType+" id="+m_data.ExchangeItemId);
                    return GachaUseCurrencyType.Unknown;
                }
            }
            /// <summary>交換情報：必要通貨数.</summary>
            public int CurrencyQuantity { get { return m_data.ExchangeQuantity; } }

            /// <summary>購入可能？</summary>
			public bool IsPurchasable { get; private set; }

            /// <summary>有償通貨ガチャかどうか.</summary>
			public bool IsToll { get { return CurrencyType == GachaUseCurrencyType.PaidGem; } }

			/// <summary>ジェムガチャかどうか.</summary>
			public bool IsGem { get { return CurrencyType == GachaUseCurrencyType.PaidGem || CurrencyType == GachaUseCurrencyType.FreeGem; } }
            
            /// <summary>ガチャ使用通貨に応じた所持通貨数.</summary>
            public int HaveCurrencyValue
			{
				get {
					switch(CurrencyType){
						case GachaUseCurrencyType.PaidGem:
							return AwsModule.UserData.UserData.PaidGemCount;
						case GachaUseCurrencyType.FreeGem:
							return AwsModule.UserData.UserData.GemCount;
						case GachaUseCurrencyType.Money:
							return AwsModule.UserData.UserData.GoldCount;
						case GachaUseCurrencyType.EventPoint:
                            return 0;    // TODO : イベントポイントはまだ持ってない. 2018/4/11時点.
						case GachaUseCurrencyType.TicketOne:
						case GachaUseCurrencyType.TicketOneR4:
							return ConsumerData.CacheGet(m_data.ExchangeItemId) != null ? ConsumerData.CacheGet(m_data.ExchangeItemId).Count: 0;

					}
					Debug.LogError("[View_Gacha_List] CurrencyType Error!! " + this.Type.name + "ガチャ " + ContentsName + " unknown currency. currencyType=" + CurrencyType);
					return 0;
				}
			}

            /// <summary>通貨が足りているかどうかのチェック.</summary>
			public bool CheckEnoughCurrency()
            {
                switch (CurrencyType) {
					default:
						Debug.Log("CheckEnoughCurrency : type="+CurrencyType+" IsEnough=" + (CurrencyQuantity <= HaveCurrencyValue).ToString());
						return CurrencyQuantity <= HaveCurrencyValue;
                }
            }

            /// <summary>
            /// 引いた後に呼ぶ.情報の差分更新を行う.
            /// </summary>
            public void UpdateDrawCount()
			{
				switch(DrawLimitaionType){
					case PurchaseLimitationEnum.once_per_day:   // 1日1回
						IsPurchasable = false;
						break;
					default:
						if(Type.Enum == GachaTypeEnum.weapon_gacha){
							IsPurchasable = CheckEnoughCurrency() && AwsModule.UserData.UserData.WeaponBagCapacity >= WeaponData.CacheGetAll().Count();
						}else{
							IsPurchasable = CheckEnoughCurrency();
							Debug.Log("UpdateDrawCount CurrencyType=" + CurrencyType + " IsPurchasable=" + IsPurchasable);
						}                  
						break;
				}
			}

			public RowData(GachaProductData data)
            {
                m_data = data;
				IsPurchasable = data.IsPurchasable;
				StartDate = DateTime.Parse(data.StartDate, null, DateTimeStyles.RoundtripKind);
                EndDate = DateTime.Parse(data.EndDate, null, DateTimeStyles.RoundtripKind);
            }

            private GachaProductData m_data;
        }
	}
}

/// <summary>
/// enum : ガチャで使用する通貨タイプ.
/// </summary>
public enum GachaUseCurrencyType
{
	Unknown, 
    PaidGem,
    FreeGem,
    Money,
    EventPoint,
	TicketOne,      // 単回ガチャチケット
	TicketOneR4,    // 星4確定ガチャチケット.
}