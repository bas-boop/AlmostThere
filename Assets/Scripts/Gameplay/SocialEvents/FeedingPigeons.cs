using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Framework.FollowMovement;
using Framework.InteractSystem;

namespace Gameplay.SocialEvents
{
    public sealed class FeedingPigeons : Interactable
    {
        [SerializeField] private Followed player;
        [SerializeField] private float feedingTime = 1;
        [SerializeField] private float followTime = 1;
        [SerializeField] private List<Follower> pigeons;
        
        // todo: animations for feeding
        [SerializeField] private UnityEvent onFeed = new();
        [SerializeField] private UnityEvent onFeedDone = new();
        
        public override void Interact()
        {
            if (!p_canInteract)
                return;

            StartCoroutine(FeedingSequence());
        }

        private IEnumerator FeedingSequence()
        {
            onFeed?.Invoke();
            yield return new WaitForSeconds(feedingTime);
            
            foreach (Follower pigeon in pigeons)
            {
                player.AddSegment(pigeon);
            }
            
            Invoke(nameof(UnFollow), followTime);
            onFeedDone?.Invoke();
        }

        private void UnFollow()
        {
            for (int i = pigeons.Count - 1; i >= 0; i--)
            {
                Follower pigeon = pigeons[i];
                player.RemoveSegment(pigeon); // BUG: turn off gameobject?
                pigeon.gameObject.SetActive(true);
                // todo: pigeon fly away?
            }
        }
    }
}