using System;
using UnityEngine;

namespace DrunkSimulator.Quest
{
    public abstract class QuestStep : MonoBehaviour
    {
        private bool isFinished = false;

        private string questID;
        private int stepIndex;

        public void InitializeQuestStep(string questID, int stepIndex, string questStepState)
        {
            this.questID = questID;
            this.stepIndex = stepIndex;
            
            if(!String.IsNullOrEmpty(questStepState))
            {
                SetQuestStepState(questStepState);
            }
        }
        
        protected void FinishQuestStep()
        {
            if (!isFinished)
            {
                isFinished = true;
                
                EventBus<OnAdvanceQuestEvent>.Raise(new OnAdvanceQuestEvent()
                {
                    questID = questID,
                });
                
                Destroy(this.gameObject);
            }
        }

        protected void ChangeState(string newState)
        {
            EventBus<OnQuestStepStateChangeEvent>.Raise(new OnQuestStepStateChangeEvent()
            {
                questID = questID,
                stepIndex = stepIndex,
                questStepState = new QuestStepState(newState)
            });
        }

        protected abstract void SetQuestStepState(string newState);
    }

}
