using UnityEngine;

namespace Framework.InteractSystem
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected GameObject buttonPrompt;
        [SerializeField] protected GameObject arrow;
        
        protected bool p_canInteract;

        private void Awake() => SetCanInteract(false);

        public abstract void Interact(GameObject sender);

        public void SetCanInteract(bool target)
        {
            p_canInteract = target;
            buttonPrompt.SetActive(p_canInteract);
            arrow.SetActive(!p_canInteract);
        }
    }
}