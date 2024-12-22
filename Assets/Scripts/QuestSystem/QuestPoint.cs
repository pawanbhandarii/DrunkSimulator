
using UnityEngine;

namespace DrunkSimulator.Quest
{
    [RequireComponent(typeof(SphereCollider))]
    public class QuestPoint : MonoBehaviour
    {
        [Header("Quest")] [SerializeField] private SO_QuestSystemInfo questInfoForPoint;

        [Header("Config")] 
        [SerializeField] private bool _startPoint = true;
        [SerializeField] private bool _endPoint = true;

        private string _questID;
        
        private bool _playerIsNear = false;

        private EQuestState currentQuestState;

        private EventBinding<OnQuestStateChangeEvent> OnQuestStateChange;
        
        private QuestIcon questIcon;
        
        private void Awake()
        {
            _questID = questInfoForPoint.ID;
            questIcon = GetComponentInChildren<QuestIcon>();
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
                if (currentQuestState.Equals(EQuestState.CAN_START) && _startPoint)
                {
                    EventBus<OnStartQuestEvent>.Raise(new OnStartQuestEvent()
                    {
                        questID = questInfoForPoint.ID,
                    });
                }
                else if (currentQuestState.Equals(EQuestState.CAN_FINISH) && _endPoint)
                {
                    EventBus<OnFinishQuestEvent>.Raise(new OnFinishQuestEvent()
                    {
                        questID = questInfoForPoint.ID,
                    });
                }
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
                questIcon.SetState(currentQuestState, _startPoint, _endPoint);
            }
        }
    }
}

