using UnityEngine;

using Framework;
using Framework.Extensions;
using UnityEngine.Events;

namespace Gameplay.PublicTransport
{
    public class TransitVehicle : MonoBehaviour
    {
        private const float STOP_RANGE = 0.1f;
        
        [SerializeField] private Route route;
        [SerializeField] private Timer timer;
        [SerializeField] private float speed = 1;
        [SerializeField] private bool shouldMove;
        [SerializeField] private bool isCanceled;

        [SerializeField] private UnityEvent onCancel = new();

        private float _currentSpeed;
        private Waypoint _currentStop;

        public Route Route => route;

        private void Awake()
        {
            route.Init(speed);
            _currentSpeed = speed;
            shouldMove = true;
        }

        private void Start()
        {
            route.onCancelRoute = onCancel;
            _currentStop = route.GetNextStopLocation();
        }

        private void Update()
        {
            if (isCanceled)
                return;
            
            if (!shouldMove)
                return;

            Vector3 currentStopPosition = _currentStop.transform.position;
            
            if (transform.position.IsWithinRange(currentStopPosition, STOP_RANGE))
                UpdateCurrentStop();
            
            Vector3 dir = currentStopPosition - transform.position;
            transform.Translate(dir.normalized * (_currentSpeed * Time.deltaTime), Space.World);
        }

        public void Move()
        {
            shouldMove = true;
        }

        public void Cancel()
        {
            isCanceled = true;
            onCancel?.Invoke();
        }
        
        private void UpdateCurrentStop()
        {
            if (_currentStop is Stop)
                Stop();
            
            _currentStop = route.GetNextStopLocation();
        }

        private void Stop()
        {
            shouldMove = false;
            timer.RestartTimer();
        }
    }
}