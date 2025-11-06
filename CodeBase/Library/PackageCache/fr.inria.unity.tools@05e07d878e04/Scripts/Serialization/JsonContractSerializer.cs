using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace InriaTools.Serialization
{
    public static class JsonContractSerializer
    {
        #region Methods

        /// <summary>
        /// Loads a class instance from a JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string data)
        {
            TextReader reader = new StringReader(data);
            using (XmlReader stringReader = XmlReader.Create(reader))
            {
                DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(T));
                T res;
                try
                {
                    res = (T)dataContractSerializer.ReadObject(stringReader);
                }
                catch (System.Exception e)
                {
                    throw new InvalidDataException("Could not use the given data to deserialize : " + data, e);
                }
                return res;
            }
        }

        /// <summary>
        /// Deserializes XML data into an existing instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void Deserialize<T>(T instance, string data)
        {
            T tmp = Deserialize<T>(data);
            try
            {
                MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(T));
                FormatterServices.PopulateObjectMembers(instance, members, FormatterServices.GetObjectData(tmp, members));
            }
            catch (System.Exception e)
            {
                throw new InvalidDataException("Could not set the instance members : " + data, e);
            }
        }

        /// <summary>
        /// Serializes a class instance into an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize<T>(T data)
        {
            StringWriter stream = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(T));
                dataContractSerializer.WriteObject(writer, data);
            }
            return stream.ToString();
        }

        #endregion
    }
}