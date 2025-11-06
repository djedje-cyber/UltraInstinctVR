using System;
using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("Check if a game object (or one of its children) has a component of a specific type")]
    public class HasComponentOfTypeSensor : AUnitySensor
    {
        #region Fields

        /// <summary>
        /// Event context entry for the first or only component found
        /// </summary>
        [EventContextEntry]
        protected static string COMPONENT = "Component";

        /// <summary>
        /// Event context entry for all components found
        /// </summary>
        [EventContextEntry]
        protected static string COMPONENTS = "Components";

        [ConfigurationParameter("Game Object", Necessity.Required)]
        protected GameObject gameObject;

        [ConfigurationParameter("Component Type", Necessity.Required)]
        [ProvideConstraint(typeof(Component))]
        protected Type componentType;

        [ConfigurationParameter("Include Children", necessity: Necessity.Optional, initialValue: false)]
        [ProvideConstraint(typeof(Component))]
        protected bool includeChildren;

        protected SimpleDictionary eventContext = new();

        public HasComponentOfTypeSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override Result SafeSensorCheck()
        {
            Component[] result = null;
            if (includeChildren)
            {
                result = gameObject.GetComponentsInChildren(componentType);
            }
            else
            {
                result = gameObject.GetComponents(componentType);
            }
            bool hasComponent = result != null && result.Length > 0;
            if (hasComponent)
            {
                eventContext.Clear();
                eventContext.Add(COMPONENT, result[0]);
                for (int i = 0; i < result.Length; i++)
                    eventContext.Add(COMPONENTS + "." + i, result[i]);
            }
            else
            {
                eventContext.Clear();
            }
            return new Result(hasComponent, eventContext);
        }

        #endregion
    }
}
