using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public partial class ElementAffinitySettingTable {
    Dictionary<Tuple<ElementEnum, ElementEnum>, ElementAffinity> _dict;
    partial void InitExtension ()
    {
        _dict = _dataList.ToDictionary(x => Tuple.Create(x.element, x.targe_element), x => x.affinity);
    }

    public ElementAffinity GetElementAffinity(ElementEnum baseElement, ElementEnum targetElement)
    {
        var key = Tuple.Create(baseElement, targetElement);
        if (!_dict.ContainsKey(key)) {
            // 該当データがない場合は通常として扱う
            return MasterDataTable.element_affinity [ElementAffinityEnum.normal];
        }
        return _dict[key];
    }
}
