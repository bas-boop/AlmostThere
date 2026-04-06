using UnityEngine;
using UnityEngine.Events;

namespace Player.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Movement : MonoBehaviour
    {
        private const float ROTATION_THRESHOLD = 0.001f;
        
        [SerializeField] private MovementSettings walkSetting;
        [SerializeField] private MovementSettings bikeSetting;
        [SerializeField] private Transform visuals;

        [SerializeField] private UnityEvent onDisableMovement = new();
        [SerializeField] private UnityEvent onEnableMovement = new();
        
        private MovementSettings _currentSettings;
        private Rigidbody _rigidbody;
        private Vector2 _moveDirection;
        private bool _canMove = true;
        private Quaternion _rotation;

        private void Awake()
        {
            _currentSettings = walkSetting;
            _rotation = transform.rotation;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.Sleep();
        }

        private void FixedUpdate()
        {
            if (_canMove)
                Move();
        }

        public void SetMoveDirection(Vector2 targetDirection) => _moveDirection = targetDirection;

        public void SwapMovementSettings() => _currentSettings = _currentSettings == walkSetting
            ? bikeSetting : walkSetting;

        public void ToggleCanMove()
        {
            _canMove = !_canMove;
            
            if (_canMove)
                onEnableMovement?.Invoke();
            else
            {
                _rigidbody.linearVelocity = Vector3.zero;
                onDisableMovement?.Invoke();
            }
        }

        public void ResetRotation() => transform.rotation = _rotation;

        private void Move()
        {
            Quaternion yRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Vector3 localMove = new(_moveDirection.x, 0, _moveDirection.y);
            Vector3 worldMove = yRotation * localMove;

            Vector3 newVelocity = _rigidbody.linearVelocity;
            newVelocity.x = worldMove.x * _currentSettings.movementSpeed;
            newVelocity.z = worldMove.z * _currentSettings.movementSpeed;

            _rigidbody.linearVelocity = newVelocity;

            if (worldMove.sqrMagnitude <= ROTATION_THRESHOLD)
                return;
            
            Quaternion targetRotation = Quaternion.LookRotation(worldMove);
            visuals.rotation = Quaternion.Slerp(visuals.rotation, targetRotation, Time.fixedDeltaTime * _currentSettings.rotationSpeed);
        }
    }
}