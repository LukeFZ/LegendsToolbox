using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerEntityUniqueId : BadgerVarULong
{
    public override BadgerObjectType Type => BadgerObjectType.EntityUniqueId;

    public BadgerEntityUniqueId(string name, ulong value = 0) : base(name, value) { }
}