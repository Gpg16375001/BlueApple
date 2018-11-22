using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventQuestSaveData {
    [Serializable]
    public class EventQuestSaveDataParts {
        [SerializeField]
        public int EventId;

        [SerializeField]
        public DateTime RemoveAt;

        [SerializeField]
        public List<int> ReadedScenario = new List<int> ();

        public EventQuestSaveDataParts(int eventId)
        {
            EventId = eventId;
        }
    }

    [SerializeField]
    private List<EventQuestSaveDataParts> SaveData = new List<EventQuestSaveDataParts> ();

    public List<int> GetReadedScenario(int eventId)
    {
        var data = SaveData.Find (x => x.EventId == eventId);
        if (data != null) {
            return data.ReadedScenario;
        }
        return new List<int> ();
    }

    public void SetReadedScenario(int eventId, int settingId)
    {
        var data = SaveData.Find (x => x.EventId == eventId);
        if (data == null) {
            data = new EventQuestSaveDataParts (eventId);
            SaveData.Add (data);
        }
        data.ReadedScenario.Add (settingId);
    }

    public void ResetReadedScenario(int eventId)
    {
        var data = SaveData.Find (x => x.EventId == eventId);
        if (data == null) {
            data = new EventQuestSaveDataParts (eventId);
            SaveData.Add (data);
        }
        data.ReadedScenario.Clear();
    }
}
