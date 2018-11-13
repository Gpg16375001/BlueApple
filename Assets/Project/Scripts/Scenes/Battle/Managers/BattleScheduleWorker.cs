using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// バトルのスケジューラーで使用するワーカークラスの定義
///

/// <summary>
/// Worker.
/// </summary>
public interface BattleScheduleWorker 
{
    IEnumerator Work ();

}

/// <summary>
/// IEnumerator worker.
/// </summary>
public class IEnumeratorWorker : BattleScheduleWorker
{
    private IEnumerator m_Worker;

    public IEnumeratorWorker(IEnumerator worker)
    {
        m_Worker = worker;
    }

    public IEnumerator Work()
    {
        for (; m_Worker.MoveNext ();) {
            yield return m_Worker.Current;
        }
    }
}

/// <summary>
/// Action worker.
/// </summary>
public class ActionWorker : BattleScheduleWorker
{
    private Action m_ActionCallback;

    public ActionWorker(Action action)
    {
        m_ActionCallback = action;
    }

    public IEnumerator Work()
    {
        if (m_ActionCallback != null) {
            m_ActionCallback ();
        }
        yield break;
    }
}

/// <summary>
/// Wait time worker.
/// </summary>
public class WaitTimeWorker : BattleScheduleWorker
{
    private float m_WaitTime;

    public WaitTimeWorker(float time)
    {
        m_WaitTime = time;
    }

    public IEnumerator Work()
    {
        yield return new WaitForSeconds(m_WaitTime);
    }
}

/// <summary>
/// Wait until worker.
/// </summary>
public class WaitUntilWorker : BattleScheduleWorker
{
    private Func<bool> m_Until;

    public WaitUntilWorker(Func<bool> until)
    {
        m_Until = until;
    }

    public IEnumerator Work()
    {
        yield return new WaitUntil(m_Until);
    }
}
