using System.Collections.Generic;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

[FunctionDescription("Lambda Sensor — driven by DetectInteraction")]
public class LambdaSensor : AInUnityStepSensor
{
    public LambdaSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext)
        : base(@event, nameValueListMap, new ContextHolder(externalContext, scenarioContext, sequenceContext))
    { }

    public override void SafeReset() { }

    public override Result UnityStepSensorCheck()
    {
        // Result already captured by DetectInteraction in XareusExtensions
        return new Result(DetectInteractionInterceptor.LastResult, new SimpleDictionary());
    }
}