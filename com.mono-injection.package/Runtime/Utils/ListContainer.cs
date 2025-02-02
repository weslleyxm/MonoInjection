using System;
using System.Collections.Generic; 
using UnityEngine;

namespace MonoInjection
{
    [Serializable]
    public class ListContainer<T>
    {
        [SerializeField] private List<T> items = new List<T>();

        public List<T> Items => items;
        public bool Contains(T item) => items.Contains(item);

        public void Add(T item) => items.Add(item);
    }
}