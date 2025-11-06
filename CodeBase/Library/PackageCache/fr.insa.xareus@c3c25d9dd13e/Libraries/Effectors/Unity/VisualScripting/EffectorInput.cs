#if VISUAL_SCRIPTING

using Unity.VisualScripting;

#endif

namespace Xareus.Scenarios.Unity.VisualScripting
{
#if VISUAL_SCRIPTING

    [UnitCategory("Events/Xareus")]
    [UnitTitle("Effector Update")]
    public class EffectorInput : EventUnit<EmptyEventArgs>
    {
        #region Fields

        public static string INPUT_EVENT = "EffectorInputEvent";

        #endregion



        #region Fields

        protected override bool register => true;

        #endregion

        #region Methods

        public override EventHook GetHook(GraphReference reference)
        {
            // We create a hook with the parent game object as target because the effector must be able to trigger the event on this graph only
            return new EventHook(INPUT_EVENT, reference.gameObject);
        }

        #endregion
    }

#endif
}
