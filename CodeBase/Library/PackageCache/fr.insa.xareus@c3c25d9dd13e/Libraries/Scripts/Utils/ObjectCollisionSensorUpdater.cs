using System;
using System.Linq;

using UnityEngine;

namespace Xareus.Scenarios.Unity.Utils
{

    public static class ObjectCollisionSensorUpdater
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void RegisterScenarioUpdate()
        {
            Scenario.ScenarioBeforeLoadEvent += UpdateSensors;
        }

        private static void UpdateSensors(object sender, EventArgs arg)
        {
            Scenario scenario = sender as Scenario;
            scenario.GetAllTransitions().SelectMany(transition => transition.@event.sensorCheck.Where(sensorFunction => typeof(ObjectCollisionSensor).IsAssignableFrom(sensorFunction.FunctionType))).ToList().ForEach(UpdateSensor);
        }


        private static void UpdateSensor(Function obj)
        {
            bool inCollisionValue = true;
            Parameter negateParameter = obj.param.FirstOrDefault(p => p.name == ObjectCollisionSensor.NEGATE_PARAMETER);
            Parameter inCollisionParameter = obj.param.FirstOrDefault(p => p.name == ObjectCollisionSensor.IN_COLLISION);
            if (inCollisionParameter != null)
            {
                if (ValueParser.ConvertFromString<Type>(inCollisionParameter.type) != typeof(bool))
                {
                    Debug.LogWarning("The parameter " + ObjectCollisionSensor.IN_COLLISION + " of the sensor " + obj.FunctionType + " is not a Bool Parameter and will not be updated ! Please update it manually");
                    return;
                }

                inCollisionValue = ValueParser.ConvertFromParameter<bool>(inCollisionParameter);

                // if false, we need to invert the negate value
                if (!inCollisionValue)
                {
                    // we need to create the Negate parameter if it does not exist
                    if (negateParameter == null)
                    {
                        negateParameter = ValueParser.ConvertToParameter<bool>(!inCollisionValue);
                        negateParameter.name = ObjectCollisionSensor.NEGATE_PARAMETER;
                        obj.param.Add(negateParameter);
                    }
                    else
                    {
                        negateParameter.value = ValueParser.ConvertTo<string>(!inCollisionValue);
                    }
                }
                // in the end, remove the obsolete parameter
                obj.param.Remove(inCollisionParameter);
                Debug.Log("Updated Objetc Collision Sensor parameters");


            }
        }

    }
}
