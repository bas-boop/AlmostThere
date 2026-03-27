using System;
using System.Collections;
using UnityEngine;

using Framework.InteractSystem;
using Player.MovementSystem;

namespace Gameplay.PublicTransport
{
    public sealed class TransitDoor : Interactable
    {
        [SerializeField] private Transform playerSitPosition;
        [SerializeField] private float sitDuration = 0.3f;
        [SerializeField] private float outsideTransitDistance = 2;

        private bool _isPlayerInTransit;
        private bool _isTransitStoped;
        private Vector3 _doorPosition;
        private Movement _movement;

        public override void Interact(GameObject sender)
        {
            if (_movement == null)
                _movement = sender.GetComponent<Movement>();

            if (!p_canInteract
                || !_isTransitStoped)
                return;
            
            if (_isPlayerInTransit)
            {
                _doorPosition = playerSitPosition.position;
                _doorPosition += transform.right * outsideTransitDistance;
                sender.transform.SetParent(null);
                _movement.ResetRotation();
                StartCoroutine(LerpPlayerToSit(sender.transform, _doorPosition));
            }
            else
            {
                sender.transform.SetParent(transform);
                StartCoroutine(LerpPlayerToSit(sender.transform, playerSitPosition.position));
            }

            _movement.ToggleCanMove();
            _isPlayerInTransit = !_isPlayerInTransit;
        }

        public void IsStopped(bool target) => _isTransitStoped = target;

        private IEnumerator LerpPlayerToSit(Transform player, Vector3 targetPosition)
        {
            float elapsed = 0f;
            Vector3 startPosition = player.position;

            while (elapsed < sitDuration)
            {
                float t = elapsed / sitDuration;

                player.position = Vector3.Lerp(startPosition, targetPosition, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            player.position = targetPosition;
        }
    }
}