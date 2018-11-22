using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

/// <summary>
/// エディタ拡張：バッチビルド周り.
/// </summary>
public class BatchBuild
{
	/// <summary>
	/// ビルド時のiOSプロビショニング.
    /// </summary>
	public static string ProvisioningAtBuildTime { get; private set; }

    /// <summary>
    /// ビルドがリリースビルドかどうか.
    /// </summary>
	public static bool IsReleaseBuild { get; private set; }


	/// <summary>
	/// Androidビルド. 開発用.
	/// </summary>
	[MenuItem("Build/Android/ROM Android_Dev")]
	public static void AndroidDev()
	{
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_DEVELOP;");
		PlayerSettings.productName = "DevSeven";
		PlayerSettings.bundleVersion = "0.0.1";
		PlayerSettings.Android.bundleVersionCode = 100000100;
		PlayerSettings.applicationIdentifier = "com.smilelab.devseven";
        var buildOption = BuildOptions.Development;
        var enableProfiler = System.Environment.GetEnvironmentVariable ("ENABLE_PROFILER");
        Debug.Log ("enableProfiler = " + enableProfiler);
        if (enableProfiler == "true") {
            buildOption |= BuildOptions.ConnectWithProfiler;
        }

        // URL Schemaの書き換えを行う
        var manifestPath = System.IO.Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        var manifest = System.IO.File.ReadAllText(manifestPath);
        manifest = manifest.Replace ("<data android:scheme=\"com.smilelab.seven\" />", "<data android:scheme=\"com.smilelab.devseven\" />");
        System.IO.File.WriteAllText (manifestPath, manifest);

        Build(BuildTargetGroup.Android, BuildTarget.Android, buildOption);
	}
    /// <summary>
    /// Androidビルド. ベータ版.
    /// </summary>
    [MenuItem("Build/Android/ROM Android_β")]
    public static void AndroidBeta()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_BETA;");
		PlayerSettings.productName = "Seven";   // TODO : 仮タイトル
		PlayerSettings.bundleVersion = "0.1.0";
		PlayerSettings.Android.bundleVersionCode = 100010000;
		PlayerSettings.applicationIdentifier = "com.smilelab.seven";      
        Build(BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }
    /// <summary>
    /// Androidビルド 本番用.
    /// </summary>
    [MenuItem("Build/Android/ROM Android_Release")]
    public static void Android()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_RELEASE;");
		PlayerSettings.productName = "プレカトゥス";   // TODO : 仮タイトル
		PlayerSettings.bundleVersion = "1.0.5";
        PlayerSettings.Android.bundleVersionCode = 101000500;
		PlayerSettings.applicationIdentifier = "jp.fg.precatus";

        // URL Schemaの書き換えを行う
        var manifestPath = System.IO.Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        var manifest = System.IO.File.ReadAllText(manifestPath);
        manifest = manifest.Replace ("<data android:scheme=\"com.smilelab.seven\" />", "<data android:scheme=\"com.smilelab.devseven\" />");
        System.IO.File.WriteAllText (manifestPath, manifest);

        Build(BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }

	/// <summary>
    /// AuMarketビルド. 開発用.
    /// </summary>
	[MenuItem("Build/Android/ROM AuMarket_Dev")]
	public static void AuMarketDev()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_DEVELOP;");
        PlayerSettings.productName = "DevSeven";
        PlayerSettings.bundleVersion = "0.0.1";
		PlayerSettings.Android.bundleVersionCode = 200000100;
        PlayerSettings.applicationIdentifier = "com.smilelab.devseven";
        var buildOption = BuildOptions.Development;
        var enableProfiler = System.Environment.GetEnvironmentVariable("ENABLE_PROFILER");
        Debug.Log("enableProfiler = " + enableProfiler);
        if (enableProfiler == "true") {
            buildOption |= BuildOptions.ConnectWithProfiler;
        }

        // URL Schemaの書き換えを行う
        var manifestPath = System.IO.Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
        var manifest = System.IO.File.ReadAllText(manifestPath);
        manifest = manifest.Replace ("<data android:scheme=\"com.smilelab.seven\" />", "<data android:scheme=\"com.smilelab.devseven\" />");
        System.IO.File.WriteAllText (manifestPath, manifest);

        Build(BuildTargetGroup.Android, BuildTarget.Android, buildOption);
    }
    /// <summary>
	/// AuMarketビルド. ベータ版.
    /// </summary>
	[MenuItem("Build/Android/ROM AuMarket_β")]
	public static void AuMarketBeta()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_BETA;");
		PlayerSettings.productName = "プレカトゥス";
        PlayerSettings.bundleVersion = "0.1.0";
		PlayerSettings.Android.bundleVersionCode = 200010000;
        PlayerSettings.applicationIdentifier = "com.smilelab.seven";
        Build(BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }
    /// <summary>
	/// AuMarketビルド 本番用.
    /// </summary>
	[MenuItem("Build/Android/ROM AuMarket_Release")]
	public static void AuMarket()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "DEFINE_RELEASE;");
		PlayerSettings.productName = "プレカトゥス";
        PlayerSettings.bundleVersion = "1.0.5";
		PlayerSettings.Android.bundleVersionCode = 201000500;
		PlayerSettings.applicationIdentifier = "jp.fg.precatus";
        Build(BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }

	/// <summary>
	/// iOSビルド. 開発用.
	/// </summary>
    [MenuItem("Build/iOS/ROM iOS_Dev")]
	public static void iOSDev()
	{
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "DEFINE_DEVELOP;");
		PlayerSettings.productName = "DevSeven";
		PlayerSettings.bundleVersion = PlayerSettings.iOS.buildNumber = "0.0.1";
		PlayerSettings.applicationIdentifier = "com.smilelab.devseven";
		PlayerSettings.iOS.iOSManualProvisioningProfileID = "f38f648f-d574-4ee2-a965-44671b63dfcb";
		ProvisioningAtBuildTime = "seven_dev_adhoc";
		IsReleaseBuild = false;
		var buildOption = BuildOptions.Development;
        var enableProfiler = System.Environment.GetEnvironmentVariable ("ENABLE_PROFILER");
        Debug.Log ("enableProfiler = " + enableProfiler);
        if (enableProfiler == "true") {
            buildOption |= BuildOptions.ConnectWithProfiler;
        }

        Build(BuildTargetGroup.iOS, BuildTarget.iOS, buildOption);
	}
    /// <summary>
    /// iOSビルド. ベータ版.
    /// </summary>
    [MenuItem("Build/iOS/ROM iOS_β")]
    public static void iOSBeta()
    {
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "DEFINE_BETA;");
		PlayerSettings.productName = "プレカトゥス";
		PlayerSettings.bundleVersion = PlayerSettings.iOS.buildNumber = "0.1.0";
		PlayerSettings.applicationIdentifier = "com.smilelab.seven";
		PlayerSettings.iOS.iOSManualProvisioningProfileID = "e38857c5-11a0-48fd-994b-4709dfe3d2b8";
		ProvisioningAtBuildTime = "seven_adhoc";
		IsReleaseBuild = false;
        Build(BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None);
    }
    /// <summary>
    /// iOSビルド. 本番用.
    /// </summary>
    [MenuItem("Build/iOS/ROM iOS_Release")]
    public static void iOS()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "DEFINE_RELEASE;");
		PlayerSettings.productName = "プレカトゥス";
		PlayerSettings.bundleVersion = PlayerSettings.iOS.buildNumber = "1.0.5";
		PlayerSettings.applicationIdentifier = "jp.fg.precatus";
		PlayerSettings.iOS.iOSManualProvisioningProfileID = "225abe5b-bdb0-4bdc-9061-c2bd12a0558e";
		ProvisioningAtBuildTime = "LIBRA OF PRECATUS_appstore";
		IsReleaseBuild = true;
        Build(BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None/*BuildOptions.SymlinkLibraries*/);
    }
	/// <summary>
    /// iOSビルド. 本番サーバー接続検証用adhoc.
    /// </summary>
    [MenuItem("Build/iOS/ROM iOS_Release")]
    public static void iOSReleaseAchoc()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "DEFINE_RELEASE_ADHOC;");
        PlayerSettings.productName = "プレカトゥス";
		PlayerSettings.bundleVersion = PlayerSettings.iOS.buildNumber = "1.0.5";
		PlayerSettings.applicationIdentifier = "com.smilelab.devseven";
        PlayerSettings.iOS.iOSManualProvisioningProfileID = "80f5086e-55b0-4a37-8c7f-486338111284";
		ProvisioningAtBuildTime = "seven_dev_adhoc";
        IsReleaseBuild = false; // 検証のためこちらのプロビジョニングを使用する.リリースビルドとしない.
        Build(BuildTargetGroup.iOS, BuildTarget.iOS, BuildOptions.None/*BuildOptions.SymlinkLibraries*/);
    }

	// ビルド処理.
    private static void Build(BuildTargetGroup buildGroup, BuildTarget target, BuildOptions option)
	{
		// BuildTargetをtargetに合わせる.
		if(EditorUserBuildSettings.selectedBuildTargetGroup != buildGroup || EditorUserBuildSettings.activeBuildTarget != target){
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, target);
		}

        // 証明書パスワード.
		if(target == BuildTarget.Android){
			var pass = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("RkdwcnRuODAyNA=="));
			PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = pass;
		}

		// シーン
		var scenes = from i in EditorBuildSettings.scenes
			select i.path;
		
		// 出力先
        var destName = target == BuildTarget.Android ? "Android.apk": "xcode";
		
		// 実行
		var eMsg = BuildPipeline.BuildPlayer(scenes.ToArray(), destName, target, option);
        Debug.Log( string.IsNullOrEmpty(eMsg) ? "Success" : "Error!! : "+eMsg );
	}
}
