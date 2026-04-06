using UnityEngine;

namespace UI
{
    public sealed class MapToggeler : MonoBehaviour
    {
        [SerializeField] private GameObject map;

        private bool _isVisible;
        
        public void Toggle()
        {
            _isVisible = !_isVisible;
            map.SetActive(_isVisible);
        }
    }
}