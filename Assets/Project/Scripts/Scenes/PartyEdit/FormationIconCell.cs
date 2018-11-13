using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationIconCell : MonoBehaviour {
    [SerializeField]
    private GameObject Red;

    public void SetVisble(bool disp, bool redDisp)
    {
        gameObject.SetActive (disp);
        Red.SetActive (disp && redDisp);
    }
}
