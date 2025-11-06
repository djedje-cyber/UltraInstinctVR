using InriaTools.Serialization;

using UnityEngine;

namespace InriaTools.Samples.Serialization
{
    public class SerializationTest : MonoBehaviour
    {
        #region Fields

        SerializationClassTest testClass;
        SerializationMainClassTest testMainClass;

        #endregion

        #region Methods

        // Start is called before the first frame update
        void Start()
        {
            testClass = new SerializationClassTest();
            Serializer.SaveToFile(testClass, "testClass.txt", Serializer.SerializerType.XmlSerializer);
            Serializer.SaveToFile(testClass, "testClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            Serializer.SaveToFile(testClass, "testClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            SerializationClassTest xmlTest = Serializer.LoadFromFile<SerializationClassTest>("testClass.txt", Serializer.SerializerType.XmlSerializer);
            SerializationClassTest xmlTestContact = Serializer.LoadFromFile<SerializationClassTest>("testClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            SerializationClassTest jsonTest = Serializer.LoadFromFile<SerializationClassTest>("testClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            testClass.Compare(xmlTest);
            testClass.Compare(xmlTestContact);
            testClass.Compare(jsonTest);

            SerializationClassTest xmlTest2 = new SerializationClassTest();
            Serializer.LoadFromFile<SerializationClassTest>(xmlTest2, "testClass.txt", Serializer.SerializerType.XmlSerializer);
            SerializationClassTest xmlTestContact2 = new SerializationClassTest();
            Serializer.LoadFromFile<SerializationClassTest>(xmlTestContact2, "testClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            SerializationClassTest jsonTest2 = new SerializationClassTest();
            Serializer.LoadFromFile<SerializationClassTest>(jsonTest2, "testClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            xmlTest2.Compare(xmlTest);
            xmlTestContact2.Compare(xmlTestContact);
            jsonTest2.Compare(jsonTest);

            testMainClass = new SerializationMainClassTest();

            Serializer.SaveToFile(testMainClass, "testMainClass.txt", Serializer.SerializerType.XmlSerializer);
            Serializer.SaveToFile(testMainClass, "testMainClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            Serializer.SaveToFile(testMainClass, "testMainClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            SerializationMainClassTest xmlTestMainClass = Serializer.LoadFromFile<SerializationMainClassTest>("testMainClass.txt", Serializer.SerializerType.XmlSerializer);
            SerializationMainClassTest xmlTestContractMainClass = Serializer.LoadFromFile<SerializationMainClassTest>("testMainClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            SerializationMainClassTest jsonTestMainClass = Serializer.LoadFromFile<SerializationMainClassTest>("testMainClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            testMainClass.Compare(xmlTestMainClass);
            testMainClass.Compare(xmlTestContractMainClass);
            testMainClass.Compare(jsonTestMainClass);

            SerializationMainClassTest xmlTestMainClass2 = new SerializationMainClassTest();
            Serializer.LoadFromFile<SerializationMainClassTest>(xmlTestMainClass2, "testMainClass.txt", Serializer.SerializerType.XmlSerializer);
            SerializationMainClassTest xmlTestContractMainClass2 = new SerializationMainClassTest();
            Serializer.LoadFromFile<SerializationMainClassTest>(xmlTestContractMainClass2, "testMainClassContract.txt", Serializer.SerializerType.DataContractSerializer);
            SerializationMainClassTest jsonTestMainClass2 = new SerializationMainClassTest();
            Serializer.LoadFromFile<SerializationMainClassTest>(jsonTestMainClass2, "testMainClassContractJson.txt", Serializer.SerializerType.DataContractJsonSerializer);

            xmlTestMainClass2.Compare(xmlTestMainClass);
            xmlTestContractMainClass2.Compare(xmlTestContractMainClass);
            jsonTestMainClass2.Compare(jsonTestMainClass);
        }

        // Update is called once per frame
        void Update()
        {
        }

        #endregion
    }
}
