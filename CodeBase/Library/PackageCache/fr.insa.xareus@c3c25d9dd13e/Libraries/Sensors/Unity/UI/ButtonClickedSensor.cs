using System.Collections.Generic;

using UnityEngine.UI;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity.UI
{
    [Renamed("SEVEN.Unity.UI.ButtonClickedSensor", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Unity.UI.ButtonClickedSensor", "Xareus.Unity.Librairies")]
    public class ButtonClickedSensor : AUnitySensor
    {
        #region Fields

        [EventContextEntry()]
        public static readonly string BUTTON = "Button";

        /// <summary>
        /// The result
        /// </summary>
        protected SimpleDictionary eventContext;

        [ConfigurationParameter("Button", Necessity.Required)]
        protected Button button_clicked;

        private bool clicked;

        #endregion

        #region Constructors

        public ButtonClickedSensor(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeStopCheck()
        {
            button_clicked.onClick.RemoveListener(ReceiveClick);
        }

        public override void SafeReset()
        {
            clicked = false;
            if (button_clicked == null)
                throw new System.InvalidOperationException(nameof(button_clicked) + " cannot be null");

            eventContext = new SimpleDictionary
            {
                { BUTTON , button_clicked }
            };
            button_clicked.onClick.AddListener(ReceiveClick);
        }

        public override Result SafeSensorCheck()
        {
            return new Result(clicked, eventContext);
        }

        public void ReceiveClick()
        {
            clicked = true;
            UpdateScenario();
        }

        #endregion
    }
}
