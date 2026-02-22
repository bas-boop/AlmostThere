using UnityEngine;

namespace Framework.CameraSystem
{
    [DefaultExecutionOrder(9999)] // camera follow should be later than movement to avoid jitter
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float followThreshold = 2f;
        [SerializeField] private float stopThreshold = 1f;
        [SerializeField] private float followLerpSpeed = 2f;
        [SerializeField] private float stopLerpSpeed = 1f;

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
            {
                _isFollowing = true;
                _currentLerpSpeed = followLerpSpeed;
            }

            if (_isFollowing
                && distance <= stopThreshold)
            {
                _isFollowing = false;
                _currentLerpSpeed = stopLerpSpeed;
            }

            if (!_isFollowing)
                return;

            _mainCamera.transform.position = Vector3.Lerp(
                _mainCamera.transform.position,
                targetPosition,
                _currentLerpSpeed * Time.deltaTime
            );
        }
    }
}