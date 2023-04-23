using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerList : BadgerObject
{
    public override BadgerObjectType Type => BadgerObjectType.List;
    public List<Dictionary<string, BadgerObject>> Value { get; set; }

    public BadgerList(string name, List<Dictionary<string, BadgerObject>>? value = null) : base(name)
    {
        Value = value ?? new List<Dictionary<string, BadgerObject>>();
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
       writer.WriteVarUInt32(Value.Count);
       if (Value.Count == 0)
           return;

       WriteList(writer, Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new List<Dictionary<string, BadgerObject>>();
        var count = checked((int)reader.ReadVarUInt32());
        if (count == 0)
            return;

        Value = ReadList(reader, count);
    }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
    {
        var currentIndent = string.Join("", Enumerable.Repeat(indentString, indentLevel));
        sb.Append('[');

        if (Value.Count == 0)
        {
            sb.Append(']');
        }
        else
        {
            sb.AppendLine();

            var i = 0;
            foreach (var item in Value)
            {
                sb.AppendLine($"{currentIndent + indentString}{{");

                var j = 0;
                foreach (var value in item)
                {
                    j++;
                    value.Value.Print(sb, indentLevel + 2, indentString);
                    sb.AppendLine(j == item.Count ? "" : ",");
                }

                i++;
                sb.Append($"{currentIndent + indentString}}}");
                sb.AppendLine(i == Value.Count ? "" : ",");
            }

            sb.Append($"{currentIndent}]");
        }
    }
}