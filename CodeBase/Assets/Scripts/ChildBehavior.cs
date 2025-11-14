using System.Collections;
using UnityEngine;




namespace ChildBahaviorSpace
{


    /// <summary>
    /// Class <c>ChildBehavior</c>Represents a child behavior in a Unity scene that interacts with a parent controller.
    /// </summary>
    /// <remarks>This class is responsible for managing the lifecycle of a child object, including
    /// initialization and executing a predefined action. It communicates with a parent controller to signal the
    /// completion of its action. The behavior includes a delay between initialization and action completion, which can
    /// be customized by modifying the <c>duration</c> field.</remarks>
    public class ChildBehavior : MonoBehaviour
    {
        private float duration = 30f; // Delay between actions
        private ParentController parentController;



        /// <summary>
        /// Method <c>Initialize</c> Initializes the child behavior with a reference to its parent controller.
        /// </summary>
        /// <param name="controller"></param>
        public void Initialize(ParentController controller)
        {
            if (parentController != null)
            {
                Debug.LogWarning($"⚠ {gameObject.name} has already been initialized !");
                return;
            }

            parentController = controller;
            Debug.Log($"🎬 {gameObject.name} initialised !");
            StartCoroutine(ExecuteAction());
        }



        /// <summary>
        /// Method <c>ExecuteAction</c> Simulates the execution of an action by the child behavior.
        /// </summary>
        /// <returns></returns>
        IEnumerator ExecuteAction()
        {
            Debug.Log($"➡ {gameObject.name} begins his interaction !");

            // Simulation of an action (e.g., rotation, animation, etc.)
            yield return new WaitForSeconds(duration);

            Debug.Log($"✅ {gameObject.name} has terminated his execution !");

            // Inform the parent that this child has finished
            parentController.ChildFinished();
        }
    }
}
