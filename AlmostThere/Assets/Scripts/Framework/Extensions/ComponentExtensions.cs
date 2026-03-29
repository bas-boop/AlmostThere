using UnityEngine;

namespace Framework.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Tries to get a component of type <typeparamref name="T"/> in the children of the GameObject (excluding the parent).
        /// Returns true if found, false otherwise.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="gameObject">The GameObject whose children will be searched.</param>
        /// <param name="component">The found component of type <typeparamref name="T"/>, or null if not found.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects in the search.</param>
        /// <returns>True if a component of type <typeparamref name="T"/> was found in the children, false otherwise.</returns>
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

        /// <summary>
        /// Tries to get a component of type <typeparamref name="T"/> in the children of the source Component's GameObject (excluding the parent).
        /// Returns true if found, false otherwise.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="source">The Component whose GameObject's children will be searched.</param>
        /// <param name="component">The found component of type <typeparamref name="T"/>, or null if not found.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects in the search.</param>
        /// <returns>True if a component of type <typeparamref name="T"/> was found in the children, false otherwise.</returns>
        public static bool TryGetComponentInChildren<T>(
            this Component source,
            out T component,
            bool includeInactive = false)
            where T : Component
            => source.gameObject.TryGetComponentInChildren(out component, includeInactive);
    }
}