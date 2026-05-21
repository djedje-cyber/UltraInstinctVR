using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
public class TransitionAttribute : Attribute
{


     public object Value { get; }

    public TransitionAttribute(object value)
    {
        Value = value;
    }

}
