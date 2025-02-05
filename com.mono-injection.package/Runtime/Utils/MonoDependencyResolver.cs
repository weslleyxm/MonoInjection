using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonoInjection
{
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

        public void Reset()
        {
            dependents.Clear();
            dependence.Clear();
        }    
    }
}
