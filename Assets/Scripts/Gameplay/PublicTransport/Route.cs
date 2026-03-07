using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PublicTransport
{
    [Serializable]
    public struct Route
    {
        [SerializeField] private List<Waypoint> stops;
        [SerializeField] private List<float> timeBetweenStops;

        private float _speed;
        private int _currentStop;

        public void Start(float speed)
        {
            _speed = speed;
            CalculateTimeBetweenStops();
            foreach (var VARIABLE in timeBetweenStops)
            {
                Debug.Log(VARIABLE);
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
                
                timeBetweenStops.Add(segmentDistance / _speed);
                segmentDistance = 0f;
            }
        }
    }
}