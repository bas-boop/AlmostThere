using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.EndingPrototype
{ 
    public class EndingUI : MonoBehaviour
    {
        [SerializeField] private EndingUIState winState;
        [SerializeField] private ChatManager chatManager;

        [SerializeField] private List<MessageData> winMessages;
        [SerializeField] private List<MessageData> loseMessages;

        private void OnEnable()
        {
            winState.OnWinOrLose += HandleEndingState;
        }

        private void HandleEndingState(EndingUIState.WinState state)
        {
            if (state == EndingUIState.WinState.WIN)
            {
                StartCoroutine(SendMessages(winMessages));
            }
            else
            {
                StartCoroutine(SendMessages(loseMessages));
            }
        }

        private IEnumerator SendMessages(List<MessageData> messageList)
        {
            for (int i = 0; i < messageList.Count; i++)
            {
                float randomWaitTime = Random.Range(0.5f, 1.5f);

                chatManager.SendMessage(messageList[i]);

                yield return new WaitForSeconds(randomWaitTime);    
            }
        }
    }
}

