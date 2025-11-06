using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    [FunctionDescription("Concatenates the strings (using ToString()) to display them using Unity's log system")]
    public class LogEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Log Type", LogType.Log, Necessity.Required)]
        protected LogType LogType;

        [ConfigurationParameter("Strings", Necessity.Required)]
        protected List<object> Strings;

        #endregion

        #region Constructors

        public LogEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            Debug.unityLogger.Log(LogType, string.Join(" ", Strings.Select(entry => entry.ToString())));
        }

        #endregion
    }
}
