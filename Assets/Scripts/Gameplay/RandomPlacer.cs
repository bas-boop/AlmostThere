using UnityEngine;

using Framework.Extensions;

namespace Gameplay
{
    public sealed class RandomPlacer : MonoBehaviour
    {
        private const int MAX_PLACEMENT_ATTEMPTS = 15;
        
        [Header("References")]
        [SerializeField] private Transform objectToCheckDistance;
        [SerializeField] private Transform[] locations;

        [Header("Settings")]
        [SerializeField] private bool shouldCheckDistance;
        [SerializeField] private float minimalDistance = 10;
        
        private void Start()
        {
            if (shouldCheckDistance)
                SetPositionWithDistance(objectToCheckDistance);
            else
                transform.position = CollectionExtensions.GetRandomItem(locations).position;
        }

        private void SetPositionWithDistance(Transform targetToCheckDistance)
        {
            Vector3 pos = targetToCheckDistance.position;
            Vector3 newPos;
            int attempts = 0;
            
            while (true)
            {
                newPos = CollectionExtensions.GetRandomItem(locations).position;

                if (attempts < MAX_PLACEMENT_ATTEMPTS
                    && newPos.IsWithinRange(pos, minimalDistance))
                {
                    attempts++;
                    continue;
                }
                
                break;
            }

            transform.position = newPos;
        }

        private void OnDrawGizmos()
        {
            if (shouldCheckDistance)
            {
                Gizmos.color = Color.green;
                
                foreach (Transform location in locations)
                {
                    Gizmos.DrawWireSphere(location.position, minimalDistance);
                }
            }
            else
            {
                Gizmos.color = Color.red;
                
                foreach (Transform location in locations)
                {
                    Gizmos.DrawWireSphere(location.position, 1);
                }      
            }
        }
    }
}