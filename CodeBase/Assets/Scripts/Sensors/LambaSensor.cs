using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using System.Collections.Generic;

/// <summary>
/// Default sensor used when test uses DetectInteraction lambdas.
/// Always returns true — TestSuiteRunner handles the actual check.
/// </summary>
[FunctionDescription("Lambda Sensor")]
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
        // Always true — TestSuiteRunner drives the actual logic
        return new Result(true, new SimpleDictionary());
    }
}