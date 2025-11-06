using System;
using System.Collections.Generic;
using System.Reflection;

using Xareus.Relations.Scenarios.Unity;
using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;
using Xareus.Unity;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// A effector that calls a method in a TypeWithScenario
    /// </summary>
    [Renamed("SEVEN.InvokeFiveTypeWithScenarioMethodEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.InvokeFiveTypeWithScenarioMethodEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Relations.Scenarios.InvokeTypeWithScenarioMethodEffector", "Xareus.Unity.Librairies")]
    [Renamed("Xareus.Relations.Scenarios.InvokeTypeWithScenarioMethodEffector", "Xareus.Unity.Libraries")]
    [FunctionDescription("", "TypeWithScenario")]
    public class InvokeTypeWithScenarioMethodEffector : AUnityEffector
    {
        #region Const

        // We need those public so that the converters and custom edit boxes can access them
#pragma warning disable S2339 // Public constant fields should not be used
        public const string TYPE = "Type";
        public const string METHOD = "Method";
        public const string PARAMETERS = "Parameters";
#pragma warning restore S2339 // Public constant fields should not be used

        #endregion

        #region Fields

        [ConfigurationParameter(TYPE, Necessity.Required)]
        [ProvideConstraint(typeof(XUType))]
        protected Type type;

        [ConfigurationParameter(METHOD, Necessity.Required)]
        [Provider(TYPE)]
        protected MethodInfo method;

        [ConfigurationParameter(PARAMETERS, Necessity.Required)]
        [Provider(METHOD)]
        protected MethodParameters parameters = new();

        /// <summary>
        /// Instance of XUType to affect
        /// </summary>
        private XUType affectedType;

        #endregion

        #region Constructors

        protected InvokeTypeWithScenarioMethodEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            if (Contexts.ExternalContext is not TypeWithScenario)
                throw new InvalidOperationException(nameof(InvokeTypeWithScenarioMethodEffector) + " must only be use in a scenario for " + nameof(TypeWithScenario));

            // Get affected Type
            affectedType = (Contexts.ExternalContext as TypeWithScenario).XuObject.GetXuType(type);

            // Check that the local object has the given Type
            if (affectedType == null)
                throw new ArgumentException(string.Format("The object with internal scenario doesn't have the given " + nameof(XUType) + " '{0}'", type.Name));
        }

        public override void SafeEffectorUpdate()
        {
            method.Invoke(affectedType, parameters.ToArray());
        }

        #endregion
    }
}
