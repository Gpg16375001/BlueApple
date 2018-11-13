using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

using SmileLab;

/// <summary>
/// バトルWave演出Wave数設定用
/// </summary>
public class View_BattleWaveCountUp : ViewBase {
    public void SetWaveCount(int waveCount)
    {
        var battleWaveNumberAtlas = Resources.Load<SpriteAtlas> ("Atlases/BattleWaveNumber");

        var Num01Sprite = GetScript<SpriteRenderer> ("Wave_Num01");
        var Num02Sprite = GetScript<SpriteRenderer> ("Wave_Num02");
        var Num01b1Sprite = GetScript<SpriteRenderer> ("Wave_Num01b1");
        var Num02b1Sprite = GetScript<SpriteRenderer> ("Wave_Num02b1");
        var Num01b2Sprite = GetScript<SpriteRenderer> ("Wave_Num01b2");
        var Num02b2Sprite = GetScript<SpriteRenderer> ("Wave_Num02b2");

        waveCount = Mathf.Min (waveCount, 99);
        if (waveCount >= 10) {
            Num02Sprite.gameObject.SetActive (true);
            Num01Sprite.gameObject.SetActive (true);
            Num02b1Sprite.gameObject.SetActive (true);
            Num01b1Sprite.gameObject.SetActive (true);
            Num02b2Sprite.gameObject.SetActive (true);
            Num01b2Sprite.gameObject.SetActive (true);

            var spt2 = battleWaveNumberAtlas.GetSprite (string.Format("eff_BattleWaveNum{0:D2}", (waveCount / 10)));
            Num02Sprite.sprite = spt2;
            Num02b1Sprite.sprite = spt2;
            Num02b2Sprite.sprite = spt2;

            var spt1 = battleWaveNumberAtlas.GetSprite (string.Format ("eff_BattleWaveNum{0:D2}", (waveCount % 10)));
            Num01Sprite.sprite = spt1;
            Num01b1Sprite.sprite = spt1;
            Num01b2Sprite.sprite = spt1;
        } else {
            Num02Sprite.gameObject.SetActive (false);
            Num01Sprite.gameObject.SetActive (true);
            Num02b1Sprite.gameObject.SetActive (false);
            Num01b1Sprite.gameObject.SetActive (true);
            Num02b2Sprite.gameObject.SetActive (false);
            Num01b2Sprite.gameObject.SetActive (true);

            var spt1 = battleWaveNumberAtlas.GetSprite (string.Format ("eff_BattleWaveNum{0:D2}", waveCount));
            Num01Sprite.sprite = spt1;
            Num01b1Sprite.sprite = spt1;
            Num01b2Sprite.sprite = spt1;
        }

    }
}
