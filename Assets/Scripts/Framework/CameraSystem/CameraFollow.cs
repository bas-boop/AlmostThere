using UnityEngine;

namespace Framework.CameraSystem
{
    [DefaultExecutionOrder(9999)] // camera follow should be later than movement to avoid jitter
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float followThreshold = 2;
        [SerializeField] private float lerpSpeed = 2;
        
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            _mainCamera.transform.position = followTarget.position + offset;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = followTarget.position + offset;

            if ((_mainCamera.transform.position - targetPosition).magnitude <= followThreshold)
                return;

            _mainCamera.transform.position = Vector3.Lerp(
                _mainCamera.transform.position,
                targetPosition,
                lerpSpeed * Time.deltaTime
            );
        }
    }
}