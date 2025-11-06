using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;
using Xareus.Scenarios.Variables;

namespace Xareus.Scenarios.Unity
{
    [Renamed("SEVEN.Unity.SelectChildObjects", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.SelectChildObjects", "Xareus.Unity.Librairies")]
    public class SelectChildObjects : AUnityEffector
    {
        #region Fields

        /// <summary>
        /// The parent object
        /// </summary>
        [ConfigurationParameter("Object", "The object whose children will all be selected", Necessity.Required)]
        protected GameObject parentObject;

        [ContextVariable("Children")]
        protected ContextVariable<List<GameObject>> children;

        [ContextVariable("Number", "Number of children found")]
        protected ContextVariable<int> number;

        #endregion

        #region Constructors

        protected SelectChildObjects(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            List<GameObject> childrenList = new();
            for (int i = 0; i < parentObject.transform.childCount; ++i)
                childrenList.Add(parentObject.transform.GetChild(i).gameObject);
            children.Set(childrenList);
            number.Set(parentObject.transform.childCount);
        }

        #endregion
    }
}
