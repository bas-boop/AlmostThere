using UnityEngine;
using UnityEngine.Events;

using Framework;
using Framework.Audio;
using Framework.Extensions;
using Player;

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

        [Header("Audio")]
        [SerializeField] private AudioHandler startAudio;
        [SerializeField] private AudioHandler stopAudio;
        [SerializeField] private AudioHandler engineAudio;
        [SerializeField] private AudioHandler hornAudio;
        [SerializeField] private float accelSharpness = 2f;   // how fast it ramps up
        [SerializeField] private float decelSharpness = 3f;   // how fast it ramps down
        [SerializeField] private float maxAudioDistance = 20f;
        
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

        private Transform _player;
        private Waypoint _currentStop;
        private float _currentSpeed;
        private float _fakeAcceleration;
        private float _targetAcceleration;

        public Route Route => route;

        private void Awake()
        {
            route.Init(moveSpeed);
            _currentSpeed = moveSpeed;
            shouldMove = true;
        }

        private void Start()
        {
            _player = PlayerVisuals.Instance.transform;
            
            route.onCancelRoute = onCancel;
            route.onDelayRoute = onDelay;
            _currentStop = route.GetNextStopLocation();
            Move();
        }

        private void Update()
        {
            UpdateEngineAudio();

            if (isCanceled
                || !shouldMove)
                return;
            
            Vector3 currentStopPosition = _currentStop.transform.position;
            engineAudio.Play();
            
            if (transform.position.IsWithinRange(currentStopPosition, STOP_RANGE))
                UpdateCurrentStop();
            
            UpdateLocationAndRotation(currentStopPosition);
        }

        private void UpdateEngineAudio()
        {
            float distanceToPlayer = Vector3.Distance(_player.position, transform.position);
            float distanceFactor = 1f - Mathf.Clamp01(distanceToPlayer / maxAudioDistance);
            float sharpness = _targetAcceleration > _fakeAcceleration ? accelSharpness : decelSharpness;
            _fakeAcceleration = Mathf.Lerp(_fakeAcceleration, _targetAcceleration, Time.deltaTime * sharpness);
            float finalValue = _fakeAcceleration * distanceFactor;
            engineAudio.SetParamValue(finalValue);
        }

        public void Move()
        {
            shouldMove = true;
            Door(false);
            startAudio.Play();
            _targetAcceleration = 1;
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
            _targetAcceleration = 0;
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
            stopAudio.Play();
            onReachedStop?.Invoke();
        }

        private void Door(bool shouldOpen)
        {
            tempDoor.SetActive(!shouldOpen);
        }
    }
}