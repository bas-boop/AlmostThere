using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputParser : MonoBehaviour
    {
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
            
            // todo: should be in a movement class
            Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
            transform.position += movement * 0.01f;
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