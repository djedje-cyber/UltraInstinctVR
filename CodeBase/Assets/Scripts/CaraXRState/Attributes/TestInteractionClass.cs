using System;

[AttributeUsage(AttributeTargets.Class)]
public class TestInteractionClassAttribute : Attribute
{
    // No constructor needed — inheriting from Attribute is enough
}
