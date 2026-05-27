using System;

[AttributeUsage(
    AttributeTargets.Method |   // [Transition] methods
    AttributeTargets.Class |   // class-level places
    AttributeTargets.Field |   // field-level places
    AttributeTargets.Property,   // property-level places
    AllowMultiple = true
)]
public class PlaceAttribute : Attribute
{
    public int Id { get; }
    public PlaceAttribute(int id) { Id = id; }
}