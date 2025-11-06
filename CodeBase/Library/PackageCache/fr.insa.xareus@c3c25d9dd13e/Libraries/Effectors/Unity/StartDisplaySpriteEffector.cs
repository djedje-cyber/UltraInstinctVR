using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [Renamed("FIVE.Utils.Effectors.StartDisplaySpriteEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.Unity.StartDisplaySpriteEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.StartDisplaySpriteEffector", "Xareus.Unity.Librairies")]
    public class StartDisplaySpriteEffector : AUnityEffector
    {
        #region Fields

#pragma warning disable 0649

        //Source
        [ConfigurationParameter("Sprite", Necessity.Required)]
        private Sprite sprite;

        //Output
        [ConfigurationParameter("UI Image", Necessity.Required)]
        private UnityEngine.UI.Image Image;

#pragma warning restore 0649

        #endregion

        #region Constructors

        public StartDisplaySpriteEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            Image.sprite = sprite;
            Image.enabled = true;
        }

        #endregion
    }
}
