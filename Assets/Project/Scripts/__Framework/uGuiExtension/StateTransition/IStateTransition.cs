using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace SmileLab.UI {
    public enum TransitionState {
        Normal,
        Highlighted,
        Pressed,
        Disabled
    }

    public interface IStateTransition {
        void DoStateTransition(TransitionState state, bool instant);
    }
}