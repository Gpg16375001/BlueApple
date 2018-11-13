using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScheduler : MonoBehaviour 
{
    private Queue<BattleScheduleWorker> BattleSchedule = new Queue<BattleScheduleWorker>();

    private static BattleScheduler m_Instance;
    public static BattleScheduler Instance {
        get {
            return m_Instance;
        }
    }

    void Awake()
    {
        if (m_Instance != null) {
            m_Instance.Dispose (true);
        }
        m_Instance = this;
        m_Instance.Init ();
    }

    void OnDestroy()
    {
        Dispose (true);
    }

    private void Init()
    {
        m_IsWork = true;
        m_ScheduleWorker = StartCoroutine (ScheduleWorker ());
    }

    public void Dispose(bool immediately = false)
    {
        m_IsWork = false;
        if (immediately && m_ScheduleWorker != null) {
            StopCoroutine (m_ScheduleWorker);
            m_ScheduleWorker = null;
        }
    }

    public void Clear()
    {
        BattleSchedule.Clear ();
    }
        
    public void AddSchedule(IEnumerator worker)
    {
        BattleSchedule.Enqueue (new IEnumeratorWorker(worker));
    }

    public void AddAction(Action action)
    {
        BattleSchedule.Enqueue (new ActionWorker(action));
    }

    public void AddWaitUntil(Func<bool> until)
    {
        BattleSchedule.Enqueue (new WaitUntilWorker(until));
    }

    private IEnumerator ScheduleWorker()
    {
        while(m_IsWork) {
            yield return new WaitUntil (() => BattleSchedule.Count > 0);

            var worker =  BattleSchedule.Dequeue ();
            yield return worker.Work();
        }
    }

    private Coroutine m_ScheduleWorker;
    private bool m_IsWork = false;
}
