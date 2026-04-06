using UI.Phonetesting;
using UnityEngine;

public class PhoneInputHandler : MonoBehaviour
{
    [SerializeField] private UIStateMachine _StateMachine;

    public void HandleInput()
    {
        _StateMachine.TogglePhone();
    }
}
