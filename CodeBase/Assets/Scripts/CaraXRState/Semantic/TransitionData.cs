using System.Collections.Generic;
using UnityEngine;



public class TransitionData
{
    public int TransitionId;
    public string Label;
    public int UpstreamPlace;
    public List<int> DownstreamPlaces;
    public string SensorClass;
    public string EffectorClass;
    public List<ParamData> SensorParams;
    public List<ParamData> EffectorParams;
    public List<GameObject> GameObjects; 
}
public class ParamData
{
    public string Name;
    public string Value;
    public string Type;
    public bool IsGuid;
    public GameObject GameObject; 
}