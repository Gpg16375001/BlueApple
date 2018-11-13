using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface : バトルのミッションチェッカー共通モジュール.
/// </summary>
public interface IBattleMissionChecker
{
    /// <summary>開始以前に既に達成済みかどうか(前回クリアなど).</summary>
    bool IsAlreadyAchived { get; }
    /// <summary>達成しているか.</summary>
    bool IsAchived { get; }
    /// <summary>そのミッションの説明文.</summary>
    string ExplanatoryText { get; }
    /// <summary>進捗更新.</summary>
    void UpdateProgress(BattleMissionProgressData data);
}
