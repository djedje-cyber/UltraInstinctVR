using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class FinalStateAttribute : Attribute
{
    // No constructor needed — inheriting from Attribute is enough
}
