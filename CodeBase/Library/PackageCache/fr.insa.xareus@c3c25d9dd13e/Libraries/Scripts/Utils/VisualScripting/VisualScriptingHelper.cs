#if VISUAL_SCRIPTING

using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using Xareus.Utils;

namespace Xareus.Scenarios.Unity.VisualScripting
{
    public static class VisualScriptingHelper
    {
        #region Methods

        public static void SetGraphVariables(Dictionary<string, Parameter> parameters, GraphReference graphReference)
        {
            foreach (VariableDeclaration variable in (graphReference.machine as ScriptMachine).graph.variables)
            {
                parameters.TryGetValue(variable.name, out Parameter parameter);
                if (parameter != null)
                {
                    string typeName = TypeUtils.CleanTypeName(variable.typeHandle.Identification);
                    Type fieldType = ValueParser.ConvertFromString<Type>(typeName);
                    global::Unity.VisualScripting.Variables.Graph(graphReference).Set(variable.name, ValueParser.ConvertFromParameter(parameter, fieldType));
                }
            }
        }

        #endregion
    }
}

#endif
