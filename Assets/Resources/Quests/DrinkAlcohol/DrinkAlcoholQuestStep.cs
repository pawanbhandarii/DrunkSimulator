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
            _amountOfAlcoholDrunk += eventData.alcholAmount;

            if (_amountOfAlcoholDrunk >= amountOfAlcoholToDrink)
            {
                FinishQuestStep();
            }
        }
        
    }
}

