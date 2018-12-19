using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

using SmileLab;
using SmileLab.Net.API;

public class IconLoadSetting {
    public ItemTypeEnum type;
    public int id;
	public int rarity;


    public event IconLoader.IconLoadedEvent loaded;

    public bool IsEmpty {
        get {
            return loaded == null;
        }
    }
    public void Loaded(Sprite icon)
    {
        if (loaded != null) {
            loaded.Invoke (this, icon);
        }
    }

    public override bool Equals (object obj)
    {
        IconLoadSetting data = obj as IconLoadSetting;
		return data != null && data.type == this.type && data.id == this.id && data.rarity == this.rarity;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode ();
    }

}

// TODO: 武具アイコンやマギカイトもここにまとめる
public static class IconLoader {
    public delegate void IconLoadedEvent(IconLoadSetting data, Sprite icon);

    private const int LoadCoroutineCount = 5;

    public static void Init()
    {
        m_Reference = new Dictionary<UniRx.Tuple<ItemTypeEnum, int, int>, WeakReference> ();
        m_LoadRequestQueue = new Queue<IconLoadSetting> ();
        m_LoadingList = new List<IconLoadSetting> ();

        m_IsLooping = true;
        m_CoroutineInfos = new CoroutineAgent.CoroutineInfo[LoadCoroutineCount];
        for (int i = 0; i < LoadCoroutineCount; ++i) {
            m_CoroutineInfos[i] = CoroutineAgent.CreateCoroutine (CoLoadIcon ());
            CoroutineAgent.Execute (m_CoroutineInfos[i]);
        }
    }

    public static void Dispose()
    {
        m_LoadRequestQueue.Clear ();
        if (m_CoroutineInfos != null) {
            m_IsLooping = false;
            for (int i = 0; i < LoadCoroutineCount; ++i) {
                CoroutineAgent.Stop (m_CoroutineInfos [i], true);
                m_CoroutineInfos [i] = null;
            }
            m_CoroutineInfos = null;
        }
        m_LoadingList.Clear ();
        m_Reference.Clear ();
    }

    // ElementIcon
    public static Sprite LoadElementIcon(Element element)
    {
        var elementIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ElementIcon");
        return elementIconAtlas.GetSprite (((int)element.Enum).ToString ());
    }

    public static Sprite LoadElementLIcon(Element element)
    {
        var elementIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ElementIcon");
        return elementIconAtlas.GetSprite (string.Format("{0}L", (int)element.Enum));
    }
    // EmblemIcon
    public static Sprite LoadEmblem(Belonging belonging)
    {
        var elementIconAtlas = Resources.Load<SpriteAtlas>("Atlases/EmblemIcon");
        return elementIconAtlas.GetSprite (((int)belonging.Enum).ToString ());
    }

    // ConditionIcon
    public static Sprite LoadConditionIcon(Condition condition)
    {
        var elementIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ConditionIcon");
        return elementIconAtlas.GetSprite (condition.id.ToString ());
    }

    // ListIconFrame
    public static Sprite LoadRoleIcon(CardRoleEnum role)
    {
        var listIconAtlas = Resources.Load<SpriteAtlas>("Atlases/RoleTypeIcon");
        return listIconAtlas.GetSprite (((int)role).ToString());
    }

    // ListIconBG
    public static Sprite LoadListIconBg(int rarity)
    {
        rarity = Mathf.Clamp (rarity, 2, 6);
        var listIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ListIcon");
        return listIconAtlas.GetSprite (string.Format("ListIconBg_R{0}", rarity));
    }
    public static Sprite LoadWeaponListIconBg(int rarity)
	{
		rarity = Mathf.Clamp(rarity, 2, 5);
        var listIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ListWeaponIcon");
        return listIconAtlas.GetSprite(string.Format("ListWeaponIconBg_R{0}", rarity));
	}

    // ListIconFrame
    public static Sprite LoadListIconFrame(int rarity)
    {
		rarity = Mathf.Clamp (rarity, 2, 6);
        var listIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ListIcon");
        return listIconAtlas.GetSprite (string.Format("ListIconFrame_R{0}", rarity));
    }
	public static Sprite LoadWeaponListIconFrame(int rarity)
    {
        rarity = Mathf.Clamp(rarity, 2, 5);
        var listIconAtlas = Resources.Load<SpriteAtlas>("Atlases/ListWeaponIcon");
        return listIconAtlas.GetSprite(string.Format("ListWeaponIconFrame_R{0}", rarity));
    }
    
    /// <summary>
    /// カード用のアイコンロード処理
    /// </summary>
    // サーバーデータからのロード
    public static void LoadCardIcon(CardData card, IconLoadedEvent didLoaded)
    {
		LoadCardIcon (card.CardId, card.Rarity, didLoaded);
    }

    // マスターデータからのロード
    public static void LoadCardIcon(CardCard card, IconLoadedEvent didLoaded)
    {
		LoadCardIcon (card.id, card.rarity, didLoaded);
    }

    // カードIDからのロード
    public static void LoadCardIcon(int cardId, IconLoadedEvent didLoaded)
    {
		LoadIcon(ItemTypeEnum.card, cardId, didLoaded);
    }
	public static void LoadCardIcon(int cardId, int rarity, IconLoadedEvent didLoaded)
    {
		LoadIcon(ItemTypeEnum.card, cardId, rarity, didLoaded);
    }

	/// <summary>
    /// 武器用のアイコンロード処理
    /// </summary>
    // 武器IDからのロード
	public static void LoadWeaponIcon(int weaponId, IconLoadedEvent didLoaded)
	{
		LoadIcon(ItemTypeEnum.weapon, weaponId, didLoaded);
	}

    /// <summary>
    /// マギカイトアイコンロード
    /// </summary>
    /// <param name="magikiteId">Magikite identifier.</param>
    /// <param name="didLoaded">Did loaded.</param>
    public static void LoadMagikite(int magikiteId, IconLoadedEvent didLoaded)
    {
        LoadIcon(ItemTypeEnum.magikite, magikiteId, didLoaded);
    }

    /// <summary>
    /// 素材アイコンロード
    /// </summary>
    /// <param name="materialId">Material identifier.</param>
    /// <param name="didLoaded">Did loaded.</param>
    public static void LoadMaterial(int materialId, IconLoadedEvent didLoaded)
    {
        LoadIcon(ItemTypeEnum.material, materialId, didLoaded);
    }

    /// <summary>
    /// 消費アイテムアイコンロード
    /// </summary>
    /// <param name="consumerId">Consumer identifier.</param>
    /// <param name="didLoaded">Did loaded.</param>
    public static void LoadConsumer(int consumerId, IconLoadedEvent didLoaded)
    {
        LoadIcon(ItemTypeEnum.consumer, consumerId, didLoaded);
    }

	/// <summary>
	/// 消費アイテムアイコンロード
	/// </summary>
	/// <param name="consumerId">Consumer identifier.</param>
	/// <param name="didLoaded">Did loaded.</param>
	public static void LoadEventPoint(int eventId, IconLoadedEvent didLoaded)
	{
		LoadIcon(ItemTypeEnum.event_point, eventId, didLoaded);
	}

    // アイコンロード汎用処理
	private static void LoadIcon(ItemTypeEnum type, int id, int rarity, IconLoadedEvent didLoaded)
    {
		UniRx.Tuple<ItemTypeEnum, int, int> key = new UniRx.Tuple<ItemTypeEnum, int, int>(type, id, rarity);

        WeakReference reference;
        if (m_Reference.TryGetValue(key, out reference)) {
            if (reference.IsAlive) {
                didLoaded(
                    new IconLoadSetting() {
                        type = type,
                        id = id,
						rarity = rarity
                    }, reference.Target as Sprite);
                return;
            }
            // 死んでたらキーを削除してロードキューにつむ
            m_Reference.Remove(key);
        }

		var loadSetting = GetExistLoadSetting(type, id, rarity);
        // すでに同じアイコンロード用のデータがキューに積まれていたらロード完了時呼び出しをまとめてしまう。
        if (loadSetting != null) {
            loadSetting.loaded += didLoaded;
            return;
        }

		loadSetting = new IconLoadSetting() {
			type = type,
			id = id,
			rarity = rarity
        };
        loadSetting.loaded += didLoaded;
        m_LoadRequestQueue.Enqueue(loadSetting);
    }
	private static void LoadIcon(ItemTypeEnum type, int id, IconLoadedEvent didLoaded)
	{
		LoadIcon(type, id, 0, didLoaded);
	}

    private static IconLoadSetting GetExistLoadSetting(ItemTypeEnum type, int id, int rarity)
    {
		var loadSetting = m_LoadRequestQueue.FirstOrDefault (x => x.type == type && x.id == id && x.rarity == rarity);
        if (loadSetting == null) {
			loadSetting = m_LoadingList.FirstOrDefault(x => x.type == type && x.id == id && x.rarity == rarity);
        }
        return loadSetting;
    }

	public static void RemoveLoadedEvent(ItemTypeEnum type, int id, IconLoadedEvent didLoaded)
    {
		RemoveLoadedEvent(type, id, 0, didLoaded);
    }
	public static void RemoveLoadedEvent(ItemTypeEnum type, int id, int rarity, IconLoadedEvent didLoaded)
    {
        var loadSetting = GetExistLoadSetting(type, id, rarity);
        if (loadSetting != null) {
            loadSetting.loaded -= didLoaded;
        }
    }
        
    private static bool m_IsLooping;
    static IEnumerator CoLoadIcon()
    {
        while (m_IsLooping) {
            if (m_LoadRequestQueue.Count > 0) {
                var loadData = m_LoadRequestQueue.Dequeue ();

                if (loadData.IsEmpty) {
                    continue;
                }

                m_LoadingList.Add (loadData);
                switch (loadData.type) {
                case ItemTypeEnum.card:
                    yield return CoLoadCardIcon (loadData);
                    break;
                case ItemTypeEnum.weapon:
                    yield return CoLoadWeaponIcon (loadData);
                    break;
                case ItemTypeEnum.magikite:
                    yield return CoLoadMagikiteIcon (loadData);
                    break;
                case ItemTypeEnum.material:
                    yield return CoLoadMaterialIcon (loadData);
                    break;
                case ItemTypeEnum.consumer:
                    yield return CoLoadConsumerIcon (loadData);
                    break;
				case ItemTypeEnum.event_point:
					yield return CoLoadEventQuestPointIcon (loadData);
					break;
                default:
                    break;
                }
                m_LoadingList.Remove (loadData);
            } else {
                yield return new WaitUntil (() => m_LoadRequestQueue.Count > 0);
            }
        }
    }

    static IEnumerator CoLoadCardIcon(IconLoadSetting data)
    {
        var UnitIconAssetBundle = DLCManager.AssetBundleFromFile (DLCManager.DLC_FOLDER.Icon, "unit_icon");
        if (UnitIconAssetBundle == null) {
            Debug.LogError ("Not Load AssetBundle unit_icon");
            data.Loaded(null);
            yield break;
        }
  
		var iconName = string.Format("{0}_r{1}", data.id, data.rarity);
		if (!Array.Exists(UnitIconAssetBundle.assetbundle.GetAllAssetNames(), an => an.Contains(iconName))){
			iconName = data.id.ToString();
		}
		var asyncObj = UnitIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(iconName);
        yield return asyncObj;

        var spr = asyncObj.asset as Sprite;
        data.Loaded(spr);
		m_Reference.Add (new UniRx.Tuple<ItemTypeEnum, int, int> (data.type, data.id, data.rarity), new WeakReference (spr));

        data = null;
        asyncObj = null;
        UnitIconAssetBundle.Unload (false);
    }

	static IEnumerator CoLoadWeaponIcon(IconLoadSetting data)
    {
		var WeaponIconAssetBundle = DLCManager.AssetBundleFromFile(DLCManager.DLC_FOLDER.Icon, "weapon_icon");
        if (WeaponIconAssetBundle == null) {
            Debug.LogError ("Not Load AssetBundle weapon_icon");
            data.Loaded(null);
            yield break;
        }
        var asyncObj = WeaponIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(data.id.ToString());
        yield return asyncObj;

        var spr = asyncObj.asset as Sprite;
        data.Loaded(spr);
		m_Reference.Add(new UniRx.Tuple<ItemTypeEnum, int, int>(data.type, data.id, data.rarity), new WeakReference(spr));

        data = null;
        asyncObj = null;
        WeaponIconAssetBundle.Unload(false);
    }

    static IEnumerator CoLoadMagikiteIcon(IconLoadSetting data)
    {
        var MagikiteIconAssetBundle = DLCManager.AssetBundleFromFile(DLCManager.DLC_FOLDER.Icon, "magikite_icon");
        if (MagikiteIconAssetBundle == null) {
            Debug.LogError ("Not Load AssetBundle magikite_icon");
            data.Loaded(null);
            yield break;
        }
        var asyncObj = MagikiteIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(data.id.ToString());
        yield return asyncObj;

        var spr = asyncObj.asset as Sprite;
        data.Loaded(spr);
		m_Reference.Add(new UniRx.Tuple<ItemTypeEnum, int, int>(data.type, data.id, data.rarity), new WeakReference(spr));

        data = null;
        asyncObj = null;
        MagikiteIconAssetBundle.Unload(false);
    }

    static IEnumerator CoLoadMaterialIcon(IconLoadSetting data)
    {
        var MaterialIconAssetBundle = DLCManager.AssetBundleFromFile(DLCManager.DLC_FOLDER.Icon, "material_icon");
        if (MaterialIconAssetBundle == null) {
            Debug.LogError ("Not Load AssetBundle material_icon");
            data.Loaded(null);
            yield break;
        }
        var asyncObj = MaterialIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(data.id.ToString());
        yield return asyncObj;

        var spr = asyncObj.asset as Sprite;
        data.Loaded(spr);
		m_Reference.Add(new UniRx.Tuple<ItemTypeEnum, int, int>(data.type, data.id, data.rarity), new WeakReference(spr));

        data = null;
        asyncObj = null;
        MaterialIconAssetBundle.Unload(false);
    }

    static IEnumerator CoLoadConsumerIcon(IconLoadSetting data)
    {
        var ConsumerIconAssetBundle = DLCManager.AssetBundleFromFile(DLCManager.DLC_FOLDER.Icon, "consumer_icon");
        if (ConsumerIconAssetBundle == null) {
            Debug.LogError ("Not Load AssetBundle consumer_icon");
            data.Loaded(null);
            yield break;
        }
        var asyncObj = ConsumerIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(data.id.ToString());
        yield return asyncObj;

        var spr = asyncObj.asset as Sprite;
        data.Loaded(spr);
		m_Reference.Add(new UniRx.Tuple<ItemTypeEnum, int, int>(data.type, data.id, data.rarity), new WeakReference(spr));

        data = null;
        asyncObj = null;
        ConsumerIconAssetBundle.Unload(false);
    }

	static IEnumerator CoLoadEventQuestPointIcon(IconLoadSetting data)
	{
		var EventQuestPointIconAssetBundle = DLCManager.AssetBundleFromFile(DLCManager.DLC_FOLDER.Icon, "eventquestpoint_icon");
		if (EventQuestPointIconAssetBundle == null) {
            // なくても問題はないのでエラーは出さないでスルーする
			//Debug.LogError ("Not Load AssetBundle eventquestpoint_icon");
            var CurrencyIconAtlas = Resources.Load<SpriteAtlas>("Atlases/CurrencyIcon");
            data.Loaded(CurrencyIconAtlas.GetSprite ("IconEventPoint"));
			yield break;
		}
		var asyncObj = EventQuestPointIconAssetBundle.assetbundle.LoadAssetAsync<Sprite>(data.id.ToString());
		yield return asyncObj;

		var spr = asyncObj.asset as Sprite;
		data.Loaded(spr);
		m_Reference.Add(new UniRx.Tuple<ItemTypeEnum, int, int>(data.type, data.id, data.rarity), new WeakReference(spr));

		data = null;
		asyncObj = null;
		EventQuestPointIconAssetBundle.Unload(false);
	}

    static Queue<IconLoadSetting> m_LoadRequestQueue;
    static List<IconLoadSetting> m_LoadingList;

    static CoroutineAgent.CoroutineInfo[] m_CoroutineInfos;

    // WeakReferenceでオブジェクトをおっておいて生きてたら再使用する。
    static Dictionary<UniRx.Tuple<ItemTypeEnum, int, int>, WeakReference> m_Reference;
}
