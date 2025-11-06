using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Change the color of the given renderer
    /// </summary>
    [Renamed("SEVEN.Unity.ChangeColorEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.ChangeColorEffector", "Xareus.Unity.Librairies")]
    public class ChangeColorEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Renderer", Necessity.Required)]
        protected Renderer renderer;

        [ConfigurationParameter("Color", Necessity.Required)]
        protected Color color;

        #endregion

        #region Constructors

        protected ChangeColorEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            renderer.material.color = color;
        }

        #endregion
    }
}
