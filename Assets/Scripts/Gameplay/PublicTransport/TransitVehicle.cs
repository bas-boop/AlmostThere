using UnityEngine;
using UnityEngine.Events;

using Framework;
using Framework.Extensions;

namespace Gameplay.PublicTransport
{
    public class TransitVehicle : MonoBehaviour
    {
        private const float STOP_RANGE = 0.1f;
        private const float ROTATION_THRESHOLD = 0.001f;
        
        [Header("References")]
        [SerializeField] private Route route;
        [SerializeField] private Timer timerStop;
        [SerializeField] private Timer timerDelay;
        [SerializeField] private GameObject tempDoor;
        
        [Header("Attributes")]
        [SerializeField] private float moveSpeed = 1;
        [SerializeField] private float rotationSpeed = 1;
        [SerializeField] private Vector2 randomDelayTime = Vector2.one;
        [SerializeField] private bool shouldMove;
        [SerializeField] private bool isCanceled;

        [Header("Events")]
        [SerializeField] private UnityEvent onCancel = new();
        [SerializeField] private UnityEvent<float> onDelay = new();
        [SerializeField] private UnityEvent onReachedStop = new();
        [SerializeField] private UnityEvent onLeaveStop = new();

        private float _currentSpeed;
        private Waypoint _currentStop;

        public Route Route => route;

        private void Awake()
        {
            route.Init(moveSpeed);
            _currentSpeed = moveSpeed;
            shouldMove = true;
        }

        private void Start()
        {
            route.onCancelRoute = onCancel;
            route.onDelayRoute = onDelay;
            _currentStop = route.GetNextStopLocation();
        }

        private void Update()
        {
            if (isCanceled
                || !shouldMove)
                return;

            Vector3 currentStopPosition = _currentStop.transform.position;
            
            if (transform.position.IsWithinRange(currentStopPosition, STOP_RANGE))
                UpdateCurrentStop();
            
            UpdateLocationAndRotation(currentStopPosition);
        }
        
        public void Move()
        {
            shouldMove = true;
            Door(false);
            onLeaveStop?.Invoke();
        }

        public void Cancel()
        {
            isCanceled = true;
            onCancel?.Invoke();
        }

        public void Delay()
        {
            float delayTime = randomDelayTime.GetRandomInBetween();
            timerDelay.RestartTimer(delayTime);
            onDelay?.Invoke(delayTime);
            Stop(timerDelay);
        }
        
        private void UpdateCurrentStop()
        {
            if (_currentStop is Stop)
                Stop(timerStop);
            
            _currentStop = route.GetNextStopLocation();
        }

        private void UpdateLocationAndRotation(Vector3 currentStopPosition)
        {
            Vector3 dir = currentStopPosition - transform.position;
            transform.Translate(dir.normalized * (_currentSpeed * Time.deltaTime), Space.World);
    
            Vector3 flatDir = new (dir.x, 0, dir.z);

            if (flatDir.sqrMagnitude <= ROTATION_THRESHOLD)
                return;
    
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        private void Stop(Timer timer)
        {
            shouldMove = false;
            Door(true);
            timer.RestartTimer();
            onReachedStop?.Invoke();
        }

        private void Door(bool shouldOpen)
        {
            tempDoor.SetActive(!shouldOpen);
        }
    }
}