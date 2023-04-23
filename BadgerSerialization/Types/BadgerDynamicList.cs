using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerDynamicList : BadgerList
{
    public override BadgerObjectType Type => BadgerObjectType.DynamicList;

    public BadgerDynamicList(string name, List<Dictionary<string, BadgerObject>>? value = null) : base(name)
    {
        Value = value ?? new List<Dictionary<string, BadgerObject>>();
    }

    public override void Serialize(BadgerBinaryWriter writer)
    {
        WriteList(writer, Value);
    }

    public override void Deserialize(BadgerBinaryReader reader, uint maxLength)
    {
        Value = new List<Dictionary<string, BadgerObject>>();

        var properties = new Dictionary<string, List<BadgerObject>>();

        var type = reader.ReadType();
        while (type != BadgerObjectType.End)
        {
            var name = reader.ReadBadgerString();
            var length = reader.ReadVarUInt32();
            var list = new List<BadgerObject>();

            var endPosition = reader.BaseStream.Position + length;

            while (reader.BaseStream.Position < endPosition)
            {
                var obj = GetObjectForType(type, name);
                obj.Deserialize(reader, length);
                list.Add(obj);
            }

            properties[name] = list;
            type = reader.ReadType();
        }

        if (properties.Count == 0)
            return;

        for (int i = 0; i < properties.Values.First().Count; i++)
        {
            var currentObj = new Dictionary<string, BadgerObject>();
            foreach (var pair in properties)
                currentObj[pair.Key] = pair.Value[i];

            Value.Add(currentObj);
        }
    }
}