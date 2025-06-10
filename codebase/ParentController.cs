using UnityEngine;
using System.Collections;

public class ParentController : MonoBehaviour
{
    public GameObject[] childrenPrefabs; // Références aux enfants
    private int currentChildIndex = 0;

    void Start()
    {
        Debug.Log("🚀 Initialisation of the test suite");
        
        // Désactiver tous les enfants au début
        foreach (GameObject child in childrenPrefabs)
        {
            child.SetActive(false);
        }

        if (childrenPrefabs.Length > 0)
        {
            StartNextChild();
        }
    }

    public void StartNextChild()
    {
        Debug.Log($"📢 StartNextChild() appelé - Index actuel : {currentChildIndex}");

        if (currentChildIndex < childrenPrefabs.Length)
        {
            GameObject child = childrenPrefabs[currentChildIndex];

            if (!child.activeSelf) // Vérifie si l'enfant est déjà activé
            {
                child.SetActive(true);
                Debug.Log($"▶ Activation de {child.name}");

                ChildBehavior childScript = child.GetComponent<ChildBehavior>();
                if (childScript != null)
                {
                    childScript.Initialize(this);
                }
            }
            else
            {
                Debug.LogWarning($"⚠ {child.name} était déjà activé !");
            }
        }
    }


    public void ChildFinished()
    {
        Debug.Log($"⏹ {childrenPrefabs[currentChildIndex].name} terminé ! - Passage à l'enfant suivant.");
        
        StartCoroutine(DeactivateAfterDelay(childrenPrefabs[currentChildIndex]));

        currentChildIndex++;

        if (currentChildIndex < childrenPrefabs.Length)
        {
            Debug.Log($"➡ Lancement du prochain enfant : {childrenPrefabs[currentChildIndex].name}");
            StartNextChild();
        }
        else
        {
            Debug.Log("✅ Test Suite terminated !");
            Debug.Log("ACTION_DONE");
        }
    }


    public IEnumerator DeactivateAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f); // Petit délai avant désactivation
        obj.SetActive(false);
    }
}
