#if ENABLE_INPUT_SYSTEM
#if !INPUT_SYSTEM
#warning InputSystem is enabled but the InputSystem package is not installed. Please install it to use InputSystem features.
#else

using System;
using System.ComponentModel;
using System.Globalization;

using UnityEngine.InputSystem;

using Xareus.Scenarios;
using Xareus.Scenarios.Utilities;

namespace Xareus.Unity.Libraries.Converters
{
    [TypeConverterFor(typeof(InputAction))]
    public class InputActionConverter : System.ComponentModel.TypeConverter
    {
        #region Fields

        protected const string PARENT_INPUT_ACTION_MAP_PARAM_NAME = "InputActionMap";

        protected const string NAME_PARAM_NAME = "Name";

        #endregion

        #region Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Parameter) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Parameter) || base.CanConvertFrom(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is Parameter val)
            {
                Parameter inputActionsAssetParam = val.GetParameter(PARENT_INPUT_ACTION_MAP_PARAM_NAME);
                if (inputActionsAssetParam == null)
                    return null;
                Parameter nameParam = val.GetParameter(NAME_PARAM_NAME);
                if (nameParam == null)
                    return null;
                InputActionMap inputActionMap = ValueParser.ConvertFromParameter<InputActionMap>(inputActionsAssetParam);
                InputAction inputAction = inputActionMap.FindAction(nameParam.value, false);
                return inputAction;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (CanConvertTo(context, destinationType) && destinationType == typeof(Parameter))
            {
                Parameter res = new();
                if (value == null)
                    return res;

                InputAction inputAction = value as InputAction;

                Parameter inputActionMap = ValueParser.ConvertToParameter(inputAction.actionMap);
                inputActionMap.name = PARENT_INPUT_ACTION_MAP_PARAM_NAME;

                res.AddParameter(inputActionMap);

                Parameter nameParam = new(NAME_PARAM_NAME, inputAction.name, ValueParser.ConvertToString(typeof(string)));

                res.AddParameter(nameParam);
                return res;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}

#endif
#endif
