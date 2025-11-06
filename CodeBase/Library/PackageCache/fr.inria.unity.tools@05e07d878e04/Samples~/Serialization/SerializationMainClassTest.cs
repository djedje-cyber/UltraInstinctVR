using System;
using System.Runtime.Serialization;

using UnityEngine;

namespace InriaTools.Samples.Serialization
{
    [DataContract]
    [Serializable]
    public class SerializationMainClassTest
    {
        #region Fields

        [DataMember]
        public SerializationClassTest subclassTest;

        [DataMember]
        public Vector3 vector;

        [DataMember]
        public Vector3? vectorNull;

        [DataMember]
        public Vector3? vectorNull2;

        #endregion

        #region Methods

        public SerializationMainClassTest()
        {
            subclassTest = new SerializationClassTest();
            vector = Vector3.forward;
            vectorNull2 = Vector3.right;
        }

        public void Compare(SerializationMainClassTest other)
        {
            Debug.Log(CompareString(other));
        }

        public string CompareString(SerializationMainClassTest other)
        {
            return string.Format("Comparison : \n" +
                "subclassTest \n {0}\n\n" +
                "vector \t {1}:{2}\n" +
                "vectorNull \t {3}:{4}\n" +
                "vectorNull2 \t {5}:{6}\n",
                subclassTest.CompareString(other.subclassTest),
                vector, other.vector,
                vectorNull, other.vectorNull,
                vectorNull2, other.vectorNull2);
        }

        #endregion
    }
}