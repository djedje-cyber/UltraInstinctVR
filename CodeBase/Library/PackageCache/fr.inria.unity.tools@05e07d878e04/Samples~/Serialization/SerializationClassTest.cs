using System;
using System.Runtime.Serialization;

using UnityEngine;

namespace InriaTools.Samples.Serialization
{
    [DataContract]
    [Serializable]
    public class SerializationClassTest
    {
        #region Enums

        public enum MyEnum
        {
            FALSE,
            TRUE
        };

        #endregion

        #region Fields

        public MyEnum testEnum;

        [DataMember]
        public bool bool1;

        protected bool bool2;
        private bool bool3;
        bool bool4;

        [DataMember]
        private bool bool5;

        public SerializationClassTest()
        {
            testEnum = MyEnum.TRUE;
            bool1 = true;
            bool2 = true;
            bool3 = true;
            bool4 = true;
            bool5 = true;
        }

        public void Compare(SerializationClassTest other)
        {
            Debug.Log(CompareString(other));
        }

        public string CompareString(SerializationClassTest other)
        {
            return string.Format("Comparison : \n" +
                "testEnum \t {0}:{1}\n" +
                "bool1 \t {2}:{3}\n" +
                "bool2 \t {4}:{5}\n" +
                "bool3 \t {6}:{7}\n" +
                "bool4 \t {8}:{9}\n" +
                "bool5 \t {10}:{11}\n",
                testEnum, other.testEnum,
                bool1, other.bool1,
                bool2, other.bool2,
                bool3, other.bool3,
                bool4, other.bool4,
                bool5, other.bool5);
        }

        #endregion
    }
}