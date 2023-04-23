using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerOptionalHashedEnum : BadgerHashedEnum
{
    public override BadgerObjectType Type => BadgerObjectType.OptionalHashedEnum;
    public BadgerOptionalHashedEnum(string name, uint value = 0) : base(name, value) { }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        var hasValue = Value != 0;
        writer.Write(hasValue);

        if (hasValue)
            base.Serialize(writer);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        if (!reader.ReadBoolean())
            return;

        base.Deserialize(reader, maxLength - 1);
    }
}