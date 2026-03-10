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
        private const string TIME_FORMAT_HOUR = @"hh\:mm";
        private const string TIME_FORMAT2 = @"mm\m";

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
            _transitRoute.onCancelRoute.AddListener(Canel);
            _transitRoute.onDelayRoute.AddListener(Delay);
            CreatTimeObjects();
            
            if (_transitRoute.TimeBetweenStops.Count != stopNames.Count)
                Debug.LogWarning("The amount of stop names and actual stops are not the same."
                        + $" - {gameObject.name} has {_transitRoute.TimeBetweenStops.Count} and not {stopNames.Count}");
            
            SetupTimes();
        }

        private void CreatTimeObjects()
        {
            int l = _transitRoute.TimeBetweenStops.Count;
            
            for (int i = 0; i < l; i++)
            {
                GameObject go = Instantiate(timePrefab, transform);
                
                if (go.TryGetComponentInChildren(out TMP_Text text))
                    times.Add(text);
            }
        }

        private void SetupTimes(float extraTime = 0)
        {
            float totalTime = 0;
            int l = _transitRoute.TimeBetweenStops.Count;
            
            for (int i = 0; i < l; i++)
            {
                TMP_Text timeText = times[i];
                totalTime += _transitRoute.TimeBetweenStops[i];
                
                if (extraTime == 0)
                    timeText.text = $"{GetTime(totalTime / timeScale + startTime)} - {stopNames[i]}";
                else
                    timeText.text = $"{GetTime(totalTime / timeScale + startTime)} <color=\"red\">+{GetTime(extraTime, true)}</color> - {stopNames[i]}";
            }
        }

        private string GetTime(float time, bool minutsOnly = false)
        {
            TimeSpan ts = TimeSpan.FromMinutes(Mathf.RoundToInt(time));
            return ts.ToString(minutsOnly ? TIME_FORMAT2 : TIME_FORMAT_HOUR);
        }

        private void Delay(float time)
        {
            SetupTimes(time);
        }
        
        private void Canel()
        {
            for (int i = 0; i < times.Count; i++)
            {
                TMP_Text time = times[i];
                time.text = $"XX:XX - {stopNames[i]}";
                time.color = Color.red;
            }
        }
    }
}