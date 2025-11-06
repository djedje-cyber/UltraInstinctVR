using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [Renamed("Xareus.Scenarios.Unity.InvokeUnityObjectMethodEffector", "Xareus.Unity.Librairies", typeof(InvokeMethodEffectorParameterConverter))]
    [Renamed("Xareus.Scenarios.Unity.InvokeUnityObjectMethodEffector", "Xareus.Unity.Libraries", typeof(InvokeMethodEffectorParameterConverter))]
    [Renamed("Xareus.Scenarios.Unity.InvokeXareusObjectMethodEffector", "Xareus.Unity.Librairies", typeof(InvokeMethodEffectorParameterConverter))]
    [Renamed("Xareus.Scenarios.Unity.InvokeXareusObjectMethodEffector", "Xareus.Unity.Libraries", typeof(InvokeMethodEffectorParameterConverter))]
    [FunctionDescription("", "Members")]
    public class InvokeMethodEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string COMPONENT = "Component";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Fields

        [ConfigurationParameter(COMPONENT, Necessity.Required)]
        protected Component component;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [Provider(COMPONENT)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        #endregion

        #region Constructors

        protected InvokeMethodEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in transition {Event.Parent.label} ({Event.Parent.id})");
        }

        public override void SafeEffectorUpdate()
        {
            method.Invoke(component, parameters.ToArray());
        }

        #endregion
    }
}
