using System.Collections.Generic;

using UnityEngine;

using Xareus.Relations;
using Xareus.Relations.Unity;

namespace Xareus.Samples.RelationEngineQueries
{
    /// <summary>
    /// This class shows examples of how to query the Relation Engine for objects
    /// </summary>
    public class ObjectQueryExample : MonoBehaviour
    {
        #region Fields

        public List<XUObject> ObjectsTheQueryWillLookInto;

        #endregion

        #region Methods

        /// <summary>
        /// This method will be called at the next Unity Update step when the query is done
        /// </summary>
        public static void ResultCallback(IEnumerable<IObject> objects)
        {
            string resultString = string.Join(", ", objects);
            Debug.Log("Async Query Result: " + resultString);
        }

        /// <summary>
        /// Query the Relation Engine for all possible realizations and call the callback when the query is done
        /// </summary>
        private void AsyncrhonousQuery()
        {
            ObjectQueryParameters directQueryParameters = ObjectQueryParameters.GetQueryAllObjects(RelationEngine.Instance.Engine);

            // Create a type search and assign its parameters
            TypeSearch typeSearch = new()
            {
                Type = new Type(typeof(SampleType)),
                // Every object that holds the type TestType exactly once
                MinOccurs = 1,
                MaxOccurs = 1
            };

            directQueryParameters.TypesToHave.Add(typeSearch);

            RelationEngine.Instance.GetObjects(directQueryParameters, ResultCallback);
        }

        private void Start()
        {
            DirectQuery();
            AsyncrhonousQuery();
        }

        /// <summary>
        /// Query the Relation Engine for all possible realizations (blocking the main thread)
        /// </summary>
        private void DirectQuery()
        {
            ObjectQueryParameters directQueryParameters = new();
            foreach (XUObject xuObject in ObjectsTheQueryWillLookInto)
                directQueryParameters.ObjectsToLookInto.Add(xuObject);

            // Create a type search and assign its parameters
            TypeSearch typeSearch = new()
            {
                Type = new Type(typeof(SampleType)),
                // Every object that holds the type TestType once or twice will be returned
                MinOccurs = 1,
                MaxOccurs = 2
            };

            directQueryParameters.TypesToHave.Add(typeSearch);

            List<IObject> directResult = RelationEngine.Instance.GetObjects(directQueryParameters);

            string resultString = string.Join(", ", directResult);
            Debug.Log("Direct Query Result: " + resultString);
        }

        #endregion
    }
}
