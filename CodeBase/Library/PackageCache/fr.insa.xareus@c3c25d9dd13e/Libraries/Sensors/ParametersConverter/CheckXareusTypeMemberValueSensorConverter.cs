using System.Collections.Generic;
using System.Linq;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations.ParametersConverter
{
    public class CheckXareusTypeMemberValueSensorConverter : IFunctionParametersConverter
    {
        #region Fields

        private const string PREVIOUS_TYPE_PARAMETER = "type";
        private const string PREVIOUS_UFTYPE_PARAMETER = "ufType";
        private const string PREVIOUS_MEMBER_PARAMETER = "member";
        private const string PREVIOUS_VALUE_PARAMETER = "value";
        private const string NEW_TYPE_PARAMETER = "Type";
        private const string NEW_XAREUSTYPE_PARAMETER = "Xareus Type";
        private const string NEW_MEMBER_PARAMETER = "Member";
        private const string NEW_VALUE_PARAMETER = "Value";

        #endregion

        #region Methods

        public List<Parameter> Convert(List<Parameter> parameters)
        {
            List<Parameter> res = new List<Parameter>();

            Parameter xareusTypeParam = parameters.FirstOrDefault(param => param.name == PREVIOUS_UFTYPE_PARAMETER);
            if (xareusTypeParam != null)
            {
                xareusTypeParam.name = NEW_XAREUSTYPE_PARAMETER;
                xareusTypeParam.type = ValueParser.ConvertToString<System.Type>(typeof(XUType));
                res.Add(xareusTypeParam);
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
            return res;
        }

        #endregion
    }
}