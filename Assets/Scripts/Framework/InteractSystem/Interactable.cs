using UnityEngine;

namespace Framework.InteractSystem
{
    public abstract class Interactable : MonoBehaviour
    {
        protected bool p_canInteract;
        
        public abstract void Interact();

        public void SetCanInteract(bool target) => p_canInteract = target;
    }
}