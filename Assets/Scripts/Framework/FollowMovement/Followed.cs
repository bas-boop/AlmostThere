using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.FollowMovement
{
    /// <summary>
    /// A followed that sets the follower to follow the correct transform.
    /// </summary>
    [DefaultExecutionOrder(1)]
    public sealed class Followed : MonoBehaviour
    {
        private const string SAME_FOLLOWER_ERROR = "The segment is already part of the train.";
        private const string FOLLOWER_NOT_FOUND_ERROR = "The segment is not part of the train.";
        
        [SerializeField] private Follower segmentPrefab;
        [SerializeField] private bool useTrainStats = true;
        [SerializeField] private int amount;
        [SerializeField] private FollowerStats stats;
        [Space(20)]
        [SerializeField] private Follower[] segments;

        [SerializeField] private UnityEvent onAddSegment = new ();
        [SerializeField] private UnityEvent onRemoveSegment = new ();
        
        private void Awake() => InitSegments();
        
        private void InitSegments()
        {
            int l = segments.Length;
            
            if (l == 0)
            {
                if (amount == 0)
                    return;
                
                for (int i = 0; i < amount; i++)
                {
                    Follower ts = Instantiate(segmentPrefab);
                    AddSegment(ts);
                }
            }
            
            for (int index = 0; index < l; index++)
            {
                if (useTrainStats)
                    segments[index].StatsInUse = stats;
                else
                    segments[index].SetDefaultStats();
                
                if (index == 0)
                {
                    segments[index].FollowTarget = transform;
                    continue;
                }
                
                segments[index].FollowTarget = segments[index - 1].transform;
            }
        }

        public void AddSegment(Follower newSegment)
        {
            if (segments.Any(segment => segment == newSegment))
            {
                Debug.LogWarning(SAME_FOLLOWER_ERROR);
                return;
            }
            
            if (useTrainStats)
                newSegment.StatsInUse = stats;
            else
                newSegment.SetDefaultStats();
            
            newSegment.FollowTarget = segments.Length > 0 
                ? segments[^1].transform 
                : transform;
            
            Array.Resize(ref segments, segments.Length + 1);
            segments[^1] = newSegment;
            newSegment.CanFollow = true;
            onAddSegment?.Invoke();
        }

        public bool RemoveSegment(Follower segmentToRemove)
        {
            int index = Array.IndexOf(segments, segmentToRemove);

            if (index == -1)
            {
                Debug.LogWarning(FOLLOWER_NOT_FOUND_ERROR);
                return false;
            }

            for (int i = index; i < segments.Length - 1; i++)
            {
                segments[i] = segments[i + 1];
                segments[i].FollowTarget = i > 0 
                    ? segments[i - 1].transform 
                    : transform;
            }

            Array.Resize(ref segments, segments.Length - 1);
            segmentToRemove.CanFollow = false;
            onRemoveSegment?.Invoke();
            return true;
        }
    }
}