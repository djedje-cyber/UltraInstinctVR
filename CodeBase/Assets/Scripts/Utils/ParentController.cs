using UnityEngine;
using System.Collections;
using ChildBahaviorSpace;



///<summary>
///Class <c>ParentController</c> manages the sequential activation and deactivation of child GameObjects in a Unity scene.
///</summary>
public class ParentController : MonoBehaviour
{
    public GameObject[] childrenPrefabs; 
    private int currentChildIndex = 0;


    ///<summary>
    ///Class <c>Start</c> initializes the ParentController by deactivating all child GameObjects and starting the first child. 
    ///</summary>
    void Start()
    {
        Debug.Log("🚀 Initialisation of the test suite");
        
       
        foreach (GameObject child in childrenPrefabs)
        {
            child.SetActive(false);
        }

        if (childrenPrefabs.Length > 0)
        {
            StartNextChild();
        }
    }


    /// <summary>
    /// Class <c>StartNextChild</c> activates the next child GameObject in the sequence and initializes its behavior script.
    /// </summary>
    public void StartNextChild()
    {
        Debug.Log($"📢 StartNextChild() called - Actual Index : {currentChildIndex}");

        if (currentChildIndex < childrenPrefabs.Length)
        {
            GameObject child = childrenPrefabs[currentChildIndex];

            if (!child.activeSelf) // Vérifie si l'enfant est déjà activé
            {
                child.SetActive(true);
                Debug.Log($"▶ Activation of {child.name}");

                ChildBehavior childScript = child.GetComponent<ChildBehavior>();
                if (childScript != null)
                {
                    childScript.Initialize(this);
                }
            }
            else
            {
                Debug.LogWarning($"⚠ {child.name} was already activated !");
            }
        }
    }


    /// <summary>
    /// Class   <c>ChildFinished</c> handles the completion of a child GameObject's behavior, deactivates it, and starts the next child in the sequence.
    /// </summary>

    public void ChildFinished()
    {
        Debug.Log($"⏹ {childrenPrefabs[currentChildIndex].name} finish ! - Moving on to the next child");
        
        StartCoroutine(DeactivateAfterDelay(childrenPrefabs[currentChildIndex]));

        currentChildIndex++;

        if (currentChildIndex < childrenPrefabs.Length)
        {
            Debug.Log($"➡ Launch of the next child : {childrenPrefabs[currentChildIndex].name}");
            StartNextChild();
        }
        else
        {
            Debug.Log("✅ Test Suite terminated !");
            Debug.Log("ACTION_DONE");
        }
    }


    /// <summary>
    /// Class <c>DeactivateAfterDelay</c> deactivates a GameObject after a short delay to ensure proper cleanup.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>

    public IEnumerator DeactivateAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }
}
