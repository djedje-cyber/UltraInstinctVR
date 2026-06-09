using System;

[AttributeUsage(
    AttributeTargets.Method |
    AttributeTargets.Field |
    AttributeTargets.Property,
    AllowMultiple = false
)]
public class FinalStateAttribute : Attribute { }
