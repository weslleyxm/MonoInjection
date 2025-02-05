using System;


namespace MonoInjection
{
    public class ServiceBuilder<T> where T : class
    {
        /// <summary>
        /// Registers a component of type T from the hierarchy
        /// Throws an exception if the component is not found
        /// </summary>
        /// <returns>The current instance of ServiceBuilder</returns>
        public ServiceBuilder<T> FromComponentInHierarchy()
        {
#if UNITY_6
            var component = UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
#else
            var component = UnityEngine.Object.FindFirstObjectByType(typeof(T)) as T;
#endif
            if (component == null)
            {
                throw new Exception($"Component of type {typeof(T)} not found in the hierarchy!");
            }
            ServiceLocator.InternalRegister(component);
            return this;
        }

        /// <summary>
        /// Registers a given instance of type T
        /// </summary>
        /// <param name="instance">The instance to register</param>
        /// <returns>The current instance of ServiceBuilder</returns>
        public ServiceBuilder<T> FromInstance(T instance)
        {
            ServiceLocator.InternalRegister(instance, typeof(T));
            return this;
        }
    }
}
