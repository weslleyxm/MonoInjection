using System;  


namespace MonoInjection
{
    public class ServiceBuilder<T> where T : class
    {
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

        public ServiceBuilder<T> FromInstance(T instance)
        {
            ServiceLocator.InternalRegister(instance);
            return this;
        } 
    }
}
