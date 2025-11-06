using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    ///
    /// </summary>
    [Renamed("Xareus.Scenarios.Unity.ChangeParentEffector", "Xareus.Unity.Librairies")]
    [FunctionDescription("Change the parent of an object. If no parent is specified, the object will be put at the root of the scene")]
    public class ChangeParentEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Object", Necessity.Required)]
        protected Transform @object;

        [ConfigurationParameter("Parent", "If not specified, the object will be put at the root of the scene", Necessity.Optional)]
        protected Transform parent;

        #endregion

        #region Constructors

        public ChangeParentEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            @object.parent = parent;
        }

        #endregion
    }
}
