using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmileLab;

public class ListItem_BuffStatusIcon : ViewBase {

    public void Init(Condition condition)
    {
        GetScript<Image> ("Icon").overrideSprite = IconLoader.LoadConditionIcon (condition);
    }
}
