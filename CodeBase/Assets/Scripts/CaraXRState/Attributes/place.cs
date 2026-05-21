using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
public class PlaceAttribute : Attribute
{
    public object Value { get; }

    public PlaceAttribute(object value)
    {
        Value = value;
    }
}