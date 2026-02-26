using Player.Movement;
using UnityEngine;

namespace Gameplay
{
    public sealed class BikeActivator : MonoBehaviour
    {
        [SerializeField] private GameObject bike;
        [SerializeField] private Walking walking;
        [SerializeField] private float bikeSpeed = 10;

        public void Activate()
        {
            bike.SetActive(true);
            walking.speed = bikeSpeed;
        }
    }
}