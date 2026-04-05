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

        public void HandleEndingState(int state) => HandleEndingState((WinState) state);
        
        public void HandleEndingState(WinState state)
        {
            switch (state)
            {
                case WinState.WIN:
                    StartCoroutine(SendMessages(winMessages));
                    break;
                case WinState.LOSE:
                default:
                    StartCoroutine(SendMessages(loseMessages));
                    break;
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

