using System.Text;
using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerDictionary : BadgerList
{
    public override BadgerObjectType Type => BadgerObjectType.Dictionary;

    public BadgerDictionary(string name, List<Dictionary<string, BadgerObject>>? value = null) : base(name, value) { }

    public override void PrintValue(StringBuilder sb, int indentLevel, string indentString)
    {
        var currentIndent = string.Join("", Enumerable.Repeat(indentString, indentLevel));
        sb.AppendLine("{");

        var i = 0;
        foreach (var item in Value)
        {
            var key = item.Values.First();
            var properties = item.Values;

            sb.Append(currentIndent + indentString);
            key.PrintValue(sb, indentLevel + 1, currentIndent);
            sb.AppendLine(": {");

            var j = 0;
            foreach (var value in properties)
            {
                value.Print(sb, indentLevel + 2, indentString);

                j++;
                sb.AppendLine(j == item.Values.Count ? "" : ",");
            }

            i++;
            sb.Append($"{currentIndent + indentString}}}");
            sb.AppendLine(i == Value.Count ? "" : ",");
        }

        sb.Append($"{currentIndent}}}");
    }
}