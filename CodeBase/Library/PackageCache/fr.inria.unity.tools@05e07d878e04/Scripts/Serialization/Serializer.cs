using System.IO;

using UnityEngine;

namespace InriaTools.Serialization
{
    public static class Serializer
    {
        #region Enums

        public enum SerializerType
        {
            XmlSerializer,
            DataContractSerializer,
            DataContractJsonSerializer
        }

        #endregion

        #region Methods

        #region Methods

        /// <summary>
        /// Save a class instance data into a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="serializerType"></param>
        /// <param name="overwrite">Overwrite the file if it exists</param>
        public static void SaveToFile<T>(T data, string filePath, SerializerType serializerType = SerializerType.XmlSerializer, bool overwrite = true)
        {
            if (!overwrite && File.Exists(filePath))
            {
                Debug.LogWarningFormat("File {0} already exists", filePath);
                return;
            }
            using (StreamWriter stream = new StreamWriter(filePath, false))
            {
                string dataText = Serialize(data, serializerType);
                stream.Write(dataText);
            }
        }

        /// <summary>
        /// Loads a class instance from a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static T LoadFromFile<T>(string filePath, SerializerType serializerType = SerializerType.XmlSerializer)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogErrorFormat("File {0} does not exists, not loading...", filePath);
                return default;
            }
            string dataText = File.ReadAllText(filePath);
            return Deserialize<T>(dataText, serializerType);
        }

        /// <summary>
        /// Loads an XML file data into an existing instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="filePath"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static void LoadFromFile<T>(T instance, string filePath, SerializerType serializerType = SerializerType.XmlSerializer)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogErrorFormat("File {0} does not exists, not loading...", filePath);
                return;
            }
            string dataText = File.ReadAllText(filePath);
            Deserialize<T>(instance, dataText, serializerType);
        }

        /// <summary>
        /// Loads a class instance from an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string data, SerializerType serializerType = SerializerType.XmlSerializer)
        {
            switch (serializerType)
            {
                case SerializerType.XmlSerializer:
                    return XmlSerializer.Deserialize<T>(data);

                case SerializerType.DataContractSerializer:
                    return XmlContractSerializer.Deserialize<T>(data);

                case SerializerType.DataContractJsonSerializer:
                    return JsonContractSerializer.Deserialize<T>(data);
            }
            return default;
        }

        /// <summary>
        /// Deserializes XML data into an existing instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="data"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static void Deserialize<T>(T instance, string data, SerializerType serializerType = SerializerType.XmlSerializer)
        {
            switch (serializerType)
            {
                case SerializerType.XmlSerializer:
                    XmlSerializer.Deserialize<T>(instance, data);
                    break;

                case SerializerType.DataContractSerializer:
                    XmlContractSerializer.Deserialize<T>(instance, data);
                    break;

                case SerializerType.DataContractJsonSerializer:
                    JsonContractSerializer.Deserialize<T>(instance, data);
                    break;
            }
        }

        /// <summary>
        /// Serializes a class instance into a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static string Serialize<T>(T data, SerializerType serializerType = SerializerType.XmlSerializer)
        {
            switch (serializerType)
            {
                case SerializerType.XmlSerializer:
                    return XmlSerializer.Serialize(data);

                case SerializerType.DataContractSerializer:
                    return XmlContractSerializer.Serialize(data);

                case SerializerType.DataContractJsonSerializer:
                    return JsonContractSerializer.Serialize(data);
            }
            return string.Empty;
        }

        #endregion

        #endregion
    }
}