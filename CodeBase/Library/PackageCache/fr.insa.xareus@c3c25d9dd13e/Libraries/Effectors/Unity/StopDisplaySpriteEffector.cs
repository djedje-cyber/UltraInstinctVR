using System;
using System.Collections.Generic;
using System.Linq;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StopDisplaySpriteEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StopDisplaySpriteEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StopDisplaySpriteEffector", "Xareus.Unity.Librairies")]
    [Obsolete("Use SetEnabledEffector instead")]
    [ObsoleteFunction("Use SetEnabledEffector instead", typeof(SetEnabledEffector), typeof(StopDisplaySpriteEffector), nameof(ConvertParameters))]
    public class StopDisplaySpriteEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        //Output
        [ConfigurationParameter("UI Image", Necessity.Required)]
        private UnityEngine.UI.Image Image;

#pragma warning restore 0649

        #endregion

        #region Constructors

        public StopDisplaySpriteEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

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
            if (inParameters != null && inParameters.Any(param => param.name == "UI Image"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "UI Image");
                previousParameter.name = "Behaviour";
                res.Add(previousParameter);
            }
            return res;
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            Image.enabled = false;
        }

        #endregion
    }
}
