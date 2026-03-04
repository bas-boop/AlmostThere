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
        private float _acctualTime;

        private void Update()
        {
            if (transform.position.IsWithinRange(_currentStop, 0.1f))
            {
                UpdateCurrentStop();
            }
            
            Vector3 dir = _currentStop - transform.position;
            transform.Translate(dir.normalized * (speed * Time.deltaTime), Space.World);
            _acctualTime += Time.deltaTime;
        }

        private void UpdateCurrentStop()
        {
            _lastStop = _currentStop;
            _currentStop = route.GetNextStopLocation();
            
            float distance = (_currentStop - _lastStop).magnitude;
            //Debug.Log(distance);
            float time = distance / speed;
            Debug.Log(time + " " + _acctualTime);
            _acctualTime = 0;
        }
    }
}