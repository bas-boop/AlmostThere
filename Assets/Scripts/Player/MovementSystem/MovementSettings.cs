using UnityEngine;

namespace Player.MovementSystem
{
    [CreateAssetMenu(fileName = "NewMovementSetting", menuName = "AlmostThere/Movement", order = 0)]
    public class MovementSettings : ScriptableObject
    {
        public float movementSpeed = 1;
        public float rotationSpeed = 1;
    }
}