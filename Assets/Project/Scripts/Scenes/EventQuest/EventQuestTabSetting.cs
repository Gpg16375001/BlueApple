using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab;
using SmileLab.UI;

public class EventQuestTabSetting : MonoBehaviour {
    [SerializeField]
    public EventQuestStageTypeEnum StageType;

    [SerializeField]
    private CustomButton TabButton;

    [SerializeField]
    private GameObject NewImage;

    bool _IsNew = false;
    public bool IsNew {
        get {
            return _IsNew;
        }
        set {
            if (_IsNew != value) {
                _IsNew = value;
                if (NewImage != null) {
                    NewImage.SetActive (_IsNew);
                }
            }
        }
    }

    public bool IsHighlight {
        get {
            if (TabButton != null) {
                return TabButton.ForceHighlight;
            }
            return false;
        }
        set {
            if (TabButton != null) {
                TabButton.ForceHighlight = value;
            }
        }
    }

    public void SetTabCallback(Action<EventQuestStageTypeEnum> tapAction)
    {
        m_TapAction = tapAction;
    }

    void Start()
    {
        if (TabButton != null) {
            TabButton.onClick.RemoveAllListeners ();
            TabButton.onClick.AddListener (DidTap);
        }
    }

    void DidTap()
    {
        if (m_TapAction != null) {
            m_TapAction (StageType);
        }
    }


    Action<EventQuestStageTypeEnum> m_TapAction;
}
