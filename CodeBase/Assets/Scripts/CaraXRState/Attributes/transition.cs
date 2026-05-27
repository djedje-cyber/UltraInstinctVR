using System;

[AttributeUsage(AttributeTargets.Method)]
public class TransitionAttribute : Attribute
{
    public int Id { get; }
    public int UpstreamPlace { get; }
    public TransitionAttribute(int id, int upstreamPlace = 0)
    {
        Id = id;
        UpstreamPlace = upstreamPlace;
    }
}


