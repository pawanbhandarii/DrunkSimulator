
using UnityEngine;

namespace DrunkSimulator.Quest
{
    [RequireComponent(typeof(SphereCollider))]
    public class QuestPoint : MonoBehaviour
    {
        [Header("Quest")] [SerializeField] private SO_QuestSystemInfo questInfoForPoint;

        private string _questID;
        
        private bool _playerIsNear = false;

        private EQuestState currentQuestState;

        private EventBinding<OnQuestStateChangeEvent> OnQuestStateChange;
        
        private void Awake()
        {
            _questID = questInfoForPoint.ID;
        }

        private void OnEnable()
        {
            OnQuestStateChange = new EventBinding<OnQuestStateChangeEvent>(QuestStateChanged);
            EventBus<OnQuestStateChangeEvent>.Register(OnQuestStateChange);
        }

        private void OnDisable()
        {
            EventBus<OnQuestStateChangeEvent>.Deregister(OnQuestStateChange);
        }

        private void Update()
        {
            if (!_playerIsNear) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventBus<OnStartQuestEvent>.Raise(new OnStartQuestEvent()
                {
                    questID = _questID
                });
                EventBus<OnAdvanceQuestEvent>.Raise(new OnAdvanceQuestEvent()
                {
                    questID = _questID
                });
                EventBus<OnFinishQuestEvent>.Raise(new OnFinishQuestEvent()
                {
                    questID = _questID
                });
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerIsNear = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerIsNear = false;
            }
        }

        private void QuestStateChanged(OnQuestStateChangeEvent e)
        {
            if (e.quest.Info.ID.Equals(_questID))
            {
                currentQuestState = e.quest.State;
                Debug.Log($"Quest with ID: {_questID} Updated to State: {currentQuestState}");
            }
        }
    }
}

