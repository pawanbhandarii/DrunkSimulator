using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class QuestEvent
    {
        public void StartQuest(string questID)
        {
            EventBus<OnStartQuestEvent>.Raise(new OnStartQuestEvent()
            {
                questID = questID
            });
        }

        public void AdvanceQuest(string questID)
        {
            EventBus<OnAdvanceQuestEvent>.Raise(new OnAdvanceQuestEvent()
            {
                questID = questID
            });
        }

        public void FinishQuest(string questID)
        {
            EventBus<OnFinishQuestEvent>.Raise(new OnFinishQuestEvent()
            {
                questID = questID 
            });
        }

        public void QuestStateChange(Quest quest)
        {
            EventBus<OnQuestStateChangeEvent>.Raise(new OnQuestStateChangeEvent()
            {
                quest = quest
            });
        }
    }
}
