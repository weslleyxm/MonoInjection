using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonoInjection
{
    /// <summary>
    /// It uses SerializableDictionary to store lists of dependent and dependence types for each scene
    /// </summary>
    [CreateAssetMenu(fileName = "MonoDependents", menuName = "MonoInjection/MonoDependents")]
    public class MonoDependencyResolver : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<string, ListContainer<string>> dependents = new();
        [SerializeField] private SerializableDictionary<string, ListContainer<string>> dependence = new();

        private static MonoDependencyResolver monoDependents;
        public static MonoDependencyResolver Instance
        {
            get
            {
                if (monoDependents == null)
                {
                    monoDependents = Resources.Load<MonoDependencyResolver>("MonoDependents");
                }

                return monoDependents;
            }
        }

        private string SceneName => SceneManager.GetActiveScene().name;

        public IReadOnlyList<string> Dependents =>
            dependents.TryGetValue(SceneName, out var list) ? list.Items : Array.Empty<string>();

        public IReadOnlyList<string> Dependence =>
            dependence.TryGetValue(SceneName, out var list) ? list.Items : Array.Empty<string>();

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
                dependentList = new ListContainer<string>();
                dependents[SceneName] = dependentList;
            }

            if (!dependentList.Contains(qualifiedName))
            {
                dependentList.Add(qualifiedName);
            }

            if (!dependence.TryGetValue(SceneName, out var dependenceList))
            {
                dependenceList = new ListContainer<string>();
                dependence[SceneName] = dependenceList;
            }

            foreach (var field in fields)
            {
                string dependenceQualifiedName = field.FieldType.AssemblyQualifiedName;
                if (dependenceQualifiedName != null && !dependenceList.Contains(dependenceQualifiedName))
                {
                    dependenceList.Add(dependenceQualifiedName);
                }
            }
        }

        /// <summary>
        /// Resets the resolver by clearing all dependents and dependencies
        /// </summary>
        public void Reset()
        {
            if (dependents.ContainsKey(SceneName))
                dependents[SceneName] = new ListContainer<string>();

            if (dependence.ContainsKey(SceneName))
                dependence[SceneName] = new ListContainer<string>();
        }
    }
}
