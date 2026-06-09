using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using System.Collections.Generic;

/// <summary>
/// Default effector used when test uses Expect lambdas.
/// Does nothing — TestSuiteRunner handles the actual check.
/// </summary>
[FunctionDescription("Lambda Effector")]
public class LambdaEffector : AUnityEffector
{
    public LambdaEffector(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext,
        IContext eventContext)
        : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext, eventContext))
    { }

    public override void SafeReset() { }

    public override void SafeEffectorUpdate()
    {
    }
}