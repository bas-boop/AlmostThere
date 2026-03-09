using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PublicTransport
{
    [Serializable]
    public struct Route
    {
        [SerializeField] private List<Waypoint> stops;

        private float _speed;
        private int _currentStop;
        
        [field:SerializeField] public List<float> TimeBetweenStops { get; private set; }

        public void Start(float speed)
        {
            _speed = speed;
            CalculateTimeBetweenStops();
            
            foreach (float timeBetweenStop in TimeBetweenStops)
            {
                Debug.Log(timeBetweenStop);
            }
        }

        public Waypoint GetNextStopLocation()
        {
            _currentStop++;
            return stops[(_currentStop % stops.Count + stops.Count) % stops.Count];
        }

        private void CalculateTimeBetweenStops()
        {
            int l = stops.Count;
            float segmentDistance = 0f;

            for (int i = 0; i < l; i++)
            {
                int next = (i + 1) % l;
                segmentDistance += (stops[next].transform.position - stops[i].transform.position).magnitude;

                if (stops[next] is not Stop)
                    continue;
                
                TimeBetweenStops.Add(segmentDistance / _speed);
                segmentDistance = 0f;
            }
        }
    }
}