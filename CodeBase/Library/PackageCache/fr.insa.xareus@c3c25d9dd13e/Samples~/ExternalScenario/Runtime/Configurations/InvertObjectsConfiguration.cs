using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

public class InvertObjectsConfiguration : AUnityConfiguration
{
    #region Fields

    [ConfigurationParameter("Object1", "The first object to move", Necessity = Necessity.Required)]
    public GameObject Object1;

    [ConfigurationParameter("Object2", "The second object to move", Necessity = Necessity.Required)]
    public GameObject Object2;

    #endregion

    #region Constructors

    public InvertObjectsConfiguration(ASequence sequence, Dictionary<string, Parameter> parameters, ContextHolder contexts) : base(sequence, parameters, contexts)
    {
    }

    #endregion

    #region Methods

    public override IContext SafeConfigure()
    {
        // Create the configuration context with all the variables we want to use in the sequence
        SimpleDictionary res = new()
        {
            ["Object1"] = Object1,
            ["Object2"] = Object2,
            ["Transform1"] = Object1.transform,
            ["Transform2"] = Object2.transform,
            ["Position1"] = Object1.transform.position,
            ["Position2"] = Object2.transform.position
        };
        return res;
    }

    #endregion
}
