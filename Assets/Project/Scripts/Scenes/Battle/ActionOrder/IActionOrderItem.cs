using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionOrderItem {
    ActionOrderItemType ItemType {
        get;
    }
    int Index {
        get;
    }
    bool IsPlayer {
        get;
    }
    int Weight {
        get;
    }
    bool IsRemove {
        get;
    }

    void SubWeight (int value);
    void ResetWeight ();
}
