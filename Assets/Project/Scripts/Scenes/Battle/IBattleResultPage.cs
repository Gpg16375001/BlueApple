using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface : バトル結果画面のページ共通動作.
/// </summary>
public interface IBattleResultPage
{
	/// <summary>リザルト内のページインデックス.</summary>
	int Index { get; }
	/// <summary>演出中？</summary>
	bool IsEffecting { get; }

	/// <summary>開く.</summary>
	void Open();
	/// <summary>閉じる.</summary>
	void Close(Action didEnd);
	/// <summary>アニメーションを強制的に即時終了させる.</summary>
	void ForceImmediateEndAnimation();
}
