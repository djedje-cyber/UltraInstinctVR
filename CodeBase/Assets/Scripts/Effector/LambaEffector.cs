using System.Collections.Generic;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

[FunctionDescription("Lambda Effector — driven by Expect")]
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
        // Logging already handled by Expect() in XareusExtensions
    }
}