using System.Diagnostics;
using System.Numerics;
using System.Text;
using BadgerSerialization.Models;

namespace BadgerSerialization;

public class BadgerBinaryWriter : BinaryWriter
{
    public BadgerBinaryWriter(Stream input) : base(input) { }

    public BadgerBinaryWriter(Stream input, Encoding encoding) : base(input, encoding) { }

    public BadgerBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

    [DebuggerStepThrough]
    public void WriteVarUInt64(ulong value)
    {
        do
        {
            var val = value & 0x7f;
            value >>= 7;

            if (value != 0)
                val |= 0x80;

            Write((byte) val);
        } while (value != 0);
    }

    [DebuggerStepThrough]
    public void WriteVarUInt32(uint value)
        => WriteVarUInt64(value);

    [DebuggerStepThrough]
    public void WriteVarUInt32(int value)
        => WriteVarUInt32((uint) value);

    [DebuggerStepThrough]
    public void WriteBadgerString(string value)
    {
        var strBytes = Encoding.UTF8.GetBytes(value);
        WriteVarUInt32(strBytes.Length);
        Write(strBytes);
    }

    [DebuggerStepThrough]
    public void Write(Vector3 value)
    {
        Write(value.X);
        Write(value.Y);
        Write(value.Z);
    }

    [DebuggerStepThrough]
    public void Write(Vector2 value)
    {
        Write(value.X);
        Write(value.Y);
    }

    [DebuggerStepThrough]
    public void Write(SerializedBlock value)
    {
        WriteVarUInt64(value.NameHash);
        Write((byte)value.States.Count);
        foreach (var state in value.States)
        {
            WriteVarUInt64(state.Key);
            Write(state.Value);
        }
    }
}