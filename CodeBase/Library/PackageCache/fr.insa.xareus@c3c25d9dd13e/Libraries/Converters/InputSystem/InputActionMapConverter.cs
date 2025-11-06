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
    [TypeConverterFor(typeof(InputActionMap))]
    public class InputActionMapConverter : System.ComponentModel.TypeConverter
    {
        #region Fields

        protected const string PARENT_INPUT_ACTION_ASSET_PARAM_NAME = "InputActionAsset";

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
                Parameter inputActionsAssetParam = val.GetParameter(PARENT_INPUT_ACTION_ASSET_PARAM_NAME);
                if (inputActionsAssetParam == null)
                    return null;
                Parameter nameParam = val.GetParameter(NAME_PARAM_NAME);
                if (nameParam == null)
                    return null;
                InputActionAsset inputActionAsset = ValueParser.ConvertFromParameter<InputActionAsset>(inputActionsAssetParam);
                InputActionMap inputActionMap = inputActionAsset.FindActionMap(nameParam.value, false);
                return inputActionMap;
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

                InputActionMap inputActionMap = value as InputActionMap;

                Parameter inputActionsAsset = ValueParser.ConvertToParameter(inputActionMap.asset);
                inputActionsAsset.name = PARENT_INPUT_ACTION_ASSET_PARAM_NAME;

                res.AddParameter(inputActionsAsset);

                Parameter nameParam = new(NAME_PARAM_NAME, inputActionMap.name, ValueParser.ConvertToString(typeof(string)));

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
