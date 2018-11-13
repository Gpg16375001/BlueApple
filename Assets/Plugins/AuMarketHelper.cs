using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AuMarketHelper : MonoBehaviour
{
	#if UNITY_ANDROID
	AndroidJavaObject auMarketHelper;

	public void Init()
	{
		auMarketHelper = new AndroidJavaObject("com.kddi.aumarkethelper.unitywrapper.AuMarketHelper");
		auMarketHelper.Call("Init", this.name);
	}

	public void Bind()
	{
		if (auMarketHelper == null) {
			Debug.Log("auMarketHelper is null");
			return;
		}
		auMarketHelper.Call("Bind");
	}

	public void  Unbind()
	{
		if (auMarketHelper == null) {
			Debug.Log("auMarketHelper is null");
			return;
		}
		auMarketHelper.Call("Unbind");
	}
	
	public void ConfirmReceipt(string itemId)
	{
		if (auMarketHelper == null) {
			Debug.Log("auMarketHelper is null");
			return;
		}
		auMarketHelper.Call("confirmReceipt", itemId);
	}

	public void IssueReceipt(string itemId)
	{
		if (auMarketHelper == null) {
			Debug.Log("auMarketHelper is null");
			return;
		}
		auMarketHelper.Call("issueReceipt", itemId);
	}

	public void InvalidateItem(string itemId)
	{
		if (auMarketHelper == null) {
			Debug.Log("auMarketHelper is null");
			return;
		}
		auMarketHelper.Call("invalidateItem", itemId);
	}
	#endif
}