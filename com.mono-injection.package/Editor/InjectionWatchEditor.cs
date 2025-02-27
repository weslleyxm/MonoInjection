﻿using UnityEditor;
using UnityEngine; 
using System;
using System.Linq;

namespace MonoInjection
{
    [InitializeOnLoad]
    public static class InjectionWatchEditor
    {
        static InjectionWatchEditor() 
        {
            EditorApplication.hierarchyChanged += CheckForNewScripts;
        }

        private static void CheckForNewScripts()
        {
            if (!Application.isPlaying)
            {
                MonoDependencyResolver.Instance.Reset();

                MonoBehaviour[] components = GameObject.FindObjectsOfType<MonoBehaviour>();

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
