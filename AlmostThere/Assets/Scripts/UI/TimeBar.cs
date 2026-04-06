using UnityEngine;

using Framework;
using Framework.Extensions;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class TimeBar : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Timer timer;
        [SerializeField] private float width = 2;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update() => SetWidth(timer.GetTimePercentage());

        public void SetWidth(float target)
        {
            target = Mathf.Clamp01(target);
            
            Vector3 newScale = rectTransform.localScale;
            newScale.SetX(target * width);
            rectTransform.localScale = newScale;
        }
    }
}