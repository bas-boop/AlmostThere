using System;
using UnityEngine;

namespace UI.EndingPrototype
{
    public class EndingUIState : MonoBehaviour
    {
        #region Enums
        public enum WinState { WIN, LOSE }
        #endregion

        #region Events
        public event Action<WinState> OnWinOrLose;
        #endregion

        #region Properties
        public WinState currentWinState;
        #endregion

        #region Public Voids

        public void WinStateTesting()
        {
            SetWinState(currentWinState);
        }

        public void SetWinState(WinState state)
        {
            currentWinState = state;
            OnWinOrLose?.Invoke(state);
        }

        #endregion
    }
}
