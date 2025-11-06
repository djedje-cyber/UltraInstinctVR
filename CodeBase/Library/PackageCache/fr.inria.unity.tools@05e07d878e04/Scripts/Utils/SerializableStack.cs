using System;
using System.Collections.Generic;

using UnityEngine;

namespace InriaTools.Utils
{
    /// <summary>
    /// 
    /// Serializable dictionary.
    /// In order to work, you must create an internal class that inherit from SerializableStack and mark it Serializable, like this example :
    /// --- [Serializable] public class SerializableStackOfInt : SerializableStack<int> { }
    ///
    /// And then initiate your dictionary as usual with this class :
    /// [SerializeField]
    /// SerializableStackOfInt dict = new SerializableStackOfInt();
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SerializableStack<T> : Stack<T>, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField]
        private List<T> stack = new();

        #endregion

        #region Methods

        public void OnBeforeSerialize()
        {
            stack.Clear();

            foreach (T element in this)
            {
                stack.Add(element);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < stack.Count; i++)
                Push(stack[i]);
        }

        #endregion
    }
}
