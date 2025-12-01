using UnityEngine;

/// <summary>
/// Class <c>VRGreed</c> implements a greedy movement strategy for VR interaction tests in Unity.
/// </summary>
public class VRGreed : VRTest
{
    bool move;


    /// <summary>
    /// Method <c>Initialize</c> initializes the VRGreed movement state.
    /// </summary>
    public override void Initialize()
    {
        move = true;
    }

    /// <summary>
    /// Method <c>Move</c> determines the next position based on the current movement state.
    /// </summary>
    /// <returns></returns>
    public override Vector3 Move()
    {
        if (move)
        {
            UpdateMoves();
            System.Random rnd = new System.Random();
            int n = rnd.Next(0, moves.Count);
            return transform.position + moves[n] * moveStep;
        }
        else
        {
            return transform.position;
        }
    }

    /// <summary>
    /// Method <c>Turn</c> determines the next rotation, potentially changing the movement state.
    /// </summary>
    /// <returns></returns>

    public override Quaternion Turn()
    {
        System.Random rnd = new System.Random();
        int r = rnd.Next(0, 2);
        if (r == 0)
        {
            move = false;
            UpdateTurns();
            int n = rnd.Next(0, turns.Count);
            internalangle = internalangle + turns[n] * turnStep;

            return Quaternion.Euler(internalangle);
        }
        else
        {
            move = true;
            return transform.rotation;
        }
    }
}