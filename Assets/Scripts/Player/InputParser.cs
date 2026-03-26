using Player.Movement;
using UI;
using UI.Phonetesting;
using UI.StateEnum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputParser : MonoBehaviour
    {
        [SerializeField] private Walking walking;
        [SerializeField] private MapToggeler mapToggeler;
        [SerializeField] private UIStateMachine uiState;

        [SerializeField] private UnityEvent onTesting = new();
        
        private PlayerInput _playerInput;
        private InputActionAsset _inputActionAsset;
        
        private void Awake()
        {
            GetReferences();
            Init();
        }

        private void Update()
        {
            if (uiState.CurrentPhoneUIState == PhoneUIState.OPEN)
                return;

            Vector2 moveInput = _inputActionAsset["Move"].ReadValue<Vector2>();
            walking.SetMoveDirection(moveInput);
        }

        private void OnEnable() => AddListeners();

        private void OnDisable() => RemoveListeners();

        private void GetReferences()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Init() => _inputActionAsset = _playerInput.actions;

        private void AddListeners()
        {
            _inputActionAsset["Interact"].performed += InteractAction;
            _inputActionAsset["Testing"].performed += TestingAction;
        }

        private void RemoveListeners()
        {
            _inputActionAsset["Interact"].performed -= InteractAction;
            _inputActionAsset["Testing"].performed -= TestingAction;
        }
        
        #region Context

        private void InteractAction(InputAction.CallbackContext context)
        {
            if (mapToggeler)
                mapToggeler.Toggle();
        }

        private void TestingAction(InputAction.CallbackContext context) => onTesting?.Invoke();

        #endregion
    }
}