using TMPro;
using UnityEngine;

using Framework;

namespace UI
{
    public sealed class TimeUi : MonoBehaviour
    {
        [SerializeField] private float displayStartTime; // should be in minutes, eg 08:30
        [SerializeField] private float displayEndTime; // should be in minutes, eg 09:00
        [SerializeField] private float gameplayTime; // be in seconds, 90 seconds
        [SerializeField] private TMP_Text text;
        [SerializeField] private Timer timer;

        private void Awake()
        {
            timer.RestartTimer(gameplayTime);
        }

        private void Update() => UpdateTimeDisplay();

        private void UpdateTimeDisplay()
        {
            // Get normalized progress (0 = start, 1 = end) based on elapsed time
            float elapsed = gameplayTime - timer.GetCurrentTime();
            float t = Mathf.Clamp01(elapsed / gameplayTime);

            // Lerp between display start and end time (both in minutes)
            float currentMinutes = Mathf.Lerp(displayStartTime, displayEndTime, t);

            // Convert to hours and minutes
            int totalMinutes = Mathf.FloorToInt(currentMinutes);
            int minutes = totalMinutes % 60;
            
            // Also derive seconds within the current minute
            float minuteFraction = currentMinutes - totalMinutes;
            int seconds = Mathf.FloorToInt(minuteFraction * 60f);

            // Format as HH:MM:SS (or HH:MM if you prefer)
            text.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}