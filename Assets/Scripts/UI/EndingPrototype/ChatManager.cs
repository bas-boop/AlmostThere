using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private RectTransform chatContent;
    [SerializeField] private GameObject messageRowPrefab;
    [SerializeField] private ScrollRect scrollRect;

    public void SendMessage(MessageData messageInfo)
    {
        GameObject row = Instantiate(messageRowPrefab, chatContent);

        MessageRowUI rowUI = row.GetComponent<MessageRowUI>();
        rowUI.SetSide(messageInfo.sent);

        MessageBubble bubble = row.GetComponentInChildren<MessageBubble>();
        bubble.SetMessage(messageInfo.message, messageInfo.imageSprite, messageInfo.doesNotHaveText);

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
