using System;
using UnityEngine;

using Framework;
using Framework.Extensions;

namespace Gameplay.PublicTransport
{
    public class TransitVehicle : MonoBehaviour
    {
        private const float STOP_RANGE = 0.1f;
        
        [SerializeField] private Route route;
        [SerializeField] private Timer timer;
        [SerializeField] private float speed = 1;
        [SerializeField] private bool shouldMove;

        private float _currentSpeed;
        private Waypoint _currentStop;

        public Route Route => route;

        private void Awake()
        {
            route.Start(speed);
            _currentSpeed = speed;
            shouldMove = true;
        }

        private void Start()
        {
            _currentStop = route.GetNextStopLocation();
        }

        private void Update()
        {
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