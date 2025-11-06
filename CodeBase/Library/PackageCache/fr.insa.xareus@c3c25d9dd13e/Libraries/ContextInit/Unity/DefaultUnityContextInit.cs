using System.Collections.Generic;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Unity
{
    [OverrideClass("Xareus.Scenarios.ContextInit.DefaultContextInit", "Xareus.Scenarios")]
    [Renamed("Xareus.Unity.DefaultUnityContextInit", "Xareus.Unity.Librairies")]
    public class DefaultUnityContextInit : AUnityContextInit
    {
        #region Fields

        [ConfigurationParameter(Scenarios.ContextInit.DefaultContextInit.VALUES_PARAMETER)]
        protected Dictionary<ScenarioVariablePath, object> Values = new();

        #endregion

        #region Constructors

        /// <see cref="AContextInit.AContextInit(Dictionary{string, Parameter}, ContextHolder)"/>
        public DefaultUnityContextInit(Scenario scenario, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(scenario, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override IContext SafeContextInit()
        {
            IContext context = new SimpleDictionary();

            // copy all values in the context
            foreach (KeyValuePair<ScenarioVariablePath, object> entries in Values)
                context.SetValue(entries.Key, entries.Value);

            return context;
        }

        #endregion
    }
}
