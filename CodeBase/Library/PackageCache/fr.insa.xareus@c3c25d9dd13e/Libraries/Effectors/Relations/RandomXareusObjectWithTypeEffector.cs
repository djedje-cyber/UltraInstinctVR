using System;
using System.Collections.Generic;

using UnityEngine;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

using SystemRandom = System.Random;

namespace Xareus.Scenarios.Relations
{
    /// <summary>
    /// Selects elements from the list of objects of the given type. The selection is random.
    /// </summary>
    [Renamed("SEVEN.SelectFromTypeEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.SelectFromFiveTypeEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Relations.RandomXareusObjectWithTypeEffector", "Xareus.Unity.Librairies")]
    public class RandomXareusObjectWithTypeEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameterAttribute("nbElem", Necessity.Required)]
        protected int nbElem;

        [ConfigurationParameterAttribute("type", Necessity.Required)]
        [ProvideConstraint(typeof(XUType))]
        protected Type type;

        [ConfigurationParameterAttribute("resultList", Necessity.Required)]
        protected string resultList;

        private readonly List<XUObject> output = new();

        #endregion

        #region Constructors

        public RandomXareusObjectWithTypeEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        #region Miscellaneous

        public override void SafeEffectorUpdate()
        {
            //Setup
            object[] availableTypeComponentsArray = Resources.FindObjectsOfTypeAll(type);
            List<XUType> availableXuTypeComponents = new();

            foreach (object availableTypeComponent in availableTypeComponentsArray)
            {
                XUType availableXuTypeComponent = availableTypeComponent as XUType;
                if (availableXuTypeComponent != null)
                {
                    availableXuTypeComponents.Add(availableXuTypeComponent);
                }
            }

            SystemRandom random = new();

            for (int i = 0; i < nbElem; i++)
            {
                if (availableXuTypeComponents.Count < 1)
                {
                    Debug.LogWarning("No available objects left");
                    break;
                }

                //Selecting object
                int randomIndex = random.Next(availableXuTypeComponents.Count);
                XUType selectedObjectXuTypeComponent = availableXuTypeComponents[randomIndex];
                availableXuTypeComponents.RemoveAt(randomIndex);

                //Fetching the XUObject matching the Type
                XUObject selectedObjectXuObjectComponent = selectedObjectXuTypeComponent.GetComponent<XUObject>();
                if (selectedObjectXuObjectComponent == null)
                {
                    throw new MissingComponentException("A get object with a " + nameof(XUType) + " must contain an " + nameof(XUObject) + " component");
                }

                output.Add(selectedObjectXuObjectComponent);

                //Set values in token
                ValueParser.SetValue(resultList + "[" + i + "]", output[i], Contexts);
            }
        }

        #endregion

        #endregion
    }
}
