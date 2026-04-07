using UnityEngine;

using Framework.Audio;
using Framework.InteractSystem;
using Framework.TriggerArea;
using Player.MovementSystem;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public sealed class Bike : Interactable
    {
        [SerializeField] private Transform playerVisual;
        [SerializeField] private GameObject indicator;
        [SerializeField] private Movement player;
        [SerializeField] private AudioHandler audioHandler;

        [SerializeField, Range(0, 1)] private float chanceToBrake = 0.001f;
        
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

        private void Update()
        {
            if (!_isUsed)
                return;

            if (Random.value < chanceToBrake)
            {
                audioHandler.Play();
                GetOffBike();
                player.SwapMovementSettings();
                enabled = false;
                indicator.SetActive(false);
                GetComponent<TriggerArea>().enabled = false;
            }
        }

        public override void Interact(GameObject sender)
        {
            if (!p_canInteract)
                return;
            
            player.SwapMovementSettings();

            if (_isUsed)
            {
                GetOffBike();
            }
            else
            {
                _isUsed = true;
                buttonPrompt.SetActive(false);
                transform.SetParent(playerVisual.transform);
                transform.position = transform.parent.position;
                transform.rotation = transform.parent.rotation;
            }
        }

        private void GetOffBike()
        {
            _isUsed = false;
            transform.SetParent(null);
        }
    }
}