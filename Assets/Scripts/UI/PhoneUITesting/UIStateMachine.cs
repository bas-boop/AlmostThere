using System;
using UnityEngine;

public class UIStateMachine : MonoBehaviour
{
    public enum PhoneUIState { FirstTime, Open, Close }
    public enum RoutePlannerSate { Open, Close }

    public event Action<PhoneUIState> OnPhoneStateChanged;
    public event Action<RoutePlannerSate> OnRoutePlannerStateChanged;

    public PhoneUIState CurrentPhoneUIState { get; private set; } = PhoneUIState.FirstTime;
    public RoutePlannerSate CurrentRoutePlannerSate { get; private set; } = RoutePlannerSate.Close;

    public void TogglePhone()
    {
        var next = CurrentPhoneUIState switch
        {
            PhoneUIState.FirstTime => PhoneUIState.Open,
            PhoneUIState.Open => PhoneUIState.Close,
            PhoneUIState.Close => PhoneUIState.Open,
            _                   => PhoneUIState.Close,

        };
        SetPhoneState(next);
    }

    public void SetPhoneState(PhoneUIState state)
    {
        CurrentPhoneUIState = state;
        OnPhoneStateChanged?.Invoke(state);
    }

    public void SetRoutePlanner(RoutePlannerSate state)
    {
        CurrentRoutePlannerSate = state;
        OnRoutePlannerStateChanged?.Invoke(state);
    }

}
