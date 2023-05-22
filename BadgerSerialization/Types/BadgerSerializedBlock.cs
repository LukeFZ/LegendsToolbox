using System.Diagnostics;
using System.Text;
using BadgerSerialization.Core;
using BadgerSerialization.Models;

namespace BadgerSerialization.Types;

public class BadgerSerializedBlock : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.SerializedBlock;

    public SerializedBlock Value { get; set; }

    public BadgerSerializedBlock(string name, SerializedBlock data = default) : base(name)
    {
        Value = data;
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        if (Value.EntityRuntimeId != 0)
            throw new NotImplementedException();

        writer.WriteVarUInt32(Value.EntityRuntimeId + 1);

        var hasBlock = Value.EntityRuntimeId != 0;
        writer.Write(hasBlock);

        if (hasBlock)
        {
            writer.Write(Value);
        }
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new SerializedBlock();

        var entityRuntimeId = reader.ReadVarUInt32() - 1;
        Debug.Assert(entityRuntimeId == uint.MaxValue, "entityRuntimeId == uint.MaxValue");

        if (entityRuntimeId == uint.MaxValue)
        {
            var hasSerializedBlock = reader.ReadBoolean();
            if (!hasSerializedBlock)
                return;

            Value = reader.ReadSerializedBlock();
        }
        else
        {
            Value = new SerializedBlock(0, new Dictionary<ulong, byte>(), entityRuntimeId);
        }
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}