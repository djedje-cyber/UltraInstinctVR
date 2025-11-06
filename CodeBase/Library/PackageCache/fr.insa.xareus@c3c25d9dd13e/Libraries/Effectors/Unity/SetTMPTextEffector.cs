#if TEXT_MESH_PRO
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Change the TMP text of the given Text object. TextMeshPro is required to work
    /// </summary>
    [FunctionDescriptionAttribute("Effector to change the TMP text of the given Text object. TextMeshPro is required to work", "Unity/Text")]
    public class SetTMPTextEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Text Object", Necessity.Required)]
        protected TMP_Text textObject;

        [ConfigurationParameter("Text", Necessity.Required, Description = "If both text and textAsset are specified, textAsset will prevail")]
        protected string text;

        [ConfigurationParameter("TextAsset", Necessity.Optional, Description = "If both text and textAsset are specified, textAsset will prevail")]
        protected TextAsset textAsset;

        #endregion

        #region Constructors

        protected SetTMPTextEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
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
#endif
