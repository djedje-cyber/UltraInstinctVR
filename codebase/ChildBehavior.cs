using System.Collections;
using UnityEngine;

public class ChildBehavior : MonoBehaviour
{
    public float duration = 2f; // Durée avant de passer au suivant
    private ParentController parentController;

    public void Initialize(ParentController controller)
    {
        if (parentController != null)
        {
            Debug.LogWarning($"⚠ {gameObject.name} a déjà été initialisé !");
            return;
        }

        parentController = controller;
        Debug.Log($"🎬 {gameObject.name} initialisé !");
        StartCoroutine(ExecuteAction());
    }


    IEnumerator ExecuteAction()
    {
        Debug.Log($"➡ {gameObject.name} begins his interaction !");
        
        // Simulation d’une action (ex: rotation, animation, etc.)
        yield return new WaitForSeconds(duration);

        Debug.Log($"✅ {gameObject.name} has terminated his execution !");

        // Informer le parent que cet enfant a fini
        parentController.ChildFinished();
    }
}
