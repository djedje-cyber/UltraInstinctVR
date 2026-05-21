using BNG;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[TestInteractionClass]
public class TestAbstractUndoRedo
{

    [InitialState]
    private GameObject controller;

    [InitialState]

    private Button buttonUndo;

    [InitialState]
    private Button buttonRedo;


    [Transition(1)] [Place(1)]

    public void MoveArm()
    {
    
    }


    [Transition(2)] [Place(2)]

    public void select()
    {
        
    }


    [Transition(3)] [Place(3)] [Place(4)]
    public void click()
    {
        
    }


    [Transition(4)] [Place(5)]
    public void unclickButtonA()
    {
        
    }


    [Transition(5)][Place(1)]
    public void unclickButtonB()
    {
        
    }


    //[FinalState]
    //Controller.expect()

}