using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using SmileLab;

public class ListItem_SelectItem : ViewBase
{
    public void Init(int index)
    {
        var txtpNum = GetScript<TextMeshProUGUI> ("txtp_Num");
        txtpNum.SetText (index.ToString ());
    }
}
