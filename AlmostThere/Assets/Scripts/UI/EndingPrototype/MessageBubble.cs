using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EndingPrototype 
{ 
    [RequireComponent(typeof(ContentSizeFitter))]
    [RequireComponent(typeof(LayoutGroup))]
    public class MessageBubble : MonoBehaviour
    {
        public Color backgroundColor;

        [SerializeField, Range(100, 350)] private float maxWidth = 220f;

        [SerializeField] private RectTransform background;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image messageImage;

        public void SetMessage(string text, Color messageBackgroundColor, Sprite sprite = null, bool doesNotHaveText = false)
        {

            background.gameObject.GetComponent<Image>().color = messageBackgroundColor;

            messageImage.gameObject.SetActive(false);

            if (doesNotHaveText)
                messageText.gameObject.SetActive(false);

            bool hasImage = false;

            if (messageText != null)
            {
                messageText.text = text;
            }

            if (sprite != null)
            {
                messageImage.sprite = sprite;
                hasImage = true;
                messageImage.gameObject.SetActive(true);
            }

            ApplyWidth(hasImage);
        }
        
        private void ApplyWidth(bool hasImage)
        {
            float paddingHorizontal = GetComponent<LayoutGroup>().padding.horizontal;
            float paddingVertical = GetComponent<LayoutGroup>().padding.vertical;
            float targetWidthText = Mathf.Min(messageText.preferredWidth + paddingHorizontal, maxWidth);
            float targetWidthImage = maxWidth;

            float targetWidth = hasImage ? targetWidthImage : targetWidthText;

            background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);

            if (hasImage && !messageText.gameObject.activeSelf)
            {
                background.gameObject.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
                messageImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth - paddingHorizontal);
                messageImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetWidth - paddingVertical);
                background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageImage.rectTransform.rect.height + (paddingVertical * 3));
            }
            else if (hasImage && messageText.gameObject.activeSelf)
            {
                messageImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth - paddingHorizontal);
                messageImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetWidth - paddingVertical);
                messageText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth - paddingHorizontal);
                background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageImage.rectTransform.rect.height + messageText.preferredHeight + (paddingVertical * 3));
            }
            else if (!hasImage && messageText.gameObject.activeSelf)
            {
                messageText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth - paddingHorizontal);
                background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageText.preferredHeight + paddingVertical);
            }
        }
    }
}
