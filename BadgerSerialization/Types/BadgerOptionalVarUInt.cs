using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalVarUInt : BadgerVarUInt
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalVarUInt;
    public BadgerOptionalVarUInt(string name, uint value = 0) : base(name, value) { }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        var hasValue = Value != 0;
        writer.Write(hasValue);

        if (hasValue)
            base.Serialize(writer);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = 0;
        if (!reader.ReadBoolean())
            return;

        base.Deserialize(reader, maxLength - 1);
    }
}