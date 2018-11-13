using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParallelCorotines {
    public static IEnumerator DoCoroutines(MonoBehaviour owner, IList<IEnumerator> coroutins)
    {
        int counter = coroutins.Count;
        int endCounter = 0;
        for (int i = 0; i < counter; ++i) {
            owner.StartCoroutine (CoDo(coroutins [i], () => ++endCounter));
        }

        yield return new WaitUntil (() => counter <= endCounter);
    }

    private static IEnumerator CoDo(IEnumerator enumerator, System.Action endCallback)
    {
        yield return enumerator;
        endCallback ();
    }
}
