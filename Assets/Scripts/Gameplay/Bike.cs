using UnityEngine;

using Player.InteractSystem;

namespace Gameplay
{
    public sealed class Bike : Interactable
    {
        [SerializeField] private Transform player;

        private bool _canBeUsed;
        private bool _isUsed;

        private void Start()
        {
            if (player == null)
            {
                player = GameObject.Find("PlayerBikeParent").transform;
                Debug.LogWarning($"Player was not set, is was found... maybe {gameObject.name}");
            }
        }

        public void SetCanBeUsed(bool target) => _canBeUsed = target;

        public override void Interact()
        {
            if (!_canBeUsed)
                return;

            if (_isUsed)
            {
                _isUsed = false;
                transform.SetParent(null);
            }
            else
            {
                _isUsed = true;
                transform.SetParent(player.transform);
                transform.position = transform.parent.position;
            }
        }
    }
}