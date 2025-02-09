using System.Linq;
using UnityEngine;

namespace MonoInjection
{
    public static class ComponentResolver
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.gameObject.GetComponent<T>();
            var refComponent = component == null ? gameObject.gameObject.AddComponent<T>() : component;
            return refComponent;
        }
         
        public static string GetIdentity(this System.Type type)
        {
            var mono = MonoBehaviour.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(mono => type.IsAssignableFrom(mono.GetType()))
            .First();

            if (mono != null)
            {
                InjectionIdentity identity = mono.gameObject.GetOrAddComponent<InjectionIdentity>();
                identity.Gen();
                return identity.Identity;
            }

            return string.Empty;
        }
    }
}