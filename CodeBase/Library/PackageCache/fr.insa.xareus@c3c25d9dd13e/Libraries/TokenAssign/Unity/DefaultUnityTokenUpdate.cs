using System.Collections.Generic;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

using static Xareus.Scenarios.Extra.DefaultTokenUpdate;

namespace Xareus.Unity
{
    [OverrideClass("Xareus.Scenarios.Extra.DefaultTokenUpdate", "Xareus.Scenarios.Extra")]
    [Renamed("Xareus.Unity.DefaultUnityTokenUpdate", "Xareus.Unity.Librairies")]
    public class DefaultUnityTokenUpdate : AUnityTokenAssign
    {
        #region Fields

        [ConfigurationParameter(TOKENDATA_PARAMETER, "Data to add to the token.")]
        protected List<TokenData> tokenDataList;

        #endregion

        #region Constructors

        /// <inheritDoc />
        /// <see cref="AUnityTokenAssign.AUnityTokenAssign(Transition, Dictionary{string, Parameter}, Dictionary{string, IContext}, List{string}, ContextHolder)"/>
        protected DefaultUnityTokenUpdate(Transition transition, Dictionary<string, Parameter> parameters, Dictionary<string, IContext> upstreamSequenceIdTokenMap,
            List<string> downstreamSequenceIdList, ContextHolder contexts)
            : base(transition, parameters, upstreamSequenceIdTokenMap, downstreamSequenceIdList, contexts)
        {
        }

        #endregion

        #region Methods

        /// <inheritDoc />
        /// <see cref="AUnityTokenAssign.SafeTokenAssign"/>
        public override void SafeTokenAssign(Dictionary<string, IContext> downstreamSequenceIdTokenMap)
        {
            if (downstreamSequenceIdTokenMap == null)
                return;

            foreach (TokenData newTokenData in tokenDataList)
            {
                List<string> downstreamSequencesIds = newTokenData.DownstreamSequences.Count > 0 ? newTokenData.DownstreamSequences : downstreamSequenceIdList;
                object value = newTokenData.Value;
                if (newTokenData.Value is Variable variableValue)
                    value = ValueParser.Parse<object>(variableValue.ToString(), Contexts);
                else if (newTokenData.Value is string stringValue)
                    value = ValueParser.Parse<object>(stringValue, Contexts);
                foreach (string downstreamSequenceId in downstreamSequencesIds)
                    downstreamSequenceIdTokenMap[downstreamSequenceId].SetValue(newTokenData.VariableName, value);
            }
        }

        #endregion
    }
}
