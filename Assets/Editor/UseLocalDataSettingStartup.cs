using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class UseLocalDataSettingStartup {

	static UseLocalDataSettingStartup()
	{
		var str = EditorUserSettings.GetConfigValue ("UseLocalDataSettingWindow_SelectDLCType");
		if(string.IsNullOrEmpty(str)) {
			return;
		}

		int SelectDLCType = 0;
		if (!int.TryParse (str, out SelectDLCType)) {
			SelectDLCType = 0;
		}

		if (SelectDLCType == 0) {
			var symbols = "DEFINE_DEVELOP;";
			PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
		} else if (SelectDLCType == 1) {
			var symbols = "DEFINE_DEVELOP;USE_LOCAL_DATA;";
			PlayerSettings.SetScriptingDefineSymbolsForGroup (EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
		}
	}
}
