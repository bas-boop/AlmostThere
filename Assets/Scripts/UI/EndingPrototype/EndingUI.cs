using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework.Extensions;

namespace UI.EndingPrototype
{ 
    public class EndingUI : MonoBehaviour
    {
        [SerializeField] private ChatManager chatManager;
        [SerializeField] private Vector2 rangeRandomTimeBetweenChat = new (0.5f, 1.5f);

        [SerializeField] private MessageList winningMessages;
        [SerializeField] private MessageList losingMessages;

        public void HandleEndingState(int state) => HandleEndingState((WinState) state);
        
        public void HandleEndingState(WinState state)
        {
            switch (state)
            {
                case WinState.WIN:
                    StartCoroutine(SendMessages(winningMessages.All()));
                    break;
                case WinState.LOSE:
                default:
                    StartCoroutine(SendMessages(losingMessages.All()));
                    break;
            }
        }

        public void AddEventMessage(MessageData messageToAdd)
        {
            winningMessages.eventMessages.Add(messageToAdd);
            losingMessages.eventMessages.Add(messageToAdd);
        }

        private IEnumerator SendMessages(List<MessageData> messageList)
        {
            foreach (MessageData messageData in messageList)
            {
                float randomWaitTime = rangeRandomTimeBetweenChat.GetRandomInBetween();
                chatManager.SendMessage(messageData);
                yield return new WaitForSeconds(randomWaitTime);
            }
        }
    }
}

