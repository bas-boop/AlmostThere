using UnityEngine;

using Framework.InteractSystem;
using Player.MovementSystem;

namespace Gameplay
{
    public sealed class Bike : Interactable
    {
        [SerializeField] private Transform playerVisual;
        [SerializeField] private Movement player;
        
        private bool _isUsed;

        private void Start()
        {
            if (player == null)
            {
                playerVisual = GameObject.Find("PlayerBikeParent").transform;
                player = FindAnyObjectByType<Movement>();
                Debug.LogWarning($"Player was not set, is was found... maybe {gameObject.name}");
            }
        }

        public override void Interact(GameObject sender)
        {
            if (!p_canInteract)
                return;
            
            player.SwapMovementSettings();

            if (_isUsed)
            {
                _isUsed = false;
                transform.SetParent(null);
            }
            else
            {
                _isUsed = true;
                icon.SetActive(false);
                transform.SetParent(playerVisual.transform);
                transform.position = transform.parent.position;
                transform.rotation = transform.parent.rotation;
            }
        }
    }
}