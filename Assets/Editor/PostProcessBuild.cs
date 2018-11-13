using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using SmileLab;

/// <summary>
/// クラス：ビルド事後処理.
/// </summary>
public static class PostProcessBuild
{
    /// <summary>
    /// ポストプロセス時のコールバック.
    /// </summary>
    /// <param name="target">ビルドターゲット.</param>
    /// <param name="path">出力先のパス（ビルド時に指定するパス）</param>
    [PostProcessBuild(200)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        Debug.Log("[PostProcessBuild] OnPostProcessBuild");
        if(target == BuildTarget.iOS){
            ProcessiOS(path);
        }
    }

    // iOSの事後処理.
    static void ProcessiOS(string path)
    {
        // plist設定.
        var plistPath = Path.Combine(path, "Info.plist");
        if(FileUtility.Exists(plistPath)){
            EditPlist(plistPath);
        }
        // pbx編集.
        var pbxPath = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
        if (FileUtility.Exists(pbxPath)) {
            EditPBXProject(pbxPath);
        }
    }
    // plist編集.
    static void EditPlist(string plistPath)
    {
        Debug.Log(">>>>>>>> EditPlist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // ローカリゼーション
        plist.root.SetString("CFBundleDevelopmentRegion", "ja_JP");

        if (PlayerSettings.applicationIdentifier != "com.smilelab.seven") {
            // URL Schemaを入れ替える
            foreach (var urlTypes in plist.root.values["CFBundleURLTypes"].AsArray().values) {
                var dict = urlTypes.AsDict ();
                var urls = dict.values ["CFBundleURLSchemes"].AsArray ();
                foreach (var url in urls.values.ToArray()) {
                    if (url.AsString () == "com.smilelab.seven") {
                        urls.values.Remove (url);
                        urls.AddString ("com.smilelab.devseven");
                    }
                }
            }
        }

        // 保存
        plist.WriteToFile(plistPath);
    }
    // PBXProject編集.
    static void EditPBXProject(string pbxPath)
    {
        Debug.Log(">>>>>>>> EditPBXProject");      
        var pbx = new PBXProject();
        pbx.ReadFromFile(pbxPath);
        var targetGuid = pbx.TargetGuidByName("Unity-iPhone");

        pbx.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");           // EnableBitCodeをfalseに.

		// ビルド設定.マニュアルにする.DEVELOPMENT_TEAMを設定するとAutomaticaryにされるっぽいので注意.
		pbx.SetBuildProperty(targetGuid, "CODE_SIGN_STYLE", "Manual");      
		if(!BatchBuild.IsReleaseBuild){
			pbx.SetBuildProperty(targetGuid, "CODE_SIGN_IDENTITY", "iPhone Distribution: SMILE-LAB CO., LTD. (N7GKJM7E62)");
		}else{
			pbx.SetBuildProperty(targetGuid, "CODE_SIGN_IDENTITY", "iPhone Distribution: FUJI GAMES, INC. (KQ4JCYSACP)");
		}      

        // プロビジョニング設定. #if ではこの回のビルド時の切り替えを判別できない.BatchBuild時に予め設定しておくようにする.
		if(!string.IsNullOrEmpty(BatchBuild.ProvisioningAtBuildTime)){
			pbx.SetBuildProperty(targetGuid, "PROVISIONING_PROFILE_SPECIFIER", BatchBuild.ProvisioningAtBuildTime);
		}

        // SmartBeat用にlibzを追加
        pbx.AddFileToBuild(targetGuid, pbx.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));      

        // Adjust用にiAd追加、Linkerフラグ-ObjCを追加、Objective-C exceptionsを有効化
        pbx.AddFrameworkToProject(targetGuid, "iAD.framework", true);
        // Enable Objective-C Exceptions = YES
        pbx.SetBuildProperty (targetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
        // "Other Linker Flags" に "-ObjC" を追加
        pbx.AddBuildProperty (targetGuid, "OTHER_LDFLAGS", "-ObjC");

		// --- Capabilitiesの設定 ---
		// リモート通知
    /*
		// TODO: フォーラムから応用 いい方法を知りたい
		var entitlementsMaster =  ScriptableObject.CreateInstance<iOSEntitlementsSetting>();
		DefaultAsset entitlementsFile = null;
		if (!BatchBuild.IsReleaseBuild) {
			entitlementsFile = entitlementsMaster.RomDev;
		} else {
			entitlementsFile = entitlementsMaster.RomRelease;
		}
		ScriptableObject.DestroyImmediate(entitlementsMaster);

		if (entitlementsFile != null) {
			//ビルドの出力先を取得
			var builtProjectPath = pbxPath.Replace ("/Unity-iPhone.xcodeproj/project.pbxproj", "");
			// Copy the entitlement file to the xcode project
			var entitlementPath = AssetDatabase.GetAssetPath (entitlementsFile);
			var entitlementFileName = Path.GetFileName (entitlementPath);
			var unityTarget = PBXProject.GetUnityTargetName();
			var relativeDestination = unityTarget + "/" + entitlementFileName;
			FileUtil.CopyFileOrDirectory(entitlementPath, builtProjectPath + "/" + relativeDestination);
			// Add the pbx configs to include the entitlements files on the project
			pbx.AddFile(relativeDestination, entitlementFileName);
			// 通知の設定
			pbx.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativeDestination);
			// Add push notifications as a capability on the target
			pbx.AddBuildProperty(targetGuid, "SystemCapabilities", "{com.apple.Push = {enabled = 1;};}");
		} else {
			pbx.AddCapability(targetGuid, PBXCapabilityType.PushNotifications);
		}
    */
        pbx.AddCapability(targetGuid, PBXCapabilityType.PushNotifications);

        File.WriteAllText(pbxPath, pbx.WriteToString());
    }
}
