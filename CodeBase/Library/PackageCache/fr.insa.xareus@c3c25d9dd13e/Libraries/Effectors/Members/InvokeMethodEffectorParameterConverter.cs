using System;
using System.Collections.Generic;

using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    public class InvokeMethodEffectorParameterConverter : IFunctionParametersConverter
    {
        #region Methods

        public List<Parameter> Convert(List<Parameter> parameters)
        {
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));
            // Go through all parameters and create a new list parameters from them
            List<Parameter> res = new();
            foreach (Parameter parameter in parameters)
            {
                if (parameter.name is "Calling Object" or "callingObject")
                {
                    Parameter newParameter = new()
                    {
                        name = InvokeMethodEffector.COMPONENT,
                        type = parameter.type,
                        value = parameter.value,
                        param = parameter.param
                    };
                    res.Add(newParameter);
                }
                else
                {
                    if (parameter.name == "method")
                        parameter.name = InvokeMethodEffector.METHOD;
                    else if (parameter.name == "parameters")
                        parameter.name = InvokeMethodEffector.PARAMETERS;
                    res.Add(parameter);
                }
            }
            return res;
        }

        #endregion
    }
}
