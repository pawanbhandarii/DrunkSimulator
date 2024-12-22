using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class DrinkAlcoholQuestStep : QuestStep
    {
        [Header("Drink Alcohol")] public float amountOfAlcoholToDrink;
        
        private float _amountOfAlcoholDrunk;
        
        private EventBinding<DrankAlcoholEvent> _drinkAlcoholEvent;

        private void OnEnable()
        {
            _drinkAlcoholEvent = new EventBinding<DrankAlcoholEvent>(OnAlcoholDrank);
            EventBus<DrankAlcoholEvent>.Register(_drinkAlcoholEvent);
        }

        private void OnDisable()
        {
            EventBus<DrankAlcoholEvent>.Deregister(_drinkAlcoholEvent);
        }

        private void OnAlcoholDrank(DrankAlcoholEvent eventData)
        {
            if (_amountOfAlcoholDrunk < amountOfAlcoholToDrink)
            {
                _amountOfAlcoholDrunk += eventData.alcholAmount;
                UpdateState();
            }

            if (_amountOfAlcoholDrunk >= amountOfAlcoholToDrink)
            {
                FinishQuestStep();
            }
        }

        private void UpdateState()
        {
            string state = _amountOfAlcoholDrunk.ToString();
            ChangeState(state);
        }

        protected override void SetQuestStepState(string newState)
        {
            //TODO: try catch block
            this._amountOfAlcoholDrunk = System.Int32.Parse(newState);
            UpdateState();
        }
    }
}

