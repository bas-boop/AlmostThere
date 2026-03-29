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

        private EventInstance instance;
        
        public void Play()
        {
            Stop();

            instance = RuntimeManager.CreateInstance(eventRef);
            instance.start();
        }

        public void Stop()
        {
            if (instance.isValid())
                instance.stop(STOP_MODE.IMMEDIATE);
        }

        public void SetParamValue(float value)
        {
            if (instance.isValid())
                instance.setParameterByName(parameterName, value);
        }
    }
}