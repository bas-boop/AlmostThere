using UI.Phonetesting;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UI.StateEnum;

namespace UI.Phonetesting
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
            animations.SetPlayerIcon(playerIcon);
        }

        private void Start()
        {
            if (animations.PhoneHasBeenOpened)
                indicator.Initialize();
        }

        #endregion

        #region Private methods

        private void HandlePhoneState(PhoneUIState state)
        {
            if (state == PhoneUIState.OPEN
                && stateMachine.CurrentRoutePlannerState == RoutePlannerState.OPEN)
            {
                stateMachine.SetRoutePlannerState(RoutePlannerState.OPEN);
            }
        }

        private void HandleFirstOpenComplete() => indicator.Initialize();

        private void HandleSnapped(RectTransform icon)
        {
            if (icon == null)
                return;

            stateMachine.SetRoutePlannerState(RoutePlannerState.OPEN);
        }

        private void HandleReleased() => stateMachine.SetRoutePlannerState(RoutePlannerState.CLOSE);

        #endregion
    }
}