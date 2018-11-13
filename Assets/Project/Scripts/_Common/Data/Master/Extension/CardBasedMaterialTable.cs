using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class CardBasedMaterialTable
{
    public CardBasedMaterial[] EnableData {
        get {
            return DataList.Where (x => x.Enable).ToArray ();
        }
    }

}

public partial class CardBasedMaterial
{
    public bool Enable {
        get {
            var now = SmileLab.GameTime.SharedInstance.Now;
            return (!exchange_display_start_at.HasValue || exchange_display_start_at.Value <= now) &&
                (!exchange_display_end_at.HasValue || exchange_display_end_at.Value >= now);
        }
    }
}
