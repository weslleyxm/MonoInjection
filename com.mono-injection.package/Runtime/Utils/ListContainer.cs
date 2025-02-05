using System;
using System.Collections.Generic; 
using UnityEngine;

namespace MonoInjection
{
    /// <summary>
    /// A container class for managing a list of items of type T
    /// </summary>
    /// <typeparam name="T">The type of items in the list</typeparam>
    [Serializable]
    public class ListContainer<T>
    {
        /// <summary>
        /// The list of items
        /// </summary>
        [SerializeField] private List<T> items = new List<T>();

        /// <summary>
        /// Gets the list of items
        /// </summary>
        public List<T> Items => items;

        /// <summary>
        /// Checks if the list contains a specific item
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>True if the item is found in the list; otherwise, false</returns>
        public bool Contains(T item) => items.Contains(item);

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item) => items.Add(item);
    }
}