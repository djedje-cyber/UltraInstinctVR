using System.Collections;

using UnityEngine;

using Xareus.Relations.Scenarios.Unity;
using Xareus.Scenarios.Unity;

namespace Xareus.Samples.TypeWithScenarioSample
{
    public class CustomType : TypeWithScenario
    {
        #region Fields

        protected float limit;

        #endregion

        #region Properties

        [LocalExternalContextEntry("Counter")]
        protected int Counter { get; set; }

        #endregion

        #region Methods

        [LocalExternalContextEntry("Status")]
        public bool GetStatus()
        {
            return Counter > limit;
        }

        [LocalExternalContextEntry("Limit")]
        public void Limit(float value)
        {
            limit = value;
        }

        protected void Start()
        {
            // Note: TypeWithScenario calls ScenarioRunner.AddExternalContextEntriesFrom(this); on awake
            // hence registering all [LocalExternalContextEntry] and [SharedExternalContextEntry] attributes
            Counter = 0;
            StartCoroutine(IncreaseCounter());
        }

        private IEnumerator IncreaseCounter()
        {
            while (Counter < 1000)
            {
                yield return new WaitForSeconds(1);
                Counter++;
            }
        }

        #endregion
    }
}
