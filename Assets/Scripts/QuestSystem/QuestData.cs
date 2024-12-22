namespace DrunkSimulator.Quest
{
    [System.Serializable]
    public class QuestData
    {
        public EQuestState state;
        public int questStepIndex;
        public QuestStepState[] questStepStates;

        public QuestData(EQuestState state, int questStepIndex, QuestStepState[] questStepStates)
        {
            this.state = state;
            this.questStepIndex = questStepIndex;
            this.questStepStates = questStepStates;
        }
    }
}