using UI.Phonetesting;
using System.Collections;
using UnityEngine;

namespace UI.Phonetesting
{
    public class PhoneUIIndicator : MonoBehaviour
    {
        #region Enums

        private enum IndicatorState
        {
            HIDDEN,
            VISIBLE,
            HIDING,
            NEAR_ICON
        }

        #endregion

        #region Serialized fields

        [Header("References")]
        [SerializeField] private UIAnimationSetter uiAnimations;
        [SerializeField] private RectTransform iconPlaceholder;
        [SerializeField] private RectTransform mapMask;
        [SerializeField] private RectTransform offscreenIndicator;
        [SerializeField] private RectTransform offscreenIndicatorBackground;
        [SerializeField] private RectTransform offscreenIndicatorSpinner;
        [SerializeField] private RectTransform cursor;
        [SerializeField] private Canvas canvas;

        [Header("Indicator settings")]
        [SerializeField, Range(0, 1000)] private float indicatorMaxDistance = 500f;
        [SerializeField, Range(0, 1)] private float indicatorMinScale = 0.3f;
        [SerializeField, Range(0, 300)] private float indicatorThreshold = 50f;
        [SerializeField, Range(0, 500)] private float indicatorHideThreshold = 150f;

        #endregion

        #region Private fields

        private IndicatorState _indicatorState = IndicatorState.HIDDEN;
        private Coroutine _indicatorSpinner;
        private Coroutine _indicatorAppear;
        private bool _initialized = false;

        #endregion

        #region Lifecycle methods

        private void Start() => offscreenIndicator.localScale = Vector3.zero;

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the indicator and runs the first update.
        /// </summary>
        public void Initialize()
        {
            offscreenIndicator.localScale = Vector3.zero;
            _initialized = true;
            CanUpdate();
        }

        /// <summary>
        /// Ticks the indicator update if initialized.
        /// </summary>
        public void CanUpdate()
        {
            if (!_initialized)
                return;

            UpdateOffscreenIndicator();
        }

        #endregion

        #region Private methods

        private void UpdateOffscreenIndicator()
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, iconPlaceholder.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapMask, screenPos, null, out Vector2 localPoint);

            const float PADDING = 55f;

            Rect rectMask = mapMask.rect;
            float halfWidth = rectMask.width / 2f - PADDING;
            float halfHeight = rectMask.height / 2f - PADDING;

            bool isOutsideX = localPoint.x < -halfWidth || localPoint.x > halfWidth;
            bool isOutsideY = localPoint.y < -halfHeight || localPoint.y > halfHeight;
            bool isOutside = isOutsideX || isOutsideY;

            float distanceX = Mathf.Max(0, Mathf.Abs(localPoint.x) - halfWidth);
            float distanceY = Mathf.Max(0, Mathf.Abs(localPoint.y) - halfHeight);
            float distanceToEdge = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);

            Vector2 screenPosCursor = RectTransformUtility.WorldToScreenPoint(null, cursor.position);
            Vector2 screenPosIcon = RectTransformUtility.WorldToScreenPoint(null, iconPlaceholder.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapMask, screenPosCursor, null, out Vector2 cursorLocalPoint);
            float distanceToIcon = Vector2.Distance(screenPosCursor, screenPosIcon) / canvas.scaleFactor;

            if ((distanceToIcon < indicatorHideThreshold || !isOutside)
                && _indicatorState == IndicatorState.VISIBLE)
            {
                PlayHideAnimation();
            }
            else if (distanceToIcon >= indicatorThreshold
                && isOutside
                && (_indicatorState == IndicatorState.HIDDEN || _indicatorState == IndicatorState.NEAR_ICON))
            {
                PlayShowAnimation(distanceToEdge);
            }

            if (_indicatorState == IndicatorState.VISIBLE && _indicatorAppear == null)
            {
                float scaleFactor = Mathf.Lerp(1f, indicatorMinScale, Mathf.Clamp01(distanceToEdge / indicatorMaxDistance));
                offscreenIndicator.localScale = Vector3.one * scaleFactor;
            }

            if (isOutside
                && _indicatorState != IndicatorState.NEAR_ICON
                && _indicatorState != IndicatorState.HIDDEN)
            {
                Vector2 dir = (localPoint - cursorLocalPoint).normalized;
                Vector2 indicatorPos = GetEdgeIntersection(cursorLocalPoint, dir, halfWidth, halfHeight);

                offscreenIndicator.anchoredPosition = indicatorPos;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                offscreenIndicatorBackground.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void PlayHideAnimation()
        {
            _indicatorState = IndicatorState.HIDING;

            if (_indicatorSpinner != null)
            {
                StopCoroutine(_indicatorSpinner);
                _indicatorSpinner = null;
            }

            if (_indicatorAppear != null)
                StopCoroutine(_indicatorAppear);

            _indicatorAppear = StartCoroutine(uiAnimations.ScaleFromCurve(
                offscreenIndicator, uiAnimations.easeAnticipate, Vector2.zero, 0.4f,
                () => {
                    _indicatorState = IndicatorState.NEAR_ICON;
                    _indicatorAppear = null;
                    offscreenIndicator.localScale = Vector3.zero;
                }));
        }

        private void PlayShowAnimation(float distanceToEdge)
        {
            _indicatorState = IndicatorState.VISIBLE;

            if (_indicatorSpinner == null)
                _indicatorSpinner = StartCoroutine(uiAnimations.RotateAroundZ(offscreenIndicatorSpinner, 150));

            float scaleFactor = Mathf.Lerp(1f, indicatorMinScale, Mathf.Clamp01(distanceToEdge / indicatorMaxDistance));

            if (_indicatorAppear != null)
                StopCoroutine(_indicatorAppear);

            _indicatorAppear = StartCoroutine(uiAnimations.ScaleFromCurve(
                offscreenIndicator, uiAnimations.easeOvershoot, Vector2.one * scaleFactor, 0.3f,
                () => _indicatorAppear = null));
        }

        private Vector2 GetEdgeIntersection(
            Vector2 origin,
            Vector2 dir,
            float halfWidth,
            float halfHeight)
        {
            float tX = dir.x != 0 ? (Mathf.Sign(dir.x) * halfWidth - origin.x) / dir.x : float.MaxValue;
            float tY = dir.y != 0 ? (Mathf.Sign(dir.y) * halfHeight - origin.y) / dir.y : float.MaxValue;
            float t = Mathf.Min(tX, tY);

            Vector2 hit = origin + dir * t;
            hit.x = Mathf.Clamp(hit.x, -halfWidth, halfWidth);
            hit.y = Mathf.Clamp(hit.y, -halfHeight, halfHeight);
            return hit;
        }

        #endregion
    }
}