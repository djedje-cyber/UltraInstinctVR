using UnityEngine;
using UnityEngine.XR.ARFoundation;

[TestInteractionClass]
public class MoveControllerAndSelect
{
    [InitialState] [Place(0)]
    protected GameObject gameObject;
    [InitialState] [Place(0)]
    protected GameObject targetGameObject;

    [Transition(1)] [Place(1)]
    public void moveArm()
    {
        
    }


    [Transition(2)]
    [Place(2)]
    public void select() {

    }

}