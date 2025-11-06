using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StartDisplayTextAssetEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StartDisplayTextAssetEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StartDisplayTextAssetEffector", "Xareus.Unity.Librairies")]
    public class StartDisplayTextAssetEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        [ConfigurationParameter("TextAsset", Necessity.Required)]
        private TextAsset textAsset;

        [ConfigurationParameter("UI Text", Necessity.Required)]
        private UnityEngine.UI.Text text;

#pragma warning restore 0649

        #endregion

        #region Constructors

        public StartDisplayTextAssetEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            text.text = textAsset.text;
            text.enabled = true;
        }

        #endregion
    }
}
