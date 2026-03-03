using UnityEngine;

using Framework.Extensions;

namespace Gameplay.PublicTransport
{
    public class TransitVehicle : MonoBehaviour
    {
        [SerializeField] private Route route;
        [SerializeField] private float speed = 1;

        private Vector3 _lastStop;
        private Vector3 _currentStop;

        private void Update()
        {
            if (transform.position.IsWithinRange(_currentStop, 0.1f))
            {
                UpdateCurrentStop();
            }
            
            Vector3 dir = _currentStop - transform.position;
            transform.Translate(dir.normalized * (speed * Time.deltaTime), Space.World);
        }

        private void UpdateCurrentStop()
        {
            _lastStop = _currentStop;
            _currentStop = route.GetNextStopLocation();
            //todo: calculate the time between stops
            Debug.Log((_currentStop - _lastStop).magnitude);
        }
    }
}