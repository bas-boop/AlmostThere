using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EndingPrototype
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private RectTransform chatContent;
        [SerializeField] private GameObject messageRowPrefab;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Color sendColor;
        [SerializeField] private Color recieveColor;

        public void SendMessage(MessageData messageInfo)
        {
            bool isSend = messageInfo.isPlayerSendingMessage;
            Color backgroundColor = isSend ? sendColor : recieveColor;

            GameObject row = Instantiate(messageRowPrefab, chatContent);

            MessageRowUI rowUI = row.GetComponent<MessageRowUI>();
            rowUI.SetSide(messageInfo.isPlayerSendingMessage);

            MessageBubble bubble = row.GetComponentInChildren<MessageBubble>();
            bubble.SetMessage(messageInfo.message, backgroundColor, messageInfo.imageSprite, !messageInfo.hasText);

            StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return null;

            Canvas.ForceUpdateCanvases();

            LeanTween.value(gameObject, scrollRect.verticalNormalizedPosition, 0f, 0.3f).setEaseInCubic().setOnUpdate((float val) =>
            {
                scrollRect.verticalNormalizedPosition = val;
            });
        }
    }
}
