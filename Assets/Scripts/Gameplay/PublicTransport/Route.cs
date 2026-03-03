using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PublicTransport
{
    public class Route : MonoBehaviour
    {
        [SerializeField] private List<Stop> stops;

        private int _currentStop;
        
        public Vector3 GetNextStopLocation()
        {
            _currentStop++;
            return stops[(_currentStop % stops.Count + stops.Count) % stops.Count].transform.position;
        }
    }
}