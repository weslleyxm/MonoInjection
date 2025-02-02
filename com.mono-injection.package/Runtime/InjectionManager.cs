using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonoInjection
{
    public class InjectionManager : MonoBehaviour
    {
        private void Awake()
        {
            InitializeScope();
            ResolveDependencies();
        }

      
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

        private MonoBehaviour[] GetMonoBehaviours(Type type) =>
            FindObjectsOfType(type, true) as MonoBehaviour[] ?? Array.Empty<MonoBehaviour>();

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

        public static FieldInfo[] GetFieldInfos(Type type) =>
            type?.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<InjectAttribute>() != null)
                .ToArray() ?? Array.Empty<FieldInfo>();

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
