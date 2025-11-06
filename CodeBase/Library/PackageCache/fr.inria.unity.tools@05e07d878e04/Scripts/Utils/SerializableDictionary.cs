using System;
using System.Collections.Generic;

using UnityEngine;

namespace InriaTools.Utils
{
    /// <summary>
    ///
    /// Serializable dictionary.
    /// In order to work, you must create an internal class that inherit from SerializableDictionary and mark it Serializable, like this example :
    ///  --- [Serializable] public class DictionaryOfStringAndInt : SerializableDictionary<string, int>  { }
    ///
    /// And then initiate your dictionary as usual with this class :
    /// [SerializeField]
    /// DictionaryOfStringAndInt dict = new DictionaryOfStringAndInt();
    ///
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField]
        private List<TKey> keys = new();

        [SerializeField]
        private List<TValue> values = new();

        #endregion

        #region Methods

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            if (keys.Count != values.Count)
                throw new InvalidOperationException(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", keys.Count, values.Count));

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }

        #endregion
    }
}
