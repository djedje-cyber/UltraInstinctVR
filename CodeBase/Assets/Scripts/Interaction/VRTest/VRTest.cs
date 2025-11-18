using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Class <c>VRTest</c> is a base class for VR interaction tests in Unity.
/// </summary>
public class VRTest : MonoBehaviour
{
    protected static Dictionary<GameObject, ControlInfo> controls = new Dictionary<GameObject, ControlInfo>();
    protected static GameObject triggered;

    // Movement & rotation
    protected Vector3 dest;
    protected Quaternion destrotate;
    protected Vector3 internalangle;
    protected float moveStep = 1f;
    protected float turnStep = 10f;
    protected Vector3 moveUpperBound = new Vector3(7f, 4.4f, 11f);
    protected Vector3 moveLowerBound = new Vector3(-14f, 4.3f, -1f);
    protected Vector3 turnUpperBound = new Vector3(60f, 180f, 0f);
    protected Vector3 turnLowerBound = new Vector3(-60f, -180f, 0f);
    protected Vector3[] moveOpts = new Vector3[6];
    protected Vector3[] turnOpts = new Vector3[6];
    protected List<Vector3> moves = new List<Vector3>();
    protected List<Vector3> turns = new List<Vector3>();

    // Click logic
    float clickStayLength = 2f;
    float passed = 0f;
    bool clickStay;

    // Timer & Stats
    private float lastInteractionTime;
    private const float timeoutDuration = 900f; // 15 minutes
    private bool isRunning = true;
    private int objectsFound = 0;
    private float timeElapsed = 0f;
    private float reportInterval = 60f;

    // Handlers
    protected VRLogger logger;
    protected VRMovementHandler movementHandler;
    protected VRTriggerHandler triggerHandler;




    /// <summary>
    /// Method <c>Start</c> initializes the VR test environment.
    /// </summary>

    void Start()
    {
        controls.Clear();

        logger = new VRLogger();
        movementHandler = new VRMovementHandler(transform, moveStep, turnStep, moveUpperBound, moveLowerBound, turnUpperBound, turnLowerBound, moveOpts, turnOpts);
        triggerHandler = new VRTriggerHandler(transform, controls);

        movementHandler.InitMoveTurnOptions();
        dest = transform.position;
        destrotate = transform.rotation;
        internalangle = Vector3.zero;
        transform.eulerAngles = internalangle;

        clickStay = false;
        passed = 0f;

        lastInteractionTime = Time.time;

        Initialize();
        triggerHandler.FetchControls(ref objectsFound, ref lastInteractionTime);
    }


    /// <summary>
    /// Method <c>Initialize</c> performs any necessary initialization before the test starts.
    /// </summary>

    public virtual void Initialize()
    {
        logger.ClearFoundObjectsFile();
    }


    /// <summary>
    /// Method <c>FixedUpdate</c> handles the main update loop for movement, rotation, and interaction timing.
    /// </summary>

    void FixedUpdate()
    {
        if (!isRunning) return;

        if (clickStay)
        {
            passed += Time.deltaTime;
            if (passed > clickStayLength)
            {
                clickStay = false;
                passed = 0f;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, moveStep * 0.02f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destrotate, turnStep * 0.02f);

            if (transform.position == dest && transform.rotation == destrotate)
            {
                destrotate = Turn();
                dest = Move();
                Trigger();
            }
        }

        timeElapsed = Time.time - lastInteractionTime;
        if (timeElapsed >= reportInterval)
        {
            float objectsPerMinute = (objectsFound / timeElapsed) * 60f;
            Debug.Log($"Taux d'objets trouvés : {objectsPerMinute:F2} objets/minute.");

            if (objectsPerMinute == 0)
            {
                Debug.Log("Le taux d'objets trouvés est tombé à 0. Arrêt du script.");
                isRunning = false;
            }

            objectsFound = 0;
            lastInteractionTime = Time.time;
        }
    }


    /// <summary>
    /// Method <c>Move</c> determines the next movement position.
    /// </summary>
    /// <returns>The position of the player</returns>
    public virtual Vector3 Move() => transform.position;

    /// <summary>
    /// Method <c>Movable</c> checks if the desired movement position is valid.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public virtual bool Movable(Vector3 position, int flag) => movementHandler.Movable(position, flag);

    /// <summary>
    /// Method <c>Turn</c> determines the next rotation.
    /// </summary>
    /// <returns></returns>
    public virtual Quaternion Turn() => transform.rotation;


    /// <summary>
    /// Method <c>Turnable</c> checks if the desired rotation is valid.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public virtual bool Turnable(Vector3 angle, int flag) => movementHandler.Turnable(angle, flag);


    /// <summary>
    /// Method <c>UpdateMoves</c> updates the list of possible movements.
    /// </summary>
    public virtual void UpdateMoves() => movementHandler.UpdateMoves(moves);

    /// <summary>
    /// Method <c>UpdateTurns</c> updates the list of possible rotations.
    /// </summary>
    public virtual void UpdateTurns() => movementHandler.UpdateTurns(turns, internalangle);


    /// <summary>
    /// Method <c>Trigger</c> handles interaction triggers.
    /// </summary>
    public virtual void Trigger()
    {
        UpdateMoves();
        UpdateTurns();
    }

    /// <summary>
    /// Method <c>UpdateTrigger</c> updates the trigger state for the last interacted control.
    /// </summary>
    public static void UpdateTrigger()
    {
        if (controls.ContainsKey(triggered))
        {
            Debug.Log("Triggered Recorded:" + controls[triggered]);
            controls[triggered].SetTrigger();
        }
    }


    /// <summary>
    /// Method <c>pointClick</c> simulates a mouse click on the specified GameObject.
    /// </summary>
    /// <param name="obj"></param>
    protected void pointClick(GameObject obj)
    {
        Debug.Log("clicking " + obj.name);
        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
        clickStay = true;
    }


    /// <summary>
    /// Method <c>ControlInfo</c> stores information about interactive controls.
    /// </summary>

    public class ControlInfo
    {
        GameObject control;
        int triggered;
        public ControlInfo(GameObject obj) { control = obj; triggered = 0; }
        public GameObject getObject() => control;
        public int getTriggered() => triggered;
        public void SetTrigger() => triggered++;
    }
}
