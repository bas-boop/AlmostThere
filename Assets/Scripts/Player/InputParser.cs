using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputParser : MonoBehaviour
    {
        [SerializeField] private Walking walking;
        
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
        }

        private void RemoveListeners()
        {
            _inputActionAsset["Interact"].performed -= InteractAction;
        }
        
        #region Context

        private void InteractAction(InputAction.CallbackContext context) => Debug.Log("Interact");

        #endregion
    }
}