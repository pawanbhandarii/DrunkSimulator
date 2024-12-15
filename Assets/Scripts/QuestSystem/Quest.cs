using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class Quest
    {
        public SO_QuestSystemInfo Info;
        public EQuestState State;

        public int CurrentQuestStepIndex;


        public Quest(SO_QuestSystemInfo info)
        {
            Info = info;
            State = EQuestState.REQUIREMNT_NOT_MET;
            CurrentQuestStepIndex = 0;
        }

        public void MoveToNextStep()
        {
            CurrentQuestStepIndex++;
        }

        public bool CurrentStepExists()
        {
            return CurrentQuestStepIndex < Info.QuestStepsPrefabs.Length;
        }

        public void InstantiateCurrentQuestStep(Transform parentTransform)
        {
            GameObject questStepPrefab = GetCurrentQuestStepPrefab();
            if (questStepPrefab != null)
            {
                Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
            }
        }

        public GameObject GetCurrentQuestStepPrefab()
        {
            GameObject questStepPrefab = null;
            if (CurrentStepExists())
            {
                questStepPrefab = Info.QuestStepsPrefabs[CurrentQuestStepIndex];
            }
            else
            {
                Debug.LogWarning($"Step Index Out of range. No Current Step: Quest ID: {Info.ID}, StepIndex: {CurrentQuestStepIndex}");
            }

            return questStepPrefab;
        }
    }
}

