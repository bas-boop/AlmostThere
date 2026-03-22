using UnityEngine;

using Framework.Extensions;

namespace Gameplay
{
    public sealed class RandomSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Transform[] spawnPoints;

        private void Start() => player.transform.position = CollectionExtensions.GetRandomItem(spawnPoints).position;
    }
}