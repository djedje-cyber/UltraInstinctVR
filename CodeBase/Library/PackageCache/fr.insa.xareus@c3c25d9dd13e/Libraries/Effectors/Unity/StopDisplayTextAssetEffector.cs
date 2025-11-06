using System;
using System.Collections.Generic;
using System.Linq;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StopDisplayTextAssetEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StopDisplayTextAssetEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StopDisplayTextAssetEffector", "Xareus.Unity.Librairies")]
    [Obsolete("Use SetEnabledEffector instead")]
    [ObsoleteFunction("Use SetEnabledEffector instead", typeof(SetEnabledEffector), typeof(StopDisplayTextAssetEffector), nameof(ConvertParameters))]
    public class StopDisplayTextAssetEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        [ConfigurationParameter("UI Text", Necessity.Required)]
        private UnityEngine.UI.Text text;

#pragma warning restore 0649

        #endregion

        #region Constructors

        public StopDisplayTextAssetEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Conversion from obsolete effector
        /// </summary>
        /// <param name="inParameters"></param>
        /// <returns></returns>
        public static List<Parameter> ConvertParameters(List<Parameter> inParameters)
        {
            List<Parameter> res = new()
            {
                new Parameter("Enabled Value", ValueParser.ConvertToString(false))
            };
            if (inParameters != null && inParameters.Any(param => param.name == "UI Text"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "UI Text");
                previousParameter.name = "Behaviour";
                res.Add(previousParameter);
            }
            return res;
        }

        public override void SafeEffectorUpdate()
        {
            text.enabled = false;
        }

        #endregion
    }
}
