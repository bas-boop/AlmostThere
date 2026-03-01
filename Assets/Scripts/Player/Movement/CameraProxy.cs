using System;
using UnityEngine;

namespace Player.Movement
{
    public sealed class CameraProxy : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerRigidbody;

        private void Update()
        {
            if (Mathf.Abs(playerRigidbody.linearVelocity.y) > 0.1f)
            {
                Vector3 vector3 = transform.localPosition;
                vector3.z = 3;
                transform.localPosition = vector3;
            }
            else if (Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.1f)
            {
                Vector3 vector3 = transform.localPosition;
                vector3.z = 5;
                transform.localPosition = vector3;
            }
        }
    }
}