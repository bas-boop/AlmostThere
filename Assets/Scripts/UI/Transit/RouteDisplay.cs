using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Framework.Extensions;
using Gameplay.PublicTransport;

namespace UI.Transit
{
    public sealed class RouteDisplay : MonoBehaviour
    {
        private const string TIME_FORMAT = @"hh\:mm";

        [SerializeField] private TransitVehicle transitVehicle;
        [SerializeField, Tooltip("Time in minutes")] private float startTime = 540; // 9 AM
        [SerializeField] private float timeScale = 60;
        [SerializeField] private List<TMP_Text> times;
        [SerializeField] private List<string> stopNames;
        [SerializeField] private GameObject timePrefab;

        private Route _transitRoute;

        private void Start()
        {
            _transitRoute = transitVehicle.Route;
            CreatTimeObjects();
            
            SetupTimes();
        }

        private void CreatTimeObjects()
        {
            int l = _transitRoute.TimeBetweenStops.Count;
            
            for (int i = 0; i < l; i++)
            {
                GameObject go = Instantiate(timePrefab, transform);
                
                //if (go.TryGetComponent(out TMP_Text text))
                if (go.TryGetComponentInChildren(out TMP_Text text))
                    times.Add(text);
            }
        }

        private void SetupTimes()
        {
            float totalTime = 0;
            int l = _transitRoute.TimeBetweenStops.Count;
            
            for (int i = 0; i < l; i++)
            {
                TMP_Text timeText = times[i];
                totalTime += _transitRoute.TimeBetweenStops[i];
                timeText.text = $"{GetTime(totalTime / timeScale + startTime)} - {stopNames[i]}";
            }
        }

        private string GetTime(float time)
        {
            TimeSpan ts = TimeSpan.FromMinutes(Mathf.RoundToInt(time));
            return ts.ToString(TIME_FORMAT);
        }
    }
}