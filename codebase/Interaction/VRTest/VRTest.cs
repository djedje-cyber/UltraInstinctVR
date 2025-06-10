using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;
 // N'oublie pas d'inclure le namespace d'OpenXR

public class VRTest : MonoBehaviour
{
    protected static Dictionary<GameObject, ControlInfo> controls = new Dictionary<GameObject, ControlInfo>();
    protected static GameObject triggered;
    int index = 0;
    float passed = 0.0f;
    bool clickStay;

    Vector3 origin;
    Vector3 dest;
    Quaternion originrotate;
    Quaternion destrotate;
    protected Vector3 internalangle;

    protected float moveStep = 1f;
    protected float turnStep = 10f;
    protected float triggerlimit = 100f;
    protected Vector3 moveUpperBound = new Vector3(7f, 4.4f, 11f);
    protected Vector3 moveLowerBound = new Vector3(-14f, 4.3f, -1f);
    protected Vector3 turnUpperBound = new Vector3(60f, 180f, 0f);
    protected Vector3 turnLowerBound = new Vector3(-60f, -180f, 0f);

    protected Vector3[] moveOpts = new Vector3[6];
    protected Vector3[] turnOpts = new Vector3[6];
    protected List<Vector3> moves = new List<Vector3>();
    protected List<Vector3> turns = new List<Vector3>();

    float clickStayLength = 2f;
    float eventGap = 1f;

    // Ajout du timer pour l'arrêt automatique
    private float lastInteractionTime;
    private const float timeoutDuration = 900f; // 15 minutes
    private bool isRunning = true;

    // Ajout du compteur et du calcul du taux d'objets trouvés
    private int objectsFound = 0;
    private float timeElapsed = 0f;
    private float reportInterval = 60f; // Affiche le taux toutes les 60 secondes (1 minute)

    void Start()
    {
        controls.Clear();
        FetchControls();

        moveOpts[0] = new Vector3(1f, 0f, 0f);
        moveOpts[1] = new Vector3(-1f, 0f, 0f);
        moveOpts[2] = new Vector3(0f, 1f, 0f);
        moveOpts[3] = new Vector3(0f, -1f, 0f);
        moveOpts[4] = new Vector3(0f, 0f, 1f);
        moveOpts[5] = new Vector3(0f, 0f, -1f);

        turnOpts[0] = new Vector3(1f, 0f, 0f);
        turnOpts[1] = new Vector3(-1f, 0f, 0f);
        turnOpts[2] = new Vector3(0f, 1f, 0f);
        turnOpts[3] = new Vector3(0f, -1f, 0f);
        turnOpts[4] = new Vector3(0f, 0f, 1f);
        turnOpts[5] = new Vector3(0f, 0f, -1f);

        dest = transform.position;
        destrotate = transform.rotation;

        internalangle = new Vector3(0f, 0f, 0f);
        transform.eulerAngles = internalangle;

        clickStay = false;
        passed = 0f;

        // Initialisation du timer
        lastInteractionTime = Time.time;

        Initialize();
    }

    public virtual void Initialize()
    {
        string filePath = "Assets/Scripts/CoveredObjects/FoundObject.txt";
        if (File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath, false)) { }
            Debug.Log("Le fichier FoundObject.txt a été vidé.");
        }
        else
        {
            Debug.LogWarning("Le fichier FoundObject.txt n'existe pas.");
        }
    }

protected void FetchControls()
{
    // Création du dossier pour les objets trouvés
    string folderPath = "Assets/Scripts/CoveredObjects";
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    // Création du dossier pour les fichiers TESTREPLAY
    string replayFolderPath = "Assets/Scripts/TESTREPLAY";
    if (!Directory.Exists(replayFolderPath))
    {
        Directory.CreateDirectory(replayFolderPath);
    }

    // Génération d'un UUID unique et de la date courante pour le titre du fichier TESTREPLAY
    string uuid = Guid.NewGuid().ToString();
    string Date = DateTime.Now.ToString("yyyy-MM-dd");

    string replayFilePath = Path.Combine(replayFolderPath, $"TEST_REPLAY_ObjectFound_{uuid}_{Date}.txt");


    // Enregistrer dans le fichier de log dans CoveredObjects
    string coveredObjectsFilePath = Path.Combine(folderPath, "FoundObject.txt");

    // Création et ouverture des fichiers de log
    using (StreamWriter writerCoveredObjects = new StreamWriter(coveredObjectsFilePath, false))
    using (StreamWriter writerReplay = new StreamWriter(replayFilePath, false))
    {
        // Récupérer tous les objets dans la scène
        GameObject[] gos = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in gos)
        {
            // Vérification si l'objet possède un XRGrabInteractable
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null && !controls.ContainsKey(go))
            {
                // Ajouter l'objet à la liste de contrôles
                controls[go] = new ControlInfo(go);

                // Préparer les informations sur l'objet
                string objectInfo = $"{go.name}: {go.transform.position}";
                Debug.Log(objectInfo);

                // Écrire dans le fichier FoundObject.txt dans CoveredObjects
                writerCoveredObjects.WriteLine(objectInfo);

                // Écrire dans le fichier TEST_REPLAY
                writerReplay.WriteLine(objectInfo);

                // Incrémenter le compteur d'objets trouvés
                objectsFound++;

                // Attacher un événement PointerClick pour les objets XRGrabInteractable
                EventTrigger r = go.GetComponent<EventTrigger>();
                if (r == null)
                {
                    r = go.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => { UpdateTrigger(); });
                r.triggers.Add(entry);

                // Mettre à jour le timer de l'interaction
                lastInteractionTime = Time.time;
            }
        }
    }
}


    public static void UpdateTrigger()
    {
        if (controls.ContainsKey(triggered))
        {
            Debug.Log("Triggered Recorded:" + controls[triggered]);
            controls[triggered].SetTrigger();
        }
    }

    void FixedUpdate()
    {
        if (!isRunning) return; // Arrêter l'exécution du script si isRunning est false

        if (clickStay)
        {
            passed += Time.deltaTime;
            if (passed > clickStayLength)
            {
                clickStay = false;
                passed = 0.0f;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, moveStep * 0.02f);
            Quaternion q = Quaternion.RotateTowards(transform.rotation, destrotate, turnStep * 0.02f);
            transform.rotation = q;

            if (transform.position == dest && transform.rotation == destrotate)
            {
                destrotate = Turn();
                dest = Move();
                Trigger();
            }
        }

        // Mise à jour du temps écoulé
        timeElapsed = Time.time - lastInteractionTime;

        // Vérification du taux d'objets trouvés
        if (timeElapsed >= reportInterval)
        {
            float objectsPerMinute = (objectsFound / timeElapsed) * 60f; // Calcul du taux par minute
            Debug.Log($"Taux d'objets trouvés : {objectsPerMinute:F2} objets/minute.");

            // Si le taux tombe à 0, stopper uniquement ce script
            if (objectsPerMinute == 0)
            {
                Debug.Log("Le taux d'objets trouvés est tombé à 0. Arrêt du script.");
                isRunning = false;
            }

            // Réinitialisation pour le prochain calcul
            objectsFound = 0;
            lastInteractionTime = Time.time;
        }
    }

    public virtual Vector3 Move()
    {
        return transform.position;
    }

    public virtual bool Movable(Vector3 position, int flag)
    {
        switch (flag)
        {
            case 0: return position.x + moveStep < moveUpperBound.x && !Physics.Raycast(position, Vector3.right, moveStep);
            case 1: return position.x - moveStep > moveLowerBound.x && !Physics.Raycast(position, Vector3.left, moveStep);
            case 2: return position.y + moveStep < moveUpperBound.y && !Physics.Raycast(position, Vector3.up, moveStep);
            case 3: return position.y - moveStep > moveLowerBound.y && !Physics.Raycast(position, Vector3.down, moveStep);
            case 4: return position.z + moveStep < moveUpperBound.z && !Physics.Raycast(position, Vector3.forward, moveStep);
            case 5: return position.z - moveStep > moveLowerBound.z && !Physics.Raycast(position, Vector3.back, moveStep);
            default: return false;
        }
    }

    public virtual Quaternion Turn()
    {
        return transform.rotation;
    }

    public virtual bool Turnable(Vector3 angle, int flag)
    {
        switch (flag)
        {
            case 0: return angle.x + turnStep < turnUpperBound.x;
            case 1: return angle.x - turnStep > turnLowerBound.x;
            case 2: return angle.y + turnStep < turnUpperBound.y;
            case 3: return angle.y - turnStep > turnLowerBound.y;
            case 4: return angle.z + turnStep < turnUpperBound.z;
            case 5: return angle.z - turnStep > turnLowerBound.z;
            default: return false;
        }
    }

    public virtual void UpdateMoves()
    {
        moves.Clear();
        Vector3 position = transform.position;
        for (int i = 0; i < 6; i++)
        {
            if (Movable(position, i))
            {
                moves.Add(moveOpts[i]);
            }
        }
    }

    public virtual void UpdateTurns()
    {
        turns.Clear();
        Vector3 angle = internalangle;
        for (int i = 0; i < 6; i++)
        {
            if (Turnable(angle, i))
            {
                turns.Add(turnOpts[i]);
            }
        }
    }

    public virtual void Trigger()
    {
        UpdateMoves();
        UpdateTurns();
    }


    protected void pointClick(GameObject obj)
    {
        Debug.Log("clicking " + obj.name);
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
        clickStay = true;
    }



    protected class ControlInfo
    {
        GameObject control;
        int triggered;
        public ControlInfo(GameObject obj)
        {
            this.control = obj;
            this.triggered = 0;
        }
        public GameObject getObject()
        {
            return this.control;
        }
        public int getTriggered()
        {
            return this.triggered;
        }
        public void SetTrigger()
        {
            this.triggered = this.triggered + 1;
        }
    }
}
