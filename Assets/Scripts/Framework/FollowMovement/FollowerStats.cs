using System;
using UnityEngine;

namespace Framework.FollowMovement
{
    [Serializable]
    public struct FollowerStats
    {
        [Range(0.1f, 25)] public float followDistance;
        [Range(0.1f, 1)] public float smoothSpeed;
        [Range(0.1f, 5)] public float rotationSpeed;

        private FollowerStats(float? a, float? b, float? c)
        {
            followDistance = a ?? 0.5f;
            smoothSpeed = b ?? 1f;
            rotationSpeed = c ?? 5f;
        }
    }
}