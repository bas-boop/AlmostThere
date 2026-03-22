using UnityEngine;

using Framework;
using Framework.InteractSystem;
using Player.MovementSystem;

namespace Gameplay.SocialEvents
{
    public sealed class YogaEvent : Interactable
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Transform playerYogaPlace;
        [SerializeField] private Timer timer;

        private bool _isYoga;
        private Movement _playerMovement;

        private void Start()
        {
            _playerMovement = player.GetComponent<Movement>();
        }

        public override void Interact()
        {
            if (!p_canInteract)
                return;

            if (!_isYoga)
                DoYoga();
            else
                ResumeNormal();
        }

        private void DoYoga()
        {
            _isYoga = !_isYoga;
            
            _playerMovement.ToggleCanMove();
            player.transform.position = playerYogaPlace.position;
            //player.transform.rotation = playerYogaPlace.rotation;
            timer.PauseTime();
        }

        private void ResumeNormal()
        {
            _isYoga = !_isYoga;
            
            _playerMovement.ToggleCanMove();
            timer.StartTimer();
        }
    }
}