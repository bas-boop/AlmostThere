using UnityEngine;

using Framework.Extensions;

namespace Gameplay
{
    public sealed class RandomPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject objectToPlace;
        [SerializeField] private Transform[] locations;

        private void Start() => objectToPlace.transform.position = CollectionExtensions.GetRandomItem(locations).position;
    }
}