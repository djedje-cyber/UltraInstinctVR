using System;
using System.Collections.Generic;

using UnityEngine;

using Xareus.Scenarios.Context;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Unity
{
    /// <summary>
    /// Change the animator values
    /// </summary>
    [FunctionDescriptionAttribute("Effector to change the value of an animator parameter", "Unity/Animation")]
    public class SetAnimatorParameterValueEffector : AUnityEffector
    {
        #region Fields

        [ConfigurationParameter("Animator", Necessity.Required)]
        protected Animator animator;

        [ConfigurationParameter("Parameter Name", Necessity.Required)]
        protected string animatorParameterName;

        [ConfigurationParameter("New Value", "Accepted types: int, float or bool", Necessity.Required)]
        protected object newValue;

        #endregion

        #region Constructors

        protected SetAnimatorParameterValueEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts)
        {
        }

        #endregion

        #region Methods

        public override void SafeEffectorUpdate()
        {
            if (newValue is bool animBoolValue)
            {
                animator.SetBool(animatorParameterName, animBoolValue);
            }
            else if (newValue is float animFloatValue)
            {
                animator.SetFloat(animatorParameterName, animFloatValue);
            }
            else if (newValue is int animIntValue)
            {
                animator.SetInteger(animatorParameterName, animIntValue);
            }
            else
            {
                throw new ArgumentException($"{newValue.GetType()} is not supported for {nameof(newValue)}. Only bool, float and int are supported by {nameof(SetAnimatorParameterValueEffector)}");
            }
        }

        #endregion
    }
}
