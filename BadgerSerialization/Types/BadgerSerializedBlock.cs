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

        var blockId = reader.ReadVarUInt32() - 1;
        Debug.Assert(blockId != 0, "defaultBlockId != 0");

        if (blockId != 0)
        {
            var hasSerializedBlock = reader.ReadBoolean();
            if (!hasSerializedBlock)
                return;

            var nameHash = reader.ReadVarUInt64();
            var propertyCount = reader.ReadByte();

            var properties = new Dictionary<ulong, byte>();
            for (int i = 0; i < propertyCount; i++)
            {
                var propertyHash = reader.ReadVarUInt64();
                var propertyValue = reader.ReadByte();
                properties[propertyHash] = propertyValue;
            }

            Value = new SerializedBlock(blockId, nameHash, properties);
        }
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
        => sb.Append(Value);
}