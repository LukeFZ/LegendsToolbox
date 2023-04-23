using System.Collections.Generic;
using BadgerSerialization.Core;

namespace BadgerSerialization.Generator.Serialization;

public class ClassSerializationInfo
{
    public string Name { get; }
    public string Namespace { get; }
    public List<PropertySerializationInfo> Properties { get; }

    public ClassSerializationInfo(string name, string ns, List<PropertySerializationInfo> properties)
    {
        Name = name;
        Namespace = ns;
        Properties = properties;
    }
}

public class PropertySerializationInfo
{
    public string PropertyName { get; }
    public string BadgerName { get; }
    public BadgerObjectType Type { get; }
    public string ComplexTypeName { get; set; }

    public PropertySerializationInfo(string propertyName, string badgerName, BadgerObjectType type)
    {
        PropertyName = propertyName;
        BadgerName = badgerName;
        Type = type;
        ComplexTypeName = string.Empty;
    }
}