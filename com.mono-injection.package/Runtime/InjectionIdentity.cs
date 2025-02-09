using System;
using UnityEngine;

namespace MonoInjection
{
    public class InjectionIdentity : MonoBehaviour
    {
        [SerializeField] public string identity;
        public string Identity => identity; 
        internal void Gen()
        {
            if (string.IsNullOrEmpty(identity))
            {
                identity = Guid.NewGuid().ToString().
                    Replace("-", "").
                    Substring(0, 20); 
            }
        }

        public static implicit operator string(InjectionIdentity identity)
        {
            return identity.Identity;
        }
    } 
}