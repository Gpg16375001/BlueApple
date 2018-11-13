using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class ColorDefine : ViewBase
{
    [SerializeField]
    public ColorDefineSetting Setting;

    public static ColorDefine SharedInstance {
        get;
        private set;
    }

    void Awake()
    {
        if(SharedInstance != null) {
            SharedInstance.Dispose();
        }
        SharedInstance = this;
    }

    public override void Dispose ()
    {
        SharedInstance = null;
        base.Dispose ();
    }

}
