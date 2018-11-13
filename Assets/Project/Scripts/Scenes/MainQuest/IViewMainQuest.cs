using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// interface : メインクエスト画面共有インターフェイス.
/// </summary>
public interface IViewMainQuest
{
    /// <summary>起動モード.</summary>
    MainQuestBootEnum Boot { get; }
    /// <summary>破棄処理.</summary>
    void Destroy();
}
