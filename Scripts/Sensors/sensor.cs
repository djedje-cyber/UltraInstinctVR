
/**
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using System.Collections.Generic;
[FunctionDescription("A Unity Sensor")]
public class sensor : AUnitySensor
{
    // A key that will be used in the EventContext
    [EventContextEntry()]
    public static readonly string KEY = "key";

    // A configuration parameter for the sensor that will be displayed
    // in the scenario inspector
    [ConfigurationParameter("Parameter", Necessity.Required)]
    protected string parameter;
        
    // The event context that will be returned
    private SimpleDictionary eventContext = new SimpleDictionary();

    public sensor(Xareus.Scenarios.Event @event, 
        Dictionary<string, List<string>> nameValueListMap, 
        IContext externalContext, 
        IContext scenarioContext, 
        IContext sequenceContext) 
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext)
    { }

    public override void SafeReset()
    {
        //Initial operations and checks
    }

    public override Result SafeSensorCheck()
    {
        // Check performed at regular interval (each time the scenario updates)
        
        if (valid)
        {
            eventContext.Add(KEY, value);
        }
        return new Result(valid, eventContext);
    }
}

*/
