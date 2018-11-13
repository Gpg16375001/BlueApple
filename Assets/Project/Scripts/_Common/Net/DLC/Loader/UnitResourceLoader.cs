using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.U2D;

using UniRx;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;
#endif

using SmileLab;
using SmileLab.Net.API;

public class StageEnemyResourceEqualityComparer : IEqualityComparer<BattleStageEnemy>
{
    public bool Equals(BattleStageEnemy x, BattleStageEnemy y)
    {
        if (x == null && y == null) {
            return true;
        } 
        if (x == null || y == null) {
            return false;
        }

        if (x.card != null && y.card != null) {
            return x.card.id == y.card.id;
        } else if(x.card != null || y.card != null) {
            return false;
        }

        if(x.monster.resource_id.HasValue && y.monster.resource_id.HasValue) {
            return x.monster.resource_id.Value == y.monster.resource_id.Value;
        }
        if(x.monster.resource_id.HasValue) {
            return x.monster.resource_id.Value == y.monster.id;
        }
        if(y.monster.resource_id.HasValue) {
            return x.monster.id == y.monster.resource_id.Value;
        }
        return x.monster.id == y.monster.id;
    }

    public int GetHashCode(BattleStageEnemy obj)
    {
        if(obj.card != null) {
            return obj.card.id.GetHashCode ();
        }
        if (obj.monster.resource_id.HasValue) {
            return obj.monster.resource_id.Value.GetHashCode ();
        }
        return obj.monster.id.GetHashCode ();
    }
}

/// <summary>
/// ユニット関連のリソースのロードの補助クラス
/// </summary>
// TODO: 武器情報も取得してここでダウンロードできるようにしたい。
public class UnitResourceLoader : IDisposable
{
    /// <summary>
    /// Spineモデルの読み込みをするかのフラグ
    /// </summary>
    public bool IsLoadSpineModel;
    /// <summary>
    /// ロード済みSpineモデルのprefab
    /// </summary>
    /// <value>The spine model.</value>
    public GameObject SpineModel {
        get;
        private set;
    }

    /// <summary>
    /// タイムラインアイコンの読み込みをするかのフラグ
    /// </summary>
    public bool IsLoadTimeLine;
    /// <summary>
    /// タイムラインアイコンのSprite
    /// </summary>
    /// <value>The time line icon.</value>
    public Sprite TimeLineIcon {
        get;
        private set;
    }

    /// <summary>
    /// Live2Dモデルの読み込みをするかのフラグ
    /// </summary>
    public bool IsLoadLive2DModel;
    /// <summary>
    /// Live2Dモデルのprefab
    /// </summary>
    /// <value>The live2 D model.</value>
    public GameObject Live2DModel {
        get;
        private set;
    }

    /// <summary>
    /// アニメーションクリップをロードするかのフラグ
    /// </summary>
    public bool IsLoadAnimationClip;
    /// <summary>
    /// アニメーションクリップのリスト
    /// </summary>
    /// <value>The animation clip list.</value>
    public AnimationClip[] AnimationClips {
        get;
        private set;
    }

    /// <summary>
    /// 立ち絵ロード
    /// </summary>
    public bool IsLoadPortrait;
    /// <summary>
    /// 立ち絵画像
    /// </summary>
    /// <value>The portrait images.</value>
    public Dictionary<int, Sprite> PortraitImages {
        get;
        private set;
    }

    /// <summary>
    /// カード背景ロード
    /// </summary>
    public bool IsLoadCardBg;
    /// <summary>
    /// カード背景画像
    /// </summary>
    /// <value>The card bgs.</value>
    public Dictionary<int, Sprite> CardBgs {
        get;
        private set;
    }

    /// <summary>
    /// カード特殊画像
    /// </summary>
    /// <value>The card bgs.</value>
    public Dictionary<int, Sprite> SpCards {
        get;
        private set;
    }

    /// <summary>
    /// カード背景ロード
    /// </summary>
    public bool IsLoadPartyCardImage;
    /// <summary>
    /// カード背景画像
    /// </summary>
    /// <value>The card bgs.</value>
    public Dictionary<int, Sprite> PartyCardImages {
        get;
        private set;
    }

    public bool IsLoadOriginalImage;
    public Sprite OriginalImage {
        get;
        private set;
    }

    /// <summary>
    /// ボイスをダウンロードするかの指定。
    /// 基本的にはtrueとなる
    /// </summary>
    public bool IsLoadVoiceFile;
    private string VoiceSheetName;

    /// <summary>
    /// ロード済みかを示すフラグ
    /// </summary>
    /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
    public bool IsLoaded {
        get;
        private set;
    }

    // ロードするファイルのS3上のパス
    private string S3Path;

    public void LoadFlagReset()
    {
        IsLoadSpineModel = false;
        IsLoadAnimationClip = false;
        IsLoadTimeLine = false;
        IsLoadLive2DModel = false;
        IsLoadPortrait = false;
        IsLoadCardBg = false;
        IsLoadPartyCardImage = false; 
        IsLoadOriginalImage = false;
        IsLoadVoiceFile = true;
    }

    public void SetBattleResourceLoad(bool isReset=false)
    {
        if (isReset) {
            LoadFlagReset ();
        }
        IsLoadSpineModel = true;
        IsLoadAnimationClip = true;
        IsLoadTimeLine = true;
        IsLoadLive2DModel = true;
        IsLoadVoiceFile = true;
    }

    public void SetLive2DResourceLoad(bool isReset=false)
    {
        if (isReset) {
            LoadFlagReset ();
        }
        IsLoadLive2DModel = true;
        IsLoadVoiceFile = true;
    }

    // マイページなどのLive2Dロード用
    public UnitResourceLoader(CardData card)
    {
        IsLoaded = false;
        SetBattleResourceLoad (true);

        VoiceSheetName = card.Card.voice_sheet_name;
        S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Unit, string.Format ("unit_{0}", card.CardId));
    }
    public UnitResourceLoader(int cardId)
    {
        IsLoaded = false;
        SetLive2DResourceLoad (true);

        VoiceSheetName = MasterDataTable.card[cardId].voice_sheet_name;
        S3Path = DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Unit, string.Format("unit_{0}", cardId));
    }

    // 敵リソースのロード用
    public UnitResourceLoader(BattleStageEnemy enemy)
    {
        IsLoaded = false;
        SetBattleResourceLoad (true);
        IsLoadTimeLine = false;
        IsLoadLive2DModel = false;
        IsLoadVoiceFile = true;

        VoiceSheetName = enemy.monster.voice_sheet_name;
        if (enemy.card != null) {
            S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Unit, string.Format ("unit_{0}", enemy.card.id));
        } else if (enemy.monster.resource_id.HasValue) {
            S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Enemy, string.Format ("enemy_{0}", enemy.monster.resource_id.Value));
        } else {
            S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Enemy, string.Format ("enemy_{0}", enemy.monster.id));
        }
    }

    // バトル時ロード用
    public UnitResourceLoader(BattleLogic.Parameter parameter)
    {
        SetBattleResourceLoad (true);

        VoiceSheetName = parameter.VoiceFileName;
        if (parameter.IsCard) {
            S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Unit, string.Format ("unit_{0}", parameter.ID));
        } else {
            IsLoadTimeLine = false;
            IsLoadLive2DModel = false;
            if (parameter.ResourceIsCard) {
                S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Unit, string.Format ("unit_{0}", parameter.ResourceID));
            } else {
                S3Path = DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Enemy, string.Format ("enemy_{0}", parameter.ResourceID));
            }
        }
    }

    // ロード開始
    public void LoadResource(Action<UnitResourceLoader> didLoad, MonoBehaviour behaviour = null)
    {
        // 何もロードフラグが立っていない
        if (!IsLoadSpineModel &&
            !IsLoadTimeLine &&
            !IsLoadLive2DModel &&
            !IsLoadAnimationClip &&
            !IsLoadPortrait &&
            !IsLoadCardBg &&
            !IsLoadPartyCardImage &&
            !IsLoadOriginalImage && 
            !IsLoadVoiceFile)
        {
            didLoad (this);
            return;
        }

        // すでにロード済み
        if (IsLoaded) {
            didLoad (this);
            return;
        }

        // ボイスデータのロード
        if (IsLoadVoiceFile && !string.IsNullOrEmpty (VoiceSheetName)) {
            List<string> voiceFileList = new List<string>();
            var sheet = MasterDataTable.cri_sound_cue [VoiceSheetName];
            if (sheet != null) {
                voiceFileList.Add (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.acb", sheet.ACBFileName)));
                if (!string.IsNullOrEmpty (sheet.AWBFileName)) {
                    voiceFileList.Add (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.awb", sheet.AWBFileName)));
                }
                DLCManager.DownloadFiles (voiceFileList,
                    (ret) => {
                        DLCManager.AssetBundleFromFileOrDownload (S3Path,
                            (ABRef) => {
                                if (isDisposed)
                                    return;
                                var obsever = ObservableUnity.FromCoroutine<UnitResourceLoader> (
                                              (observer, cancellatio) => CoLoadResource (ABRef, observer, cancellatio)
                                          ).Subscribe (didLoad, ex => {
                                    Debug.LogException (ex);
                                });
                            }
                        );
                    }
                );
            }
            return;
        }

        DLCManager.AssetBundleFromFileOrDownload (S3Path,
            (ABRef) => {
                var obsever = ObservableUnity.FromCoroutine<UnitResourceLoader>(
                    (observer, cancellatio) => CoLoadResource(ABRef, observer, cancellatio)
                ).Subscribe(didLoad, ex => { Debug.LogException(ex); });
            }
        );
    }

    public Sprite GetCardBg(int rarity) {
        if (CardBgs == null || CardBgs.Count <= 0 || !CardBgs.Keys.Any (x => x <= rarity)) {
            // コモンBGを使用する
            rarity = Mathf.Clamp (rarity, 2, 5);
            var cardAtlas = Resources.Load<SpriteAtlas>("Atlases/Card");
            return cardAtlas.GetSprite (string.Format("CardBg_R{0}", rarity));
        } 
        rarity = CardBgs.Keys.Where (x => x <= rarity).Max ();
        return CardBgs [rarity];
    }

    public Sprite GetSpCard(int rarity) {
        if (SpCards == null || SpCards.Count <= 0 || !SpCards.Keys.Any(x => x <= rarity)) {
            return null;
        } 
        rarity = SpCards.Keys.Where (x => x <= rarity).Max ();
        return SpCards [rarity];
    }

    public Sprite GetPartyCardImage(int rarity) {
        if (PartyCardImages == null || PartyCardImages.Count <= 0 || !PartyCardImages.Keys.Any(x => x <= rarity)) {
            return null;
        }
        rarity = PartyCardImages.Keys.Where (x => x <= rarity).Max ();
        return PartyCardImages [rarity];
    }

    public Sprite GetCardFrame(int rarity) {
        // コモンフレームを使用する
        rarity = Mathf.Clamp (rarity, 2, 5);
        var cardAtlas = Resources.Load<SpriteAtlas>("Atlases/Card");
        return cardAtlas.GetSprite (string.Format("CardFrame_R{0}", rarity));
    }

    public Sprite GetPortrait(int rarity) {
        if (PortraitImages == null || PortraitImages.Count <= 0 || !PortraitImages.Keys.Any(x => x <= rarity)) {
            return null;
        }
        rarity = PortraitImages.Keys.Where (x => x <= rarity).Max ();
        return PortraitImages [rarity];
    }


    private static readonly string TimeLineIconName = "icon_tl";
    private static readonly string SpineModelName = "spinemodel";
    private static readonly string Live2DModelName = "live2dmodel";
    private static readonly string PortraitName = "portrait";
    private static readonly string CardBgName = "cardbg";
    private static readonly string SpCardName = "spcard";
    private static readonly string PartyCardName = "partycard";
    private static readonly string OriginalImageName = "originalimage";

    private IEnumerator CoLoadResource(AssetBundleRef ABRef, IObserver<UnitResourceLoader> observer, CancellationToken cancel)
    {
        if (IsLoadTimeLine) {
            // タイムラインアイコン
            var assetLoad = ABRef.assetbundle.LoadAssetAsync<Sprite> (TimeLineIconName);
            yield return assetLoad;
            TimeLineIcon = assetLoad.asset as Sprite;
            if (TimeLineIcon == null) {
                ABRef.Unload (false);
                observer.OnError(new Exception("TimeLineIcon Load faield"));
                yield break;                
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadSpineModel) {
            // Spine
            var assetLoad = ABRef.assetbundle.LoadAssetAsync<GameObject> (SpineModelName);
            yield return assetLoad;
            SpineModel = assetLoad.asset as GameObject;
            if (SpineModel == null) {
                ABRef.Unload (false);
                observer.OnError(new Exception("SpineModel Load faield"));
                yield break;                
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadLive2DModel) {
            // Live2D
            var assetLoad = ABRef.assetbundle.LoadAssetAsync<GameObject> (Live2DModelName);
            yield return assetLoad;
            Live2DModel = assetLoad.asset as GameObject;
            if (Live2DModel == null) {
                ABRef.Unload (false);
                observer.OnError(new Exception("Live2DModel Load faield"));
                yield break;                
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadAnimationClip) {
            // Animation Clip
            var assetLoad = ABRef.assetbundle.LoadAllAssetsAsync<AnimationClip> ();
            yield return assetLoad;

            AnimationClips = assetLoad.allAssets.Select (x => x as AnimationClip).ToArray ();

            if (AnimationClips == null || AnimationClips.Length <= 0) {
                ABRef.Unload (false);
                observer.OnError(new Exception("AnimationClips Load faield"));
                yield break;                
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadPortrait) {

            List<AssetBundleRequest> requests = new List<AssetBundleRequest>();
            foreach(var name in GetAssetNames (ABRef.assetbundle, PortraitName).ToArray()) {
                requests.Add (ABRef.assetbundle.LoadAssetAsync<Sprite> (name));
            }
            yield return new WaitUntil (() => requests.All (x => x.isDone));

            PortraitImages = new Dictionary<int, Sprite> ();
            foreach (var request in requests) {
                var portrait = request.asset as Sprite;
                if (portrait == null) {
                    ABRef.Unload (false);
                    observer.OnError(new Exception("PortraitImage Load faield"));
                    yield break;
                }
                int rarity = 0;
                Regex reg = new Regex(@"Portrait_R(?<rarity>.*)");
                Match match = reg.Match(portrait.name);
                if (match.Success) {
                    int.TryParse (match.Groups ["rarity"].Value, out rarity);
                }
                if (PortraitImages.ContainsKey (rarity)) {
                    continue;
                }
                PortraitImages.Add (rarity, portrait);
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadCardBg) {
            // 複数個のロードがありうる
            List<AssetBundleRequest> requests = new List<AssetBundleRequest> ();
            foreach (var name in GetAssetNames (ABRef.assetbundle, CardBgName).ToArray()) {
                requests.Add (ABRef.assetbundle.LoadAssetAsync<Sprite> (name));
            }
            yield return new WaitUntil (() => requests.All (x => x.isDone));

            CardBgs = new Dictionary<int, Sprite> ();
            foreach (var request in requests) {
                var cardBg = request.asset as Sprite;
                if (cardBg == null) {
                    ABRef.Unload (false);
                    observer.OnError (new Exception ("CardBg Load faield"));
                    yield break;
                }
                int rarity = 0;
                Regex reg = new Regex (@"CardBg_R(?<rarity>.*)");
                Match match = reg.Match (cardBg.name);
                if (match.Success) {
                    int.TryParse (match.Groups ["rarity"].Value, out rarity);
                }
                if (CardBgs.ContainsKey (rarity)) {
                    continue;
                }
                CardBgs.Add (rarity, cardBg);
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadCardBg) {
            // 複数個のロードがありうる
            // SpCardの取得
            List<AssetBundleRequest> requests = new List<AssetBundleRequest>();
            foreach(var name in GetAssetNames (ABRef.assetbundle, SpCardName).ToArray()) {
                requests.Add (ABRef.assetbundle.LoadAssetAsync<Sprite> (name));
            }
            yield return new WaitUntil (() => requests.All (x => x.isDone));

            SpCards = new Dictionary<int, Sprite> ();
            foreach (var request in requests) {
                var cardBg = request.asset as Sprite;
                if (cardBg == null) {
                    ABRef.Unload (false);
                    observer.OnError(new Exception("SpCard Load faield"));
                    yield break;
                }
                int rarity = 0;
                Regex reg = new Regex(@"SpCard_R(?<rarity>.*)");
                Match match = reg.Match(cardBg.name);
                if (match.Success) {
                    int.TryParse (match.Groups ["rarity"].Value, out rarity);
                }
                if (SpCards.ContainsKey (rarity)) {
                    continue;
                }
                SpCards.Add (rarity, cardBg);
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadPartyCardImage) {
            // 複数個のロードがありうる
            List<AssetBundleRequest> requests = new List<AssetBundleRequest>();
            foreach(var name in GetAssetNames (ABRef.assetbundle, PartyCardName).ToArray()) {
                requests.Add (ABRef.assetbundle.LoadAssetAsync<Sprite> (name));
            }
            yield return new WaitUntil (() => requests.All (x => x.isDone));

            PartyCardImages = new Dictionary<int, Sprite> ();
            foreach (var request in requests) {
                var cardBg = request.asset as Sprite;
                if (cardBg == null) {
                    ABRef.Unload (false);
                    observer.OnError(new Exception("PartyCard Load faield"));
                    yield break;
                }
                int rarity = 0;
                Regex reg = new Regex(@"PartyCard_R(?<rarity>.*)");
                Match match = reg.Match(cardBg.name);
                if (match.Success) {
                    int.TryParse (match.Groups ["rarity"].Value, out rarity);
                }
                if (PartyCardImages.ContainsKey (rarity)) {
                    continue;
                }
                PartyCardImages.Add (rarity, cardBg);
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        if (IsLoadOriginalImage) {
            if (ABRef.assetbundle.Contains (OriginalImageName)) {
                // タイムラインアイコン
                var assetLoad = ABRef.assetbundle.LoadAssetAsync<Sprite> (OriginalImageName);
                yield return assetLoad;
                OriginalImage = assetLoad.asset as Sprite;
                if (OriginalImage == null) {
                    ABRef.Unload (false);
                    observer.OnError(new Exception("OriginalImage Load faield"));
                    yield break;                
                }
            }
        }

        if(cancel.IsCancellationRequested) {
            ABRef.Unload (false);
            yield break;
        }

        IsLoaded = true;

        ABRef.Unload (false);
        observer.OnNext(this);
        observer.OnCompleted();
    }

    private IEnumerable<string> GetAssetNames(AssetBundle asset, string name)
    {
        var allAssetNames = asset.GetAllAssetNames ();
        int count = allAssetNames.Length;
        for (int i = 0; i < count; ++i) {
            if (allAssetNames [i].Contains (name)) {
                yield return allAssetNames [i];
            }
        }
    }

    public void Dispose()
    {
        TimeLineIcon = null;
        SpineModel = null;
        Live2DModel = null;
        AnimationClips = null;
        if (PortraitImages != null) {
            PortraitImages.Clear ();
            PortraitImages = null;
        }
        if (CardBgs != null) {
            CardBgs.Clear ();
            CardBgs = null;
        }
        if (SpCards != null) {
            SpCards.Clear ();
            SpCards = null;
        }
        if (PartyCardImages != null) {
            PartyCardImages.Clear ();
            PartyCardImages = null;
        }
        OriginalImage = null;
        IsLoaded = false;

        if (m_Observer != null) {
            ObservableUnity.DoOnCancel (m_Observer, null);
            m_Observer = null;
        }

        isDisposed = true;
    }

    bool isDisposed = false;
    IObservable<UnitResourceLoader> m_Observer;
}
