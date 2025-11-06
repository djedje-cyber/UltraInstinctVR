using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Call the SetActive method of the given GameObject with the setActiveValue.
    /// </summary>
    [Renamed("SEVEN.Unity.SetActiveEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SetActiveEffector", "Xareus.Unity.Librairies")]
    [FunctionDescription("Activates or deactivates a GameObject\n" +
        "Same as `gameObject.SetActive(setActiveValue);`", "Unity")]
    public class SetActiveEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("GameObject", Necessity.Required)]
        protected GameObject gameObject;

        [ConfigurationParameter("SetActive Value", Necessity.Required)]
        protected bool setActiveValue;

        #endregion

        #region Constructors

        protected SetActiveEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            gameObject.SetActive(setActiveValue);
        }

        #endregion
    }
}
