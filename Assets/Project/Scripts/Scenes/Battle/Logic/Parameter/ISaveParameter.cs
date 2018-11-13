using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleLogic {
    public interface ISaveParameter<T> {
        T CreateSaveData ();
        T UpdateSaveData ();
        void Load (T saveData);
        void Reversion (Parameter unit, T saveData);
        bool IsModify ();
    }
}
