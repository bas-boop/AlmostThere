using UI.Phonetesting;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UI.StateEnum;

namespace UI.Phonetesting
{
    public class PhoneUIAnimations : MonoBehaviour
    {
        #region Serialized fields

        [Header("References")]
        [SerializeField] private UIStateMachine stateMachine;
        [SerializeField] private UIAnimationSetter uiAnimations;
        [SerializeField] private Image phoneIcon;
        [SerializeField] private RectTransform phoneIconHolder;
        [SerializeField] private RectTransform phoneInputHolder;
        [SerializeField] private RectTransform startingNotification;
        [SerializeField] private RectTransform iconPlaceholder;
        [SerializeField] private RectTransform phoneTransform;
        [SerializeField] private RectTransform phoneOpenTransform;
        [SerializeField] private RectTransform phoneClosedTransform;
        [SerializeField] private RectTransform mapParent;
        [SerializeField] private RectTransform routePlannerPanel;

        [Header("Phone icon animation")]
        [SerializeField, Range(0, 20)] private float phoneIconAnimationSpeed;
        [SerializeField, Range(0, 30)] private float phoneIconRotationAmount = 10f;
        [SerializeField, Range(0, 3)] private float phoneIconScaleAmount = 1.2f;
        [SerializeField, Range(0, 10)] private float phoneIconInterval = 2f;
        [SerializeField, Range(0, 10)] private int beforeLoopAmount = 2;
        [SerializeField, Range(0, 2)] private float routePlanningBackgroundAnimationSpeed = 0.5f;
        [SerializeField, Range(0, 20)] private float phoneIconClosingAnimationSpeed;
        [SerializeField, Range(0, 1)] private float iconClosingDelay = .3f;
        [SerializeField, Range(100, 350)] private float startingNotificationOffset = 200f;

        [Tooltip("The phone has been opened at least once.")]
        [SerializeField] private bool phoneHasBeenOpened = false;

        #endregion

        #region Private fields

        private Coroutine _currentStateToggleCoroutine;
        private Coroutine _currentIconScaleCoroutine;
        private Coroutine _currentIconInputScaleCoroutine;
        private Coroutine _currentNotification;
        private Coroutine _routePlannerOpenClose;
        private RectTransform _playerIcon;
        private bool _isPlayingStartingNotification = false;

        #endregion

        #region Properties

        public bool PhoneHasBeenOpened => phoneHasBeenOpened;

        #endregion

        #region Events

        public event Action OnFirstOpenAnimationComplete;

        #endregion

        #region Lifecycle methods

        private void OnEnable()
        {
            stateMachine.OnPhoneStateChanged += HandlePhoneState;
            stateMachine.OnRoutePlannerStateChanged += HandleRoutePlannerState;
        }

        private void OnDisable()
        {
            stateMachine.OnPhoneStateChanged -= HandlePhoneState;
            stateMachine.OnRoutePlannerStateChanged -= HandleRoutePlannerState;
        }

        private void Start()
        {
            if (!phoneHasBeenOpened)
            {
                _currentIconScaleCoroutine = StartCoroutine(PlayPhoneIconAnimation());
                iconPlaceholder.localScale = Vector3.zero;
            }

            if (stateMachine.CurrentPhoneUIState != PhoneUIState.OPEN)
            {
                phoneTransform.SetPositionAndRotation(
                    phoneClosedTransform.position,
                    phoneClosedTransform.rotation);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the player icon reference used for map frame calculation.
        /// </summary>
        /// <param name="playerIcon">The player icon RectTransform.</param>
        public void SetPlayerIcon(RectTransform playerIcon) => _playerIcon = playerIcon;

        #endregion

        #region Private methods

        private void HandlePhoneState(PhoneUIState state)
        {
            if (_isPlayingStartingNotification)
                return;

            if (state == PhoneUIState.OPEN && !phoneHasBeenOpened)
                _currentNotification = StartCoroutine(PlayStartingNotification());

            PlayPhoneIconsAnimation(state);
        }

        private void HandleRoutePlannerState(RoutePlannerState state)
        {
            Debug.Log($"HandleRoutePlannerState aangeroepen: {state}");

            if (_routePlannerOpenClose != null)
                StopCoroutine(_routePlannerOpenClose);

            bool open = state == RoutePlannerState.OPEN;
            _routePlannerOpenClose = StartCoroutine(PlayRoutePlannerPanelAnim(open));
        }

        private void PlayPhoneIconsAnimation(PhoneUIState state)
        {
            if (_currentStateToggleCoroutine != null)
                StopCoroutine(_currentStateToggleCoroutine);

            _currentStateToggleCoroutine = StartCoroutine(PlayPhoneStateAnimation(state));

            if (_currentIconScaleCoroutine != null)
                StopCoroutine(_currentIconScaleCoroutine);

            _currentIconScaleCoroutine = StartCoroutine(PlayCloseOpenIconAnim(phoneIconHolder));

            if (_currentIconInputScaleCoroutine != null)
                StopCoroutine(_currentIconInputScaleCoroutine);

            _currentIconInputScaleCoroutine = StartCoroutine(PlayCloseOpenIconAnim(phoneInputHolder, iconClosingDelay));
        }

        private IEnumerator PlayPhoneStateAnimation(PhoneUIState state)
        {
            float timer = 0;
            RectTransform transformTo = state == PhoneUIState.OPEN
                ? phoneOpenTransform
                : phoneClosedTransform;

            Vector3 originalPosition = phoneTransform.anchoredPosition;
            Quaternion originalRotation = phoneTransform.rotation;

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                uiAnimations.AnimatePosition(phoneTransform, uiAnimations.easeInOut, originalPosition, transformTo.anchoredPosition, timer);
                uiAnimations.AnimateRotation(phoneTransform, uiAnimations.easeInOut, originalRotation, transformTo.rotation, timer);
                yield return null;
            }
        }

        private IEnumerator PlayPhoneIconAnimation()
        {
            float timer = 0;
            int loopCount = 0;

            while (!phoneHasBeenOpened)
            {
                timer += Time.deltaTime * phoneIconAnimationSpeed;

                float rotationMult = uiAnimations.easeShake.Evaluate(timer);
                float scaleMult = uiAnimations.easeShake.Evaluate(timer / (beforeLoopAmount / 2));

                phoneIcon.rectTransform.rotation = Quaternion.Euler(0, 0, phoneIconRotationAmount * rotationMult);
                phoneIcon.rectTransform.localScale = Vector3.one * (1 + phoneIconScaleAmount * scaleMult);

                if (timer > 1)
                {
                    if (loopCount++ == beforeLoopAmount)
                    {
                        yield return new WaitForSeconds(phoneIconInterval);
                        loopCount = 0;
                    }

                    timer = 0;
                }

                yield return null;
            }
        }

        private IEnumerator PlayCloseOpenIconAnim(RectTransform target, float delay = 0)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            bool isOpen = stateMachine.CurrentPhoneUIState == PhoneUIState.OPEN;

            Vector3 originalScale = target.localScale;
            Vector3 endScale = isOpen ? Vector3.zero : Vector3.one;
            AnimationCurve curve = isOpen ? uiAnimations.easeAnticipate : uiAnimations.easeOvershoot;

            if (!isOpen)
                yield return new WaitForSeconds(.5f);

            StartCoroutine(uiAnimations.ScaleFromCurve(target, curve, endScale, phoneIconClosingAnimationSpeed, ()=> target.localScale = endScale));


            
        }

        private IEnumerator PlayStartingNotification()
        {
            _isPlayingStartingNotification = true;
            Vector2 originalAnchorPos = startingNotification.anchoredPosition;

            yield return new WaitForSeconds(1f);

            float timer = 0f;
            Vector2 anchorPos = startingNotification.anchoredPosition -= new Vector2(0, startingNotificationOffset);

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                uiAnimations.AnimatePosition(startingNotification, uiAnimations.easeOvershoot, originalAnchorPos, anchorPos, timer);
                yield return null;
            }

            yield return new WaitForSeconds(2);

            anchorPos = startingNotification.anchoredPosition;
            float timer2 = 0f;

            while (timer2 < 1f)
            {
                timer2 += Time.deltaTime;
                uiAnimations.AnimatePosition(startingNotification, uiAnimations.easeOvershoot, anchorPos, originalAnchorPos, timer2);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            float timer3 = 0f;
            Vector2 startScale = Vector2.zero;

            while (timer3 < 1f)
            {
                timer3 += Time.deltaTime * 1.5f;
                uiAnimations.AnimateScale(iconPlaceholder, uiAnimations.easeOvershoot, startScale, Vector2.one, timer3);
                yield return null;
            }

            phoneHasBeenOpened = true;
            _isPlayingStartingNotification = false;
            OnFirstOpenAnimationComplete?.Invoke();
        }

        private IEnumerator PlayRoutePlannerPanelAnim(bool open)
        {
            const float MOVE_AMOUNT_PANEL = 150f;

            Vector2 startPosRoutePlannerPanel = routePlannerPanel.anchoredPosition;
            Vector2 startPosMapParent = mapParent.anchoredPosition;
            Vector3 startScaleMapParent = mapParent.localScale;

            Vector2 transformToPanel = open ? new Vector2(MOVE_AMOUNT_PANEL, 0) : new Vector2(-MOVE_AMOUNT_PANEL, 0);
            Vector2 targetPosMapParent = startPosMapParent;
            Vector3 targetScaleMapParent = startScaleMapParent;

            if (open)
            {
                CalculateMapFrame(out targetPosMapParent, out targetScaleMapParent);
                Debug.Log($"targetPos: {targetPosMapParent}, targetScale: {targetScaleMapParent}");
            }
            else
            {
                targetPosMapParent = Vector2.zero;
                targetScaleMapParent = Vector3.one;
            }

            float timer = 0f;

            while (timer < 1f)
            {
                timer += Time.deltaTime / routePlanningBackgroundAnimationSpeed;
                uiAnimations.AnimatePosition(routePlannerPanel, uiAnimations.easeInOut, startPosRoutePlannerPanel, transformToPanel, timer);
                uiAnimations.AnimatePosition(mapParent, uiAnimations.easeInOut, startPosMapParent, targetPosMapParent, timer * 1.5f);
                uiAnimations.AnimateScale(mapParent, uiAnimations.easeInOut, startScaleMapParent, targetScaleMapParent, timer * 1.8f);
                yield return null;
            }
        }

        private void CalculateMapFrame(out Vector2 pos, out Vector3 scale)
        {
            const float BOUNDING_BOX_PADDING = 450f;

            float panelWidth = routePlannerPanel.rect.width;
            float usableWidth = mapParent.rect.width - panelWidth;

            Vector2 iconPos = WorldToMap(iconPlaceholder);
            Vector2 playerPos = WorldToMap(_playerIcon);

            Vector2 center = (iconPos + playerPos) / 2;

            float boundingBoxX = Mathf.Abs(iconPos.x - playerPos.x) + BOUNDING_BOX_PADDING;
            float boundingBoxY = Mathf.Abs(iconPos.y - playerPos.y) + BOUNDING_BOX_PADDING;

            float scaleX = usableWidth / boundingBoxX;
            float scaleY = usableWidth / boundingBoxY;
            float newScale = Mathf.Min(scaleX, scaleY, 1f);

            Vector2 mapCenter = new Vector2(panelWidth / 2f, 0);
            pos = mapCenter - center * newScale;
            scale = Vector3.one * newScale;
        }

        private Vector2 WorldToMap(RectTransform target)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapParent, screenPos, null, out Vector2 localPoint);
            return localPoint;
        }

        #endregion
    }
}