using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using Xareus.Scenarios.Time.Unity;
using Xareus.Unity.Editor.Scenarios.UI.Fields;
using Xareus.Unity.Editor.Scenarios.UI.Group;

namespace Xareus.Unity.Editor.Scenarios.UI
{
    [FunctionEditBox(typeof(UnityTimeSensor), "Simplified", false)]
    public class UnityTimeSensorEditBox : DefaultEditBox
    {
        #region Fields

        private readonly ConstantVariableModeGroup minimumField;
        private readonly TimeUnitDropdownField unitField;

        #endregion

        #region Properties

        protected override IEnumerable<ABaseField> Fields => new List<ABaseField> { minimumField, unitField };

        #endregion

        #region Constructors 

        public UnityTimeSensorEditBox() : base()
        {
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("TimeSensorEditBox");
            visualTree.CloneTree(this);

            minimumField = this.Q<ConstantVariableModeGroup>("minimum");

            unitField = this.Q<TimeUnitDropdownField>("unit");
        }

        #endregion

        #region Methods 

        public override void Bind(Type selectedFunction, ScenarioFunction function)
        {
            minimumField.Bind(function, function.GetParameter("minimum"));

            unitField.GetStringForValue = (value) => value.ToString().ToLower() + "s";
            unitField.Bind(function, function.GetOrCreateParameter("unit", typeof(Xareus.Scenarios.Time.TimeUnit), Xareus.Scenarios.Time.TimeUnit.SECOND).Item2);
            unitField.SetValues(Enum.GetValues(typeof(Xareus.Scenarios.Time.TimeUnit)).Cast<Xareus.Scenarios.Time.TimeUnit>());
            unitField.Enable();

            UpdateAllFieldsFromParameters();
        }

        #endregion
    }
}
