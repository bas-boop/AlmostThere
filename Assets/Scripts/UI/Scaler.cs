using UnityEngine;

using Framework;

namespace UI
{
    public sealed class Scaler : MonoBehaviour
    {
        [SerializeField] private Timer timer;
        [SerializeField] private float startScale = 10;
        [SerializeField] private float multiplier = 1;

        private float _currentScale;

        private void Start()
        {
            _currentScale = startScale;
            transform.localScale = new(_currentScale, 1, 1);
        }

        private void Update()
        {
            float normalizedTime = Mathf.Clamp01(timer.GetCurrentTime() / timer.GetMaxTime());
            _currentScale = normalizedTime * startScale * multiplier;
            
            transform.localScale = new(_currentScale, 1, 1);
        }
    }
}