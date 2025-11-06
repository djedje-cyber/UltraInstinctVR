using System.Collections.Generic;

using UnityEngine;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Utilities;

using SystemRandom = System.Random;

namespace Xareus.Scenarios.Relations
{
    /// <summary>
    /// Selects Xareus types from a given list
    /// </summary>
    [Renamed("SEVEN.SelectFromListEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.SelectFiveTypesFromListEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Relations.SelectXareusTypesFromListEffector", "Xareus.Unity.Librairies")]
    public class SelectXareusTypesFromListEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameterAttribute("nbElem", Necessity.Required)]
        protected int nbElem;

        [ConfigurationParameterAttribute("list", Necessity.Required)]
        protected List<XUType> list;

        [ConfigurationParameterAttribute("resultList", Necessity.Required)]
        protected string resultList;

        private readonly List<XUObject> output = new();

        #endregion

        #region Constructors

        public SelectXareusTypesFromListEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        #region Miscellaneous

        public override void SafeEffectorUpdate()
        {
            if (list == null)
                Debug.LogWarning("null : " + list);

            //Setup
            List<XUType> availableObjects = new(list);
            SystemRandom random = new();

            for (int i = 0; i < nbElem; i++)
            {
                if (availableObjects.Count < 1)
                {
                    Debug.LogWarning("No available objects left");
                    break;
                }

                //Selects object
                int randomIndex = random.Next(availableObjects.Count);
                XUType selectedObjectXuTypeComponent = availableObjects[randomIndex];
                availableObjects.RemoveAt(randomIndex);

                //Fetches the XUObject matching the XUType
                XUObject selectedObjectXuObjectComponent = selectedObjectXuTypeComponent.GetComponent<XUObject>();
                if (selectedObjectXuObjectComponent == null)
                {
                    throw new MissingComponentException("A get object with a " + nameof(XUType) + " must contain an " + nameof(XUObject) + " component");
                }

                output.Add(selectedObjectXuObjectComponent);

                //Sets values in token
                ValueParser.SetValue(resultList + "[" + i + "]", output[i], Contexts);
            }
        }

        #endregion

        #endregion
    }
}
