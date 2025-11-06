using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SelectComponents", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SelectComponents", "Xareus.Unity.Librairies")]
    public class SelectComponents : AUnityEffector
    {
        #region Fields

        /// <summary>
        /// The parent object
        /// </summary>
        [ConfigurationParameter("Object", "The object whose components will all be selected", Necessity.Required)]
        protected GameObject parentObject;

        [ConfigurationParameter("Include Children", "Also check recursively in all children", Necessity.Required)]
        protected bool includeChildren = false;

        [ConfigurationParameter("Component Type", Necessity.Required)]
        [ProvideConstraint(typeof(Component))]
        protected Type componentType;

        [ContextVariable("Components")]
        protected ContextVariable<List<Component>> components;

        [ContextVariable("Number", "Number of components found")]
        protected ContextVariable<int> number;

        #endregion

        #region Constructors

        protected SelectComponents(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            List<Component> res = includeChildren ? parentObject.GetComponents(componentType).ToList() : parentObject.GetComponentsInChildren(componentType).ToList();
            components.Set(res);
            number.Set(res.Count);
        }

        #endregion
    }
}
