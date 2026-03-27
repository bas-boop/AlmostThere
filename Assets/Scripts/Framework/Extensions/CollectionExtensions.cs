using System.Collections.Generic;

namespace Framework.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandomItem<T>(IList<T> list)
        {
            int r = UnityEngine.Random.Range(0, list.Count);
            return list[r];
        }
    }
}