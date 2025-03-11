using System.Collections;
using UnityEngine;

public class TeleportRunner : MonoBehaviour
{
    public MonoBehaviour script1;
    public MonoBehaviour script2;
    public MonoBehaviour script3;

    public string methodName = "Execute"; // Méthode coroutine à appeler

    void Start()
    {
        StartCoroutine(RunScripts());
    }

    IEnumerator RunScripts()
    {
        yield return StartCoroutine(RunScript(script1));
        yield return StartCoroutine(RunScript(script2));
        yield return StartCoroutine(RunScript(script3));

        Debug.Log("Tous les scripts ont été exécutés !");
    }

    IEnumerator RunScript(MonoBehaviour script)
    {
        if (script != null)
        {
            // Vérifie si le script contient la méthode
            var method = script.GetType().GetMethod(methodName);
            if (method != null)
            {
                yield return StartCoroutine((IEnumerator)method.Invoke(script, null));
            }
            else
            {
                Debug.LogWarning($"La méthode {methodName} est introuvable sur {script.name}.");
            }
        }
        else
        {
            Debug.LogWarning("Un script est manquant !");
        }
    }
}
