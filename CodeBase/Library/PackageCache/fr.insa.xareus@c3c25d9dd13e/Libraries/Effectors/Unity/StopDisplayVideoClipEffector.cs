using System;
using System.Collections.Generic;
using System.Linq;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StopDisplayVideoClipEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StopDisplayVideoClipEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StopDisplayVideoClipEffector", "Xareus.Unity.Librairies")]
    [Obsolete("Use SetEnabledEffector instead")]
    [ObsoleteFunction("Use SetEnabledEffector instead", typeof(SetEnabledEffector), typeof(StopDisplayVideoClipEffector), nameof(ConvertParameters))]
    public class StopDisplayVideoClipEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        //Output
        [ConfigurationParameter("UI Raw Image", Necessity.Required)]
        private UnityEngine.UI.RawImage RawImage;

#pragma warning restore 0649

        #endregion

        #region Constructors

        public StopDisplayVideoClipEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
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
            if (inParameters != null && inParameters.Any(param => param.name == "UI Raw Image"))
            {
                Parameter previousParameter = inParameters.Find(param => param.name == "UI Raw Image");
                previousParameter.name = "Behaviour";
                res.Add(previousParameter);
            }
            return res;
        }

        public override void SafeEffectorUpdate()
        {
            RawImage.enabled = false;
        }

        #endregion
    }
}
