using System;
using System.Collections.Generic; 

namespace MonoInjection
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static ServiceBuilder<T> Bind<T>() where T : class
        {
            return new ServiceBuilder<T>();
        }
   
        public static T Resolve<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {typeof(T)} not registered!");
        }

        public static object Resolve(Type type) 
        {  
            if (_services.TryGetValue(type, out var service))
            { 
                return service;
            }
            throw new Exception($"Service of type {type} not registered!");
        }

        public static void Remove<T>() where T : class
        {
            if (_services.ContainsKey(typeof(T)))
            {
                _services.Remove(typeof(T)); 
            }
        }

        public static void Clear() => _services.Clear();

        internal static void InternalRegister<T>(T service)
        {
            Type serviceType = typeof(T);
            Type concreteType = serviceType.IsInterface ? service.GetType() : serviceType;

            if (_services.ContainsKey(concreteType))
            {
                throw new Exception($"Service of type {concreteType} is already registered!");
            } 

            _services[concreteType] = service; 
        }

        internal static void InternalRegister(object service, Type type)
        {
            if (_services.ContainsKey(type))
            {
                throw new Exception($"Service of type {type} is already registered!");
            }

            _services[type] = service;
        }
    }
}