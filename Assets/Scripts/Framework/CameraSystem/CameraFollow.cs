using UnityEngine;

namespace Framework.CameraSystem
{
    [DefaultExecutionOrder(9999)] // camera follow should be later than movement to avoid jitter
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 overshoot;
        [SerializeField, Range(0, 15)] private float followThreshold = 2f;
        [SerializeField, Range(0, 15)] private float stopThreshold = 0.5f;
        [SerializeField, Range(0, 15)] private float followLerpSpeed = 5f;
        [SerializeField, Range(0, 15)] private float stopLerpSpeed = 2f;
        [SerializeField, Range(0, 15)] private float lerpSpeedSmoothing = 3f;

        private Camera _mainCamera;
        private bool _isFollowing;
        private float _currentLerpSpeed;

        private void Start()
        {
            _mainCamera = Camera.main;
            _mainCamera.transform.position = followTarget.position + offset;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = followTarget.position + offset;
            float distance = (_mainCamera.transform.position - targetPosition).magnitude;

            if (!_isFollowing
                && distance >= followThreshold)
                _isFollowing = true;

            if (_isFollowing
                && distance <= stopThreshold)
                _isFollowing = false;

            float targetLerpSpeed = _isFollowing ? followLerpSpeed : stopLerpSpeed;
            _currentLerpSpeed = Mathf.Lerp(_currentLerpSpeed, targetLerpSpeed, lerpSpeedSmoothing * Time.deltaTime);

            if (distance > stopThreshold)
            {
                targetPosition -= overshoot;
                
                _mainCamera.transform.position = Vector3.Lerp(
                    _mainCamera.transform.position,
                    targetPosition,
                    _currentLerpSpeed * Time.deltaTime
                );
            }
        }
    }
}