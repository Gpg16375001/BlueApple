using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using SmileLab;
using SmileLab.Net.API;
using SmileLab.UI;

public class ListItem_VoicePlay : ViewBase {

    public void Init(CardData card, CardDetailVoiceSetting setting, string sheetName)
    {
		m_card = card;
        m_SheetName = sheetName;
        m_CueName = setting.voice_cue_id;
  
		if(setting.IsReleaseVoice(m_card)){
			this.GetScript<TextMeshProUGUI>("txtp_VoiceTitle").SetText(setting.display_name);
			this.GetScript<TextMeshProUGUI>("txtp_UnlockNotes").gameObject.SetActive(false);
            this.GetScript<CustomButton>("bt_Play").onClick.AddListener(PlayVoice);         
		}else{
			this.GetScript<TextMeshProUGUI>("txtp_VoiceTitle").SetText("???");
			this.GetScript<TextMeshProUGUI>("txtp_UnlockNotes").text = setting.UnlockText();
			this.GetScript<CustomButton>("bt_Play").interactable = false;
		}
		this.UpdateNewBadge();
    }   

    /// <summary>
    /// Newバッジアイコン更新.
    /// </summary>
    public void UpdateNewBadge()
	{
		var data = AwsModule.CardModifiedData.List.Find(d => d.CardId == m_card.CardId);
		if(data == null){
			return;
		}
		if(data.NeedSeeReleaseVoiceDictionary == null){
			return;
		}
		var bNew = data.NeedSeeReleaseVoiceDictionary[(int)m_CueName];
		this.GetScript<Image>("img_TabNew").gameObject.SetActive(bNew);
	}

    void PlayVoice()
    {
        SoundManager.SharedInstance.PlayVoice (m_SheetName, m_CueName);
    }

	private CardData m_card;
    SoundVoiceCueEnum m_CueName;
    string m_SheetName;
}
