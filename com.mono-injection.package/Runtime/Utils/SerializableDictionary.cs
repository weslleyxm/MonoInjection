using System.Collections.Generic;
using UnityEngine;

namespace MonoInjection
{
    /// <summary>
    /// A serializable dictionary that can be used with Unity's serialization system
    /// </summary>
    /// <typeparam name="K">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="V">The type of the values in the dictionary</typeparam>
    [System.Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<K> m_Keys = new List<K>();
        [SerializeField] private List<V> m_Values = new List<V>();

        /// <summary>
        /// Called before the object is serialized
        /// Clears the keys and values lists and populates them with the current dictionary entries
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            using Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<K, V> current = enumerator.Current;
                m_Keys.Add(current.Key);
                m_Values.Add(current.Value);
            }
        }

        /// <summary>
        /// Called after the object is deserialized
        /// Clears the dictionary and repopulates it with the entries from the keys and values lists
        /// </summary>
        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}