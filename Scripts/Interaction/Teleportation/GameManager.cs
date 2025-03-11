using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MonoBehaviour script1;
    public MonoBehaviour script2;
    public MonoBehaviour script3;
    private int currentScript = 0;

    void Start()
    {
        // Désactive tous les scripts au début
        script1.enabled = false;
        script2.enabled = false;
        script3.enabled = false;

        // Démarrer le premier script
        ExecuteNextScript();
    }

    public void ExecuteNextScript()
    {
        if (currentScript == 0)
        {
            script1.enabled = true;
        }
        else if (currentScript == 1)
        {
            script1.enabled = false;
            script2.enabled = true;
        }
        else if (currentScript == 2)
        {
            script2.enabled = false;
            script3.enabled = true;
        }
    }

    public void ScriptFinished()
    {
        currentScript++;
        ExecuteNextScript();
    }
}
