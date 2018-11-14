using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using TMPro;

using SmileLab;


/// <summary>
/// 宴操作用モジュール.
/// </summary>
public class UtageModule : ViewBase
{
    /// <summary>
    /// 共通インスタンス
    /// </summary>
    public static UtageModule SharedInstance { get; private set; }

    /// <summary>
    /// エンジン側のカスタムインプット.
    /// </summary>
	public bool IsCustomInput 
	{
		get {
			if(m_core == null){
				return false;
			}
			if(m_core.Engine == null){
				return false;
			}
			return m_core.Engine.UiManager.IsInputTrigCustom;
		}
		set {
			if (m_core != null && m_core.Engine != null) {
				m_core.Engine.UiManager.IsInputTrigCustom = value;
            }
		}
	}   

    /// <summary>
    /// スキップボタンの表示/非表示.
    /// </summary>
	public bool IsHideSkipButton 
	{
		get {
			return m_core == null || m_core.IsHideSkipButton;
		}	
		set {
			m_core.IsHideSkipButton = value;         
		}
	}
    public bool IsSkip
	{
		get {
			return m_core != null ? m_core.Engine.Config.IsSkip : false;
		}
		set {
			if(m_core != null){
				m_core.Engine.Config.IsSkip = value;
				m_core.GetScript<Canvas>("View_SkipFade").gameObject.SetActive(false);
			}

		}
	}

	/// <summary>
	/// オートボタンの表示/非表示
	/// </summary>
	public bool IsHideAutoButton
	{
		get {
			return m_core == null || m_core.IsHideAutoButton;
		}
		set {
			if (m_core != null) {
				//TODO; 一時的に隠すだけであれば一旦オートフラグをどこかにキープする必要があるかもしれない
				m_core.IsHideAutoButton = value;
			}
		}
	}


	/// <summary>
	/// 章分けを使用したシナリオロード.
	/// </summary>
	public void LoadUseChapter(string projectName, Action didLoad, params string[] chapters)
    {
        m_core = AdvCore.CreateOrGet(projectName, chapters);
		m_core.LoadScenario(didLoad, true);      
    }
	/// <summary>
    /// 章分けを使用したシナリオロード.プログレス表示あり/なし指定.
    /// </summary>
	public void LoadUseChapter(string projectName, Action didLoad, bool bShowProgress, params string[] chapters)
    {
        m_core = AdvCore.CreateOrGet(projectName, chapters);
        m_core.LoadScenario(didLoad, bShowProgress);
    }

    public void SetCoreDontDestroy()
	{
		if(m_core == null){
			return;
		}
		GameObject.DontDestroyOnLoad(m_core.gameObject);
	}
    public void DestroyCore()
	{
		if (m_core == null) {
            return;
        }
		GameObject.Destroy(m_core.gameObject);
	}

    /// <summary>
    /// シナリオ再生開始.
    /// </summary>
    public void StartScenario(string labelStart, Action didEnd, bool bDisposeCache = false)
    {
		if(m_core == null){
			return;
		} 
		if(m_core.Engine.IsLoading){
			return;
		}
		//オートボタンの表示(チュート以外)とオート設定の引き継ぎ
		this.IsHideAutoButton = !(AwsModule.ProgressData.TutorialStageNum == -1);
		m_core.Engine.Config.IsAutoBrPage = !this.IsHideAutoButton ? AwsModule.LocalData.Scenario_Auto : false;
		this.IsCustomInput = false;
        m_core.StartScenario(labelStart, () => {
			if(bDisposeCache){
				this.ClearCache();
			}
            if(didEnd != null){
                didEnd();
            }
        });
        this.StartPollingEngine();  // エンジン側とのポーリングを開始.
    }

    /// <summary>
    /// 自己紹介再生.
    /// </summary>
	public void StartIntro(string labelStart, string cv, Action didEnd, bool bDisposeCache = false)
	{
		if (m_core == null) {
            return;
        }
        if (m_core.Engine.IsLoading) {
            return;
        }
		//オートボタンの非表示とオートをオフに設定
		this.IsHideAutoButton = true;
		m_core.Engine.Config.IsAutoBrPage = false;
		this.IsCustomInput = false;
		m_core.StartIntro(labelStart, cv, () => {
            if (bDisposeCache) {
                this.ClearCache();
            }
            if (didEnd != null) {
                didEnd();
            }
        });
        this.StartPollingEngine();  // エンジン側とのポーリングを開始.
	}

    /// <summary>
    /// キャッシュクリア.可能であれば.
    /// </summary>
    public void ClearCache()
    {
        if(m_core == null || m_core.IsDestroyed){
            return;
        }
		if(m_core.Engine != null){
			m_core.Engine.ClearOnEnd();
			m_core.Engine.ScenarioPlayer.Clear();
			m_core.Engine.DataManager.ClearCache();
			m_core.Engine.Page.Clear();
			m_core.Engine.ClearCustomCommand();
		}
		var afMng = AssetFileManager.GetInstance();
		if(afMng != null){
			afMng.ClearCurrentSetting();
		}
		var abInfoMng = m_core.GetScript<AssetBundleInfoManager>("FileManager");
        if (abInfoMng != null) {
            abInfoMng.DeleteAllCache();
        }
    }

    /// <summary>
    /// 指定ラベル先にジャンプ.
    /// </summary>
    public void JumpLabel(string label)
	{
		if (m_core == null || m_core.IsDestroyed || m_core.Engine == null) {
            return;
        }
		m_core.Engine.JumpScenario(label);
	}

    /// <summary>
    /// 指定パラメータに値を設定する.
    /// </summary>
	public void SetAdvParam(string key, object parameter)
	{
		if (m_core == null || m_core.IsDestroyed || m_core.Engine == null) {
            return;
        }
		m_core.Engine.Param.TrySetParameter(key, parameter);
	}

    /// <summary>
    /// アドベンチャーコアの表示/非表示.
    /// </summary>
    public void SetActiveCore(bool bActive)
	{
		if (m_core == null) {
            return;
        }
		if(m_core.Engine == null){
			return;
		}
		foreach(var cam in m_core.Engine.CameraManager.GetComponentsInChildren<Camera>(true)){
			cam.enabled = bActive;
		}
	}

    private void OnEnable()
    {
        if (m_core == null || m_core.Engine == null || !m_core.Engine.isActiveAndEnabled) {
            return;
        }
        this.StartPollingEngine();
    }
    // UI類など宴側のエンジンの状態と同期する必要がある処理はここでポーリングしておく.
    private void StartPollingEngine()
    {
        if (m_enginePollingRoutine != null) {
            this.StopCoroutine(m_enginePollingRoutine);
        }
        m_enginePollingRoutine = this.StartCoroutine(this.PollingUtageEngineState());
    }
    IEnumerator PollingUtageEngineState()
    {
        while(m_core != null && m_core.Engine != null && m_core.Engine.isActiveAndEnabled){
            // 独自UIのOn/Off設定.
			var bActiveEngine = m_core.Engine.UiManager.IsShowingMenuButton && m_core.Engine.UiManager.Status == AdvUiManager.UiStatus.Default;
            if (m_bPrevEngineState != bActiveEngine) {
				m_core.SetActiveOriginalUI(bActiveEngine);
                m_bPrevEngineState = bActiveEngine;
            }
            yield return null;
        }
    }
    private Coroutine m_enginePollingRoutine;
    private bool m_bPrevEngineState = false;

    private void Awake()
    {
        if (SharedInstance != null) {
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }

    private AdvCore m_core;



    #region internal.

    // private class : アドベンチャーコア.
    private class AdvCore : ViewBase
    {
        /// <summary>エンジン.</summary>
        public AdvEngine Engine { get; private set; }

        /// <summary>スキップボタン隠す？</summary>
		public bool IsHideSkipButton { get; set; }

		/// <summary>オートボタン隠す？</summary>
		public bool IsHideAutoButton { get; set; }

        /// <summary>生成の必要性があれば.すでに生成済みのものがあればそこから取ってくる.</summary>
        public static AdvCore CreateOrGet(string projectName, params string[] chapters)
        {
            if (instance == null || instance.IsDestroyed) {
                var go = GameObjectEx.LoadAndCreateObject("_Common/UtageAdvCore");
                instance = go.GetOrAddComponent<AdvCore>();
            }
			instance.InitInternal(projectName, chapters);
            return instance;
        }
        private static AdvCore instance;
        private void InitInternal(string projectName, params string[] chapters)
        {
			// 既にあればリロード.
			if(Engine != null){
				Engine.EndScenario();
				Engine.ClearOnStart();
				m_starter.Scenarios = null;
			}
            // スターターの設定.
            m_starter = instance.GetComponentInChildren<AdvEngineStarter>(true);
            m_starter.Strage = AdvEngineStarter.StrageType.Server;
            m_starter.RootResourceDir = projectName;
            m_starter.ServerUrl = DLCManager.URL_ASSET_UTAGE;
            m_starter.UseChapter = chapters.Length > 0;
            if (m_starter.UseChapter) {
                foreach (var c in chapters) {
                    if (m_starter.ChapterNames.Contains(c)) {
						continue;
                    }
					m_starter.ChapterNames.Add(projectName+"_"+c);
                }
            }
            // エンジン設定.
            Engine = instance.GetComponentInChildren<AdvEngine>(true);
			// 独自UI
            m_listOtherUiObj.Add(GetScript<Button>("SkipButton").gameObject);
			m_listOtherUiObj.Add(GetScript<Button>("AutoButton").gameObject);

            // ボタン
            this.SetCanvasButtonMsg("SkipButton", DidTapSkip);
			this.SetCanvasButtonMsg("AutoButton", DidTapAuto);
        }

        /// <summary>
        /// シナリオ非同期読み.
        /// </summary>
		public void LoadScenario(Action didLoad, bool bShowProgress = true)
        {
			this.StartCoroutine(LoadAsyncScenario(didLoad, bShowProgress));
        }
		private IEnumerator LoadAsyncScenario(Action didLoad, bool bShowProgress = true)
        {         
			this.StartCoroutine(m_starter.LoadEngineAsync(() => {
				Debug.LogError("[UtageModule] LoadError!!");
				PopupManager.OpenPopupSystemOK("通信エラー。\n再起動します。", () => ScreenChanger.SharedInstance.Reboot());
			}));

            yield return null;
   
			int countDownLoading = 0;
            int maxCountDownLoad = 0;
            do {
                countDownLoading = AssetFileManager.CountDownloading();
                maxCountDownLoad = Mathf.Max(maxCountDownLoad, countDownLoading);
				if (bShowProgress && maxCountDownLoad > 0) {
                    View_FadePanel.SharedInstance.SetProgress(1.0f * (maxCountDownLoad - countDownLoading) / maxCountDownLoad);
                }
                yield return null;
            } while (Engine.IsLoading);
			View_FadePanel.SharedInstance.DeativeProgress();

            if (didLoad != null) {
                didLoad();
            }
        }

        /// <summary>
        /// シナリオの再生開始.
        /// </summary>
        public void StartScenario(string startLabel = "", Action didEnd = null)
        {
            // 未読もスキップ可能に.
            Engine.Config.IsSkipUnread = Engine.Config.IsStopSkipInSelection = true;

            // 何かしらのラベルを指定しているものの見つからないラベルだった場合は当然エラー.
            if (!string.IsNullOrEmpty(startLabel) && Engine.DataManager.FindScenarioLabelData(startLabel) == null) {
                Debug.LogError("[UtageModule] StartScenario Error!! : label=" + startLabel + " is not found.");
				PopupManager.OpenPopupSystemOK("データに不整合がありました。\nタイトルに戻ります。", () => ScreenChanger.SharedInstance.Reboot());
                return;
            }         
            // シナリオ開始.
            this.StartCoroutine(PlayScenarioProc(startLabel, didEnd));
        }
        /// <summary>
        /// 自己紹介開始.
        /// </summary>
        public void StartIntro(string startLabel = "", string cv = "", Action didEnd = null)
		{
			this.GetScript<TextMeshProUGUI>("txt_CharacterVoiceName").gameObject.SetActive(!string.IsNullOrEmpty(cv));
			this.GetScript<TextMeshProUGUI>("txt_CharacterVoiceName").SetText(cv);
			this.StartScenario(startLabel, didEnd);
		}

        private IEnumerator PlayScenarioProc(string startLabel, Action didEnd)
        {
			LockInputManager.SharedInstance.IsLock = true;
			yield return new WaitForEndOfFrame();   // フェード開け直後にシナリオを開始しようとするとFadeInの処理とFadeOutの処理がぶつかって正しく作用しないことがあるため確実にフレーム終わりまで待機する.
			LockInputManager.SharedInstance.IsLock = false;
			View_FadePanel.SharedInstance.FadeOutAnimAndWithoutTips(View_FadePanel.FadeColor.Black, null, true);         

            if (!string.IsNullOrEmpty(startLabel)) {
                Engine.JumpScenario(startLabel);
            } else {
                m_starter.StartEngine();
            }

            // 初期化（シナリオのDLなど）を待つ
            while (Engine.IsWaitBootLoading) yield return null;
            while (!Engine.IsStarted) yield return null;
            while (Engine.IsLoading) yield return null;
			var bFading = true;
			View_FadePanel.SharedInstance.FadeIn(View_FadePanel.FadeColor.Black, () => {
			this.GetScript<Button>("SkipButton").gameObject.SetActive(!IsHideSkipButton);
			//オートボタン表示設定とオート状態表示の設定
			this.GetScript<Button>("AutoButton").gameObject.SetActive(!IsHideAutoButton);
			this.GetScript<Transform> ("AutoActive").gameObject.SetActive (!IsHideAutoButton && AwsModule.LocalData.Scenario_Auto);
				bFading = false;
			});

            // シナリオの終わり を待つ
			while (!Engine.IsEndScenario || bFading) {
                yield return null;
            }
            // ボイスは全部停止.
			Utage.SoundManager.GetInstance().System.StopGroup(Utage.SoundManager.IdVoice, 0);
            if (didEnd != null) {
                didEnd();
            }
        }

        /// <summary>
        /// オリジナルUIのアクティブ設定.
        /// </summary>
        public void SetActiveOriginalUI(bool bActive)
		{
			if(m_listOtherUiObj == null || m_listOtherUiObj.Count <= 0){
				return;
			}
			m_listOtherUiObj.Where(ui => ui != null)
			                .Select(ui => { ui.SetActive(bActive); return true; })
			                .ToList();
			// 例外.
			if(IsHideSkipButton){
				this.GetScript<Button>("SkipButton").gameObject.SetActive(false);
			}
			if (IsHideAutoButton) {
				this.GetScript<Button>("AutoButton").gameObject.SetActive(false);
			}
		}

        #region ButtonDelegate

        // ボタン : スキップ押下.
        void DidTapSkip()
        {
            if (Engine.SelectionManager.IsWaitInput) {
                return;
            }
            // バックログの状態の場合は先に進めないようにする。
            if (Engine.UiManager.Status == AdvUiManager.UiStatus.Backlog) {
                return;
            }
            Engine.Config.IsSkip = true;
            this.PlaySkipFade(true, () => {
                this.StartCoroutine(this.SkipToSelectionWait());
            });
        }
        IEnumerator SkipToSelectionWait()
        {
            do {
                if (Engine.IsEndScenario) {
                    break;
                }
                if (Engine.SelectionManager.IsWaitInput) {
                    break;
                }
                yield return null;
            } while (true);
            Engine.Config.IsSkip = false;
            this.PlaySkipFade(false);
        }
        // スキップ専用フェード再生.
        private void PlaySkipFade(bool bIn, Action didEnd = null)
        {
            if (m_fadeRoutine != null) {
                this.StopCoroutine(m_fadeRoutine);
            }
            m_fadeRoutine = this.StartCoroutine(PlaySkipFadeProc(bIn, didEnd));
        }
        private IEnumerator PlaySkipFadeProc(bool bIn, Action didEnd)
        {
            this.GetScript<Canvas>("View_SkipFade").gameObject.SetActive(true);
            var animName = bIn ? "AdvSkipIn" : "AdvSkipOut";
            var anim = this.GetScript<Animation>("View_SkipFade/AnimParts");
            yield return null;
            anim.Play(animName);
            do {
                yield return null;
            } while (anim.isPlaying);
            if (didEnd != null) {
                didEnd();
            }
            if (!bIn) {
                this.GetScript<Canvas>("View_SkipFade").gameObject.SetActive(false);
            }
        }
        private Coroutine m_fadeRoutine;

		// ボタン : オート押下
		void DidTapAuto() {
			if (Engine.SelectionManager.IsWaitInput) {
				return;
			}
			Engine.Config.ToggleAuto ();
			this.GetScript<Transform> ("AutoActive").gameObject.SetActive (Engine.Config.IsAutoBrPage);
		}
        #endregion

        private void Start()
        {
            this.GetScript<Canvas>("View_SkipFade").gameObject.AttachUguiRootComponent(CameraHelper.SharedInstance.FadeCamera, 0f);
        }

        private AdvEngineStarter m_starter;
		private static List<GameObject> m_listOtherUiObj = new List<GameObject>(); // 宴エンジンで管理していない、独自管理のUIオブジェクトリスト.
    }

    #endregion
}

