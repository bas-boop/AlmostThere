using UnityEngine;

namespace Gameplay.PublicTransport
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Waypoint : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = false;
        }
    }
}