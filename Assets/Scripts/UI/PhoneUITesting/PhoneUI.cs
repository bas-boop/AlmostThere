using AlmostThere.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AlmostThere.Phone
{
    public class PhoneUI : MonoBehaviour
    {
        #region Serialized fields

        [Header("References")]
        [SerializeField] private UIStateMachine stateMachine;
        [SerializeField] private PhoneUIAnimations animations;
        [SerializeField] private PhoneUIIndicator indicator;
        [SerializeField] private MapSnapper mapSnapper;
        [SerializeField] private RectTransform mapPlaceholder;
        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private PlayerInput playerInput;

        [Space(10)]
        [SerializeField, Range(50, 175)] private float mapMoveSpeed = 160f;

        #endregion

        #region Private fields

        private InputAction _mapMoveInput;

        #endregion

        #region Lifecycle methods

        private void OnEnable()
        {
            stateMachine.OnPhoneStateChanged += HandlePhoneState;
            mapSnapper.OnSnapped += HandleSnapped;
            mapSnapper.OnReleased += HandleReleased;
            animations.OnFirstOpenAnimationComplete += HandleFirstOpenComplete;
        }

        private void OnDisable()
        {
            stateMachine.OnPhoneStateChanged -= HandlePhoneState;
            mapSnapper.OnSnapped -= HandleSnapped;
            mapSnapper.OnReleased -= HandleReleased;
            animations.OnFirstOpenAnimationComplete -= HandleFirstOpenComplete;
        }

        private void Awake()
        {
            var actionMap = playerInput.actions.FindActionMap("Action map");
            _mapMoveInput = actionMap.FindAction("Map_Move_Test");
            animations.SetPlayerIcon(playerIcon);
        }

        private void Start()
        {
            if (animations.PhoneHasBeenOpened)
                indicator.Initialize();
        }

        private void Update()
        {
            if (stateMachine.CurrentPhoneUIState != UIStateMachine.PhoneUIState.OPEN)
                return;

            Vector2 direction = _mapMoveInput.ReadValue<Vector2>();

            if (direction != Vector2.zero)
                mapPlaceholder.anchoredPosition -= direction * (mapMoveSpeed * Time.deltaTime);

            indicator.CanUpdate();
        }

        #endregion

        #region Private methods

        private void HandlePhoneState(UIStateMachine.PhoneUIState state)
        {
            if (state == UIStateMachine.PhoneUIState.OPEN
                && stateMachine.CurrentRoutePlannerState == UIStateMachine.RoutePlannerState.OPEN)
            {
                stateMachine.SetRoutePlannerState(UIStateMachine.RoutePlannerState.OPEN);
            }
        }

        private void HandleFirstOpenComplete() => indicator.Initialize();

        private void HandleSnapped(RectTransform icon)
        {
            if (icon == null)
                return;

            StartCoroutine(TemporaryDisableMapInput(.6f));
            stateMachine.SetRoutePlannerState(UIStateMachine.RoutePlannerState.OPEN);
        }

        private void HandleReleased() => stateMachine.SetRoutePlannerState(UIStateMachine.RoutePlannerState.CLOSE);

        private IEnumerator TemporaryDisableMapInput(float time)
        {
            _mapMoveInput.Disable();

            yield return new WaitForSeconds(time);

            _mapMoveInput.Enable();
        }

        #endregion
    }
}