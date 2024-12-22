using System;
using System.Collections.Generic;
using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class QuestManager : MonoBehaviour
    {
        [Header("Config")] public bool LoadQuestState = true;
        
        private Dictionary<string, Quest> _questsMap;

        private EventBinding<OnStartQuestEvent> StartQuestEvent;
        private EventBinding<OnAdvanceQuestEvent> AdvanceQuestEvent;
        private EventBinding<OnFinishQuestEvent> FinishQuestEvent;
        private EventBinding<OnQuestStepStateChangeEvent> QuestStepStateChangeEvent;
        
        private EventBinding<OnAlcholLevelChangeEvent> AlcholLevelChangeEvent;

        #region Quest Requirement Parameters

        private int currentAlcholLevel = 1;

        #endregion
        
        private void Awake()
        {
            _questsMap = CreateQuestsMap();
            
        }

        private void Start()
        {
            //Broadcast the initial state of all quest in startup
            foreach (Quest quest in _questsMap.Values)
            {
                //initialize any laoded quest steps
                if (quest.State.Equals(EQuestState.IN_PROGRESS))
                {
                    quest.InstantiateCurrentQuestStep(this.transform);
                }
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
            AlcholLevelChangeEvent = new EventBinding<OnAlcholLevelChangeEvent>(AlcholLevelChange);
            QuestStepStateChangeEvent = new EventBinding<OnQuestStepStateChangeEvent>(QuestStepStateChange);
            
            
            EventBus<OnStartQuestEvent>.Register(StartQuestEvent);
            EventBus<OnAdvanceQuestEvent>.Register(AdvanceQuestEvent);
            EventBus<OnFinishQuestEvent>.Register(FinishQuestEvent);
            EventBus<OnAlcholLevelChangeEvent>.Register(AlcholLevelChangeEvent);
            EventBus<OnQuestStepStateChangeEvent>.Register(QuestStepStateChangeEvent);
        }

        private void OnDisable()
        {
            EventBus<OnStartQuestEvent>.Deregister(StartQuestEvent);
            EventBus<OnAdvanceQuestEvent>.Deregister(AdvanceQuestEvent);
            EventBus<OnFinishQuestEvent>.Deregister(FinishQuestEvent);
            EventBus<OnQuestStepStateChangeEvent>.Deregister(QuestStepStateChangeEvent);
            EventBus<OnAlcholLevelChangeEvent>.Deregister(AlcholLevelChangeEvent);
        }

        private void Update()
        {
            // loop through each quest 
            foreach (Quest quest in _questsMap.Values)
            {
                //if we are nowmeeting the requirements, switch over to the can_start state
                if (quest.State == EQuestState.REQUIREMNT_NOT_MET && CheckRequirementMeet(quest))
                {
                    ChangeQuestState(quest.Info.ID, EQuestState.CAN_START);
                }
            }
        }

        private void ChangeQuestState(string questID, EQuestState newState)
        {
            Quest quest = GetQuestByID(questID);
            quest.State = newState;
            EventBus<OnQuestStateChangeEvent>.Raise(new OnQuestStateChangeEvent()
            {
                quest = quest,
            });
        }

        private bool CheckRequirementMeet(Quest quest)
        {
            //Initially true and prove to be false and Check player level requirements
            bool metRequirements = !(currentAlcholLevel < quest.Info.AlcholRequirement);

            //check quest prerequisties for completion
            foreach (SO_QuestSystemInfo questSystemInfo in quest.Info.QuestPrerequisites)
            {
                if (GetQuestByID(questSystemInfo.ID).State != EQuestState.FINISHED)
                {
                    metRequirements = false;
                }
            }
            
            return metRequirements;
        }
        
        private void StartQuest(OnStartQuestEvent e)
        {
            Quest quest = GetQuestByID(e.questID);
            quest.InstantiateCurrentQuestStep(this.transform);
            ChangeQuestState(quest.Info.ID, EQuestState.IN_PROGRESS);
        }
        
        private void AdvanceQuest(OnAdvanceQuestEvent e)
        {
            Quest quest = GetQuestByID(e.questID);
            
            //Move on to the next step
            quest.MoveToNextStep();
            
            //Check if there are any more steps, yes then initialize those steps
            if (quest.CurrentStepExists())
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            //else finish the quest
            else
            {
                ChangeQuestState(quest.Info.ID, EQuestState.CAN_FINISH);
            }
        }
        
        private void FinishQuest(OnFinishQuestEvent e)
        {
            Quest quest = GetQuestByID(e.questID);
            ClaimRewards(quest);
            ChangeQuestState(quest.Info.ID, EQuestState.FINISHED);
        }

        private void QuestStepStateChange(OnQuestStepStateChangeEvent e)
        {
            Quest quest = GetQuestByID(e.questID);
            quest.StoreQuestStepState(e.questStepState, e.stepIndex);
            ChangeQuestState(e.questID, quest.State);
        }
        
        private void ClaimRewards(Quest quest)
        {
            Debug.Log($"Rewards claimed : {quest.Info.ID} {quest.Info.GoldReward}");
        }

        private void AlcholLevelChange(OnAlcholLevelChangeEvent e)
        {
            currentAlcholLevel = e.level;
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
                idToQuestMap.Add(questInfo.ID, LoadQuest(questInfo));
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

        private void OnApplicationQuit()
        {
            foreach (var questsMapValue in _questsMap.Values)
            {
                SaveQuests(questsMapValue);
            }
        }

        private void SaveQuests(Quest quest)
        {
            try
            {
                QuestData questData = quest.GetQuestData();
                string serializedData = JsonUtility.ToJson(questData);
                //TODO: More Versatile Save Method
                PlayerPrefs.SetString(quest.Info.ID, serializedData);
                Debug.Log(serializedData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to savae quest with id: {quest.Info.ID} with Exception: {e}");
            }
        }

        private Quest LoadQuest(SO_QuestSystemInfo questSystemInfo)
        {
            Quest quest = null;
            try
            {
                if (PlayerPrefs.HasKey(questSystemInfo.ID) && LoadQuestState)
                {
                    string serializedData = PlayerPrefs.GetString(questSystemInfo.ID);
                    QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                    quest = new Quest(questSystemInfo, questData.state, questData.questStepIndex, questData.questStepStates);
                }
                else
                {
                    quest = new Quest(questSystemInfo);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load quest with id: {questSystemInfo.ID} with Exception: {e}");
            }
            return quest;
        }
    }
}

