using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonoInjection
{
    /// <summary>
    /// It uses SerializableDictionary to store lists of dependent and dependence types for each scene
    /// </summary> 
    public class MonoDependencyResolver : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<string, ListContainer<Identity>> dependents = new();
        [SerializeField] private SerializableDictionary<string, ListContainer<Identity>> dependence = new();

#if UNITY_EDITOR
        [SerializeField] private static InjectionManager injectionManager;
#endif
        private static MonoDependencyResolver monoDependents;
        public static MonoDependencyResolver Instance
        {
            get
            {
                if (monoDependents == null)
                {
#if UNITY_EDITOR
                    injectionManager = FindObjectOfType<InjectionManager>();
#endif
                    monoDependents = Resources.Load<MonoDependencyResolver>("MonoDependents");
                }

                return monoDependents;
            }
        }

        private string SceneName => SceneManager.GetActiveScene().name;

        public IReadOnlyList<Identity> Dependents =>
            dependents.TryGetValue(SceneName, out var list) ? list.Items : Array.Empty<Identity>();

        public IReadOnlyList<Identity> Dependence =>
            dependence.TryGetValue(SceneName, out var list) ? list.Items : Array.Empty<Identity>();

        /// <summary>
        /// Adds a dependent type and its field dependencies to the resolver
        /// </summary>
        /// <param name="scriptType">The type of the dependent script</param>
        /// <param name="fields">The fields that the script depends on</param>
        public void AddDependent(Type scriptType, params FieldInfo[] fields)
        {
            if (scriptType == null || fields == null || fields.Length == 0) return;

            string qualifiedName = scriptType.AssemblyQualifiedName;
            if (qualifiedName == null) return;

            if (!dependents.TryGetValue(SceneName, out var dependentList))
            {
                dependentList = new ListContainer<Identity>();
                dependents[SceneName] = dependentList;
            }

            dependentList.Add(new Identity
            {
                identity = scriptType.GetIdentity(),
                qualifiedName = qualifiedName,
            });

            if (!dependence.TryGetValue(SceneName, out var dependenceList))
            {
                dependenceList = new ListContainer<Identity>();
                dependence[SceneName] = dependenceList;
            }

            foreach (var field in fields)
            {
                string dependenceQualifiedName = field.FieldType.AssemblyQualifiedName;

                dependenceList.Add(new Identity
                {
                    identity = field.FieldType.GetIdentity(),
                    qualifiedName = dependenceQualifiedName,
                });
            }

#if UNITY_EDITOR
            injectionManager.Populate();
#endif
        }

        /// <summary>
        /// Resets the resolver by clearing all dependents and dependencies
        /// </summary>
        public void Reset()
        {
            if (dependents.ContainsKey(SceneName))
                dependents[SceneName] = new();

            if (dependence.ContainsKey(SceneName))
                dependence[SceneName] = new();
        }
    }
}
