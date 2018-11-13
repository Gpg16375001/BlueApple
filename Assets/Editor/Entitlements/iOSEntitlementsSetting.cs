using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class iOSEntitlementsSetting : ScriptableObject {
	/// <summary> 開発用のEntitlementsファイル </summary>
	public DefaultAsset RomDev;
	/// <summary> β用のEntitlementsファイル </summary>
	public DefaultAsset RomBeta;
	/// <summary> 本番用のEntitlementsファイル </summary>
	public DefaultAsset RomRelease;
}