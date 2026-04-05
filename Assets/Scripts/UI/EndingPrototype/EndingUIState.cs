using System;
using UnityEngine;

namespace UI.EndingPrototype
{
    public class EndingUIState : MonoBehaviour
    {
        public event Action<WinState> OnWinOrLose;

        public WinState currentWinState;

        public void WinStateTesting()
        {
            SetWinState(currentWinState);
        }

        public void SetWinState(WinState state)
        {
            currentWinState = state;
            OnWinOrLose?.Invoke(state);
        }
    }
}
