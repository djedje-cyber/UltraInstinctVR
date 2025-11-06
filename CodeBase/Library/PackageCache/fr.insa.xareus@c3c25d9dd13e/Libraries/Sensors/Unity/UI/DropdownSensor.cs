using System.Collections.Generic;

using UnityEngine.UI;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity.UI
{
    [Renamed("Xareus.Scenarios.Unity.UI.DropdownSensor", "Xareus.Unity.Librairies")]
    public class DropdownSensor : AUnitySensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string DROPDOWN = "Dropdown";

        [EventContextEntry()]
        public static readonly string VALUE = "Value";

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext;

        [ConfigurationParameter("Dropdown", Necessity.Required)]
        protected Dropdown dropdown;

        [ConfigurationParameter("Value", Necessity.Required)]
        protected int value;

        private bool valid;

        #endregion

        #region Constructors

        public DropdownSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
            dropdown.onValueChanged.RemoveListener(ValueChanged);
        }

        public override void SafeReset()
        {
            if (dropdown == null)
                throw new System.InvalidOperationException(nameof(dropdown) + " cannot be null");

            // Check if the toggle is in the right state already
            valid = dropdown.value == value;

            eventContext = new SimpleDictionary
            {
                { DROPDOWN , dropdown },
                { VALUE , dropdown.value }
            };

            // If not, listen to the toggle
            if (!valid)
                dropdown.onValueChanged.AddListener(ValueChanged);
        }

        public override Result SafeSensorCheck()
        {
            return new Result(valid, eventContext);
        }

        private void ValueChanged(int arg0)
        {
            valid = dropdown.value == value;
            if (valid)
            {
                eventContext[VALUE] = arg0;
                UpdateScenario();
            }
        }

        #endregion
    }
}
