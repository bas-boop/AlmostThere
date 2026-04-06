using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class PhoneMover : MonoBehaviour
    {
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private float offscreenDistance;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private RectTransform _rect;
        private Vector2 _centerPos;
        private Vector2 _hiddenPos;
        private Coroutine _current;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _centerPos = _rect.anchoredPosition;
            _hiddenPos = _centerPos - new Vector2(0, offscreenDistance);
            _rect.anchoredPosition = _hiddenPos;
        }

        public void SlideIn()
        {
            if (_current != null)
                StopCoroutine(_current);
            
            _current = StartCoroutine(Animate(_hiddenPos, _centerPos));
        }

        public void SlideOut()
        {
            if (_current != null)
                StopCoroutine(_current);
            
            _current = StartCoroutine(Animate(_centerPos, _hiddenPos));
        }

        private System.Collections.IEnumerator Animate(Vector2 from, Vector2 to)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(Mathf.Clamp01(elapsed / duration));
                _rect.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
                yield return null;
            }

            _rect.anchoredPosition = to;
            _current = null;
        }
    }
}