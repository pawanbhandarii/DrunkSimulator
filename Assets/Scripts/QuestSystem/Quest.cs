using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class Quest
    {
        public SO_QuestSystemInfo Info;
        public EQuestState State;

        public int CurrentQuestStepIndex;
        
        private QuestStepState[] _questStepStates;

        public Quest(SO_QuestSystemInfo info)
        {
            Info = info;
            State = EQuestState.REQUIREMNT_NOT_MET;
            CurrentQuestStepIndex = 0;
            this._questStepStates = new QuestStepState[info.QuestStepsPrefabs.Length];

            for (int i = 0; i < _questStepStates.Length; i++)
            {
                _questStepStates[i] = new QuestStepState();
            }
        }

        public Quest(SO_QuestSystemInfo info, EQuestState state, int currentQuestStepIndex,
            QuestStepState[] questStepStates)
        {
            Info = info;
            State = state;
            this._questStepStates = questStepStates;
            CurrentQuestStepIndex = currentQuestStepIndex;
            
            // If the quest sptep states and prefabs are of diffrent lengths
            if (this._questStepStates.Length != info.QuestStepsPrefabs.Length)
            {
                Debug.LogWarning($"Quest Step Prefabs and Quest Step States are of dirrent length." +
                                 $"This indicates something changed woth the QuestInfo and the save data is now out of sync." +
                                 $"Reset your data as this might cause an issue." +
                                 $"QuestID: {this.Info.ID}");
            }
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
                // TODO: Object Pooling for QuestSteps
                QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform)
                    .GetComponent<QuestStep>();
                questStep.InitializeQuestStep(Info.ID, CurrentQuestStepIndex, _questStepStates[CurrentQuestStepIndex].state);
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

        public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
        {
            if (stepIndex < _questStepStates.Length)
            {
                _questStepStates[stepIndex].state = questStepState.state;
            }
            else
            {
                Debug.LogWarning($"Tried to access quest step data, but stepIndex out of range." +
                                 $"Quest ID: {Info.ID}, StepIndex: {stepIndex} ");
            }
        }

        public QuestData GetQuestData()
        {
            return new QuestData(State, CurrentQuestStepIndex, _questStepStates);
        }
    }
}

