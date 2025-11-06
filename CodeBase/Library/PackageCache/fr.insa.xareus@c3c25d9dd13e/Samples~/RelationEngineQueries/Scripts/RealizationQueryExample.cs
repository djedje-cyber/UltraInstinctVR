using System.Collections.Generic;

using UnityEngine;

using Xareus.Relations;
using Xareus.Relations.Unity;

namespace Xareus.Samples.RelationEngineQueries
{
    /// <summary>
    /// This class shows examples of how to query the Relation Engine for realizations
    /// </summary>
    public class RealizationQueryExample : MonoBehaviour
    {
        #region Fields

        public List<XUObject> MandatoryObjects;
        public List<XUObject> OptionalObjects;
        public List<XURelation> Relations;

        #endregion

        #region Methods

        /// <summary>
        /// This method will be called at the next Unity Update step when the query is done
        /// </summary>
        public static void ResultCallback(IEnumerable<XURealization> realizations)
        {
            string resultString = string.Join(", ", realizations);
            Debug.Log("Async Query Result: " + resultString);
        }

        /// <summary>
        /// Query the Relation Engine for all possible realizations and call the callback when the query is done
        /// </summary>
        private static void AsyncrhonousQuery()
        {
            // This will query all possible realization in the scene
            RealizationQueryParameters directQueryParameters = RealizationQueryParameters.GetQueryAllOptionalAllRelation(RelationEngine.Instance.Engine);

            RelationEngine.Instance.GetRealizations(directQueryParameters, ResultCallback);
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
            RealizationQueryParameters directQueryParameters = new();

            // The mandatory objects must be present in the result
            foreach (XUObject xuObject in MandatoryObjects)
                directQueryParameters.MandatoryObjects.Add(xuObject);

            // The optional objects may be present in the result
            foreach (XUObject xuObject in OptionalObjects)
                directQueryParameters.OptionalObjects.Add(xuObject);

            foreach (XURelation relation in Relations)
                directQueryParameters.RelationsToUse.Add(relation);

            List<XURealization> directResult = RelationEngine.Instance.GetRealizations(directQueryParameters);

            string resultString = string.Join(", ", directResult);
            Debug.Log("Direct Query Result: " + resultString);
        }

        #endregion
    }
}
