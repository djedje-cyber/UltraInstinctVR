using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using Xareus.Scenarios.Extra.Time;
using Xareus.Unity.Editor.Scenarios.UI.Fields;
using Xareus.Unity.Editor.Scenarios.UI.Group;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(WaitEffector), "Simplified", false)]
    public class WaitEffectorEditBox : DefaultEditBox
    {
        #region Fields

        private readonly ConstantVariableModeGroup timeField;
        private readonly TimeUnitDropdownField unitField;

        #endregion

        #region Properties

        protected override IEnumerable<ABaseField> Fields => new List<ABaseField> { timeField, unitField };

        #endregion

        #region Constructors 

        public WaitEffectorEditBox() : base()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("TimeSensorEditBox");
            visualTree.CloneTree(this);

            timeField = this.Q<ConstantVariableModeGroup>("minimum");

            unitField = this.Q<TimeUnitDropdownField>("unit");
        }

        #endregion

        #region Methods 

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            timeField.Bind(function, function.GetParameter("delay"));

            unitField.GetStringForValue = (value) => value.ToString().ToLower() + "s";
            unitField.Bind(function, function.GetOrCreateParameter("unit", typeof(Xareus.Scenarios.Time.TimeUnit), Xareus.Scenarios.Time.TimeUnit.SECOND).Item2);
            unitField.SetValues(Enum.GetValues(typeof(Xareus.Scenarios.Time.TimeUnit)).Cast<Xareus.Scenarios.Time.TimeUnit>());
            unitField.Enable();

            UpdateAllFieldsFromParameters();
        }

        #endregion
    }
}
