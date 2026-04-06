using UnityEngine;

namespace UI
{
    public sealed class UiMover : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform mapRect; // reference to the map UI element
        [SerializeField] private Vector2 offset;
        [SerializeField] private float moveScale = 1;

        private void Update()
        {
            Vector2 worldPos = new Vector2(followTarget.position.x, followTarget.position.z);
            rectTransform.SetParent(mapRect); // ensure it's a child of the map
            rectTransform.anchoredPosition = worldPos * moveScale + offset;
        }
    }
}