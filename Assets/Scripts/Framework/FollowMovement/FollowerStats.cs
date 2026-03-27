using System;
using UnityEngine;

namespace Framework.FollowMovement
{
    [Serializable]
    public struct FollowerStats
    {
        [Range(0.1f, 25)] public float followDistance;
        [Range(0.1f, 1)] public float smoothSpeed;

        private FollowerStats(float? a, float? b)
        {
            followDistance = a ?? 0.5f;
            smoothSpeed = b ?? 1f;
        }
    }
}