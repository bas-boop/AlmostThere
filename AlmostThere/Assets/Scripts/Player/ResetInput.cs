using UnityEngine;
using UnityEngine.InputSystem;

using Framework;

namespace Player
{
    public sealed class ResetInput : MonoBehaviour
    {
        [SerializeField] private SceneSwitcher sceneSwitcher;

        private InputAction _reloadAction;

        private void Awake()
        {
            _reloadAction = new InputAction(
                name: "ReloadScene",
                type: InputActionType.Button);

            _reloadAction.AddBinding("<Keyboard>/r");
            _reloadAction.AddBinding("<Gamepad>/start");

            _reloadAction.performed += OnReloadPerformed;
        }

        private void OnEnable()
        {
            _reloadAction.Enable();
        }

        private void OnDisable()
        {
            _reloadAction.Disable();
        }

        private void OnDestroy()
        {
            _reloadAction.performed -= OnReloadPerformed;
            _reloadAction.Dispose();
        }

        private void OnReloadPerformed(InputAction.CallbackContext context)
        {
            if (sceneSwitcher != null)
                sceneSwitcher.LoadScene();
        }
    }
}