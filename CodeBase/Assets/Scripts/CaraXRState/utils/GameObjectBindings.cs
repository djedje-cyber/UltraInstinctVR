using System;
using UnityEngine;

[Serializable]
public class GameObjectBinding
{
    public string FieldName;   // Name of the [InitialState] field in the test class
    public GameObject GameObject;  // The actual GameObject to inject
}