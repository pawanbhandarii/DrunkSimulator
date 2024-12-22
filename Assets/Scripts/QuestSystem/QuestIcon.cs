using System;
using UnityEngine;

namespace DrunkSimulator.Quest
{
    public class QuestIcon : MonoBehaviour
    {
        [Header("Icons")] 
        [SerializeField] private GameObject requirementNotMeetToStartIcon;
        [SerializeField] private GameObject canStartIcon;
        [SerializeField] private GameObject requirementNotMeetToFinishIcon;
        [SerializeField] private GameObject canEndIcon;

        private void Update()
        {
            Rotate();
        }

        public void SetState(EQuestState newState, bool startPoint, bool endPoint)
        {
            //Initially disable all the icons
            requirementNotMeetToStartIcon.SetActive(false);
            requirementNotMeetToFinishIcon.SetActive(false);
            canStartIcon.SetActive(false);
            canEndIcon.SetActive(false);
            
            //View icons basis on the state
            switch (newState)
            {
                case EQuestState.REQUIREMNT_NOT_MET:
                    if (startPoint)
                    {
                        requirementNotMeetToStartIcon.SetActive(true);
                    }
                    break;
                
                case EQuestState.CAN_START:
                    if (startPoint)
                    {
                        canStartIcon.SetActive(true);
                    }
                    break;
                
                case EQuestState.IN_PROGRESS:
                    if (endPoint)
                    {
                        requirementNotMeetToFinishIcon.SetActive(true);
                    }
                    break;
                
                case EQuestState.CAN_FINISH:
                    if (endPoint)
                    {
                        canEndIcon.SetActive(true);
                    }
                    break;
                
                case EQuestState.FINISHED:
                    break;
                
                default:
                    Debug.LogWarning("Unknown quest state: " + newState);
                    break;
            }
        }

        private void LookAtCamera()
        {
            Vector3 targetDirection = Camera.main.transform.forward;
            
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                20 * Time.deltaTime
            );
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }
    }
}