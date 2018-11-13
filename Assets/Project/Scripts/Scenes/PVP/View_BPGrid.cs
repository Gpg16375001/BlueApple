using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;

public class View_BPGrid : ViewBase
{
    const int BP_MAX = 5;

    [System.Serializable]
    public class BpGridItem {
        public BpGridItem(GameObject onGo, GameObject offGo)
        {
            m_OnGo = onGo;
            m_OffGo = offGo;

            _isOn = false;
            m_OnGo.SetActive(false);
            m_OffGo.SetActive(true);
        }

        private bool _isOn;
        public bool IsOn {
            get {
                return _isOn;
            }
            set {
                if (_isOn != value) {
                    _isOn = value;
                    m_OnGo.SetActive (_isOn);
                    m_OffGo.SetActive (!_isOn);
                }
            }
        }

        private GameObject m_OnGo;
        private GameObject m_OffGo;
    }

    public void Init()
    {
        BpObjects = new BpGridItem[BP_MAX];
        for (int bpCount = 1; bpCount <= BP_MAX; ++bpCount) {
            var bpIconOnGo = GetScript<RectTransform> (string.Format("BPIcon{0}/BPIconOn", bpCount)).gameObject;
            var bpIconOffGo = GetScript<RectTransform> (string.Format("BPIcon{0}/BPIconOff", bpCount)).gameObject;

            BpObjects [bpCount - 1] = new BpGridItem (bpIconOnGo, bpIconOffGo);
        }
    }

    public void UpdateBP(int bp)
    {
        prevBP = bp;
        for (int bpCount = 0; bpCount < BP_MAX; ++bpCount) {
            BpObjects[bpCount].IsOn = bp > bpCount;
        }
    }

    void Update()
    {
        int nowBP = AwsModule.UserData.BattlePoint;
        if (nowBP != prevBP) {
            UpdateBP (nowBP);
        }
    }
        
    public BpGridItem[] BpObjects;
    private int prevBP;
}
