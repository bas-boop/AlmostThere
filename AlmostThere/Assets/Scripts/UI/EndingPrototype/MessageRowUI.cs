using UnityEngine;
using UnityEngine.UI;

namespace UI.EndingPrototype
{
    public class MessageRowUI : MonoBehaviour
    {
        [SerializeField] private Transform bubble;
        [SerializeField] private Transform spacer;

        private LayoutElement spacerLayout;
        private LayoutElement bubbleLayout;

        private MessageBubble messageBubble;

        private void Awake()
        {
            spacerLayout = spacer.gameObject.GetComponent<LayoutElement>();
            if (spacerLayout != null)
                spacerLayout = spacer.gameObject.AddComponent<LayoutElement>();

            bubbleLayout = bubble.gameObject.GetComponent<LayoutElement>();
            if (bubbleLayout != null)
                bubbleLayout = bubble.gameObject.AddComponent<LayoutElement>();

            messageBubble = bubble.gameObject.GetComponent<MessageBubble>();
            if (messageBubble != null)
                messageBubble = bubble.gameObject.AddComponent<MessageBubble>();
        }

        public void SetSide(bool isRight)
        {
            spacerLayout.flexibleWidth = 1;
            spacerLayout.preferredWidth = -1;

            bubbleLayout.flexibleWidth = 0;

            if (isRight)
            {
                bubble.SetAsLastSibling();
                spacer.SetAsFirstSibling();
            }
            else
            {
                bubble.SetAsFirstSibling();
                spacer.SetAsLastSibling();
            }
        }
}
}
