using System.Collections.Generic;
using System.Linq;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations.ParametersConverter
{
    public class CompareXareusTypesMembersValuesSensorConverter : IFunctionParametersConverter
    {
        #region Fields

        private const string PREVIOUS_TYPE1_PARAMETER = "type1";
        private const string PREVIOUS_TYPE2_PARAMETER = "type2";
        private const string PREVIOUS_UFTYPE1_PARAMETER = "ufType1";
        private const string PREVIOUS_UFTYPE2_PARAMETER = "ufType2";
        private const string PREVIOUS_MEMBER1_PARAMETER = "member1";
        private const string PREVIOUS_MEMBER2_PARAMETER = "member2";
        private const string NEW_TYPE1_PARAMETER = "Type 1";
        private const string NEW_TYPE2_PARAMETER = "Type 2";
        private const string NEW_XAREUSTYPE1_PARAMETER = "Xareus Type 1";
        private const string NEW_XAREUSTYPE2_PARAMETER = "Xareus Type 2";
        private const string NEW_MEMBER1_PARAMETER = "Member 1";
        private const string NEW_MEMBER2_PARAMETER = "Member 2";

        #endregion

        #region Methods

        public List<Parameter> Convert(List<Parameter> parameters)
        {
            List<Parameter> res = new List<Parameter>();
            Parameter type1Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_TYPE1_PARAMETER);
            if (type1Param != null)
            {
                type1Param.name = NEW_TYPE1_PARAMETER;
                res.Add(type1Param);
            }
            Parameter type2Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_TYPE2_PARAMETER);
            if (type2Param != null)
            {
                type2Param.name = NEW_TYPE2_PARAMETER;
                res.Add(type2Param);
            }
            Parameter xareusType1Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_UFTYPE1_PARAMETER);
            if (xareusType1Param != null)
            {
                xareusType1Param.name = NEW_XAREUSTYPE1_PARAMETER;
                xareusType1Param.type = ValueParser.ConvertToString<System.Type>(typeof(XUType));
                res.Add(xareusType1Param);
            }
            Parameter xareusType2Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_UFTYPE2_PARAMETER);
            if (xareusType2Param != null)
            {
                xareusType2Param.name = NEW_XAREUSTYPE2_PARAMETER;
                xareusType2Param.type = ValueParser.ConvertToString<System.Type>(typeof(XUType));
                res.Add(xareusType1Param);
            }
            Parameter member1Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_MEMBER1_PARAMETER);
            if (member1Param != null)
            {
                member1Param.name = NEW_MEMBER1_PARAMETER;
                res.Add(member1Param);
            }
            Parameter member2Param = parameters.FirstOrDefault(param => param.name == PREVIOUS_MEMBER2_PARAMETER);
            if (member2Param != null)
            {
                member2Param.name = NEW_MEMBER2_PARAMETER;
                res.Add(member2Param);
            }
            return res;
        }

        #endregion
    }
}
