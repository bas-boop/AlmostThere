using UnityEngine;

namespace Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Walking : MonoBehaviour
    {
        [SerializeField] public float speed = 1;
        [SerializeField] private float rotationSpeed = 1;
        [SerializeField] private Transform bike;
        
        private Vector2 _moveDirection;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void FixedUpdate() => Move();

        public void SetMoveDirection(Vector2 targetDirection) => _moveDirection = targetDirection;

        private void Move()
        {
            Quaternion yRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            Vector3 localMove = new(_moveDirection.x, 0, _moveDirection.y);
            Vector3 worldMove = yRotation * localMove;

            Vector3 newVelocity = _rigidbody.linearVelocity;
            newVelocity.x = worldMove.x * speed;
            newVelocity.z = worldMove.z * speed;

            _rigidbody.linearVelocity = newVelocity;

            if (worldMove.sqrMagnitude <= 0.001f)
                return;
            
            Quaternion targetRotation = Quaternion.LookRotation(worldMove);
            bike.rotation = Quaternion.Slerp(bike.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}