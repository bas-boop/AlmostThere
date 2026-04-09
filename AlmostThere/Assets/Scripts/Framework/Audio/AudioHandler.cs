using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Framework.Audio
{
    public class AudioHandler : MonoBehaviour
    {
        [SerializeField] private EventReference eventRef;
        [SerializeField] private string parameterName;
        [SerializeField] private bool playOnStart;
        [SerializeField] private bool shouldLoop;

        private EventInstance _instance;
        private bool _isPlaying;

        private void Start()
        {
            if (playOnStart)
                ForcePlay();
        }

        private void Update()
        {
            if (!_instance.isValid())
                return;
            
            UpdateAudioPosition();

            if (shouldLoop
                && _isPlaying)
                CheckAndLoop();
        }

        private void OnDisable() => Stop();

        public void Play()
        {
            if (_isPlaying)
                return;

            _instance = RuntimeManager.CreateInstance(eventRef);
            
            UpdateAudioPosition();
            _instance.start();
            
            _isPlaying = true;
        }
        
        public void ForcePlay()
        {
            Stop();
            
            _instance = RuntimeManager.CreateInstance(eventRef);
            
            UpdateAudioPosition();
            _instance.start();
            
            _isPlaying = true;
        }

        public void Stop()
        {
            if (!_instance.isValid())
                return;

            _instance.stop(STOP_MODE.IMMEDIATE);
            _instance.release();
            _isPlaying = false;
        }

        public void SetParamValue(float value)
        {
            if (_instance.isValid())
                _instance.setParameterByName(parameterName, value);
        }

        private void CheckAndLoop()
        {
            _instance.getPlaybackState(out PLAYBACK_STATE state);

            if (state == PLAYBACK_STATE.STOPPED)
                ForcePlay();
        }

        private void UpdateAudioPosition()
        {
            _instance.set3DAttributes(transform.To3DAttributes());
        }
    }
}