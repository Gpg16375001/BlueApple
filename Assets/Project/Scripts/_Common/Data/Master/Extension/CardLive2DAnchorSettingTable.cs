using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CardLive2DAnchorSettingTable
{

    /// <summary>
    /// 顔中心の座標になるように調整をかける.
    /// </summary>
    public void FixPostionFaceCenter(int cardId, GameObject target, float sacle)
	{
		var setting = this[cardId];
		if(setting == null){
			return; // 設定がなければ何もしない.
		}
		var pos = target.transform.localPosition;
        pos.x = setting.x * sacle;
        pos.y = setting.y * sacle;
		target.transform.localPosition = pos;
	}
}
