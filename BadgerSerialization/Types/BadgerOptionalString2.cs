using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalString2 : BadgerOptionalString
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalString2;
    public BadgerOptionalString2(string name, string? value = null) : base(name, value) { }
}