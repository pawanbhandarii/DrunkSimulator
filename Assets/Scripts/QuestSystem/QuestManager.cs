using System;
using System.Collections.Generic;
using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class QuestManager : MonoBehaviour
    {
        private Dictionary<string, Quest> _questsMap;

        private EventBinding<OnStartQuestEvent> StartQuestEvent;
        private EventBinding<OnAdvanceQuestEvent> AdvanceQuestEvent;
        private EventBinding<OnFinishQuestEvent> FinishQuestEvent;
        
        private void Awake()
        {
            _questsMap = CreateQuestsMap();
            
        }

        private void Start()
        {
            //Broadcast the initial state of all quest in startup
            foreach (Quest quest in _questsMap.Values)
            {
                EventBus<OnQuestStateChangeEvent>.Raise(new OnQuestStateChangeEvent()
                {
                    quest = quest,
                });
            }
        }

        private void OnEnable()
        {
            StartQuestEvent = new EventBinding<OnStartQuestEvent>(StartQuest);
            AdvanceQuestEvent = new EventBinding<OnAdvanceQuestEvent>(AdvanceQuest);
            FinishQuestEvent = new EventBinding<OnFinishQuestEvent>(FinishQuest);
            
            EventBus<OnStartQuestEvent>.Register(StartQuestEvent);
            EventBus<OnAdvanceQuestEvent>.Register(AdvanceQuestEvent);
            EventBus<OnFinishQuestEvent>.Register(FinishQuestEvent);
        }

        private void OnDisable()
        {
            EventBus<OnStartQuestEvent>.Deregister(StartQuestEvent);
            EventBus<OnAdvanceQuestEvent>.Deregister(AdvanceQuestEvent);
            EventBus<OnFinishQuestEvent>.Deregister(FinishQuestEvent);
        }

        private void StartQuest(OnStartQuestEvent e)
        {
            //TODO: Start Quest
            Debug.Log($"Starting quest {e.questID}");
        }
        
        private void AdvanceQuest(OnAdvanceQuestEvent e)
        {
            //TODO: Advance Quest
            Debug.Log($"Advancing quest {e.questID}");
        }
        
        private void FinishQuest(OnFinishQuestEvent e)
        {
            //TODO: Finish Quest
            Debug.Log($"Finish quest {e.questID}");
        }
        
        
        private Dictionary<string, Quest> CreateQuestsMap()
        {
            //laod all the QuestInfoSO under Asset/Resource/Quests
            SO_QuestSystemInfo[] allQuests = Resources.LoadAll<SO_QuestSystemInfo>("Quests");
            
            // Create quest map
            Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
            foreach (SO_QuestSystemInfo questInfo in allQuests)
            {
                if (idToQuestMap.ContainsKey(questInfo.ID))
                {
                    Debug.LogWarning($"Duplicate ID found while creating quest map: {questInfo.ID}");
                }
                idToQuestMap.Add(questInfo.ID, new Quest(questInfo));
            }
            return idToQuestMap;
        }

        private Quest GetQuestByID(string id)
        {
            Quest quest = _questsMap[id];
            if (quest == null)
            {
                Debug.LogError($"ID not found in Quests map: {id}");
            }
            return quest;
        }
    }
}

