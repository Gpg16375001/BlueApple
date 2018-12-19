using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class UseLocalDataSettingWindow : EditorWindow
{

    [MenuItem ("Window/SmileLab/Use Local Data Setting")]
    public static void ShowWindow ()
    {
        var wnd = GetWindow<UseLocalDataSettingWindow> ();

        wnd.minSize = new Vector2 (800, 300);
        wnd.Show ();
    }

    public static string DLCPath {
        get {
            return EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_LocalDLCPath");
        }
    }

    private string LocalDLCPath;
	private string UnityEditorPath;
    private int SelectDLCType = 0;
	private Dictionary<DLCManager.DLC_FOLDER, bool> UseDlcTypes = new Dictionary<DLCManager.DLC_FOLDER, bool>();

	private string NamePathPlatform 
	{ 
		get {
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
				return "iOS";
			} else if(Application.platform == RuntimePlatform.Android) {
				return "Android";
			} else if(Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
				return "OSX";
			} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				return "Windows";
			} else if(Application.platform == RuntimePlatform.WebGLPlayer) {
				return "WebGL";
			}
			return "";
		} 
	}

    public void OnEnable()
    {
        LocalDLCPath = EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_LocalDLCPath");
		UnityEditorPath = EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_UnityEditorPath");
		if (!int.TryParse (EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_SelectDLCType"), out SelectDLCType)) {
			SelectDLCType = 0;
		}
		var folders = EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_UseFolders");
		foreach (var obj in System.Enum.GetValues (typeof(DLCManager.DLC_FOLDER))) {
			DLCManager.DLC_FOLDER folder = (DLCManager.DLC_FOLDER)obj;
			if (folder == DLCManager.DLC_FOLDER.MAX) {
				continue;
			}
			if (!UseDlcTypes.ContainsKey (folder)) {
				UseDlcTypes.Add (folder, false);
			}
			UseDlcTypes [folder] = folders != null && folders.Contains (folder.ToString ());
		}
    }

    public void OnGUI ()
    {
		GUILayout.Space (10);

		EditorGUILayout.BeginVertical(GUI.skin.box);
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("ローカルデータロード設定");
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (10);
			EditorGUILayout.BeginHorizontal ();
			{
				GUILayout.Label ("glare-assets UnityProject Path:");
				string localDLCPath = GUILayout.TextField (LocalDLCPath);
				if (GUILayout.Button ("...")) {
					localDLCPath = EditorUtility.OpenFolderPanel ("glare-assets Project Path", LocalDLCPath, "");
				}
				if (localDLCPath != LocalDLCPath && !string.IsNullOrEmpty (localDLCPath)) {
					EditorUserSettings.SetConfigValue ("UseLocalDataSettingWindow_LocalDLCPath", localDLCPath);
					LocalDLCPath = localDLCPath;
				}
			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (10);

			EditorGUI.BeginChangeCheck ();
			{
				SelectDLCType = GUILayout.SelectionGrid (SelectDLCType, new string[2] {
					"使用しない",
					"ローカルを使用"
				}, 2);
			}
			var selectDLCTypeChange = EditorGUI.EndChangeCheck ();

			if (selectDLCTypeChange) {
				if (SelectDLCType == 0) {
					var symbols = "DEFINE_DEVELOP;";
					PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
				} else if (SelectDLCType == 1) {
					var symbols = "DEFINE_DEVELOP;USE_LOCAL_DATA;";
					PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
				}
				EditorUserSettings.SetConfigValue ("UseLocalDataSettingWindow_SelectDLCType", SelectDLCType.ToString ());
			}
			if (SelectDLCType == 1) {
				bool allChange = false;
				EditorGUILayout.BeginHorizontal ();
				{
					if(GUILayout.Button ("全てチェック")) {
						foreach (var obj in System.Enum.GetValues (typeof(DLCManager.DLC_FOLDER))) {
							DLCManager.DLC_FOLDER folder = (DLCManager.DLC_FOLDER)obj;
							if (folder == DLCManager.DLC_FOLDER.MAX) {
								continue;
							}
							UseDlcTypes [folder] = true;
							allChange = true;
						}
					}
					if(GUILayout.Button ("全て外す")) {
						foreach (var obj in System.Enum.GetValues (typeof(DLCManager.DLC_FOLDER))) {
							DLCManager.DLC_FOLDER folder = (DLCManager.DLC_FOLDER)obj;
							if (folder == DLCManager.DLC_FOLDER.MAX) {
								continue;
							}
							UseDlcTypes [folder] = false;
							allChange = true;
						}

					}
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUI.BeginChangeCheck ();
				foreach (var obj in System.Enum.GetValues (typeof(DLCManager.DLC_FOLDER))) {
					DLCManager.DLC_FOLDER folder = (DLCManager.DLC_FOLDER)obj;
					if (folder == DLCManager.DLC_FOLDER.MAX) {
						continue;
					}
					UseDlcTypes [folder] = GUILayout.Toggle (UseDlcTypes [folder], folder.ToString ());
				}
				var useDlcTypesChange = EditorGUI.EndChangeCheck ();
				if (useDlcTypesChange || allChange) {
					EditorUserSettings.SetConfigValue ("UseLocalDataSettingWindow_UseFolders", string.Join(":", UseDlcTypes.Where(x => x.Value).Select(x => x.Key.ToString()).ToArray()));
				}

				foreach (var kv in UseDlcTypes) {
					if (kv.Value) {
						if (kv.Key == DLCManager.DLC_FOLDER.Sound) {
							if (!System.IO.Directory.Exists (string.Format ("{0}/Assets/StreamingAssets/_Export{1}/{2}", LocalDLCPath, kv.Key.ToString (), NamePathPlatform))) {
								EditorGUILayout.HelpBox (string.Format ("{0}のAssetBundleを作成してください。", kv.Key.ToString ()), MessageType.Error);
							}
						} else {
							if (!System.IO.Directory.Exists (string.Format ("{0}/AssetBundle/_Export{1}/{2}", LocalDLCPath, kv.Key.ToString (), NamePathPlatform))) {
								EditorGUILayout.HelpBox (string.Format ("{0}のAssetBundleを作成してください。", kv.Key.ToString ()), MessageType.Error);
							}
						}
					}
				}
			}
			GUILayout.Space (10);
		}
		EditorGUILayout.EndVertical();

		GUILayout.Space (10);

		EditorGUILayout.BeginVertical(GUI.skin.box);
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("AssetBundle作成関連");
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (10);

            if (isRunning) {
                EditorGUILayout.HelpBox ("ビルド中です。Windowを閉じないでください。", MessageType.Info);
            } else {
    			EditorGUILayout.BeginHorizontal ();
    			{
    				GUILayout.Label ("UnityEditor Path:");
    				string unityEditorPath = GUILayout.TextField (UnityEditorPath);

    				if (GUILayout.Button ("...")) {
#if UNITY_EDITOR_OSX
    					unityEditorPath = EditorUtility.OpenFilePanel ("UnityEditor Path", UnityEditorPath, "app");
#elif UNITY_EDITOR_WIN
    				unityEditorPath = EditorUtility.OpenFilePanel ("UnityEditor Path", UnityEditorPath, "exe");
#endif
    				}
    				if (unityEditorPath != UnityEditorPath && !string.IsNullOrEmpty (unityEditorPath)) {
    					EditorUserSettings.SetConfigValue ("UseLocalDataSettingWindow_UnityEditorPath", unityEditorPath);
    					UnityEditorPath = unityEditorPath;
    				}
    			}
    			EditorGUILayout.EndHorizontal ();

    			GUILayout.Space (10);

#if UNITY_EDITOR_OSX
    			if (!System.IO.Directory.Exists (UnityEditorPath)) {
#elif UNITY_EDITOR_WIN
    			if (!System.IO.File.Exists (UnityEditorPath)) {
#endif
    				EditorGUILayout.HelpBox ("UnityEditorのパスを設定してください。設定後AssetBundleの作成メニューを選択できます。", MessageType.Error);
    			} else if (!System.IO.Directory.Exists (LocalDLCPath)) {
    				EditorGUILayout.HelpBox ("glare-assetsのプロジェクトのパスを設定してください。設定後AssetBundleの作成メニューを選択できます。", MessageType.Error);
    			} else {
    				if (GUILayout.Button ("Create Master AssetBundle")) {
    					CreateAssetBundle ("Master");
    				}
    				if (GUILayout.Button ("Create ALL AssetBundle")) {
    					CreateAssetBundle ("ALL");
    				}
					if (GUILayout.Button ("Create Utage AssetBundle")) {
						CreateAssetBundle ("Utage");
					}
    			}
            }

			GUILayout.Space (10);
		}
		EditorGUILayout.EndVertical();
    }

	private void CreateAssetBundle(string parameter)
	{
#if UNITY_EDITOR_OSX
		if (parameter == "Master") {
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UnityEngine.AssetBundles.GraphTool.CUIUtility.BuildFromCommandline -target StandaloneOSXUniversal -collection Master", LocalDLCPath));
		} else if(parameter == "ALL") {
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UnityEngine.AssetBundles.GraphTool.CUIUtility.BuildFromCommandline -target StandaloneOSXUniversal -collection ALL", LocalDLCPath));
		} else if(parameter == "Utage") {
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UtageBuildHelper.BuildProject -target OSX", LocalDLCPath));
		}
#elif UNITY_EDITOR_WIN
		if (parameter == "Master") {
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UnityEngine.AssetBundles.GraphTool.CUIUtility.BuildFromCommandline -target StandaloneWindows64 -collection Master", LocalDLCPath));
		} else {
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UnityEngine.AssetBundles.GraphTool.CUIUtility.BuildFromCommandline -target StandaloneWindows64 -collection ALL", LocalDLCPath));
			ExecProcess (string.Format("-batchmode -quit -projectPath {0} -executeMethod UtageBuildHelper.BuildProject -target Windows", LocalDLCPath));
		}
#endif
	}

    private static Thread _thread;
    private static bool isRunning = false;
    private static Queue<string> processQueue = new Queue<string> ();
	private void ExecProcess(string arguments)
    {
        
        processQueue.Enqueue (arguments);
        if (!isRunning) {
            isRunning = true;

            _thread = new Thread (ThreadRun);
            _thread.IsBackground = true;
            _thread.Start();
        }
    }

    private void ThreadRun()
    {
        while (processQueue.Count > 0) {
            CoExecProcess (processQueue.Dequeue ());
        }
        isRunning = false;
    }

    private void CoExecProcess(string arguments)
	{
        isRunning = true;
		System.Diagnostics.Process process = new System.Diagnostics.Process ();

#if UNITY_EDITOR_OSX
		process.StartInfo.FileName = string.Format("{0}/Contents/MacOS/Unity", UnityEditorPath);
#elif UNITY_EDITOR_WIN
		process.StartInfo.FileName = UnityEditorPath;
#endif
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
		process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ErrorOutputHanlder);
		process.StartInfo.RedirectStandardInput = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.Arguments = arguments;
		process.EnableRaisingEvents = true;
		
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();

        process.WaitForExit ();
	}

	// 標準出力時.
	private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs args)
	{
		if (!string.IsNullOrEmpty(args.Data))
		{
			Debug.Log(args.Data);
		}
	}

	// エラー出力時.
	private void ErrorOutputHanlder(object sender, System.Diagnostics.DataReceivedEventArgs args)
	{
		if (!string.IsNullOrEmpty(args.Data))
		{
			Debug.Log(args.Data);
		}
	}
}
