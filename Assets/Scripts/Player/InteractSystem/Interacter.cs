using System.Collections.Generic;
using UnityEngine;

namespace Player.InteractSystem
{
    public sealed class Interacter : MonoBehaviour
    {
        private List<Interactable> _interactables = new ();
        
        private void Start()
        {
            Interactable[] allInteractables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            
            if (allInteractables.Length <= 0)
                return;
            
            foreach (Interactable currentInteractable in allInteractables)
            {
                _interactables.Add(currentInteractable);
            }
        }

        public void Interact()
        {
            int l = _interactables.Count;
            int index = -1;
            float closetedDistance = float.MaxValue;

            if (l <= 0)
                return;
            
            for (int i = 0; i < l; i++)
            {
                Interactable interactable = _interactables[i];
                float distance = Mathf.Abs(transform.position.magnitude - interactable.transform.position.magnitude);

                if (distance < closetedDistance)
                {
                    //Debug.Log($"{distance} {interactable.name}");
                    
                    closetedDistance = distance;
                    index = i;
                }
            }
            
            if (index == -1)
                return;
            
            _interactables[index].Interact();
        }
    }
}