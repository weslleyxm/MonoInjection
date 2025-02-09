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
        [SerializeField] private SerializableDictionary<string, InjectionIdentity> injectionIdentities = new();

        private void Awake()
        {
            InitializeScope();
            ResolveDependencies();
        }

        internal void Populate()
        {
            injectionIdentities = new SerializableDictionary<string, InjectionIdentity>(); 
            var identities = FindObjectsOfType<InjectionIdentity>();
            foreach (var identity in identities)
            {
                injectionIdentities.Add(identity.identity, identity);  
            }
        }

        /// <summary>
        /// Initializes the scope by registering MonoBehaviour instances in the ServiceLocator
        /// </summary>
        private void InitializeScope()
        {
            foreach (var item in MonoDependencyResolver.Instance.Dependence)
            { 
                Type type = Type.GetType(item.qualifiedName);
                var monoBehaviour = GetObject(item.identity);

                if (monoBehaviour != null)
                {
                    object component = monoBehaviour.GetComponent(type);
                    ServiceLocator.InternalRegister(component, type);    
                }
                else
                {
                    Debug.LogWarning($"No MonoBehaviour instance found for type: {type}");
                }
            }
        }

        public MonoBehaviour GetObject(string identity)
        {
            if (injectionIdentities.ContainsKey(identity))
                return injectionIdentities[identity];

            return null;
        }

        /// <summary>
        /// Resolves dependencies for all registered dependents
        /// </summary>
        private void ResolveDependencies()
        {
            foreach (var item in MonoDependencyResolver.Instance.Dependents)
            {
                Type type = Type.GetType(item.qualifiedName);
                if (type == null)
                {
                    Debug.LogWarning($"Type not found: {item}");
                    continue;
                }

                SetDependencies(GetObject(item.identity), type); 
            }
        }

        /// <summary>
        /// Sets the dependencies for a given MonoBehaviour instance
        /// </summary>
        /// <param name="behaviour">The MonoBehaviour instance</param>
        private static void SetDependencies(MonoBehaviour behaviour, Type type)
        { 
            if (behaviour == null) return; 
            var component = behaviour.GetComponent(type); 

            foreach (var field in GetFieldInfos(type))
            {
                var instance = ServiceLocator.Resolve(field.FieldType);
                if (instance != null)
                { 
                    field.SetValue(component, instance);
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
                    //SetDependencies(mono);
                }
            }
            else
            {
                Debug.LogError("Instantiation failed: Object must be a GameObject");
            }
        }
    }
}
