using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalString3 : BadgerOptionalString
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalString3;
    public BadgerOptionalString3(string name, string? value = null) : base(name, value) { }
}