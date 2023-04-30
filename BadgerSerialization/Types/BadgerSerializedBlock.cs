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
        if (Value.BlockId == 0)
            throw new NotImplementedException();

        writer.WriteVarUInt32(Value.BlockId + 1);

        var hasBlock = Value.BlockId != 0;
        writer.Write(hasBlock);

        if (hasBlock)
        {
            writer.WriteVarUInt64(Value.NameHash);
            writer.Write((byte)Value.States.Count);
            foreach (var state in Value.States)
            {
                writer.WriteVarUInt64(state.Key);
                writer.Write(state.Value);
            }
        }
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new SerializedBlock();

        var unkBlockLength = reader.ReadVarUInt32();
        Debug.Assert(unkBlockLength == 0, "unkBlockLength != 0");

        if (unkBlockLength == 0)
        {
            var hasSerializedBlock = reader.ReadBoolean();
            if (!hasSerializedBlock)
                return;

            Value = reader.ReadSerializedBlock();
        }
        else
        {
            var unkBlockBytes = reader.ReadBytes((int)unkBlockLength);
            Value = new SerializedBlock(0, 0, new Dictionary<ulong, byte>());
        }
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}