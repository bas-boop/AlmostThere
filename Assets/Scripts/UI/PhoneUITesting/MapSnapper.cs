using System;
using UnityEngine;

namespace AlmostThere.Phone
{
    public class MapSnapper : MonoBehaviour
    {
        #region Serialized fields

        [Header("References")]
        [SerializeField] private RectTransform cursor;
        [SerializeField] private RectTransform map;
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform[] icons;

        [Space(10)]
        [SerializeField, Range(0, 300)] private float snapRadius;

        #endregion

        #region Private fields

        private RectTransform _snappedIcon;

        #endregion

        #region Properties

        public RectTransform SnappedTransform => _snappedIcon;

        #endregion

        #region Events

        public event Action<RectTransform> OnSnapped;
        public event Action OnReleased;

        #endregion

        #region Lifecycle methods

        private void Update()
        {
            if (_snappedIcon == null)
                CheckForSnap();
            else
                CheckForRelease();
        }

        #endregion

        #region Private methods

        private void CheckForSnap()
        {
            int iconCount = icons.Length;

            for (int i = 0; i < iconCount; i++)
            {
                Vector2 cursorPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, cursor.position);
                Vector2 iconPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, icons[i].position);
                float distance = Vector2.Distance(cursorPos, iconPos);

                if (distance < snapRadius)
                {
                    SnapToIcon(icons[i], cursorPos, iconPos);
                    break;
                }
            }
        }

        private void CheckForRelease()
        {
            Vector2 cursorPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, cursor.position);
            Vector2 iconPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, _snappedIcon.position);
            float distance = Vector2.Distance(cursorPos, iconPos);

            if (distance > snapRadius)
            {
                _snappedIcon = null;
                OnReleased?.Invoke();
            }
        }

        private void SnapToIcon(RectTransform icon, Vector2 cursorPos, Vector2 iconPos)
        {
            Vector2 screenOffset = cursorPos - iconPos;
            Vector2 localOffset = screenOffset / canvas.scaleFactor;

            map.anchoredPosition += localOffset;
            _snappedIcon = icon;
            OnSnapped?.Invoke(icon);
        }

        #endregion
    }
}