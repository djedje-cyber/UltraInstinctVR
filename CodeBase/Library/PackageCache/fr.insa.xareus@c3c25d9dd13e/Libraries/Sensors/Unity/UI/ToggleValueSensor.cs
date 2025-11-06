using System.Collections.Generic;

using UnityEngine.UI;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity.UI
{
    [Renamed("Xareus.Scenarios.Unity.UI.ToggleValueSensor", "Xareus.Unity.Librairies")]
    public class ToggleValueSensor : AUnitySensor
    {
        #region Fields

        public enum SelectMode
        {
            Select,
            Deselect,
            Both
        }

        [EventContextEntry()]
        public static readonly string TOGGLE = "Toggle";

        [EventContextEntry()]
        public static readonly string VALUE = "Value";

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext;

        [ConfigurationParameter("Toggle", Necessity.Required)]
        protected Toggle toggle;

        [ConfigurationParameter("Validate On", Necessity.Required)]
        protected SelectMode selectionMode;

        private bool valid;

        #endregion

        #region Constructors

        public ToggleValueSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
            toggle.onValueChanged.RemoveListener(ValueChanged);
        }

        public override void SafeReset()
        {
            if (toggle == null)
                throw new System.InvalidOperationException(nameof(toggle) + " cannot be null");

            // Check if the toggle is in the right state already
            valid = (toggle.isOn && (selectionMode == SelectMode.Select || selectionMode == SelectMode.Both))
                    || (!toggle.isOn && (selectionMode == SelectMode.Deselect || selectionMode == SelectMode.Both));

            eventContext = new SimpleDictionary
            {
                { TOGGLE , toggle },
                { VALUE , toggle.isOn }
            };

            // If not, listen to the toggle
            if (!valid)
                toggle.onValueChanged.AddListener(ValueChanged);
        }

        public override Result SafeSensorCheck()
        {
            return new Result(valid, eventContext);
        }

        private void ValueChanged(bool arg0)
        {
            valid = (arg0 && (selectionMode == SelectMode.Select || selectionMode == SelectMode.Both))
                || (!arg0 && (selectionMode == SelectMode.Deselect || selectionMode == SelectMode.Both));
            if (valid)
            {
                eventContext[VALUE] = arg0;
                UpdateScenario();
            }
        }

        #endregion
    }
}
