using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace InriaTools.Serialization
{
    public static class XmlSerializer
    {
        #region Methods

        /// <summary>
        /// Loads a class instance from an XML string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string data)
        {
            using (StringReader stringReader = new StringReader(data))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T res;
                try
                {
                    res = (T)xmlSerializer.Deserialize(stringReader);
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
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stream, data);
            return stream.ToString();
        }

        #endregion
    }
}