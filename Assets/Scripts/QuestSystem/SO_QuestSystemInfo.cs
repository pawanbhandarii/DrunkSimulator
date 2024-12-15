using System;
using UnityEngine;

namespace DrunkSimulator.Quest
{
    [CreateAssetMenu(fileName = "newQuestSystemInfo", menuName = "Scriptable Objects/Quests/QuestSystemInfo", order = 1)]
    public class SO_QuestSystemInfo : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }

        [Header("General")] public string DisplayName;

        [Header("Requirements")] 
        public int AlcholRequirement;
        public SO_QuestSystemInfo[] QuestPrerequisites;
    
        [Header("Steps")]
        public GameObject[] QuestStepsPrefabs;

        [Header("Rewards")] public int GoldReward;  

        private void OnValidate()
        {
#if UNITY_EDITOR
            ID = this.name;
            UnityEditor.EditorUtility.SetDirty((this));
#endif
        }
    }
}

