using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Unity;
using UnityEngine;
using System.Collections.Generic;
using System;

[FunctionDescription("An Object Selection Detection Sensor")]
public class ObjectSelectionSensor : AInUnityStepSensor
{
    // Clé pour ajouter l'objet sélectionné dans le contexte d'événement
    [EventContextEntry()]
    public static readonly string SELECTED_OBJECT_KEY = "SelectedObject";

    private SimpleDictionary eventContext = new SimpleDictionary();
    private string lastSelectedObject = "";

    public ObjectSelectionSensor(Xareus.Scenarios.Event @event,
        Dictionary<string, Xareus.Scenarios.Parameter> nameValueListMap,
        IContext externalContext,
        IContext scenarioContext,
        IContext sequenceContext)
        : base(@event, nameValueListMap, externalContext, scenarioContext, sequenceContext)
    { }

    public override void SafeReset()
    {
        // Réinitialiser le contexte d'événement
        eventContext.Clear();
        lastSelectedObject = "";
    }

    public override Result UnityStepSensorCheck()
    {
        if (!string.IsNullOrEmpty(lastSelectedObject))
        {
            string selectedObjectKey = $"{SELECTED_OBJECT_KEY}_{Guid.NewGuid()}";
            eventContext[selectedObjectKey] = lastSelectedObject;
            lastSelectedObject = ""; // Reset après enregistrement
            return new Result(true, eventContext);
        }
        
        return new Result(false, eventContext);
    }

    // Méthode pour détecter une sélection via collision
    private void OnTriggerEnter(Collider other)
    {
        lastSelectedObject = other.gameObject.name;
    }
}
