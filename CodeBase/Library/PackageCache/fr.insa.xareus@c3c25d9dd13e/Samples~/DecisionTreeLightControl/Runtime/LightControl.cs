using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;

namespace Xareus.Samples.DecisionTreeLightControl
{
    [RequireComponent(typeof(DecisionTreeEvaluation))]
    public class LightControl : MonoBehaviour
    {
        #region Fields

        public Light lightToControl;
        public DecisionTreeEvaluation decisionTreeEvaluation;
        public bool lightOn;
        public float power;

        #endregion

        #region Methods

        // Start is called before the first frame update
        private void Start()
        {
            decisionTreeEvaluation ??= GetComponent<DecisionTreeEvaluation>();
        }

        // Update is called once per frame
        private void Update()
        {
            IContext result = decisionTreeEvaluation.Evaluate();
            lightOn = (bool)result.GetValueOrDefault("LightOn", false);
            power = (float)result.GetValueOrDefault("Power", 0f);

            if (lightOn)
            {
                lightToControl.intensity = power;
            }
            else
            {
                lightToControl.intensity = 0;
            }
        }

        #endregion
    }
}
