using UnityEngine;
using UnityEngine.XR.ARFoundation;

[TestInteractionClass]
public class MoveControllerAndSelect
{
    [InitialState]
    protected GameObject gameObject;
    [InitialState]
    protected GameObject targetGameObject;

    [Transition(1)] [Place(1)]
    public void moveArm()
    {
        
    }


    [Transition(2)] [Place(2)]
    public void select() {

    }

}