using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using SmileLab;
using SmileLab.Net;

/// <summary>
/// クラス：CRIサウンド操作.
/// </summary>
public class CriSoundControll : ISoundControll
{
    /// <summary>
    /// プロパティ：BGM 音量. [0, 1.0f]
    /// </summary>
    public float VolumeBGM {
        get { return m_volumeBGM; }
        set { 
            m_volumeBGM = value; 
            CriAtomExCategory.SetVolume ("BGM", m_volumeBGM);
        }
    }

    private float m_volumeBGM;

    /// <summary>
    /// プロパティ：SE 音量. [0, 1.0f]
    /// </summary>
    public float VolumeSE {
        get { return m_volumeSE; }
        set {
            m_volumeSE = value;
            CriAtomExCategory.SetVolume ("SE", m_volumeSE);
        }
    }

    private float m_volumeSE;

    /// <summary>
    /// プロパティ：Voice 音量. [0, 1.0f]
    /// </summary>
    public float VolumeVoice {
        get { return m_volumeVoice; }
        set {
            m_volumeVoice = value;
            CriAtomExCategory.SetVolume ("VOICE", m_volumeVoice);
        }
    }

    private float m_volumeVoice;

    /// <summary>
    /// Playerの最大管理数を指定
    /// </summary>
    private int m_playerMax = 10;

    /// <summary>
    /// プレイヤーが足りない時に勝手にPlayerを追加するオプション
    /// </summary>
    private bool m_autoAddPlayerMissingTime = false;

    private static readonly string acfFile = "Sound/Glare.acf";

    /// <summary>
    /// サウンドが鳴っている？
    /// </summary>
    public bool IsPlay {
        get {
            return this.IsPlayBGM && this.IsPlaySE && this.IsPlayVoice;
        }
    }

    /// <summary>
    /// サウンドが鳴っている？ BGM
    /// </summary>
    public bool IsPlayBGM { 
        get {
            return m_bgmPlayer != null && IsPlayPlayer (m_bgmPlayer);
        } 
    }

    /// <summary>
    /// サウンドが鳴っている？ : SE
    /// </summary>
    public bool IsPlaySE {
        get {
            return m_sePlayerList.Any (x => IsPlayPlayer (x));
        }
    }

    /// <summary>
    /// サウンドが鳴っている？ BGM
    /// </summary>
    public bool IsPlayVoice { 
        get {
            return m_voicePlayer != null && IsPlayPlayer (m_voicePlayer);
        } 
    }

    private CriAtomExPlayer[] m_players;
    private CriAtomExPlayer m_bgmPlayer;
    private string m_bgmCueName = null;
    private List<CriAtomExPlayer> m_sePlayerList = new List<CriAtomExPlayer> ();
    private CriAtomExPlayer m_voicePlayer;

    private Dictionary<int, CriAtomExPlayerOutputAnalyzer> m_analyzerDict = new Dictionary<int, CriAtomExPlayerOutputAnalyzer> ();

    private int nowBgmPlayIndex = -1;
    private CriAtomExAcb[] m_bgmCache;
    private string[] m_bgmCacheCueName;
    private SoundCache m_soundCache;

    /// <summary>
    /// 破棄メソッド.
    /// </summary>
    public void Dispose ()
    {
        // 管理領域をクリア
        m_bgmPlayer = null;
        m_sePlayerList.Clear ();
        m_voicePlayer = null;

        // CriAtomExPlayerを削除
        if (m_players != null) {
            foreach (var player in m_players) {
                if (IsPlayPlayer (player)) {
                    player.StopWithoutReleaseTime ();
                }
                player.DetachFader ();
                player.Dispose ();
            }
            m_players = null;
        }

        if (m_soundCache != null) {
            m_soundCache.Dispose ();
            m_soundCache = null;
        }

        if (m_bgmCache != null) {
            foreach (var acb in m_bgmCache) {
                if (acb != null) {
                    acb.Dispose ();
                }
            }
            m_bgmCache = null;
        }
        m_bgmCacheCueName = null;
        nowBgmPlayIndex = -1;

        if (CriWareInitializer.IsInitialized ()) {
            CriAtomEx.UnregisterAcf ();
        }
    }

    /// <summary>
    /// 初期化.SharedInstanceから.
    /// </summary>
    public void Init (SoundManager manager)
    {
        if (!CriWareInitializer.IsInitialized ()) {
            // 初期化済みでなければmanagerオブジェクトをDontDestroyOnLoadに登録し必須のオブジェクトを子に移動
            // CRIフレームワークの初期化.
            var initializer = manager.GetScript<CriWareInitializer> ("CriWareLibraryInitializer");
            var errothandler = manager.GetScript<CriWareErrorHandler> ("CriWareErrorHandler");

            GameObject.DontDestroyOnLoad (CriWare.managerObject);
            initializer.transform.SetParent (CriWare.managerObject.transform);
            errothandler.transform.SetParent (CriWare.managerObject.transform);

            // 初期化処理内部で重複初期化されないようにされている。
            // InitializeとDestroyの回数が合わなくなると問題があるので必ず毎回Initializeを呼ぶ
            initializer.Initialize ();
        } else {
            CriWareInitializer initializer = null;
            if (CriWare.managerObject != null) {
                initializer = CriWare.managerObject.GetComponentInChildren<CriWareInitializer> ();
            }
            if (initializer == null) {
                initializer = manager.GetScript<CriWareInitializer> ("CriWareLibraryInitializer");
            }

            // 初期化処理内部で重複初期化されないようにされている。
            // InitializeとDestroyの回数が合わなくなると問題があるので必ず毎回Initializeを呼ぶ
            initializer.Initialize ();
        }

        // 初期化済みであればACFの登録
        if (CriWareInitializer.IsInitialized ()) {
            // ACFファイルの登録
#if !UNITY_ANDROID || UNITY_EDITOR
            CriAtomEx.RegisterAcf (null, Application.streamingAssetsPath + "/" + acfFile);
#else
            CriAtomEx.RegisterAcf (null, acfFile);
#endif
        }

        if (m_players == null) {
            // playerの初期化
            m_players = new CriAtomExPlayer[m_playerMax];
            for (int i = 0; i < m_playerMax; ++i) {
                var player = m_players [i] = new CriAtomExPlayer ();
                player.ResetParameters ();
                player.AttachFader ();
                player.ResetFaderParameters ();
            }
        }

        if(m_soundCache == null) {
            m_soundCache = new SoundCache (6);  // キャッシュのキャパはCriFsのバインダー数に合わせる
        }
        if (m_bgmCache == null) {
            m_bgmCache = new CriAtomExAcb[2];
            m_bgmCacheCueName = new string[2];
        }

        this.VolumeBGM = AwsModule.LocalData.Volume_BGM;
        this.VolumeSE = AwsModule.LocalData.Volume_SE;
        this.VolumeVoice = AwsModule.LocalData.Volume_Voice;
    }

    /// <summary>
    /// BGM再生.
    /// </summary>
    public void PlayBGM (SoundClipName clip, bool bLoop = false, bool forcePlay = false)
    {
        var info = clip.ToSoundClipInfo ();
        if (info == null) {
            return;
        }
        var acb = LoadACB (info.CueSheetInfo, true);
        PlayBGM (acb, info.CueName, bLoop, forcePlay);
    }

    public void PlayBGM (string clipName, bool bLoop = false, bool forcePlay = false)
    {
        var info = MasterDataTable.cri_sound_clip [clipName];
        if (info == null) {
            return;
        }
        var acb = LoadACB (info.CueSheetInfo, true);
        PlayBGM (acb, info.CueName, bLoop, forcePlay);
    }

    public int PlayBGMReturnIndex (string clipName, bool bLoop = false, bool forcePlay = false)
    {
        var info = MasterDataTable.cri_sound_clip [clipName];
        if (info == null) {
            return -1;
        }
        var acb = LoadACB (info.CueSheetInfo, true);
        return PlayBGM (acb, info.CueName, bLoop, forcePlay);
    }

    /// <summary>
    /// BGM再生.
    /// </summary>
    private int PlayBGM (CriAtomExAcb acb, string cueName, bool bLoop = false, bool forcePlay = false, int fadeInTime = 500, int fadeOutTime = 500)
    {
        // BGMは同じCueNameであれば鳴らし直しをしない。
        if (m_bgmCueName == cueName && !forcePlay) {
            if (m_bgmPlayer != null && IsPlayPlayer(m_bgmPlayer)) {
                // 宴でボリューム制御されている可能性があるため
                m_bgmPlayer.SetVolume (1.0f);
                m_bgmPlayer.UpdateAll ();
            }
            return GetPlayerIndex (m_bgmPlayer);
        }

        StopBGM ();
        if (acb == null || string.IsNullOrEmpty (cueName)) {
            m_bgmPlayer = null;
            return -1;
        }
        m_bgmCueName = cueName;
        if (m_bgmPlayer == null) {
            m_bgmPlayer = GetPlayer (true);
            m_bgmPlayer.Loop (bLoop);
            m_bgmPlayer.SetFadeInTime (fadeInTime);
            m_bgmPlayer.SetFadeOutTime (fadeOutTime);
        }
        // 宴でボリューム制御されている可能性があるため
        m_bgmPlayer.SetVolume (1.0f);
        m_bgmPlayer.UpdateAll ();

        m_bgmPlayer.SetCue (acb, cueName);
        m_bgmPlayer.Start ();
        return GetPlayerIndex (m_bgmPlayer);
    }

    public void StopBGM ()
    {
        if (m_bgmPlayer != null) {
            if (IsPlayPlayer (m_bgmPlayer)) {
                m_bgmPlayer.Stop ();
            }
            m_bgmCueName = null;
        }
    }

    /// <summary>
    /// SE再生.
    /// </summary>
    public int PlaySE (SoundClipName clip, bool bLoop = false)
    {
        var info = clip.ToSoundClipInfo ();
        if (info == null)
            return -1;
        var acb = LoadACB (info.CueSheetInfo);
        return PlaySE (acb, info.CueName, bLoop);
    }
    public int PlaySE (string clipName, bool bLoop = false)
    {
        var info = MasterDataTable.cri_sound_clip [clipName];
        if (info == null)
            return -1;
        var acb = LoadACB (info.CueSheetInfo);
        return PlaySE (acb, info.CueName, bLoop);
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="acb">Acb.</param>
    /// <param name="cueName">Cue name.</param>
    private int PlaySE (CriAtomExAcb acb, string cueName, bool bLoop = false, int fadeInTime = 0, int fadeOutTime = 0)
    {
        if (acb == null || string.IsNullOrEmpty (cueName)) {
            return -1;
        }
        var sePlayer = GetPlayer ();
        if (sePlayer == null) {
            return -1;
        }
        sePlayer.Loop (bLoop);
        sePlayer.SetFadeInTime (fadeInTime);
        sePlayer.SetFadeOutTime (fadeOutTime);
        sePlayer.SetCue (acb, cueName);
        sePlayer.Start ();

        m_sePlayerList.Add (sePlayer);

        return GetPlayerIndex (sePlayer);
    }

    /// <summary>
    /// Stops the SE
    /// </summary>
    /// <param name="index">Player index.</param>
    public void StopSE (int index = -1)
    {
        // indexがマイナス値の時は全部止める。
        if (index < 0) {
            foreach (var player in m_sePlayerList) {
                player.Stop ();
            }
            m_sePlayerList.Clear ();
        } else {
            var player = GetPlayerFromIndex (index);
            if (player != null) {
                // すでにリムーブされている可能性があるのでリストに残っているか確認
                if (m_sePlayerList.Contains (player)) {
                    player.Stop ();
                    m_sePlayerList.Remove (player);
                }
            }
        }
    }

    /// <summary>
    /// Voice再生.
    /// </summary>
    public void PlayVoice (string sheetName, string cueName)
    {
        // TODO: あとで仕様を決めて実装する。
        var sheet = MasterDataTable.cri_sound_cue[sheetName];
        if(sheet == null) {
            return;
        }
        var acb = LoadACB(sheet);
        PlayVoice (acb, cueName);
    }

    public bool ContainsVoice (string sheetName, string cueName)
    {
        // TODO: あとで仕様を決めて実装する。
        var sheet = MasterDataTable.cri_sound_cue[sheetName];
        if(sheet == null) {
            return false;
        }
        var acb = LoadACB(sheet);
        if (acb == null) {
            return false;
        }
        return acb.Exists (cueName);
    }

    /// <summary>
    /// Voice再生
    /// </summary>
    /// <param name="acb">Acb.</param>
    /// <param name="cueName">Cue name.</param>
    private void PlayVoice (CriAtomExAcb acb, string cueName)
    {
        StopVoice ();
        if (acb == null || string.IsNullOrEmpty (cueName) || !acb.Exists(cueName)) {
            return;
        }
        m_voicePlayer = GetPlayer (true);
        m_voicePlayer.SetCue (acb, cueName);
        m_voicePlayer.Start ();
    }

    /// <summary>
    /// Voiceの停止
    /// </summary>
    public void StopVoice ()
    {
        if (m_voicePlayer != null) {
            m_voicePlayer.Stop ();
            m_voicePlayer = null;
        }
    }

    /// <summary>
    /// すべての音声の再生を止める
    /// </summary>
    public void StopAll ()
    {
        int maxCount = m_players.Length;
        for (int i = 0; i < maxCount; ++i) {
            var player = m_players [i];
            if (IsPlayPlayer (player)) {
                player.Stop ();
            }
        }

        m_bgmPlayer = null;
        m_bgmCueName = null;
        m_sePlayerList.Clear ();
        m_voicePlayer = null;
    }

    /// <summary>
    /// すべてのサウンドをPause状態にする
    /// </summary>
    public void Pause ()
    {
        if (m_players == null)
            return;

        int maxCount = m_players.Length;
        for (int i = 0; i < maxCount; ++i) {
            var player = m_players [i];
            if (IsPlayPlayer (player)) {
                player.Pause (true);
            }
        }
    }

    /// <summary>
    /// すべてのサウンドのPause状態を解除する
    /// </summary>
    public void Resume ()
    {
        if (m_players == null)
            return;

        int maxCount = m_players.Length;
        for (int i = 0; i < maxCount; ++i) {
            var player = m_players [i];
            if (player.IsPaused ()) {
                player.Pause (false);
            }
        }
    }

    /// <summary>
    /// 指定キューシートが存在するしたらCriAtomExAcbを返す
    /// </summary>
    /// <returns>ロード済みのCriAtomExAcbデータを返す。nullの時はまだロードされていない。</returns>
    /// <param name="cueSheetName">キューシート名</param>
    public CriAtomExAcb LoadedCueSheet (string cueSheetName)
    {
        return m_soundCache [cueSheetName];
    }

    /// <summary>
    /// キューシート名からAdx2データの読み込み
    /// </summary>
    /// <returns>The AC.</returns>
    /// <param name="cueSheetName">Cue sheet name.</param>
    public CriAtomExAcb LoadACB (string cueSheetName, bool loadBgm = false)
    {
        var cueSheetInfo = CriSoundClipNameHelper.GetCueSheetInfo (cueSheetName);
        if (cueSheetInfo == null || string.IsNullOrEmpty (cueSheetInfo.CueSheetName)) {
            Debug.LogErrorFormat ("cueSheetInfo not found or sheet name is empty sheet name = {0}", cueSheetName);
            return null;
        }
        return LoadACB (cueSheetInfo, loadBgm);
    }

    /// <summary>
    /// キューシート名からAdx2データの読み込み
    /// </summary>
    /// <returns>The AC.</returns>
    /// <param name="cueSheetName">Cue sheet name.</param>
    public bool DownloadedACB (string cueSheetName)
    {
        var cueSheetInfo = CriSoundClipNameHelper.GetCueSheetInfo (cueSheetName);
        if (cueSheetInfo == null || string.IsNullOrEmpty (cueSheetInfo.CueSheetName)) {
            Debug.LogErrorFormat ("cueSheetInfo not found or sheet name is empty sheet name = {0}", cueSheetName);
            return true;
        }
        bool needDownload = DLCManager.IsNeedDownload (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.acb", cueSheetInfo.ACBFileName)));
        if (!needDownload && !string.IsNullOrEmpty (cueSheetInfo.AWBFileName)) {
            needDownload |= DLCManager.IsNeedDownload (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.awb", cueSheetInfo.AWBFileName)));
        }
        return !needDownload;
    }

    /// 宴サウンド管理のための関数群
    /// 宴以外からはここの関数を直接呼ばないようにして欲しい。 

    /// <summary>
    /// 再生準備を行う
    /// </summary>
    /// <returns>プレイヤーの管理番号</returns>
    /// <param name="acb">Acb.</param>
    /// <param name="cueName">Cue name.</param>
    /// <param name="bLoop">If set to <c>true</c> b loop.</param>
    /// <param name="fadeInTime">Fade in time.</param>
    /// <param name="fadeOutTime">Fade out time.</param>
    public int SoundPrepare (CriAtomExAcb acb, string cueName, bool bLoop = false, bool bBindAnalyzer = false, int fadeInTime = 0, int fadeOutTime = 0)
    {
        var player = GetPlayer ();
        if (player != null) {
            int index = GetPlayerIndex (player);
            if (bBindAnalyzer) {
                var analyzer = CreateAnalyzer (index, player);
            }
            player.Loop (bLoop);
            player.SetFadeInTime (fadeInTime);
            player.SetFadeOutTime (fadeOutTime);
            player.SetCue (acb, cueName);
            return index;
        }
        return -1;
    }

    /// <summary>
    /// Playerの再生を行う
    /// </summary>
    /// <param name="index">Index.</param>
    public void StartFromIndex (int index)
    {
        var player = GetPlayerFromIndex (index);
        if (player != null) {
            player.Start ();
        }
    }

    /// <summary>
    /// Playerの停止を行う
    /// </summary>
    /// <param name="index">Index.</param>
    public void StopFromIndex (int index, bool immediate=false)
    {
        var player = GetPlayerFromIndex (index);
        if (player != null) {
            if (immediate) {
                player.StopWithoutReleaseTime ();
            } else {
                player.Stop ();
            }
            if (player == m_bgmPlayer) {
                m_bgmPlayer = null;
            } else if (player == m_voicePlayer) {
                m_voicePlayer = null;
            } else {
                m_sePlayerList.Remove (player);
            }
        }
    }

    /// <summary>
    /// Sets the volume.
    /// </summary>
    /// <param name="playerIndex">Player index.</param>
    /// <param name="volume">Volume.</param>
    public void SetVolumeFromIndex (int index, float volume)
    {
        var player = GetPlayerFromIndex (index);
        if (player != null) {
            player.SetVolume (volume);
            player.UpdateAll ();
        }
    }

    /// <summary>
    /// Determines whether this instance is play from index the specified index.
    /// </summary>
    /// <returns><c>true</c> if this instance is play from index the specified index; otherwise, <c>false</c>.</returns>
    /// <param name="index">Index.</param>
    public bool IsPlayFromIndex (int index)
    {
        var player = GetPlayerFromIndex (index);
        if (player != null) {
            return IsPlayPlayer (player);
        }
        return false;
    }

    /// <summary>
    /// Gets the index of the player status from.
    /// </summary>
    /// <returns>The player status from index.</returns>
    /// <param name="index">Index.</param>
    public CriAtomExPlayer.Status GetPlayerStatusFromIndex (int index)
    {
        var player = GetPlayerFromIndex (index);
        if (player != null) {
            return player.GetStatus ();
        }
        return CriAtomExPlayer.Status.Stop;
    }

    /// <summary>
    /// Gets the index of the analyzer from.
    /// </summary>
    /// <returns>The analyzer from index.</returns>
    /// <param name="index">Index.</param>
    public CriAtomExPlayerOutputAnalyzer GetAnalyzerFromIndex (int index)
    {
        CriAtomExPlayerOutputAnalyzer analyzer = null;
        m_analyzerDict.TryGetValue (index, out analyzer);
        return analyzer;
    }

    /// <summary>
    /// Removes the index of the analyzer from.
    /// </summary>
    /// <param name="index">Index.</param>
    public void RemoveAnalyzerFromIndex (int index)
    {
        CriAtomExPlayerOutputAnalyzer analyzer = null;
        if (m_analyzerDict.TryGetValue (index, out analyzer)) {
            analyzer.Dispose ();
            m_analyzerDict.Remove (index);
        }
    }

    // ACB取得共通処理.キャッシュから取得を試みる.
    private CriAtomExAcb LoadACB (CriCueSheet cueSheetInfo, bool loadBgm = false)
    {
        int index = Array.FindIndex (m_bgmCacheCueName, x => x == cueSheetInfo.CueSheetName);
        if (index >= 0) {
            return m_bgmCache [index];
        }
        if (loadBgm) {
            nowBgmPlayIndex = (nowBgmPlayIndex + 1) % 2;
            if (m_bgmCache [nowBgmPlayIndex] != null) {
                m_bgmCache [nowBgmPlayIndex].Dispose ();
            }
            m_bgmCacheCueName[nowBgmPlayIndex] = cueSheetInfo.CueSheetName;
            m_bgmCache [nowBgmPlayIndex] = CriAtomExAcb.LoadAcbFile (null, cueSheetInfo.AcbFilePath, cueSheetInfo.AwbFilePath);
            return m_bgmCache [nowBgmPlayIndex];
        } 
        return m_soundCache [cueSheetInfo.CueSheetName] ??
            (m_soundCache [cueSheetInfo.CueSheetName] = CriAtomExAcb.LoadAcbFile (null, cueSheetInfo.AcbFilePath, cueSheetInfo.AwbFilePath));
    }

    // 空いているPlayerを取得
    // Playerが見つかった場合はパラメーターはResetする。
    private CriAtomExPlayer GetPlayer (bool force = false)
    {
        var player = m_players.FirstOrDefault (x => !IsPlayPlayer (x) && x != m_bgmPlayer);
        if (player == null) {
            // 足りない時に勝手にプレイヤーを追加する。
            if (m_autoAddPlayerMissingTime) {
                Array.Resize (ref m_players, m_players.Length + 1);
                player = m_players [m_players.Length - 1] = new CriAtomExPlayer ();
                player.ResetParameters ();
                player.AttachFader ();
                player.ResetFaderParameters ();
            } else if (force) {
                if (m_sePlayerList.Count > 0) {
                    // 一番古いSEを止めてPlayerを開ける。
                    var sePlayer = m_sePlayerList.First ();
                    m_sePlayerList.Remove (sePlayer);
                    sePlayer.StopWithoutReleaseTime ();
                    player = sePlayer;
                } else if(m_voicePlayer != null) {
                    var voicePlayer = m_voicePlayer;
                    m_voicePlayer = null;
                    voicePlayer.StopWithoutReleaseTime ();
                    player = voicePlayer;                    
                }
            }
        }

        // parameterのリセットを行う
        if (player != null) {
            player.ResetParameters ();
            player.ResetFaderParameters ();

            // ボリュームをいじられたままになっている可能性があるので元に戻す。
            player.SetVolume (1.0f);
            player.UpdateAll ();
        }
        return player;
    }

    // Playerの配列インデックスを返す。
    // nullもしくは見つからない場合は-1を返す
    private int GetPlayerIndex (CriAtomExPlayer player)
    {
        if (player == null) {
            return -1;
        }

        for (int i = 0; i < m_playerMax; ++i) {
            if (m_players [i] == player) {
                return i;
            }
        }
        return -1;
    }

    // インデックスからPlayerを取得する。
    // 範囲チェックあり(範囲外ならnullを返す)
    private CriAtomExPlayer GetPlayerFromIndex (int index)
    {
        if (index >= 0 && index < m_playerMax) {
            return m_players [index];
        }
        return null;
    }

    // CriAtomExPlayerの再生状況を取得する。
    private bool IsPlayPlayer (CriAtomExPlayer player)
    {
        var status = player.GetStatus ();
        return status == CriAtomExPlayer.Status.Playing || status == CriAtomExPlayer.Status.Prep;
    }

    private CriAtomExPlayerOutputAnalyzer CreateAnalyzer (int index, CriAtomExPlayer player)
    {
        if (player == null) {
            return null;
        }

        CriAtomExPlayerOutputAnalyzer analyzer = null;
        if(m_analyzerDict.TryGetValue(index, out analyzer)) {
            return analyzer;
        }

        // rmsしか取得しないanalyzerを作成
        analyzer = new CriAtomExPlayerOutputAnalyzer (
             new CriAtomExPlayerOutputAnalyzer.Type[1] {
                CriAtomExPlayerOutputAnalyzer.Type.LevelMeter
            },
            new CriAtomExPlayerOutputAnalyzer.Config[1] {
                new CriAtomExPlayerOutputAnalyzer.Config (0)
            }
        );
        // 再生中にアタッチしようとする場合を考慮.
		if(!analyzer.AttachExPlayer(player)){
			var time = player.GetStatus() != CriAtomExPlayer.Status.PlayEnd ? player.GetTime() : 0;
			player.Stop();
			analyzer.AttachExPlayer(player);
			player.SetStartTime(time);
			player.Start();
		}

        m_analyzerDict.Add (index, analyzer);
        return analyzer;
    }

    public void Update ()
    {
    }

    public void LateUpdate ()
    {
        // 再生が終了したPlayerをSEの管理リストから削除する。
        foreach (var se in m_sePlayerList.Where(x => !IsPlayPlayer(x)).ToList()) {
            m_sePlayerList.Remove (se);
        }

        #if false
        if(m_players != null) {
            int i = 0;
            foreach (var player in m_players) {
                Debug.LogFormat("player index={5} State:{0}  Volume={1} BGM={2} VOICE={3} SE={4}", 
                    player.GetStatus (), player.GetParameterFloat32 (CriAtomEx.Parameter.Volume),
                    m_bgmPlayer == player, m_voicePlayer == player, m_sePlayerList.Contains(player), i++
                );
            }
        }
        #endif
    }


    public void DownloadResource (SoundClipName clip, System.Action didDownload = null)
    {
        var info = clip.ToSoundClipInfo();
        if (info == null) {
            didDownload ();
            return;
        }
        DownloadResource (info.CueSheetInfo, didDownload);
    }

    public void DownloadResource (string clipName, System.Action didDownload = null)
    {
        var info = MasterDataTable.cri_sound_clip [clipName];
        if (info == null) {
            didDownload ();
            return;
        }
        DownloadResource (info.CueSheetInfo, didDownload);
    }

    public void DownloadResourceFromSheetName (string sheetName, System.Action didDownload = null)
    {
        var info = MasterDataTable.cri_sound_cue [sheetName];
        if (info == null) {
            didDownload ();
            return;
        }
        DownloadResource (info, didDownload);
    }

    public void DownloadResource(CriCueSheet sheet, System.Action didDownload)
    {
        if (sheet.isStreamingAssets) {
            didDownload ();
            return;
        }

        List<string> needDownloadFiles = new List<string> ();
        needDownloadFiles.Add(DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Sound, string.Format("{0}.acb", sheet.ACBFileName)));
        if (!string.IsNullOrEmpty (sheet.AWBFileName)) {
            needDownloadFiles.Add(DLCManager.GetS3Path(DLCManager.DLC_FOLDER.Sound, string.Format("{0}.awb", sheet.AWBFileName)));
        }
        DLCManager.DownloadFiles (needDownloadFiles, (ret) => didDownload());
    }

    public void DownloadResource(string[] clipNames, System.Action didDownload)
    {
        HashSet<CriCueSheet> sheets = new HashSet<CriCueSheet> ();
        foreach (var clipName in clipNames) {
            var info = MasterDataTable.cri_sound_clip [clipName];
            if (info == null) {
                continue;
            }
            sheets.Add (info.CueSheetInfo);
        }
        if (sheets.Count <= 0) {
            didDownload ();
            return;
        }
        DownloadResource (sheets, didDownload);
    }

    public void DownloadResource(HashSet<CriCueSheet> sheets, System.Action didDownload)
    {
        List<string> needDownloadFiles = new List<string> ();
        foreach (var sheet in sheets) {
            if (sheet.isStreamingAssets) {
                didDownload ();
                return;
            }
                
            needDownloadFiles.Add (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.acb", sheet.ACBFileName)));
            if (!string.IsNullOrEmpty (sheet.AWBFileName)) {
                needDownloadFiles.Add (DLCManager.GetS3Path (DLCManager.DLC_FOLDER.Sound, string.Format ("{0}.awb", sheet.AWBFileName)));
            }
        }
        DLCManager.DownloadFiles (needDownloadFiles, (ret) => didDownload());
    }

    // private class : サウンドLRUキャッシュ.
    private class SoundCache : LRUCache<string, CriAtomExAcb>
    {
        
        public SoundCache (uint capacity) : base (capacity)
        {
        }

        public override bool IsExsits (string key)
        {
            return this [key] != null;
        }

        protected override void DisposeItem (CriAtomExAcb cacheItem)
        {
            if (cacheItem == null) {
                return;
            }
            if (CriWareInitializer.IsInitialized ()) {
                cacheItem.Dispose ();
            }
        }
    }
}
