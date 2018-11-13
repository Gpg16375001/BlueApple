using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロード進捗取得用Interface
/// </summary>
public interface ILoadProgress
{
    float ProgressValue { get; }

}
