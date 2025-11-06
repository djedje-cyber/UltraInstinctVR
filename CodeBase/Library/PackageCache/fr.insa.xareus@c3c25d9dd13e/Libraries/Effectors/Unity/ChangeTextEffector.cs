using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Change the text of the given Text object
    /// </summary>
    [FunctionDescriptionAttribute("Effector to change the text of the given Text object", "Unity/Text")]
    [Renamed("SEVEN.Unity.ChangeTextEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.ChangeTextEffector", "Xareus.Unity.Librairies")]
    public class ChangeTextEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Text Object", Necessity.Required)]
        protected Text textObject;

        [ConfigurationParameter("Text", Necessity.Required, Description = "If both text and textAsset are specified, textAsset will prevail")]
        protected string text;

        [ConfigurationParameter("TextAsset", Necessity.Optional, Description = "If both text and textAsset are specified, textAsset will prevail")]
        protected TextAsset textAsset;

        #endregion

        #region Constructors

        protected ChangeTextEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            if (textAsset != null)
                textObject.text = textAsset.text;
            else
                textObject.text = text;
        }

        #endregion
    }
}
