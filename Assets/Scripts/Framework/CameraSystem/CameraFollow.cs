using UnityEngine;

namespace Framework.CameraSystem
{
    [DefaultExecutionOrder(9999)] // camera follow should be later than movement to avoid jitter
    public sealed class CameraFollow : MonoBehaviour
    {
        private const float MOVE_THRESHOLD = 0.00001f;
        
        [Header("References")]
        [SerializeField] private Transform followTarget;
        
        [Header("Settings")]
        [SerializeField] private Vector3 cameraPositionOffset;
        [SerializeField, Range(0, 15)] private float lookAheadDistance = 3f;
        [SerializeField, Range(0, 15)] private float lookAheadLerpSpeed = 1.5f;
        [SerializeField, Range(0, 15)] private float followThreshold = 2f;
        [SerializeField, Range(0, 15)] private float stopThreshold = 0.75f;
        [SerializeField, Range(0, 15)] private float followLerpSpeed = 0.04f;
        [SerializeField, Range(0, 30)] private float stopLerpSpeed = 30f;
        [SerializeField, Range(0, 15)] private float lerpSpeedSmoothing = 1f;
        [SerializeField, Range(0, 15), Tooltip("When the camera stops moving and it has overshot, this is the speed we go back to the center.")]
        private float cameraReturnToCenterSpeed = 6.7f;

        private Camera _mainCamera;
        private bool _isFollowing;
        private float _currentLerpSpeed;
        private Vector3 _previousTargetPosition;
        private Vector3 _smoothedMoveDirection;
        private Vector3 _currentLookAheadOffset;

        private void Start()
        {
            _mainCamera = Camera.main;
            _previousTargetPosition = followTarget.position;
            _mainCamera.transform.position = followTarget.position + cameraPositionOffset;
        }

        private void LateUpdate()
        {
            UpdateAheadOffset();

            // overshoot position, in front of player
            Vector3 targetPosition = followTarget.position + cameraPositionOffset + _currentLookAheadOffset;
            float distance = (_mainCamera.transform.position - targetPosition).magnitude;

            UpdateIsFollowing(distance);
            MoveCamera(targetPosition);
        }

        private void UpdateAheadOffset()
        {
            Vector3 rawMove = followTarget.position - _previousTargetPosition;
            _previousTargetPosition = followTarget.position;

            if (rawMove.sqrMagnitude > MOVE_THRESHOLD)
            {
                Vector3 flatMove = new Vector3(rawMove.x, 0f, rawMove.z).normalized;
                _smoothedMoveDirection = Vector3.Lerp(
                    _smoothedMoveDirection,
                    flatMove,
                    lookAheadLerpSpeed * Time.deltaTime
                );
            }
            else
            {
                _smoothedMoveDirection = Vector3.Lerp(
                    _smoothedMoveDirection,
                    Vector3.zero,
                    followLerpSpeed * cameraReturnToCenterSpeed * Time.deltaTime
                );
            }
            
            _currentLookAheadOffset = _smoothedMoveDirection * lookAheadDistance;
        }

        private void UpdateIsFollowing(float distance)
        {
            if (!_isFollowing
                && distance >= followThreshold)
                _isFollowing = true;
            
            if (_isFollowing
                && distance <= stopThreshold)
                _isFollowing = false;
        }

        private void MoveCamera(Vector3 targetPosition)
        {
            float targetLerpSpeed = _isFollowing ? followLerpSpeed : stopLerpSpeed;
            _currentLerpSpeed = Mathf.Lerp(_currentLerpSpeed, targetLerpSpeed, lerpSpeedSmoothing * Time.deltaTime);

            _mainCamera.transform.position = Vector3.Lerp(
                _mainCamera.transform.position,
                targetPosition,
                _currentLerpSpeed * Time.deltaTime
            );
        }
    }
}