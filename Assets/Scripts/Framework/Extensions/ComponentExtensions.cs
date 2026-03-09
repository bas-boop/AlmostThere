using UnityEngine;

namespace Framework.Extensions
{
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInChildren<T>(
            this GameObject gameObject,
            out T component,
            bool includeInactive = false)
            where T : Component
        {
            component = gameObject.GetComponentInChildren<T>(includeInactive);

            if (component == null
                || component.gameObject != gameObject)
                return component != null;
            
            foreach (Transform child in gameObject.transform)
            {
                component = child.GetComponentInChildren<T>(includeInactive);
                
                if (component != null)
                    return true;
            }

            component = null;
            return false;

        }

        public static bool TryGetComponentInChildren<T>(
            this Component source,
            out T component,
            bool includeInactive = false)
            where T : Component
            => source.gameObject.TryGetComponentInChildren(out component, includeInactive);
    }
}