using System.Collections;
using System.Collections.Generic;

using SmileLab;
public partial class EventQuestScenarioSetting
{
    public bool Enable()
    {
        var now = GameTime.SharedInstance.Now;
        switch (play_timing) {
        case EventQuestScenarioPlayTimingEnum.Start:
            {
                var eventQuest = MasterDataTable.event_quest [event_quest_id];
                return eventQuest.start_at <= now && eventQuest.end_at >= now;
            }
        case EventQuestScenarioPlayTimingEnum.End:
            {
                var eventQuest = MasterDataTable.event_quest [event_quest_id];
                return eventQuest.end_at <= now && eventQuest.exchange_time_limit >= now;
            }
        case EventQuestScenarioPlayTimingEnum.StartSchedule:
            if (event_quest_schedule_id.HasValue) {
                var schedule = MasterDataTable.event_quest_schedule [event_quest_schedule_id.Value];
                return schedule.start_at <= now && schedule.end_at >= now;
            }
            return false;
        case EventQuestScenarioPlayTimingEnum.EndSchedule:
            if(event_quest_schedule_id.HasValue) {
                var schedule = MasterDataTable.event_quest_schedule [event_quest_schedule_id.Value];
                var eventQuest = MasterDataTable.event_quest [event_quest_id];
                return schedule.end_at <= now && eventQuest.exchange_time_limit >= now;
            }
            return false;
        }
        return false;
    }
}
