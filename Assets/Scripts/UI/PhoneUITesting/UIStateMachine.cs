using System;
using UnityEngine;

namespace AlmostThere.UI
{
    public class UIStateMachine : MonoBehaviour
    {
        #region Enums

        public enum PhoneUIState
        {
            FIRST_TIME,
            OPEN,
            CLOSE
        }

        public enum RoutePlannerState
        {
            OPEN,
            CLOSE
        }

        #endregion

        #region Events

        public event Action<PhoneUIState> OnPhoneStateChanged;
        public event Action<RoutePlannerState> OnRoutePlannerStateChanged;

        #endregion

        #region Properties

        public PhoneUIState CurrentPhoneUIState { get; private set; } = PhoneUIState.FIRST_TIME;
        public RoutePlannerState CurrentRoutePlannerState { get; private set; } = RoutePlannerState.CLOSE;

        #endregion

        #region Public methods

        /// <summary>
        /// Toggles the phone between open and closed states.
        /// </summary>
        public void TogglePhone()
        {
            PhoneUIState next = CurrentPhoneUIState switch
            {
                PhoneUIState.FIRST_TIME => PhoneUIState.OPEN,
                PhoneUIState.OPEN => PhoneUIState.CLOSE,
                PhoneUIState.CLOSE => PhoneUIState.OPEN,
                _ => PhoneUIState.CLOSE,
            };

            SetPhoneState(next);
        }

        /// <summary>
        /// Sets the phone UI state and fires the corresponding event.
        /// </summary>
        /// <param name="state">The new phone state to set.</param>
        public void SetPhoneState(PhoneUIState state)
        {
            CurrentPhoneUIState = state;
            OnPhoneStateChanged?.Invoke(state);
        }

        /// <summary>
        /// Sets the route planner state and fires the corresponding event.
        /// </summary>
        /// <param name="state">The new route planner state to set.</param>
        public void SetRoutePlannerState(RoutePlannerState state)
        {
            CurrentRoutePlannerState = state;
            OnRoutePlannerStateChanged?.Invoke(state);
        }

        #endregion
    }
}