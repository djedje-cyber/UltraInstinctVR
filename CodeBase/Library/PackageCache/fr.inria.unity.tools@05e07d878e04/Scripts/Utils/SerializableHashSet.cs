using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace InriaTools.Utils
{
    /// <summary>
    /// Serializable HashSet.
    /// In order to work, you must create an internal class that inherit from SerializableHashSet and mark it Serializable, like this example :
    ///  --- [Serializable] public class SerializableHashSetOfInt : SerializableHashSet<int> { }
    ///
    /// And then initiate your dictionary as usual with this class :
    /// [SerializeField]
    /// SerializableHashSetOfInt dict = new SerializableHashSetOfInt();
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializableHashSet<T> : ISerializationCallbackReceiver, ISet<T>, IReadOnlyCollection<T>
    {
        #region Fields

        [SerializeField]
        private List<T> hashSetAsList = new List<T>();

        private HashSet<T> _hashSet = new HashSet<T>();

        #endregion

        #region Constructors

        // empty constructor required for Unity serialization
        public SerializableHashSet() { }

        public SerializableHashSet(IEnumerable<T> collection)
        {
            _hashSet = new HashSet<T>(collection);
        }

        #endregion

        #region Methods

        public void OnBeforeSerialize()
        {
            HashSet<T> cur = new HashSet<T>(hashSetAsList);

            foreach (T val in this)
            {
                if (!cur.Contains(val))
                {
                    hashSetAsList.Add(val);
                }
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (T val in hashSetAsList)
            {
                Add(val);
            }
        }

        #endregion

        #region Hashset reimplementation

        public int Count => _hashSet.Count;
        public bool IsReadOnly => false;

        public void ExceptWith(IEnumerable<T> other)
        {
            _hashSet.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _hashSet.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _hashSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _hashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _hashSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _hashSet.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _hashSet.UnionWith(other);
        }

        public void Clear()
        {
            _hashSet.Clear();
        }

        public bool Contains(T item)
        {
            return _hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _hashSet.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _hashSet.GetEnumerator();
        }

        public bool Add(T item)
        {
            return _hashSet.Add(item);
        }

        public bool Remove(T item)
        {
            return _hashSet.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            _hashSet.Add(item);
        }

        #endregion
    }
}
