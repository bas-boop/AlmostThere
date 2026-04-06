using UnityEngine;

namespace UI
{
    public sealed class LookAtCameraUi : MonoBehaviour
    {
        private Camera _mainCamera;
        
        private void Awake() => _mainCamera = Camera.main;

        private void Update() => transform.LookAt(_mainCamera.transform);
    }
}