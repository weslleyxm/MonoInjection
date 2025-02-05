using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonoInjection
{
    /// <summary>
    /// Manages the injection of dependencies into MonoBehaviour instances
    /// </summary>
    public class InjectionManager : MonoBehaviour
    {
        private void Awake()
        {
            InitializeScope();
            ResolveDependencies();
        }

        /// <summary>
        /// Initializes the scope by registering MonoBehaviour instances in the ServiceLocator
        /// </summary>
        private void InitializeScope()
        {
            foreach (var item in MonoDependencyResolver.Instance.Dependence)
            {
                Type type = Type.GetType(item);
                if (type == null)
                {
                    Debug.LogWarning($"Type not found: {item}");
                    continue;
                }

                var monoBehaviour = GetMonoBehaviours(type).FirstOrDefault();
                if (monoBehaviour != null)
                {
                    ServiceLocator.InternalRegister(monoBehaviour, type);
                }
                else
                {
                    Debug.LogWarning($"No MonoBehaviour instance found for type: {type}");
                }
            }
        }

        /// <summary>
        /// Resolves dependencies for all registered dependents
        /// </summary>
        private void ResolveDependencies()
        {
            foreach (var item in MonoDependencyResolver.Instance.Dependents)
            {
                Type type = Type.GetType(item);
                if (type == null)
                {
                    Debug.LogWarning($"Type not found: {item}");
                    continue;
                }

                foreach (var monoBehaviour in GetMonoBehaviours(type))
                {
                    SetDependencies(monoBehaviour);
                }
            }
        }

        /// <summary>
        /// Finds all MonoBehaviour instances of a given type
        /// </summary>
        /// <param name="type">The type of MonoBehaviour to find</param>
        /// <returns>An array of MonoBehaviour instances</returns>
        private MonoBehaviour[] GetMonoBehaviours(Type type) =>
            FindObjectsOfType(type, true) as MonoBehaviour[] ?? Array.Empty<MonoBehaviour>();

        /// <summary>
        /// Sets the dependencies for a given MonoBehaviour instance
        /// </summary>
        /// <param name="behaviour">The MonoBehaviour instance</param>
        private static void SetDependencies(MonoBehaviour behaviour)
        {
            if (behaviour == null) return;

            Type type = behaviour.GetType();
            foreach (var field in GetFieldInfos(type))
            {
                var instance = ServiceLocator.Resolve(field.FieldType);
                if (instance != null)
                {
                    field.SetValue(behaviour, instance);
                }
                else
                {
                    Debug.LogWarning($"Could not find an instance for \"{field.FieldType}\" in {type}");
                }
            }
        }

        /// <summary>
        /// Gets the fields of a given type that have the InjectAttribute
        /// </summary>
        /// <param name="type">The type to get fields from</param>
        /// <returns>An array of FieldInfo objects</returns>
        public static FieldInfo[] GetFieldInfos(Type type) =>
            type?.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<InjectAttribute>() != null)
                .ToArray() ?? Array.Empty<FieldInfo>();

        /// <summary>
        /// Instantiates a GameObject and sets its dependencies
        /// </summary>
        /// <typeparam name="T">The type of the object to instantiate</typeparam>
        /// <param name="obj">The object to instantiate</param>
        public new static void Instantiate<T>(T obj) where T : Object
        {
            if (obj == null)
            {
                Debug.LogError("Cannot instantiate a null object");
                return;
            }

            if (obj is GameObject prefab)
            {
                var instance = MonoBehaviour.Instantiate(prefab);
                foreach (var mono in instance.GetComponentsInChildren<MonoBehaviour>())
                {
                    SetDependencies(mono);
                }
            }
            else
            {
                Debug.LogError("Instantiation failed: Object must be a GameObject");
            }
        }
    }
}
