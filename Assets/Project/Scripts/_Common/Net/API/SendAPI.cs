using System.Collections;
using System;
using System.Collections.Generic;
using SmileLab.Net.API;

public static class SendAPI
{
	/// <summary>
	/// URL: /auth/
	/// blah
	/// </summary>
	public static void AuthIndex(string Username, string Password, Action<bool, ReceiveAuthIndex> didLoad)
	{
		SendAuthIndex request = new SendAuthIndex ();
		request.Username = Username;
		request.Password = Password;
		AwsModule.Request.Exec<ReceiveAuthIndex> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveAuthIndex>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/get_card_list
	/// - 所持カードリストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardDataList:
	///     CardDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - CardData
	///   CardId:
	///     カードID
	///   Exp:
	///     経験値
	///   LimitBreakGrade:
	///     限界突破グレード（0〜7）
	///   BoardDataList:
	///     BoardDataのリスト
	///   EquippedWeaponBagId:
	///     装着武器バッグID
	///   EquippedMagikiteBagIdList:
	///     装着マギカイトバッグIDのリスト（計8スロット分、装備無しスロットは「0」）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     カード初回獲得日時
	/// - BoardData
	///   Index:
	///     ボード番号
	///   IsAvailable:
	///     ボードの利用可否（True:可、False:不可）
	///   UnlockedSlotList:
	///     開放済みスロット番号のリスト
	/// </summary>
	public static void CardsGetCardList(Action<bool, ReceiveCardsGetCardList> didLoad)
	{
		SendCardsGetCardList request = new SendCardsGetCardList ();
		AwsModule.Request.Exec<ReceiveCardsGetCardList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsGetCardList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/set_weapon
	/// - 武器を指定カードにセットする（装備済みの武器は自動で外れる）
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   EquippedWeaponList:
	///     装備変更するカードのEquippedWeaponのリスト
	/// - EquippedWeapon:
	///   CardId:
	///     武器を装備させるカードのCardId
	///   WeaponBagId:
	///     武器のBagId（0指定でカードから武器を外す）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   AffectedCardDataList:
	///     影響のあったCardDataのリスト
	///   AffectedWeaponDataList:
	///     影響のあったWeaponDataのリスト
	/// </summary>
	public static void CardsSetWeapon(EquippedWeapon[] EquippedWeaponList, Action<bool, ReceiveCardsSetWeapon> didLoad)
	{
		SendCardsSetWeapon request = new SendCardsSetWeapon ();
		request.EquippedWeaponList = EquippedWeaponList;
		AwsModule.Request.Exec<ReceiveCardsSetWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsSetWeapon>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/set_magikite
	/// - マギカイトを指定カードの各スロットに装備する（装備済みのマギカイトは自動で外れる）
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   EquippedMagikiteList:
	///     装備変更するカードのEquippedMagikiteのリスト
	/// - EquippedMagikite:
	///   CardId:
	///     マギカイトを装備させるカードのCardId
	///   MagikiteBagIdList:
	///     装備するマギカイトのBagIdのリスト（0指定でカードから外す）
	///     開放した育成ボードに応じて最大8個
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   AffectedCardDataList:
	///     影響のあったCardDataのリスト
	///   AffectedMagikiteDataList:
	///     影響のあったMagikiteDataのリスト
	/// </summary>
	public static void CardsSetMagikite(EquippedMagikite[] EquippedMagikiteList, Action<bool, ReceiveCardsSetMagikite> didLoad)
	{
		SendCardsSetMagikite request = new SendCardsSetMagikite ();
		request.EquippedMagikiteList = EquippedMagikiteList;
		AwsModule.Request.Exec<ReceiveCardsSetMagikite> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsSetMagikite>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/reinforce_card
	/// - カードを強化する（EXP加算）
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   CardId:
	///     強化するカードのCardId
	///   MaterialIdList:
	///     使用する強化素材のMaterialIdのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   ReinforcementDegreeId:
	///     1:成功、2:大成功、3:超成功
	///   CardData:
	///     強化したカードのCardData
	///   UserData:
	///     ユーザーデータ
	/// </summary>
	public static void CardsReinforceCard(int CardId, int[] MaterialIdList, Action<bool, ReceiveCardsReinforceCard> didLoad)
	{
		SendCardsReinforceCard request = new SendCardsReinforceCard ();
		request.CardId = CardId;
		request.MaterialIdList = MaterialIdList;
		AwsModule.Request.Exec<ReceiveCardsReinforceCard> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsReinforceCard>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/evolve_card
	/// - カードを進化させる（レアリティアップ）
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   CardId:
	///     進化させるカードのCardId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     強化したカードのCardData
	///   UserData:
	///     ユーザーデータ
	/// </summary>
	public static void CardsEvolveCard(int CardId, Action<bool, ReceiveCardsEvolveCard> didLoad)
	{
		SendCardsEvolveCard request = new SendCardsEvolveCard ();
		request.CardId = CardId;
		AwsModule.Request.Exec<ReceiveCardsEvolveCard> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsEvolveCard>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/limit_break
	/// - カードを限界突破する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   CardId:
	///     限界突破するカードのCardId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsLimitBreak(int CardId, Action<bool, ReceiveCardsLimitBreak> didLoad)
	{
		SendCardsLimitBreak request = new SendCardsLimitBreak ();
		request.CardId = CardId;
		AwsModule.Request.Exec<ReceiveCardsLimitBreak> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsLimitBreak>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/limit_break_with_gem
	/// - ジェムを使用してカードを限界突破する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   CardId:
	///     限界突破するカードのCardId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsLimitBreakWithGem(int CardId, Action<bool, ReceiveCardsLimitBreakWithGem> didLoad)
	{
		SendCardsLimitBreakWithGem request = new SendCardsLimitBreakWithGem ();
		request.CardId = CardId;
		AwsModule.Request.Exec<ReceiveCardsLimitBreakWithGem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsLimitBreakWithGem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/unlock_board_slot
	/// - ボードの指定スロットを開放する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）  
	///   CardId:
	///     CardId
	///   BoardIndex:
	///     ボード番号（1〜、開放済みのボードに限る）
	///   SlotIndexList:
	///     スロット番号（1〜、未開放のスロットに限る）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsUnlockBoardSlot(int CardId, int BoardIndex, int[] SlotIndexList, Action<bool, ReceiveCardsUnlockBoardSlot> didLoad)
	{
		SendCardsUnlockBoardSlot request = new SendCardsUnlockBoardSlot ();
		request.CardId = CardId;
		request.BoardIndex = BoardIndex;
		request.SlotIndexList = SlotIndexList;
		AwsModule.Request.Exec<ReceiveCardsUnlockBoardSlot> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsUnlockBoardSlot>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/unlock_board_slot_with_gem
	/// - ジェムを使用してボードの指定スロットを開放する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）  
	///   CardId:
	///     CardId
	///   BoardIndex:
	///     ボード番号（1〜、開放済みのボードに限る）
	///   SlotIndexList:
	///     スロット番号（1〜、未開放のスロットに限る）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsUnlockBoardSlotWithGem(int CardId, int BoardIndex, int[] SlotIndexList, Action<bool, ReceiveCardsUnlockBoardSlotWithGem> didLoad)
	{
		SendCardsUnlockBoardSlotWithGem request = new SendCardsUnlockBoardSlotWithGem ();
		request.CardId = CardId;
		request.BoardIndex = BoardIndex;
		request.SlotIndexList = SlotIndexList;
		AwsModule.Request.Exec<ReceiveCardsUnlockBoardSlotWithGem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsUnlockBoardSlotWithGem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/unlock_all_board_slots
	/// - ボードの未開放スロットを全て開放する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）  
	///   CardId:
	///     CardId
	///   BoardIndex:
	///     ボード番号（1〜、開放済みのボードに限る）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsUnlockAllBoardSlots(int CardId, int BoardIndex, Action<bool, ReceiveCardsUnlockAllBoardSlots> didLoad)
	{
		SendCardsUnlockAllBoardSlots request = new SendCardsUnlockAllBoardSlots ();
		request.CardId = CardId;
		request.BoardIndex = BoardIndex;
		AwsModule.Request.Exec<ReceiveCardsUnlockAllBoardSlots> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsUnlockAllBoardSlots>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/cards/unlock_all_board_slots_with_gem
	/// - ジェムを使用してボードの未開放スロットを全て開放する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）  
	///   CardId:
	///     CardId
	///   BoardIndex:
	///     ボード番号（1〜、開放済みのボードに限る）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     CardData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスタバージョン番号
	/// </summary>
	public static void CardsUnlockAllBoardSlotsWithGem(int CardId, int BoardIndex, Action<bool, ReceiveCardsUnlockAllBoardSlotsWithGem> didLoad)
	{
		SendCardsUnlockAllBoardSlotsWithGem request = new SendCardsUnlockAllBoardSlotsWithGem ();
		request.CardId = CardId;
		request.BoardIndex = BoardIndex;
		AwsModule.Request.Exec<ReceiveCardsUnlockAllBoardSlotsWithGem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveCardsUnlockAllBoardSlotsWithGem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/consumers/get_consumer_list
	/// - 消費アイテムリストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   ConsumerDataList:
	///     ConsumerDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - ConsumerData
	///   ConsumerId:
	///     消費アイテムID
	///   Count:
	///     所持数
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     初回獲得日時
	/// </summary>
	public static void ConsumersGetConsumerList(Action<bool, ReceiveConsumersGetConsumerList> didLoad)
	{
		SendConsumersGetConsumerList request = new SendConsumersGetConsumerList ();
		AwsModule.Request.Exec<ReceiveConsumersGetConsumerList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveConsumersGetConsumerList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/distribution/get_history
	/// - アイテム配布履歴の所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   DistributionDataList:
	///     DistributionDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - DistributionData
	///   Description:
	///     説明文
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     アイテム数
	///   CreationDate:
	///     作成日時
	/// </summary>
	public static void DistributionGetHistory(Action<bool, ReceiveDistributionGetHistory> didLoad)
	{
		SendDistributionGetHistory request = new SendDistributionGetHistory ();
		AwsModule.Request.Exec<ReceiveDistributionGetHistory> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveDistributionGetHistory>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/event/get_product_list
	/// - 購入（交換）可能な商品の一覧所得
	/// - リクエスト
	///   EventQuestId:
	///     指定したイベント交換所を利用するために指定する
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   EventShopProductDataList:
	///     ShopProductDataのリスト
	///   UserData:
	///     ユーザデータ
	///   EventPoint:
	///     戦績コイン数（EventQuestId毎に個別管理）
	/// - EventShopProductData
	///   ShopProductId:
	///     商品ID
	///   ExchangeQuantity:
	///     必要戦績コイン数
	///   UpperLimit:
	///     購入上限数（無制限は0）
	///   StartDate:
	///     開始日時
	///   EndDate:
	///     終了日時
	///   IsPurchasable:
	///     購入可否
	///   MaxPurchaseQuantity:
	///     最大購入可能数（＝購入上限数ー既購入数、ただし購入上限数が無制限の場合は999）
	///   StockItemDataList:
	///     StockItemDataのリスト、商品に含まれるアイテムの所持数や最大所持数の確認
	/// - StockItemData
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     所持数
	///   Capacity:
	///     最大所持数
	///   CardData:
	///     CardData
	///   MagikiteData:
	///     MagikiteData
	///   WeaponData:
	///     WeaponData
	/// </summary>
	public static void EventGetProductList(int EventQuestId, Action<bool, ReceiveEventGetProductList> didLoad)
	{
		SendEventGetProductList request = new SendEventGetProductList ();
		request.EventQuestId = EventQuestId;
		AwsModule.Request.Exec<ReceiveEventGetProductList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveEventGetProductList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/event/purchase_product
	/// - 商品を購入する
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   EventQuestId:
	///     指定したイベント交換所を利用するために指定する
	///   ShopProductId:
	///     商品ID
	///   Quantity:
	///     個数
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   EventShopProductDataList:
	///     ShopProductData
	///   UserData:
	///     ユーザデータ
	///   EventPoint:
	///     戦績コイン数（EventQuestId毎に個別管理）
	/// </summary>
	public static void EventPurchaseProduct(int EventQuestId, int ShopProductId, int Quantity, Action<bool, ReceiveEventPurchaseProduct> didLoad)
	{
		SendEventPurchaseProduct request = new SendEventPurchaseProduct ();
		request.EventQuestId = EventQuestId;
		request.ShopProductId = ShopProductId;
		request.Quantity = Quantity;
		AwsModule.Request.Exec<ReceiveEventPurchaseProduct> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveEventPurchaseProduct>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/fgid/get_login_info
	/// - ＜アプリ開始済みユーザのみ＞FGIDにログインするための情報を所得する
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   LoginUrl:
	///     ブラウザ（iOS: WebView、Android:標準ブラウザ）で開くURL
	///   GameToken:
	///     FGIDとのセッションを管理するトークン
	///   IsAccociater:
	///     True:FGIDと連携済み、False:未連携
	///   CustomerId:
	///     お客様番号
	///   Nickname:
	///     ニックネーム:
	///   Exp:
	///     経験値
	/// </summary>
	public static void FgidGetLoginInfo(Action<bool, ReceiveFgidGetLoginInfo> didLoad)
	{
		SendFgidGetLoginInfo request = new SendFgidGetLoginInfo ();
		AwsModule.Request.Exec<ReceiveFgidGetLoginInfo> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveFgidGetLoginInfo>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/fgid/associate_user
	/// - 自身とFGIDとを連携させる
	///   ※FGIDにログイン後の、サイト内のアプリ起動用URLから起動（再開）した時のみコールすること
	/// - リクエスト
	///   GameToken:
	///     FGIDとのセッションを管理するトークン
	///     ※get_login_infoで受け取った値かアプリ起動用URLから所得した値（違う場合は改竄の可能性）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// </summary>
	public static void FgidAssociateUser(string GameToken, Action<bool, ReceiveFgidAssociateUser> didLoad)
	{
		SendFgidAssociateUser request = new SendFgidAssociateUser ();
		request.GameToken = GameToken;
		AwsModule.Request.Exec<ReceiveFgidAssociateUser> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveFgidAssociateUser>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /filelist/
	/// blah
	/// </summary>
	public static void FilelistIndex(Action<bool, ReceiveFilelistIndex> didLoad)
	{
		SendFilelistIndex request = new SendFilelistIndex ();
		AwsModule.Request.Exec<ReceiveFilelistIndex> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveFilelistIndex>(response, didLoad, false);
		});
	}

	/// <summary>
	/// URL: /api/gacha/get_product_list
	/// - 購入可能なガチャの一覧所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   GachaProductDataList:
	///     GachaProductDataのリスト
	///   GachaDataList:
	///     GachaDataのリスト
	///   UserData:
	///     ユーザーデータ、ジェム数などの確認用
	///   RarestCardMissCount:
	///     最高レア外れ回数
	///   RarestCardMissGachaTriggerCount:
	///     ☆4確定ガチャ強制着火回数
	/// - GachaProductData:
	///   GachaProductId:
	///     ガチャ商品ID
	///   GachaId:
	///     ガチャID
	///   GachaType:
	///     ガチャタイプ
	///   DrawCount:
	///     抽選回数
	///   ExchangeItemType:
	///     使用アイテムタイプ
	///   ExchangeItemId:
	///     使用アイテムID
	///   ExchangeQuantity:
	///     使用個数
	///   PurchaseLimitation:
	///     購入制限、1:無制限、2:1日1回
	///   StartDate:
	///     開始日時
	///   EndDate:
	///     終了日時
	///   IsPurchasable:
	///     購入可否
	///   LastPurchaseDate:
	///     最新購入日時
	/// - GachaData
	///   GachaId:
	///     ガチャID
	///   GachaType:
	///     ガチャタイプ
	///   GachaItemDataList:
	///     GachaItemDataのリスト
	/// - GachaItemData
	///   GachaItemId:
	///     ガチャアイテムID
	///   GachaGroupId:
	///     ガチャグループID
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     個数
	/// </summary>
	public static void GachaGetProductList(Action<bool, ReceiveGachaGetProductList> didLoad)
	{
		SendGachaGetProductList request = new SendGachaGetProductList ();
		AwsModule.Request.Exec<ReceiveGachaGetProductList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveGachaGetProductList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/gacha/purchase_product
	/// ガチャ商品を購入する
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   GachaProductId:
	///     ガチャ商品ID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   AcquiredGachaItemDataList:
	///     AcquiredGachaItemDataのリスト
	///   UserData:
	///     ユーザーデータ
	///   RarestCardMissCount:
	///     最高レア外れ回数
	///   RarestCardMissGachaTriggerCount:
	///     ☆4確定ガチャ強制着火回数
	///   ConsumerData:
	///     ガチャチケットの場合に、使用したチケットのConsumerData
	/// - AcquiredGachaItemData:
	///   GachaProductId:
	///     ガチャ商品ID
	///   GachaId:
	///     ガチャID
	///   GachaItemId:
	///     ガチャアイテムID
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     個数
	///   IsNew:
	///     True:新規獲得、False:入手経験あり
	///   ConvertedItemType:
	///     ＜カードの場合＞自動変換後のアイテムのアイテムタイプ
	///   ConvertedItemId:
	///     ＜カードの場合＞自動変換後のアイテムのアイテムID
	///   ConvertedQuantity:
	///     ＜カードの場合＞自動変換後のアイテムの個数
	///   CardData:
	///     CardData
	///   MagikiteData:
	///     MagikiteData
	///   WeaponData:
	///     WeaponData
	/// </summary>
	public static void GachaPurchaseProduct(int GachaProductId, Action<bool, ReceiveGachaPurchaseProduct> didLoad)
	{
		SendGachaPurchaseProduct request = new SendGachaPurchaseProduct ();
		request.GachaProductId = GachaProductId;
		AwsModule.Request.Exec<ReceiveGachaPurchaseProduct> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveGachaPurchaseProduct>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/gacha/execute_tutorial_gacha
	/// チュートリアル用ガチャを実行する
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   AcquiredGachaItemDataList:
	///     AcquiredGachaItemDataのリスト
	///   UserData:
	///     ユーザーデータ
	/// - AcquiredGachaItemData:
	///   GachaProductId:
	///     ガチャ商品ID
	///   GachaId:
	///     ガチャID
	///   GachaItemId:
	///     ガチャアイテムID
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     個数
	///   IsNew:
	///     True:新規獲得、False:入手経験あり
	///   ConvertedItemType:
	///     ＜カードの場合＞自動変換後のアイテムのアイテムタイプ
	///   ConvertedItemId:
	///     ＜カードの場合＞自動変換後のアイテムのアイテムID
	///   ConvertedQuantity:
	///     ＜カードの場合＞自動変換後のアイテムの個数
	///   CardData:
	///     CardData
	///   MagikiteData:
	///     MagikiteData
	///   WeaponData:
	///     WeaponData
	/// </summary>
	public static void GachaExecuteTutorialGacha(Action<bool, ReceiveGachaExecuteTutorialGacha> didLoad)
	{
		SendGachaExecuteTutorialGacha request = new SendGachaExecuteTutorialGacha ();
		AwsModule.Request.Exec<ReceiveGachaExecuteTutorialGacha> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveGachaExecuteTutorialGacha>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/home/login
	/// - ログイン実行
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	///   CardDataList:
	///     所持カードリスト
	///   FormationDataList:
	///     所持陣形リスト
	///   WeaponDataList:
	///     所持武器リスト
	///   MagikiteDataList:
	///     所持マギカイトリスト
	///   ConsumerDataList:
	///     所持消費アイテムリスト
	///   MaterialDataList:
	///     所持キャラ素材リスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - UserData
	///   UserId:
	///     ユーザID
	///   CustomerId:
	///     お客様番号
	///   Nickname:
	///     ニックネーム
	///   LastLoginDate:
	///     最終ログイン日時
	///   Level:
	///     レベル(マスターデータから経験値で逆引き)
	///   Exp: #自分の場合のみ
	///     経験値
	///   ActionPoint: #自分の場合のみ
	///     AP
	///   ActionPointTimeToFull: #自分の場合のみ
	///     APが全回復するまでの秒数（全回復以上で0）
	///   ActionPointFullDate: #自分の場合のみ
	///     APが全回復する日時
	///   IsFollow: #他人の場合のみ
	///     フォローしているか
	///   IsFollower: #他人の場合のみ
	///     フォローされているか
	///   FollowCount:
	///     フォロー数
	///   FolowwerCount:
	///     フォロワー数
	///   Comment:
	///     コメント
	///   MainCardData:
	///     メインカードのCardData
	///   SupportCardList:
	///     サポートカードのリスト、最大6属性分
	///   ReceivablePresentCount: #自分の場合のみ
	///     未受取のプレゼントBOX内アイテム数
	///   ReceivableLoginbonusIdList: #自分の場合のみ
	///     受取可能なログインボーナスID
	///   ReceivableMissionCount: #自分の場合のみ
	///     未受取のミッション数
	///   FreeGemCount: #自分の場合のみ
	///     無償ジェムの所持数
	///   PaidGemCount: #自分の場合のみ
	///     有償ジェムの所持数
	///   GoldCount: #自分の場合のみ
	///     ゲーム内通貨（Gold）の所持数
	///   FriendPointCount: #自分の場合のみ
	///     フレンドポイントの所持数
	///   WeaponBagCapacity: #自分の場合のみ
	///     武器の最大所持可能数
	///   MagikiteBagCapacity: #自分の場合のみ
	///     マギカイトの最大所持可能数
	/// - CardData
	///   CardId:
	///     カードID
	///   Exp:
	///     経験値
	///   LimitBreakGrade:
	///     限界突破グレード（0〜4）
	///   EquippedWeaponBagId:
	///     装着武器バッグID
	///   EquippedMagikiteBagIdList:
	///     装着マギカイトバッグIDのリスト（計8スロット分、装備無しスロットは「0」）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// - FormationData
	///   FormationId:
	///     陣形ID
	///   FormationLevel:
	///     陣形レベル（1〜）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// - WeaponData
	///   WeaponId:
	///     武器ID
	///   BagId:
	///     バッグID（ユニークID）
	///   Exp:
	///     経験値
	///   LimitBreakGrade:
	///     限界突破グレード（0〜4）
	///   IsEquipped:
	///     True:装着している、False:フリー
	///   IsLocked:
	///     True:ロック状態、False:アンロック状態
	///   CardId:
	///     装着先カードID
	///   SlotId:
	///     装着先スロットID（装着時は常に1）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// - MagikiteData
	///   MagikiteId:
	///     マギカイトID
	///   BagId:
	///     バッグID（ユニークID）
	///   HPGain:
	///     HP付与値
	///   ATKGain:
	///     ATK付与値
	///   DEFGain:
	///     DEF付与値
	///   SPDGain:
	///     素早さ付与値
	///   IsEquipped:
	///     装着しているか
	///   IsLocked:
	///     True:ロック状態、False:アンロック状態
	///   CardId:
	///     装着先カードID
	///   SlotId:
	///     装着先スロットID（装着時は1〜8）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// - ConsumerData
	///   ConsumerId:
	///     消費アイテムID
	///   Count:
	///     所持数
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     初回獲得日時
	/// - MaterialData
	///   MaterialId:
	///     素材ID
	///   Count:
	///     所持数
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     初回獲得日時
	/// </summary>
	public static void HomeLogin(Action<bool, ReceiveHomeLogin> didLoad)
	{
		SendHomeLogin request = new SendHomeLogin ();
		AwsModule.Request.Exec<ReceiveHomeLogin> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveHomeLogin>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/loginbonus/get_available_list
	/// - 利用可能なログインボーナス
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   LoginbonusDataList:
	///     LoginbonusDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void LoginbonusGetAvailableList(Action<bool, ReceiveLoginbonusGetAvailableList> didLoad)
	{
		SendLoginbonusGetAvailableList request = new SendLoginbonusGetAvailableList ();
		AwsModule.Request.Exec<ReceiveLoginbonusGetAvailableList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveLoginbonusGetAvailableList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/loginbonus/receive_item
	/// - アイテムの受取
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   LoginbonusIdList:
	///     ログインボーナスIDのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   LoginbonusDataList:
	///     LoginbonusDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void LoginbonusReceiveItem(int[] LoginbonusIdList, Action<bool, ReceiveLoginbonusReceiveItem> didLoad)
	{
		SendLoginbonusReceiveItem request = new SendLoginbonusReceiveItem ();
		request.LoginbonusIdList = LoginbonusIdList;
		AwsModule.Request.Exec<ReceiveLoginbonusReceiveItem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveLoginbonusReceiveItem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/magikites/get_magikite_list
	/// - 所持マギカイトリストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MagikiteDataList:
	///     MagikiteDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - MagikiteData
	///   MagikiteId:
	///     マギカイトID
	///   BagId:
	///     バッグID（ユニークID）
	///   HPGain:
	///     HP付与値
	///   ATKGain:
	///     ATK付与値
	///   DEFGain:
	///     DEF付与値
	///   SPDGain:
	///     素早さ付与値
	///   IsEquipped:
	///     装着しているか
	///   IsLocked:
	///     True:ロック状態、False:アンロック状態
	///   CardId:
	///     装着先カードID
	///   SlotId:
	///     装着先スロットID（装着時は1〜8）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// </summary>
	public static void MagikitesGetMagikiteList(Action<bool, ReceiveMagikitesGetMagikiteList> didLoad)
	{
		SendMagikitesGetMagikiteList request = new SendMagikitesGetMagikiteList ();
		AwsModule.Request.Exec<ReceiveMagikitesGetMagikiteList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMagikitesGetMagikiteList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/magikites/lock_magikite
	/// - マギカイトをロックする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   MagikiteBagId:
	///     ロックするマギカイトのBagId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MagikiteData:
	///     MagikiteData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void MagikitesLockMagikite(long MagikiteBagId, Action<bool, ReceiveMagikitesLockMagikite> didLoad)
	{
		SendMagikitesLockMagikite request = new SendMagikitesLockMagikite ();
		request.MagikiteBagId = MagikiteBagId;
		AwsModule.Request.Exec<ReceiveMagikitesLockMagikite> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMagikitesLockMagikite>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/magikites/unlock_magikite
	/// - マギカイトをアンロックする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   MagikiteBagId:
	///     アンロックするマギカイトのBagId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MagikiteData:
	///     MagikiteData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void MagikitesUnlockMagikite(long MagikiteBagId, Action<bool, ReceiveMagikitesUnlockMagikite> didLoad)
	{
		SendMagikitesUnlockMagikite request = new SendMagikitesUnlockMagikite ();
		request.MagikiteBagId = MagikiteBagId;
		AwsModule.Request.Exec<ReceiveMagikitesUnlockMagikite> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMagikitesUnlockMagikite>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/magikites/sell_magikite
	/// - マギカイトを売却する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   MagikiteBagIdList:
	///     売却するマギカイトのBagIdのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void MagikitesSellMagikite(long[] MagikiteBagIdList, Action<bool, ReceiveMagikitesSellMagikite> didLoad)
	{
		SendMagikitesSellMagikite request = new SendMagikitesSellMagikite ();
		request.MagikiteBagIdList = MagikiteBagIdList;
		AwsModule.Request.Exec<ReceiveMagikitesSellMagikite> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMagikitesSellMagikite>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/materials/get_material_list
	/// - キャラ用素材リストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MaterialDataList:
	///     MaterialDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - MaterialData
	///   MaterialId:
	///     素材ID
	///   Count:
	///     所持数
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     初回獲得日時
	/// </summary>
	public static void MaterialsGetMaterialList(Action<bool, ReceiveMaterialsGetMaterialList> didLoad)
	{
		SendMaterialsGetMaterialList request = new SendMaterialsGetMaterialList ();
		AwsModule.Request.Exec<ReceiveMaterialsGetMaterialList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMaterialsGetMaterialList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/missions/get_achievement
	/// ミッションの達成状況のリストを所持する
	/// - リクエスト
	/// - レスポンス
	///   MissionAchievementList:
	///     MissionAchievementのリスト
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// - MissionAchievement:
	///   MissionId:
	///     ミッションID
	///   MissionType:
	///     ミッションタイプ
	///   MissionCategory:
	///     ミッションカテゴリー
	///   ClearCount:
	///     クリア回数
	///   IsAchieved:
	///     True:達成済み、False:未達成
	///   IsReceived:
	///     True:受取済み、False:未受取
	/// </summary>
	public static void MissionsGetAchievement(Action<bool, ReceiveMissionsGetAchievement> didLoad)
	{
		SendMissionsGetAchievement request = new SendMissionsGetAchievement ();
		AwsModule.Request.Exec<ReceiveMissionsGetAchievement> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMissionsGetAchievement>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/missions/receive_item
	/// ミッション達成報酬を受け取る
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   MissionIdList:
	///     ミッションIDのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MissionRewardItemList:
	///     type: array
	///     items:
	///       type: object
	///       $ref: app.models.base.ItemData
	///   UserData:
	///     ユーザデータ
	///   MaterialDataList:
	///     キャラ素材リスト、入手したMaterialDataのリスト
	///   ConsumerDataList:
	///     消費アイテムリスト、入手したConsumerDataのリスト
	///   CardDataList:
	///     カードリスト、入手したCardDataのリスト
	///   WeaponDataList:
	///     武器リスト、入手したWeaponDataのリスト
	///   MagikiteDataList:
	///     マギカイトリスト、入手したMagikiteDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void MissionsReceiveItem(int[] MissionIdList, Action<bool, ReceiveMissionsReceiveItem> didLoad)
	{
		SendMissionsReceiveItem request = new SendMissionsReceiveItem ();
		request.MissionIdList = MissionIdList;
		AwsModule.Request.Exec<ReceiveMissionsReceiveItem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveMissionsReceiveItem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/payments/get_product_list
	/// - 購入可能な商品の一覧所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   GemProductDataList:
	///     GemProductDataのリスト
	///   MonthlyPayment:
	///     当月課金額
	///   UserData:
	///     ユーザデータ
	/// - GemProductData
	///   GemProductId:
	///     ジェム商品ID
	///   BonusId:
	///     ボーナスID
	///   PurchaseLimitation:
	///     購入制限
	///   StartDate:
	///     開始日時
	///   EndDate:
	///     終了日時
	///   IsPurchasable:
	///     購入可否
	///   LastPurchaseDate:
	///     最新購入日時
	///   MaxPurchaseQuantity:
	///     最大購入可能数
	/// </summary>
	public static void PaymentsGetProductList(Action<bool, ReceivePaymentsGetProductList> didLoad)
	{
		SendPaymentsGetProductList request = new SendPaymentsGetProductList ();
		AwsModule.Request.Exec<ReceivePaymentsGetProductList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePaymentsGetProductList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/payments/submit_receipt_with_timeout
	/// - ストアのレシートを提出する（60秒間スリープしてServerErrorを返却）
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   Receipt:
	///     レシート
	///   Signature:
	///     署名（androidの場合）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void PaymentsSubmitReceiptWithTimeout(string Receipt, string Signature, Action<bool, ReceivePaymentsSubmitReceiptWithTimeout> didLoad)
	{
		SendPaymentsSubmitReceiptWithTimeout request = new SendPaymentsSubmitReceiptWithTimeout ();
		request.Receipt = Receipt;
		request.Signature = Signature;
		AwsModule.Request.Exec<ReceivePaymentsSubmitReceiptWithTimeout> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePaymentsSubmitReceiptWithTimeout>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/payments/submit_receipt
	/// - ストアのレシートを提出する
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   Receipt:
	///     レシート
	///   Signature:
	///     署名（androidの場合）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void PaymentsSubmitReceipt(string Receipt, string Signature, Action<bool, ReceivePaymentsSubmitReceipt> didLoad)
	{
		SendPaymentsSubmitReceipt request = new SendPaymentsSubmitReceipt ();
		request.Receipt = Receipt;
		request.Signature = Signature;
		AwsModule.Request.Exec<ReceivePaymentsSubmitReceipt> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePaymentsSubmitReceipt>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/presentbox/get_receivable_list
	/// - 未受取リストの所得
	/// - リクエスト
	///   From:
	///     リストの所得開始位置、先頭が0
	///   Count:
	///     所得数
	///   SortOrder:
	///     並べ替え順 - 0:新しい順、1:古い順、2:受取期限が近い順（＝古い順？）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PresentDataList:
	///     PresentDataのリスト
	///   ReceivablePresentCount:
	///     未受取数
	///   MasterVersion:
	///     マスターバージョン番号
	/// - PresentData
	///   PresentId:
	///     ユニークID
	///   Description:
	///     説明文
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     アイテム数
	///   IsReceived:
	///     受取状況（True:受取済み、False:未受取）
	///   ModificationDate:
	///     変更日時（＝開封日）
	///   CreationDate:
	///     作成日時
	/// </summary>
	public static void PresentboxGetReceivableList(int From, int Count, int SortOrder, Action<bool, ReceivePresentboxGetReceivableList> didLoad)
	{
		SendPresentboxGetReceivableList request = new SendPresentboxGetReceivableList ();
		request.From = From;
		request.Count = Count;
		request.SortOrder = SortOrder;
		AwsModule.Request.Exec<ReceivePresentboxGetReceivableList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePresentboxGetReceivableList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/presentbox/get_received_list
	/// - 受取済リストの所得
	/// - リクエスト
	///   From:
	///     リストの所得開始位置
	///   Count:
	///     所得数
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PresentDataList:
	///     PresentDataのリスト
	///   ReceivedPresentCount:
	///     受取数
	///   MasterVersion:
	///     マスターバージョン番号
	/// - PresentData
	///   PresentId:
	///     ユニークID
	///   Description:
	///     説明文
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     アイテム数
	///   IsReceived:
	///     受取状況（True:受取済み、False:未受取）
	///   ModificationDate:
	///     変更日時（＝開封日）
	///   CreationDate:
	///     作成日時
	/// </summary>
	public static void PresentboxGetReceivedList(int From, int Count, Action<bool, ReceivePresentboxGetReceivedList> didLoad)
	{
		SendPresentboxGetReceivedList request = new SendPresentboxGetReceivedList ();
		request.From = From;
		request.Count = Count;
		AwsModule.Request.Exec<ReceivePresentboxGetReceivedList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePresentboxGetReceivedList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/presentbox/receive_item
	/// - アイテムの受取
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   PresentIdList:
	///     プレゼントボックスIDのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PresentDataList:
	///     PresentDataのリスト
	///   UserData:
	///     ユーザデータ
	///   MaterialDataList:
	///     キャラ素材リスト、入手したMaterialDataのリスト
	///   ConsumerDataList:
	///     消費アイテムリスト、入手したConsumerDataのリスト
	///   CardDataList:
	///     カードリスト、入手したCardDataのリスト
	///   WeaponDataList:
	///     武器リスト、入手したWeaponDataのリスト
	///   MagikiteDataList:
	///     マギカイトリスト、入手したMagikiteDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - PresentData
	///   PresentId:
	///     ユニークID
	///   Description:
	///     説明文
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     アイテム数
	///   IsReceived:
	///     受取状況（True:受取済み、False:未受取）
	///   ModificationDate:
	///     変更日時（＝開封日）
	///   CreationDate:
	///     作成日時
	/// </summary>
	public static void PresentboxReceiveItem(int[] PresentIdList, Action<bool, ReceivePresentboxReceiveItem> didLoad)
	{
		SendPresentboxReceiveItem request = new SendPresentboxReceiveItem ();
		request.PresentIdList = PresentIdList;
		AwsModule.Request.Exec<ReceivePresentboxReceiveItem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePresentboxReceiveItem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/pvp/get_opponent_list
	/// - 対戦者候補一覧の所得
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   OpponentPvpTeamDataList:
	///     PvpTeamDataのリスト
	///   PvpUserData:
	///     PvpUserData
	///   PvpTeamData:
	///     自分のチームデータ
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// - PvpTeamData
	///   UserId:
	///     ユーザID
	///   Nickname:
	///     ニックネーム
	///   MainCardData:
	///     メインカードのCardData
	///   MemberCardDataList: <- 廃止予定、MemberPvpCardDataListを利用してください
	///     メンバーのCardDataのリスト
	///   MemberPvpCardDataList:
	///     メンバーのPvpCardDataのリスト
	///   TotalOverallIndex:
	///     総合力
	///   RivalStrength:
	///     強敵度、0:格下、1:同格、2:格上の3段階
	///   GainWinningPoint:
	///     勝利によって獲得できる勝敗ポイント
	///   FormationId:
	///     陣形ID
	///   FormationLevel:
	///     陣形レベル
	///   RankCorrectionPercentage: <- 廃止予定、RivalStrengthを利用してください
	///     格上・同格・格下補正割合（%）、150、130、100、80、70のどれか（総合力の差に依存）
	///   ContestId:
	///     コンテストID（勝敗やポイントはコンテスト毎にリセット、開催中のコンテストIDを返却）
	///   WinCount:
	///     勝数
	///   LoseCount:
	///     敗数
	///   WinningPoint:
	///     勝敗ポイント
	/// - PvpUserData
	///   BattlePoint:
	///     バトルポイント
	///   BattlePointFullDate:
	///     満タンになる時間
	///   BattlePointTimeToFull:
	///     満タンになるまでの秒数
	///   ContestId:
	///     コンテストID（勝敗やポイントはコンテスト毎にリセット、開催中のコンテストIDを返却）
	///   WinCount:
	///     勝数
	///   LoseCount:
	///     敗数
	///   ConsecutiveWins:
	///     連勝数
	///   WinningPoint:
	///     勝敗ポイント
	///   ListUpdateGem:
	///     候補一覧の更新費用（毎日初回は0、2回目以降は所定のジェム数）
	/// </summary>
	public static void PvpGetOpponentList(Action<bool, ReceivePvpGetOpponentList> didLoad)
	{
		SendPvpGetOpponentList request = new SendPvpGetOpponentList ();
		AwsModule.Request.Exec<ReceivePvpGetOpponentList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePvpGetOpponentList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/pvp/update_opponent_list
	/// - 対戦者候補一覧を更新して所得しなおす
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   OpponentPvpTeamDataList:
	///     PvpTeamDataのリスト
	///   PvpUserData:
	///     PvpUserData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void PvpUpdateOpponentList(Action<bool, ReceivePvpUpdateOpponentList> didLoad)
	{
		SendPvpUpdateOpponentList request = new SendPvpUpdateOpponentList ();
		AwsModule.Request.Exec<ReceivePvpUpdateOpponentList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePvpUpdateOpponentList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/pvp/begin_battle
	/// - バトル開始を通知する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   ContestId:
	///     コンテストID
	///   OpponentUserId:
	///     対戦相手のユーザID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PvpBattleEntryData:
	///     バトルエントリーデータ
	///   MasterVersion:
	///     マスターバージョン番号
	/// - PvpBattleEntryData
	///   EntryId:
	///     エントリーID
	///   ContestId:
	///     コンテストID
	///   PvpTeamData:
	///     自分のチームデータ
	///   OpponentUserId:
	///     対戦相手のユーザID
	///   OpponentPvpTeamData:
	///     対戦相手のチームデータ
	/// </summary>
	public static void PvpBeginBattle(int ContestId, int OpponentUserId, Action<bool, ReceivePvpBeginBattle> didLoad)
	{
		SendPvpBeginBattle request = new SendPvpBeginBattle ();
		request.ContestId = ContestId;
		request.OpponentUserId = OpponentUserId;
		AwsModule.Request.Exec<ReceivePvpBeginBattle> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePvpBeginBattle>(response, didLoad, false);
		});
	}

	/// <summary>
	/// URL: /api/pvp/finish_battle
	/// - バトル結果を通知する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   EntryId:
	///     エントリーID
	///   IsWin:
	///     True:勝ち、False:負け
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   BaseWinningPoint: <- 廃止予定、WinningPointを利用してください
	///     基礎勝利ポイント
	///   RankCorrectedWinningPoint: <- 廃止予定、WinningPointに統合しました
	///     獲得ランク補正済み勝利ポイント（BaseWinningPointにRankCorrectionPercentageを乗じた値）
	///   ConsecutiveWinsBonusWinningPoint:
	///     獲得連勝ボーナスポイント
	///   WinningPoint:
	///     獲得勝敗ポイント
	///   PvpMedal:
	///     獲得PvPメダル
	///   PvpUserData:
	///     PvpUserData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void PvpFinishBattle(int EntryId, bool IsWin, Action<bool, ReceivePvpFinishBattle> didLoad)
	{
		SendPvpFinishBattle request = new SendPvpFinishBattle ();
		request.EntryId = EntryId;
		request.IsWin = IsWin;
		AwsModule.Request.Exec<ReceivePvpFinishBattle> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceivePvpFinishBattle>(response, didLoad, false);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_event_quest_achievement
	/// イベントクエストの達成リストを所得する
	/// - リクエスト
	///   EventQuestId:
	///     イベントクエストID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   EventQuestAchievementList:
	///     EventQuestAchievementのリスト
	/// - EventQuestAchievement
	///   StageDetailId:
	///     ステージ詳細ID
	///   StageType:
	///     1:シナリオ、2:チャレンジ
	///   IsAchieved:
	///     true:達成済み、false:未達成
	///   AchievedMissionIdList:
	///     達成済みMissionIdのリスト
	///   ReceivedMissionRewardCount:
	///     受取済みMission情報の個数（Mission全達成で1）
	///   IsOpen:
	///     true:プレイ可能、false:プレイ不可（開放条件および開始・終了日時で判定）
	///   StartDate:
	///     開始日時
	///   EndDate:
	///     終了日時
	/// </summary>
	public static void QuestsGetEventQuestAchievement(int EventQuestId, Action<bool, ReceiveQuestsGetEventQuestAchievement> didLoad)
	{
		SendQuestsGetEventQuestAchievement request = new SendQuestsGetEventQuestAchievement ();
		request.EventQuestId = EventQuestId;
		AwsModule.Request.Exec<ReceiveQuestsGetEventQuestAchievement> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetEventQuestAchievement>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_daily_achievement
	/// 曜日クエストの達成リストを所得する
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   DailyQuestAchievementList:
	///     DailyQuestAchievementのリスト
	/// - DailyQuestAchievement
	///   QuestType:
	///     4:強化素材クエスト、5:進化素材クエスト
	///   QuestId:
	///     クエストID
	///   DayOfWeek:
	///     開催曜日（0:日曜〜6:土曜）
	///   LockStatus:
	///     0:ロック無し（開催曜日一致）、1:ロック中、2:アンロック中
	///   UnlockDate:
	///     アンロックした日時、LockStatus=2の時のみ評価
	///   TimeToLock:
	///     ロックまでの残り時間（秒）、LockStatus=2の時のみ評価
	///   IsAchieved:
	///     true:達成済み、false:未達成
	///   AchievedMissionIdList:
	///     達成済みMissionIdのリスト
	///   ReceivedMissionRewardCount:
	///     受取済みMission情報の個数（Mission全達成で1）
	/// </summary>
	public static void QuestsGetDailyAchievement(Action<bool, ReceiveQuestsGetDailyAchievement> didLoad)
	{
		SendQuestsGetDailyAchievement request = new SendQuestsGetDailyAchievement ();
		AwsModule.Request.Exec<ReceiveQuestsGetDailyAchievement> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetDailyAchievement>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/unlock_daily_quest
	/// 曜日クエストのロックを外す
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   QuestType:
	///     4:強化素材クエスト、5:進化素材クエスト
	///   UnlockDayOfWeek:
	///     アンロックする曜日（0:日曜〜6:土曜）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   DailyQuestAchievementList:
	///     アンロックしたクエストのDailyQuestAchievement
	/// </summary>
	public static void QuestsUnlockDailyQuest(int QuestType, int UnlockDayOfWeek, Action<bool, ReceiveQuestsUnlockDailyQuest> didLoad)
	{
		SendQuestsUnlockDailyQuest request = new SendQuestsUnlockDailyQuest ();
		request.QuestType = QuestType;
		request.UnlockDayOfWeek = UnlockDayOfWeek;
		AwsModule.Request.Exec<ReceiveQuestsUnlockDailyQuest> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsUnlockDailyQuest>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_main_country_list
	/// メインクエストの国リストを所得する
	/// - リクエスト
	/// - レスポンス
	///   MainQuestCountryDataList:
	///     MainQuestCountryDataのリスト
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// - MainQuestCountryData:
	///   MainQuestCountry:
	///     国（1〜7）
	///   IsOpen:
	///     True:進行可、False:進行不可
	///   IsNew:
	///     True:未進行（達成クエスト無し）、False:進行済み
	///   IsClear:
	///     True:全クエストクリア済み、False:未クリア
	/// </summary>
	public static void QuestsGetMainCountryList(Action<bool, ReceiveQuestsGetMainCountryList> didLoad)
	{
		SendQuestsGetMainCountryList request = new SendQuestsGetMainCountryList ();
		AwsModule.Request.Exec<ReceiveQuestsGetMainCountryList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetMainCountryList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_main_achievement_by_country
	/// メインクエストの国別達成状況のリストを所得する
	/// - リクエスト
	///   MainQuestCountry:
	///     国（1〜7）
	/// - レスポンス
	///   QuestAchievementList:
	///     QuestAchievementのリスト
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// - QuestAchievement:
	///   QuestType:
	///     1:メインクエスト、2:サブクエスト、3:キャラクエスト
	///   QuestId:
	///     クエストID
	///   IsAchieved:
	///     true:達成済、false:未達成
	///   AchievedSelectionIdList:
	///     選択済みSelectionIdのリスト
	///   AchievedMissionIdList:
	///     達成済みMissionIdのリスト
	///   ReceivedMissionRewardCount:
	///     受取済みMission報酬の個数
	/// </summary>
	public static void QuestsGetMainAchievementByCountry(int MainQuestCountry, Action<bool, ReceiveQuestsGetMainAchievementByCountry> didLoad)
	{
		SendQuestsGetMainAchievementByCountry request = new SendQuestsGetMainAchievementByCountry ();
		request.MainQuestCountry = MainQuestCountry;
		AwsModule.Request.Exec<ReceiveQuestsGetMainAchievementByCountry> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetMainAchievementByCountry>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_achievement_by_type
	/// 指定タイプのクエストの達成状況のリストを所得する
	/// - リクエスト
	///   QuestType:
	///     クエストタイプ（1:メインクエスト、2:サブクエスト、3:キャラクエスト）
	/// - レスポンス
	///   QuestAchievementList:
	///     QuestAchievementのリスト
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// - QuestAchievement:
	///   QuestType:
	///     1:メインクエスト、2:サブクエスト、3:キャラクエスト
	///   QuestId:
	///     クエストID
	///   IsAchieved:
	///     true:達成済、false:未達成
	///   AchievedSelectionIdList:
	///     選択済みSelectionIdのリスト
	///   AchievedMissionIdList:
	///     達成済みMissionIdのリスト
	///   ReceivedMissionRewardCount:
	///     受取済みMission報酬の個数
	/// </summary>
	public static void QuestsGetAchievementByType(int QuestType, Action<bool, ReceiveQuestsGetAchievementByType> didLoad)
	{
		SendQuestsGetAchievementByType request = new SendQuestsGetAchievementByType ();
		request.QuestType = QuestType;
		AwsModule.Request.Exec<ReceiveQuestsGetAchievementByType> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetAchievementByType>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_achievement
	/// クエストの達成状況のリストを所得する
	/// - リクエスト
	/// - レスポンス
	///   QuestAchievementList:
	///     QuestAchievementのリスト
	///   ResultCode:
	///     0:成功、1以上:失敗
	/// - QuestAchievement:
	///   QuestType:
	///     1:メインクエスト、2:サブクエスト、3:キャラクエスト
	///   QuestId:
	///     クエストID
	///   IsAchieved:
	///     true:達成済、false:未達成
	///   AchievedSelectionIdList:
	///     選択済みSelectionIdのリスト
	///   AchievedMissionIdList:
	///     達成済みMissionIdのリスト
	///   ReceivedMissionRewardCount:
	///     受取済みMission報酬の個数
	/// </summary>
	public static void QuestsGetAchievement(Action<bool, ReceiveQuestsGetAchievement> didLoad)
	{
		SendQuestsGetAchievement request = new SendQuestsGetAchievement ();
		AwsModule.Request.Exec<ReceiveQuestsGetAchievement> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetAchievement>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/open_quest
	/// クエスト開始を通知する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   QuestId:
	///     クエストID（イベントクエストの場合はステージ詳細ID）
	///   StageId:
	///     ＜バトル時のみ＞ステージID
	///   MemberCardIdList:
	///     ＜バトル時のみ＞バトルに参加するメンバーのCardIdのリスト
	///   SupporterUserId（GuestUserIdから変更、当面はどちらでもOK）:
	///     ＜バトル時のみ＞バトルに連れていくサポートカードの所持ユーザのUserId
	///   SupporterCardId（GuestCardIdから変更、当面はどちらでもOK）:
	///     ＜バトル時のみ＞バトルに連れていくサポートカードのCardId
	///   OverrideExpPercentage:
	///     ユニット経験値の上乗せ率（%）
	///   OverrideGoldPercentage:
	///     ゲーム内通貨の上乗せ率（%）
	///   OverrideItemDropPercentage:
	///     アイテムドロップ確率の上乗せ率（%）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   BattleEntryData:
	///     ＜バトル時のみ＞バトルエントリーデータ
	/// - BattleEntryData:
	///   EntryId:
	///     エントリーID
	///   StageId:
	///     ステージID
	///   MemberCardIdList:
	///     バトルに参加するメンバーのCardIdのリスト
	///   SupporterUserId:（GuestUserIdから変更、当面は両方）:
	///     バトルに連れていくサポートカードの所持ユーザのUserId
	///   SupporterCardId（GuestCardIdから変更、当面は両方）:
	///     バトルに連れていくサポートカードのCardId
	///   SupporterCardData
	///     バトルに連れていくサポートカードのCardData（装備、育成ボードデータ込み）
	///   DropItemIdList:
	///     このバトルでドロップするアイテムのID（ドロップテーブルのID）のリスト
	///     StageEnemyListに統合
	///   StageEnemyList:
	///     StageEnemyDataのリスト
	///   CreationDate:
	///     バトル開始日時
	///   Status:
	///     0:戦闘中、1:戦闘勝利、99:戦闘離脱
	/// - StageEnemyData:
	///   EnemyId:
	///     敵ID（ステージ敵設定のID）
	///   DropItemList:
	///     この敵を倒した時にドロップする報酬、ItemDataのリスト
	/// </summary>
	public static void QuestsOpenQuest(int QuestId, int StageId, int[] MemberCardIdList, int SupporterUserId, int SupporterCardId, int OverrideExpPercentage, int OverrideGoldPercentage, int OverrideItemDropPercentage, Action<bool, ReceiveQuestsOpenQuest> didLoad)
	{
		SendQuestsOpenQuest request = new SendQuestsOpenQuest ();
		request.QuestId = QuestId;
		request.StageId = StageId;
		request.MemberCardIdList = MemberCardIdList;
		request.SupporterUserId = SupporterUserId;
		request.SupporterCardId = SupporterCardId;
		request.OverrideExpPercentage = OverrideExpPercentage;
		request.OverrideGoldPercentage = OverrideGoldPercentage;
		request.OverrideItemDropPercentage = OverrideItemDropPercentage;
		AwsModule.Request.Exec<ReceiveQuestsOpenQuest> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsOpenQuest>(response, didLoad, false);
		});
	}

	/// <summary>
	/// URL: /api/quests/close_quest
	/// クエスト終了を通知する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   QuestId:
	///     クエストID
	///   IsAchieved:
	///     True:達成（戦闘勝利）、False:リタイア（戦闘離脱）
	///   EntryId:
	///     ＜バトル時のみ＞バトルのエントリーID
	///   SelectionIdList:
	///     今回選択した選択肢IDのリスト
	///   MissionIdList:
	///     今回クリアしたミッションIDのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   QuestAchievement:
	///     QuestAchievement
	///   RewardItemIdList:
	///     初回クエストクリア報酬、要定義テーブル（QuestRewardItemListに変更、今後は使わない）
	///   BattleEntryData:
	///     バトルエントリーデータ
	///   QuestRewardItemList:
	///     初回クエストクリア報酬、ItemDataのリスト
	///   MissionRewardItemList:
	///     バトルミッションクリア報酬、ItemDataのリスト
	///   UserData:
	///     ユーザデータ
	///   MemberCardDataList:
	///     メンバーカードリスト、CardDataのリスト
	///   MaterialDataList:
	///     キャラ素材リスト、入手したMaterialDataのリスト
	///   ConsumerDataList:
	///     消費アイテムリスト、入手したConsumerDataのリスト
	///   WeaponDataList:
	///     武器リスト、入手したWeaponDataのリスト
	///   MagikiteDataList:
	///     マギカイトリスト、入手したMagikiteDataのリスト
	///   GainUnitExp:
	///     ユニットあたりの獲得経験値
	///   GainUserExp:
	///     プレイヤー獲得経験値
	///   GainGold:
	///     獲得ゲーム内通貨
	///   GainEventPoint:
	///     獲得イベントポイント（総数）
	///   GainBonusEventPoint:
	///     獲得ボーナスイベントポイント
	/// </summary>
	public static void QuestsCloseQuest(int QuestId, bool IsAchieved, int EntryId, int[] SelectionIdList, int[] MissionIdList, Action<bool, ReceiveQuestsCloseQuest> didLoad)
	{
		SendQuestsCloseQuest request = new SendQuestsCloseQuest ();
		request.QuestId = QuestId;
		request.IsAchieved = IsAchieved;
		request.EntryId = EntryId;
		request.SelectionIdList = SelectionIdList;
		request.MissionIdList = MissionIdList;
		AwsModule.Request.Exec<ReceiveQuestsCloseQuest> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsCloseQuest>(response, didLoad, false);
		});
	}

	/// <summary>
	/// URL: /api/quests/skip_battle
	/// チケットを使用してバトルをスキップする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   QuestId:
	///     クエストID
	///   StageId:
	///     ステージID
	///   MemberCardIdList:
	///     バトルに参加するメンバーのCardIdのリスト
	///   SkipCount:
	///     スキップ回数
	///   OverrideExpPercentage:
	///     ユニット経験値の上乗せ率（%）
	///   OverrideGoldPercentage:
	///     ゲーム内通貨の上乗せ率（%）
	///   OverrideItemDropPercentage:
	///     アイテムドロップ確率の上乗せ率（%）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   BattleEntryDataList:
	///     BattleEntryDataのリスト
	///   UserData:
	///     ユーザデータ
	///   MemberCardDataList:
	///     メンバーカードリスト、CardDataのリスト
	/// </summary>
	public static void QuestsSkipBattle(int QuestId, int StageId, int[] MemberCardIdList, int SkipCount, int OverrideExpPercentage, int OverrideGoldPercentage, int OverrideItemDropPercentage, Action<bool, ReceiveQuestsSkipBattle> didLoad)
	{
		SendQuestsSkipBattle request = new SendQuestsSkipBattle ();
		request.QuestId = QuestId;
		request.StageId = StageId;
		request.MemberCardIdList = MemberCardIdList;
		request.SkipCount = SkipCount;
		request.OverrideExpPercentage = OverrideExpPercentage;
		request.OverrideGoldPercentage = OverrideGoldPercentage;
		request.OverrideItemDropPercentage = OverrideItemDropPercentage;
		AwsModule.Request.Exec<ReceiveQuestsSkipBattle> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsSkipBattle>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/continue_quest
	/// クエスト（バトル）を継続する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   QuestId:
	///     クエストID
	///   EntryId:
	///     ＜バトル時のみ＞バトルのエントリーID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void QuestsContinueQuest(int QuestId, int EntryId, Action<bool, ReceiveQuestsContinueQuest> didLoad)
	{
		SendQuestsContinueQuest request = new SendQuestsContinueQuest ();
		request.QuestId = QuestId;
		request.EntryId = EntryId;
		AwsModule.Request.Exec<ReceiveQuestsContinueQuest> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsContinueQuest>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/quests/get_open_battle
	/// 終了していないクエスト（バトル）データを所得する
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   QuestType:
	///     1:メイン、2:サブ、3:キャラ、 4:強化素材、5:進化素材
	///   QuestId:
	///     クエストID（該当クエストが無い場合は無し）
	///   BattleEntryData:
	///     バトルエントリーデータ（該当クエストが無い場合は無し）
	/// - BattleEntryData:
	///   EntryId:
	///     エントリーID
	///   StageId:
	///     ステージID
	///   MemberCardIdList:
	///     バトルに参加するメンバーのCardIdのリスト
	///   SupporterUserId:（GuestUserIdから変更、当面は両方）:
	///     バトルに連れていくサポートカードの所持ユーザのUserId
	///   SupporterCardId（GuestCardIdから変更、当面は両方）:
	///     バトルに連れていくサポートカードのCardId
	///   SupporterCardData
	///     バトルに連れていくサポートカードのCardData（装備、育成ボードデータ込み）
	///   DropItemIdList:
	///     このバトルでドロップするアイテムのID（ドロップテーブルのID）のリスト
	///     StageEnemyListに統合
	///   StageEnemyList:
	///     StageEnemyDataのリスト
	///   CreationDate:
	///     バトル開始日時
	///   Status:
	///     0:戦闘中、1:戦闘勝利、99:戦闘離脱
	/// - StageEnemyData:
	///   EnemyId:
	///     敵ID（ステージ敵設定のID）
	///   DropItemList:
	///     この敵を倒した時にドロップする報酬、ItemDataのリスト
	/// </summary>
	public static void QuestsGetOpenBattle(Action<bool, ReceiveQuestsGetOpenBattle> didLoad)
	{
		SendQuestsGetOpenBattle request = new SendQuestsGetOpenBattle ();
		AwsModule.Request.Exec<ReceiveQuestsGetOpenBattle> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveQuestsGetOpenBattle>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /register/
	/// fuga
	/// </summary>
	public static void RegisterIndex(Action<bool, ReceiveRegisterIndex> didLoad)
	{
		SendRegisterIndex request = new SendRegisterIndex ();
		AwsModule.Request.Exec<ReceiveRegisterIndex> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveRegisterIndex>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/shop/get_product_list
	/// - 購入可能な商品の一覧所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   ShopProductDataList:
	///     ShopProductDataのリスト
	///   UserData:
	///     ユーザデータ
	/// - ShopProductData
	///   ShopProductId:
	///     ショップ商品ID
	///   ShopItemId:
	///     ショップアイテムID
	///   ShopCategory:
	///     ショップカテゴリ
	///   ExchangeItemType:
	///     使用アイテムタイプ
	///   ExchangeQuantity:
	///     使用個数
	///   PurchaseLimitation:
	///     購入制限
	///   StartDate:
	///     開始日時
	///   EndDate:
	///     終了日時
	///   IsPurchasable:
	///     購入可否
	///   LastPurchaseDate:
	///     最新購入日時
	///   MaxPurchaseQuantity:
	///     最大購入可能数、無制限は999、それ以外は購入制限との照合結果
	///   StockItemDataList:
	///     StockItemDataのリスト、商品に含まれるアイテムの所持数や最大所持数の確認
	/// - StockItemData
	///   ItemType:
	///     アイテムタイプ
	///   ItemId:
	///     アイテムID
	///   Quantity:
	///     所持数
	///   Capacity:
	///     最大所持数
	///   CardData:
	///     CardData
	///   MagikiteData:
	///     MagikiteData
	///   WeaponData:
	///     WeaponData
	/// </summary>
	public static void ShopGetProductList(Action<bool, ReceiveShopGetProductList> didLoad)
	{
		SendShopGetProductList request = new SendShopGetProductList ();
		AwsModule.Request.Exec<ReceiveShopGetProductList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveShopGetProductList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/shop/purchase_product
	/// - 商品を購入する
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   ShopProductId:
	///     ショップ商品ID
	///   Quantity:
	///     個数
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   StockItemDataList:
	///     StockItemDataのリスト
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void ShopPurchaseProduct(int ShopProductId, int Quantity, Action<bool, ReceiveShopPurchaseProduct> didLoad)
	{
		SendShopPurchaseProduct request = new SendShopPurchaseProduct ();
		request.ShopProductId = ShopProductId;
		request.Quantity = Quantity;
		AwsModule.Request.Exec<ReceiveShopPurchaseProduct> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveShopPurchaseProduct>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/shop/trade_material
	/// - 素材を交換する（かけら→カードor被り石のみを想定）
	/// - リクエスト
	///   RequestId:
	///     リクエストID
	///   CardId:
	///     カードID
	///   Quantity:
	///     個数
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardData:
	///     獲得したカードのCardData（無い場合あり）
	///   MaterialData:
	///     獲得した被り石のMaterialData（無い場合あり）
	/// </summary>
	public static void ShopTradeMaterial(int CardId, int Quantity, Action<bool, ReceiveShopTradeMaterial> didLoad)
	{
		SendShopTradeMaterial request = new SendShopTradeMaterial ();
		request.CardId = CardId;
		request.Quantity = Quantity;
		AwsModule.Request.Exec<ReceiveShopTradeMaterial> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveShopTradeMaterial>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /takeover/get_fgid_login_info
	/// - ＜引き継ぎ実行用＞FGIDにログインするための情報を所得する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   LoginUrl:
	///     ブラウザ（iOS: WebView、Android:標準ブラウザ）で開くURL
	///   GameToken:
	///     FGIDとのセッションを管理するトークン
	/// </summary>
	public static void TakeoverGetFgidLoginInfo(Action<bool, ReceiveTakeoverGetFgidLoginInfo> didLoad)
	{
		SendTakeoverGetFgidLoginInfo request = new SendTakeoverGetFgidLoginInfo ();
		AwsModule.Request.Exec<ReceiveTakeoverGetFgidLoginInfo> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTakeoverGetFgidLoginInfo>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /takeover/confirm_user
	///     - FGIDと連携済みのユーザのユーザ情報を返却する
	///       ※FGIDにログイン後の、サイト内のアプリ起動用URLから起動（再開）した時のみコールすること
	///     - リクエスト
	///       RequestId:
	///         リクエストID（多重処理防止用）
	///       GameToken:
	///         FGIDとのセッションを管理するトークン
	///         ※get_fgid_login_infoで受け取った値かアプリ起動用URLから所得した値（違う場合は改竄の可能
	/// 性）
	///     - レスポンス
	///       ResultCode:
	///         0:成功、1以上:失敗
	///       UserId:
	///         ユニークID
	///       CustomerId:
	///         お客様番号
	///       Nickname:
	///         ニックネーム:
	///       Exp:
	///         経験値
	/// </summary>
	public static void TakeoverConfirmUser(string GameToken, Action<bool, ReceiveTakeoverConfirmUser> didLoad)
	{
		SendTakeoverConfirmUser request = new SendTakeoverConfirmUser ();
		request.GameToken = GameToken;
		AwsModule.Request.Exec<ReceiveTakeoverConfirmUser> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTakeoverConfirmUser>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /takeover/resume_user
	/// - FGIDと連携済みのユーザとしてゲームを再開する
	///   ※FGIDにログイン後の、サイト内のアプリ起動用URLから起動（再開）した時のみコールすること
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   GameToken:
	///     FGIDとのセッションを管理するトークン
	///     ※get_fgid_login_infoで受け取った値かアプリ起動用URLから所得した値（違う場合は改竄の可能性）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   AuthUsername:
	///     認証用ユーザ名
	///   AuthPassword:
	///     認証用パスワード（旧ユーザ使用のパスワードは無効にする→旧ユーザはログイン不可）
	///   UserId:
	///     ユニークID
	///   CustomerId:
	///     お客様番号
	/// </summary>
	public static void TakeoverResumeUser(string GameToken, Action<bool, ReceiveTakeoverResumeUser> didLoad)
	{
		SendTakeoverResumeUser request = new SendTakeoverResumeUser ();
		request.GameToken = GameToken;
		AwsModule.Request.Exec<ReceiveTakeoverResumeUser> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTakeoverResumeUser>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/teams/get_card_list
	/// - 所持カードリストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   CardDataList:
	///     CardDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - CardData
	///   CardId:
	///     カードID
	///   Exp:
	///     経験値
	///   LimitBreakGrade:
	///     限界突破グレード（0〜4）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     カード初回獲得日時
	/// </summary>
	public static void TeamsGetCardList(Action<bool, ReceiveTeamsGetCardList> didLoad)
	{
		SendTeamsGetCardList request = new SendTeamsGetCardList ();
		AwsModule.Request.Exec<ReceiveTeamsGetCardList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTeamsGetCardList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/teams/get_formation_list
	/// - 所持陣形リストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   FormationDataList:
	///     FormationDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - FormationData
	///   FormationId:
	///     陣形ID
	///   FormationLevel:
	///     陣形レベル（1〜）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     陣形初回獲得日時
	/// </summary>
	public static void TeamsGetFormationList(Action<bool, ReceiveTeamsGetFormationList> didLoad)
	{
		SendTeamsGetFormationList request = new SendTeamsGetFormationList ();
		AwsModule.Request.Exec<ReceiveTeamsGetFormationList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTeamsGetFormationList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/text/inspect
	/// - テキスト検閲
	/// - リクエスト
	///   Text:
	///     検閲対象のテキスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   IsAccept:
	///     True:問題無し、False:却下
	/// </summary>
	public static void TextInspect(string Text, Action<bool, ReceiveTextInspect> didLoad)
	{
		SendTextInspect request = new SendTextInspect ();
		request.Text = Text;
		AwsModule.Request.Exec<ReceiveTextInspect> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveTextInspect>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/search_by_customer_id
	/// - プレイヤーIDで検索する
	/// - リクエスト
	///   CustomerId:
	///     プレイヤーID(カスタマーID)
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersSearchByCustomerId(string CustomerId, Action<bool, ReceiveUsersSearchByCustomerId> didLoad)
	{
		SendUsersSearchByCustomerId request = new SendUsersSearchByCustomerId ();
		request.CustomerId = CustomerId;
		AwsModule.Request.Exec<ReceiveUsersSearchByCustomerId> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersSearchByCustomerId>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/search_by_nickname
	/// - ニックネームで検索する
	/// - リクエスト
	///   Keyword:
	///     検索ワード
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersSearchByNickname(string Keyword, Action<bool, ReceiveUsersSearchByNickname> didLoad)
	{
		SendUsersSearchByNickname request = new SendUsersSearchByNickname ();
		request.Keyword = Keyword;
		AwsModule.Request.Exec<ReceiveUsersSearchByNickname> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersSearchByNickname>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/follow_user
	/// - 指定ユーザをフォローする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   UserId:
	///     フォローするユーザID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersFollowUser(int UserId, Action<bool, ReceiveUsersFollowUser> didLoad)
	{
		SendUsersFollowUser request = new SendUsersFollowUser ();
		request.UserId = UserId;
		AwsModule.Request.Exec<ReceiveUsersFollowUser> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersFollowUser>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/unfollow_user
	/// - 指定ユーザのフォローをやめる
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   UserId:
	///     フォローをやめるユーザID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersUnfollowUser(int UserId, Action<bool, ReceiveUsersUnfollowUser> didLoad)
	{
		SendUsersUnfollowUser request = new SendUsersUnfollowUser ();
		request.UserId = UserId;
		AwsModule.Request.Exec<ReceiveUsersUnfollowUser> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersUnfollowUser>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/remove_follower
	/// - 指定ユーザからのフォローを外す(フォロワーを外す)
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   UserId:
	///     フォローを外すユーザID
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersRemoveFollower(int UserId, Action<bool, ReceiveUsersRemoveFollower> didLoad)
	{
		SendUsersRemoveFollower request = new SendUsersRemoveFollower ();
		request.UserId = UserId;
		AwsModule.Request.Exec<ReceiveUsersRemoveFollower> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersRemoveFollower>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/get_follow_list
	/// - フォローリストの所得
	/// - リクエスト
	///   From:
	///     リストの所得開始位置、先頭が0
	///   Count:
	///     所得数
	///   SortOrder:
	///     並べ替え順 - 0:ログイン(新しい)、1:ログイン(古い)、2:レベル(昇順)、3:レベル(降順)、4:フォロー順（新しい）、5:フォロー順（古い）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersGetFollowList(int From, int Count, int SortOrder, Action<bool, ReceiveUsersGetFollowList> didLoad)
	{
		SendUsersGetFollowList request = new SendUsersGetFollowList ();
		request.From = From;
		request.Count = Count;
		request.SortOrder = SortOrder;
		AwsModule.Request.Exec<ReceiveUsersGetFollowList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersGetFollowList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/get_follower_list
	/// - フォロワー一覧の所得
	/// - リクエスト
	///   From:
	///     リストの所得開始位置、先頭が0
	///   Count:
	///     所得数
	///   SortOrder:
	///     並べ替え順 - 0:ログイン(新しい)、1:ログイン(古い)、2:レベル(昇順)、3:レベル(降順)、4:フォロー順（新しい）、5:フォロー順（古い）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersGetFollowerList(int From, int Count, int SortOrder, Action<bool, ReceiveUsersGetFollowerList> didLoad)
	{
		SendUsersGetFollowerList request = new SendUsersGetFollowerList ();
		request.From = From;
		request.Count = Count;
		request.SortOrder = SortOrder;
		AwsModule.Request.Exec<ReceiveUsersGetFollowerList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersGetFollowerList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/get_battle_supporter_list
	/// - サポート候補一覧の所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersGetBattleSupporterList(Action<bool, ReceiveUsersGetBattleSupporterList> didLoad)
	{
		SendUsersGetBattleSupporterList request = new SendUsersGetBattleSupporterList ();
		AwsModule.Request.Exec<ReceiveUsersGetBattleSupporterList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersGetBattleSupporterList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/get_friend_candidate_list
	/// - フレンド候補一覧の所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void UsersGetFriendCandidateList(Action<bool, ReceiveUsersGetFriendCandidateList> didLoad)
	{
		SendUsersGetFriendCandidateList request = new SendUsersGetFriendCandidateList ();
		AwsModule.Request.Exec<ReceiveUsersGetFriendCandidateList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersGetFriendCandidateList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/get_user_data
	/// - ユーザデータの所得
	/// - リクエスト
	///   UserIdList:
	///     所得したいユーザのUserIdのリスト、省略した場合は自分のユーザデータのみを返却する
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserDataList:
	///     UserDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - UserData
	///   UserId:
	///     ユーザID
	///   CustomerId:
	///     お客様番号
	///   Nickname:
	///     ニックネーム
	///   LastLoginDate:
	///     最終ログイン日時
	///   Level:
	///     レベル(マスターデータから経験値で逆引き)
	///   Exp: #自分の場合のみ
	///     経験値
	///   ActionPoint: #自分の場合のみ
	///     AP
	///   IsFollow: #他人の場合のみ
	///     フォローしているか
	///   IsFollower: #他人の場合のみ
	///     フォローされているか
	///   FollowDate: #他人の場合のみ
	///     自分がフォローした日時
	///   FollowerDate: #他人の場合のみ
	///     自分がフォローされた日時（相手がフォローした日時）
	///   FollowCount:
	///     フォロー数
	///   FolowwerCount:
	///     フォロワー数
	///   Comment:
	///     コメント
	///   MainCardData:
	///     メインカードのCardData
	///   SupportCardList:
	///     サポートカードのリスト
	///   ReceivablePresentCount: #自分の場合のみ
	///     未受取のプレゼントBOX内アイテム数
	///   ReceivableLoginbonusIdList: #自分の場合のみ
	///     受取可能なログインボーナスID
	/// </summary>
	public static void UsersGetUserData(int[] UserIdList, Action<bool, ReceiveUsersGetUserData> didLoad)
	{
		SendUsersGetUserData request = new SendUsersGetUserData ();
		request.UserIdList = UserIdList;
		AwsModule.Request.Exec<ReceiveUsersGetUserData> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersGetUserData>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/heal_action_point
	/// - AP回復薬の使用
	/// - リクエスト
	///   ConsumerId:
	///     消費アイテムのID
	///   Quantity:
	///     個数
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void UsersHealActionPoint(int ConsumerId, int Quantity, Action<bool, ReceiveUsersHealActionPoint> didLoad)
	{
		SendUsersHealActionPoint request = new SendUsersHealActionPoint ();
		request.ConsumerId = ConsumerId;
		request.Quantity = Quantity;
		AwsModule.Request.Exec<ReceiveUsersHealActionPoint> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersHealActionPoint>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/heal_action_point_with_gem
	/// - ジェムでAP全回復
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void UsersHealActionPointWithGem(Action<bool, ReceiveUsersHealActionPointWithGem> didLoad)
	{
		SendUsersHealActionPointWithGem request = new SendUsersHealActionPointWithGem ();
		AwsModule.Request.Exec<ReceiveUsersHealActionPointWithGem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersHealActionPointWithGem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/heal_battle_point
	/// - BP回復薬の使用
	/// - リクエスト
	///   ConsumerId:
	///     消費アイテムのID
	///   Quantity:
	///     個数
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PvpUserData:
	///     Pvpユーザデータ
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void UsersHealBattlePoint(int ConsumerId, int Quantity, Action<bool, ReceiveUsersHealBattlePoint> didLoad)
	{
		SendUsersHealBattlePoint request = new SendUsersHealBattlePoint ();
		request.ConsumerId = ConsumerId;
		request.Quantity = Quantity;
		AwsModule.Request.Exec<ReceiveUsersHealBattlePoint> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersHealBattlePoint>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/users/heal_battle_point_with_gem
	/// - ジェムでBP全回復
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   PvpUserData:
	///     Pvpユーザデータ
	///   UserData:
	///     ユーザデータ
	/// </summary>
	public static void UsersHealBattlePointWithGem(Action<bool, ReceiveUsersHealBattlePointWithGem> didLoad)
	{
		SendUsersHealBattlePointWithGem request = new SendUsersHealBattlePointWithGem ();
		AwsModule.Request.Exec<ReceiveUsersHealBattlePointWithGem> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveUsersHealBattlePointWithGem>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/get_weapon_list
	/// - 所持武器リストの所得
	/// - リクエスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   WeaponDataList:
	///     WeaponDataのリスト
	///   MasterVersion:
	///     マスターバージョン番号
	/// - WeaponData
	///   WeaponId:
	///     武器ID
	///   BagId:
	///     バッグID（ユニークID）
	///   Exp:
	///     経験値
	///   LimitBreakGrade:
	///     限界突破グレード（0〜4）
	///   IsEquipped:
	///     True:装着している、False:フリー
	///   IsLocked:
	///     True:ロック状態、False:アンロック状態
	///   CardId:
	///     装着先カードID
	///   SlotId:
	///     装着先スロットID（装着時は常に1）
	///   ModificationDate:
	///     最終更新日時
	///   CreationDate:
	///     獲得日時
	/// </summary>
	public static void WeaponsGetWeaponList(Action<bool, ReceiveWeaponsGetWeaponList> didLoad)
	{
		SendWeaponsGetWeaponList request = new SendWeaponsGetWeaponList ();
		AwsModule.Request.Exec<ReceiveWeaponsGetWeaponList> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsGetWeaponList>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/lock_weapon
	/// - 武器をロックする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   WeaponBagId:
	///     ロックする武器のBagId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   WeaponData:
	///     WeaponData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void WeaponsLockWeapon(long WeaponBagId, Action<bool, ReceiveWeaponsLockWeapon> didLoad)
	{
		SendWeaponsLockWeapon request = new SendWeaponsLockWeapon ();
		request.WeaponBagId = WeaponBagId;
		AwsModule.Request.Exec<ReceiveWeaponsLockWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsLockWeapon>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/unlock_weapon
	/// - 武器をアンロックする
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   WeaponBagId:
	///     アンロックする武器のBagId
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   WeaponData:
	///     WeaponData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void WeaponsUnlockWeapon(long WeaponBagId, Action<bool, ReceiveWeaponsUnlockWeapon> didLoad)
	{
		SendWeaponsUnlockWeapon request = new SendWeaponsUnlockWeapon ();
		request.WeaponBagId = WeaponBagId;
		AwsModule.Request.Exec<ReceiveWeaponsUnlockWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsUnlockWeapon>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/reinforce_weapon
	/// - 武器を強化する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   WeaponBagId:
	///     強化する武器のBagId
	///   MaterialWeaponBagIdList:
	///     素材にする武器のBagIdのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   ReinforcementDegreeId:
	///     1:成功、2:大成功、3:超成功
	///   WeaponData:
	///     WeaponData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void WeaponsReinforceWeapon(long WeaponBagId, long[] MaterialWeaponBagIdList, Action<bool, ReceiveWeaponsReinforceWeapon> didLoad)
	{
		SendWeaponsReinforceWeapon request = new SendWeaponsReinforceWeapon ();
		request.WeaponBagId = WeaponBagId;
		request.MaterialWeaponBagIdList = MaterialWeaponBagIdList;
		AwsModule.Request.Exec<ReceiveWeaponsReinforceWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsReinforceWeapon>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/limit_break_weapon
	/// - 武器を限界突破する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   WeaponBagId:
	///     限界突破する武器のBagId
	///   MaterialWeaponBagIdList:
	///     素材にする武器のBagIdのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   WeaponData:
	///     WeaponData
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void WeaponsLimitBreakWeapon(long WeaponBagId, long[] MaterialWeaponBagIdList, Action<bool, ReceiveWeaponsLimitBreakWeapon> didLoad)
	{
		SendWeaponsLimitBreakWeapon request = new SendWeaponsLimitBreakWeapon ();
		request.WeaponBagId = WeaponBagId;
		request.MaterialWeaponBagIdList = MaterialWeaponBagIdList;
		AwsModule.Request.Exec<ReceiveWeaponsLimitBreakWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsLimitBreakWeapon>(response, didLoad, true);
		});
	}

	/// <summary>
	/// URL: /api/weapons/sell_weapon
	/// - 武器を売却する
	/// - リクエスト
	///   RequestId:
	///     リクエストID（多重処理防止用）
	///   WeaponBagIdList:
	///     売却する武器のBagIdのリスト
	/// - レスポンス
	///   ResultCode:
	///     0:成功、1以上:失敗
	///   UserData:
	///     UserData
	///   MasterVersion:
	///     マスターバージョン番号
	/// </summary>
	public static void WeaponsSellWeapon(long[] WeaponBagIdList, Action<bool, ReceiveWeaponsSellWeapon> didLoad)
	{
		SendWeaponsSellWeapon request = new SendWeaponsSellWeapon ();
		request.WeaponBagIdList = WeaponBagIdList;
		AwsModule.Request.Exec<ReceiveWeaponsSellWeapon> (request, (response) => {
			AwsModule.Request.CheckResultCode<ReceiveWeaponsSellWeapon>(response, didLoad, true);
		});
	}

}
