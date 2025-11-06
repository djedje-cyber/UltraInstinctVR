using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    [Renamed("Xareus.Scenarios.Unity.InvokeStaticMethodEffector", "Xareus.Unity.Librairies")]
    [FunctionDescription("", "Members")]
    public class InvokeStaticMethodEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
        public const string TYPE = "Type";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        protected Type type;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [ProvideConstraint(BindingFlags.Static)]
        [Provider(TYPE)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        #endregion

        #region Constructors

        protected InvokeStaticMethodEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (method == null)
                throw new NullReferenceException($"{nameof(method)} is null in transition {Event.Parent.label} ({Event.Parent.id})");

            if (!method.IsStatic)
                throw new InvalidOperationException($"{nameof(method)} is not a static MethodInfo in {nameof(InvokeStaticMethodEffector)}");
        }

        public override void SafeEffectorUpdate()
        {
            method.Invoke(null, parameters.ToArray());
        }

        #endregion
    }
}
