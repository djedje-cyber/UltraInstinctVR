using System;
using System.Reflection;

namespace Xareus.Scenarios.Unity
{
    public static class MemberInfoExtensions
    {
        #region Methods

        public static object GetInstanceValue(this MemberInfo memberInfo, object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (memberInfo is FieldInfo fieldInfo)
                return fieldInfo.GetValue(target);
            else if (memberInfo is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(target);
            else
                throw new ArgumentException("memberInfo must be a FieldInfo or a PropertyInfo");
        }

        public static object GetStaticValue(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo && fieldInfo.IsStatic)
                return fieldInfo.GetValue(null);
            else if (memberInfo is PropertyInfo propertyInfo && Array.Exists(propertyInfo.GetAccessors(true), a => a.IsStatic))
                return propertyInfo.GetValue(null);
            else
                throw new ArgumentException("memberInfo must be a static FieldInfo or a static PropertyInfo");
        }

        public static void SetInstanceValue(this MemberInfo memberInfo, object target, object newValue)
        {
            if (memberInfo is null)
                throw new ArgumentNullException(nameof(memberInfo));

            if (memberInfo is FieldInfo fieldInfo)
            {
                object convertedValue = ValueParser.ConvertTo(fieldInfo.FieldType, newValue);
                fieldInfo.SetValue(target, convertedValue);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                object convertedValue = ValueParser.ConvertTo(propertyInfo.PropertyType, newValue);
                propertyInfo.SetValue(target, convertedValue);
            }
            else
            {
                throw new ArgumentException("memberInfo must be a FieldInfo or a PropertyInfo");
            }
        }

        public static void SetStaticValue(this MemberInfo memberInfo, object newValue)
        {
            if (memberInfo is null)
                throw new ArgumentNullException(nameof(memberInfo));

            if (memberInfo is FieldInfo fieldInfo && fieldInfo.IsStatic)
            {
                object convertedValue = ValueParser.ConvertTo(fieldInfo.FieldType, newValue);
                fieldInfo.SetValue(null, convertedValue);
            }
            else if (memberInfo is PropertyInfo propertyInfo && Array.Exists(propertyInfo.GetAccessors(true), a => a.IsStatic))
            {
                object convertedValue = ValueParser.ConvertTo(propertyInfo.PropertyType, newValue);
                propertyInfo.SetValue(null, convertedValue);
            }
            else
            {
                throw new ArgumentException("memberInfo must be a static FieldInfo or a static PropertyInfo");
            }
        }

        #endregion
    }
}
