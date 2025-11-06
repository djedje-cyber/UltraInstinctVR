using log4net;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations.ParametersConverter
{
    public class TypeValueSensorParametersConverter : IFunctionParametersConverter
    {
        #region Fields

        private const string PREVIOUS_OBJECT_PARAMETER = "object";
        private const string PREVIOUS_TYPEID_PARAMETER = "typeID";
        private const string PREVIOUS_VALUES_PARAMETER = "values";
        private const string PREVIOUS_UFOBJECT_PARAMETER = "ufObject";
        private const string PREVIOUS_TYPE_PARAMETER = "type";
        private const string PREVIOUS_MEMBER_PARAMETER = "member";
        private const string PREVIOUS_VALUE_PARAMETER = "value";
        private const string NEW_XUOBJECT_PARAMETER = "Xareus Object";
        private const string NEW_TYPE_PARAMETER = "Type";
        private const string NEW_MEMBER_PARAMETER = "Member";
        private const string NEW_VALUE_PARAMETER = "Value";

        /// <summary>
        /// The log4net logger
        /// </summary>
        static readonly ILog LOGGER = LogManager.GetLogger(typeof(TypeValueSensorParametersConverter));

        #endregion

        #region Methods

        public List<Parameter> Convert(List<Parameter> parameters)
        {
            List<Parameter> res = new List<Parameter>();

            Parameter objectParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_OBJECT_PARAMETER);
            if (objectParam != null)
            {
                objectParam.name = NEW_XUOBJECT_PARAMETER;
                objectParam.type = ValueParser.ConvertToString<System.Type>(typeof(XUObject));
                res.Add(objectParam);
            }

            objectParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_UFOBJECT_PARAMETER);
            if (objectParam != null)
            {
                objectParam.name = NEW_XUOBJECT_PARAMETER;
                objectParam.type = ValueParser.ConvertToString<System.Type>(typeof(XUObject));
                res.Add(objectParam);
            }

            Parameter typeParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_TYPE_PARAMETER);
            if (typeParam != null)
            {
                typeParam.name = NEW_TYPE_PARAMETER;
                res.Add(typeParam);
            }

            Parameter memberParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_MEMBER_PARAMETER);
            if (memberParam != null)
            {
                memberParam.name = NEW_MEMBER_PARAMETER;
                res.Add(memberParam);
            }

            Parameter valueParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_VALUE_PARAMETER);
            if (valueParam != null)
            {
                valueParam.name = NEW_VALUE_PARAMETER;
                res.Add(valueParam);
            }

            Parameter typeIdParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_TYPEID_PARAMETER);

            if (objectParam != null && typeIdParam != null)
            {
                // get the XUObject (i.e. component)
                XUObject xuObject = null;
                if (Application.isPlaying)
                    XUObject.TryGetXuObject(objectParam.value, out xuObject);
                else
                    xuObject = GameObject.FindObjectsOfType<XUObject>().FirstOrDefault(obj => obj.Id == objectParam.value);

                if (xuObject == null)
                {
                    LOGGER.WarnFormat("Could not find XUObject {0} to convert from", objectParam.value);
                    return res;
                }

                // get the XUType (i.e. component)
                XUType @type = null;

                @type = xuObject.GetXuType(typeIdParam.value);

                Parameter newTypeParam = ValueParser.ConvertToParameter<System.Type>(@type.GetType());
                newTypeParam.type = ValueParser.ConvertToString<System.Type>(typeof(System.Type));
                newTypeParam.name = NEW_TYPE_PARAMETER;
                res.Add(newTypeParam);

                ConvertValues(parameters, res, type);
            }
            return res;
        }

        private static void ConvertValues(List<Parameter> parameters, List<Parameter> res, XUType type)
        {
            MemberInfo[] members = @type.GetType().GetMembers();

            Parameter memberValueParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_VALUES_PARAMETER);
            if (parameters.Count(param => param.name == PREVIOUS_VALUES_PARAMETER) > 1 && LOGGER.IsWarnEnabled)
            {
                LOGGER.WarnFormat("Original TypeValueSensor contains multiple values entries, only one will be converted. Consider using multiple instances instead");
            }
            if (memberValueParam != null)
            {
                string[] decomposedString = memberValueParam.value.Trim(new[] { '[', ']' }).Split(',');
                if (decomposedString.Length == 2)
                {
                    MemberInfo memberInfo = members.FirstOrDefault(member => member.Name == decomposedString[0]);
                    if (memberInfo == null && LOGGER.IsWarnEnabled)
                    {
                        LOGGER.WarnFormat("Could not find member named {0} in type {1}", decomposedString[0], @type.GetType());
                    }
                    else
                    {
                        Parameter memberParam = ValueParser.ConvertToParameter<MemberInfo>(memberInfo);
                        memberParam.name = NEW_MEMBER_PARAMETER;
                        res.Add(memberParam);

                        System.Type memberType = typeof(object);
                        if (memberInfo is FieldInfo fieldInfo)
                            memberType = fieldInfo.FieldType;
                        else if (memberInfo is PropertyInfo propertyInfo)
                            memberType = propertyInfo.PropertyType;

                        Parameter valueParam = decomposedString[1];
                        valueParam.type = ValueParser.ConvertToString<System.Type>(memberType);
                        valueParam.name = NEW_VALUE_PARAMETER;
                        res.Add(valueParam);
                    }
                }
                else if (LOGGER.IsWarnEnabled)
                {
                    LOGGER.WarnFormat("Could not decompose the original value {0} into member and value info", memberValueParam.value);
                }
            }
        }

        #endregion
    }
}
