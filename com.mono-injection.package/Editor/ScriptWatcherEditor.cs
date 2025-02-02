using UnityEditor;
using UnityEngine; 
using System.Reflection;
using System;
using System.Linq;

namespace MonoInjection
{
    [InitializeOnLoad]
    public static class ScriptWatcherEditor
    { 
        static ScriptWatcherEditor()
        {
            EditorApplication.hierarchyChanged += CheckForNewScripts;
        }

        private static void CheckForNewScripts()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                Component[] components = obj.GetComponents<MonoBehaviour>();

                foreach (Component component in components)
                {
                    Type type = component.GetType();

                    var fieldsWithAttribute = InjectionManager.GetFieldInfos(type);
                     
                    if (fieldsWithAttribute.Any())
                    { 
                        MonoDependencyResolver.Instance.AddDependent(component.GetType(), fieldsWithAttribute.ToArray()); 
                    }
                }
            }
        }
    }
}
