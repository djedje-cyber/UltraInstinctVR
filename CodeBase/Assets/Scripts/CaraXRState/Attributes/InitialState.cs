using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class InitialStateAttribute : Attribute
{
    // No constructor needed — inheriting from Attribute is enough
}
