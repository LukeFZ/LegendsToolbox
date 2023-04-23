using BadgerSerialization.Core;

namespace BadgerSerialization.Types;

public class BadgerPositionList : BadgerList
{
    public override BadgerObjectType Type => BadgerObjectType.PositionList;

    public BadgerPositionList(string name, List<Dictionary<string, BadgerObject>>? value = null) : base(name, value) { }
}