using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Class <c>VRMovementHandler</c> handles VR movement and turning logic in Unity.
/// </summary>
public class VRMovementHandler
{
    Transform transform;
    float moveStep, turnStep;
    Vector3 moveUpperBound, moveLowerBound;
    Vector3 turnUpperBound, turnLowerBound;
    Vector3[] moveOpts, turnOpts;


    /// <summary>
    /// Method <c>VRMovementHandler</c> initializes a new instance of the VRMovementHandler class.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="ms"></param>
    /// <param name="ts"></param>
    /// <param name="mU"></param>
    /// <param name="mL"></param>
    /// <param name="tU"></param>
    /// <param name="tL"></param>
    /// <param name="mOpts"></param>
    /// <param name="tOpts"></param>
    public VRMovementHandler(Transform t, float ms, float ts, Vector3 mU, Vector3 mL, Vector3 tU, Vector3 tL, Vector3[] mOpts, Vector3[] tOpts)
    {
        transform = t;
        moveStep = ms;
        turnStep = ts;
        moveUpperBound = mU; moveLowerBound = mL;
        turnUpperBound = tU; turnLowerBound = tL;
        moveOpts = mOpts; turnOpts = tOpts;
    }

    /// <summary>
    /// Method <c>InitMoveTurnOptions</c> initializes movement and turning options.
    /// </summary>
    public void InitMoveTurnOptions()
    {
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
    }


    /// <summary>
    /// Method <c>Movable</c> checks if movement is possible in a given direction based on position and flag.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool Movable(Vector3 position, int flag)
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

    /// <summary>
    /// Method <c>Turnable</c> checks if turning is possible in a given direction based on angle and flag.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="flag"></param>
    /// <returns></returns>

    public bool Turnable(Vector3 angle, int flag)
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

    /// <summary>
    /// Method <c>UpdateMoves</c> updates the list of possible moves based on current position.
    /// </summary>
    /// <param name="moves"></param>
    public void UpdateMoves(List<Vector3> moves)
    {
        moves.Clear();
        Vector3 position = transform.position;
        for (int i = 0; i < 6; i++)
            if (Movable(position, i)) moves.Add(moveOpts[i]);
    }


    /// <summary>
    /// Method <c>UpdateTurns</c> updates the list of possible turns based on current internal angle.
    /// </summary>
    /// <param name="turns"></param>
    /// <param name="internalAngle"></param>
    public void UpdateTurns(List<Vector3> turns, Vector3 internalAngle)
    {
        turns.Clear();
        for (int i = 0; i < 6; i++)
            if (Turnable(internalAngle, i)) turns.Add(turnOpts[i]);
    }
}
