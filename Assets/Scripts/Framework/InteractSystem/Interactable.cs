using UnityEngine;

namespace Framework.InteractSystem
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] protected GameObject icon;
        
        protected bool p_canInteract;
        
        public abstract void Interact(GameObject sender);

        public void SetCanInteract(bool target)
        {
            p_canInteract = target;
            icon.SetActive(p_canInteract);
        }
    }
}