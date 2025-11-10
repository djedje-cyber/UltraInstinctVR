using System.Collections;
using UnityEngine;




namespace ChildBahaviorSpace
{
    public class ChildBehavior : MonoBehaviour
    {
        private float duration = 30f; // Delay between actions
        private ParentController parentController;

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
