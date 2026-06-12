using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TimeoutAttribute : Attribute
{
    public float Seconds { get; }

    public TimeoutAttribute(float seconds)
    {
        Seconds = seconds;
    }
}