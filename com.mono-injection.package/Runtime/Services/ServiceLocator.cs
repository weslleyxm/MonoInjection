using System;
using System.Collections.Generic; 

namespace MonoInjection
{
    /// <summary>
    /// A static class that provides service location functionality
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Binds a service of type T
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>A ServiceBuilder for the specified type</returns>
        public static ServiceBuilder<T> Bind<T>() where T : class
        {
            return new ServiceBuilder<T>();
        }

        /// <summary>
        /// Resolves a service of type T
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <returns>The resolved service.</returns>
        /// <exception cref="Exception">Thrown when the service is not registered</exception>
        public static T Resolve<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {typeof(T)} not registered!");
        }

        /// <summary>
        /// Resolves a service of the specified type
        /// </summary>
        /// <param name="type">The type of the service</param>
        /// <returns>The resolved service</returns>
        /// <exception cref="Exception">Thrown when the service is not registered</exception>
        public static object Resolve(Type type)
        {
            if (_services.TryGetValue(type, out var service))
            {
                return service;
            }
            throw new Exception($"Service of type {type} not registered!");
        }

        /// <summary>
        /// Removes a service of type T
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        public static void Remove<T>() where T : class
        {
            if (_services.ContainsKey(typeof(T)))
            {
                _services.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Clears all registered services
        /// </summary>
        public static void Clear() => _services.Clear();

        /// <summary>
        /// Registers a service internally
        /// </summary>
        /// <typeparam name="T">The type of the service</typeparam>
        /// <param name="service">The service instance</param>
        /// <exception cref="Exception">Thrown when the service is already registered</exception>
        internal static void InternalRegister<T>(T service)
        {
            Type serviceType = typeof(T); 
            _services[serviceType] = service;  
        }

        /// <summary>
        /// Registers a service internally
        /// </summary>
        /// <param name="service">The service instance</param>
        /// <param name="type">The type of the service</param>
        /// <exception cref="Exception">Thrown when the service is already registered</exception>
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