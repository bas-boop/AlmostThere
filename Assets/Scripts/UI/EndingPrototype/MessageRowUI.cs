using UnityEngine;
using UnityEngine.UI;

public class MessageRowUI : MonoBehaviour
{
    [SerializeField] private Transform bubble;
    [SerializeField] private Transform spacer;

    [SerializeField] private bool isRightTest = false;

    private LayoutElement spacerLayout;
    private LayoutElement bubbleLayout;

    private void Awake()
    {
        spacerLayout = spacer.gameObject.GetComponent<LayoutElement>();
        if (spacerLayout != null)
            spacerLayout = spacer.gameObject.AddComponent<LayoutElement>();

        bubbleLayout = bubble.gameObject.GetComponent<LayoutElement>();
        if (bubbleLayout != null)
            bubbleLayout = bubble.gameObject.AddComponent<LayoutElement>();
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
