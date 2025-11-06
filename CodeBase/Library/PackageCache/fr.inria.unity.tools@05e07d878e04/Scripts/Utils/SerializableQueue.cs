using System.Collections.Generic;

using UnityEngine;

namespace InriaTools.Utils
{
    /// <summary>
    /// Serializable Queue.
    /// In order to work, you must create an internal class that inherit from SerializableQueue and mark it Serializable, like this example :
    ///  --- [Serializable] public class SerializableQueueOfInt : SerializableQueue<int> { }
    ///
    /// And then initiate your dictionary as usual with this class :
    /// [SerializeField]
    /// SerializableQueueOfInt dict = new SerializableQueueOfInt();
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializableQueue<T> : Queue<T>, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField]
        private List<T> queueAsList = new List<T>();

        #endregion

        #region Methods

        public void OnBeforeSerialize()
        {
            queueAsList.Clear();

            foreach (T element in this)
            {
                queueAsList.Add(element);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < queueAsList.Count; i++)
                Enqueue(queueAsList[i]);
        }

        #endregion
    }
}
