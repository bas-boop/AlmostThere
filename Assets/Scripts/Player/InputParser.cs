using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Player.InteractSystem;
using Player.MovementSystem;
using UI;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputParser : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private Interacter interacter;
        [SerializeField] private MapToggeler mapToggeler;

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
            Vector2 moveInput = _inputActionAsset["Move"].ReadValue<Vector2>();
            movement.SetMoveDirection(moveInput);
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
            _inputActionAsset["Map"].performed += MapAction;
            _inputActionAsset["Testing"].performed += TestingAction;
        }

        private void RemoveListeners()
        {
            _inputActionAsset["Interact"].performed -= InteractAction;
            _inputActionAsset["Map"].performed -= MapAction;
            _inputActionAsset["Testing"].performed -= TestingAction;
        }
        
        #region Context

        private void InteractAction(InputAction.CallbackContext context)
        {
            if (interacter)
                interacter.Interact();
        }
        
        private void MapAction(InputAction.CallbackContext context)
        {
            if (mapToggeler)
                mapToggeler.Toggle();
        }

        private void TestingAction(InputAction.CallbackContext context) => onTesting?.Invoke();

        #endregion
    }
}