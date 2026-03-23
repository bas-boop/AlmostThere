using UnityEngine;

using Framework.InteractSystem;

namespace Gameplay.SocialEvents
{
    public sealed class PetAnimal : Interactable
    {
        public override void Interact(GameObject sender)
        {
            if (!p_canInteract)
                return;
            
            
        }
    }
}