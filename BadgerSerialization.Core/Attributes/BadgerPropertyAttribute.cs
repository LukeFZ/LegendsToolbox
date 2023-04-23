using System;

namespace BadgerSerialization.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BadgerPropertyAttribute : Attribute
{
    public string Name { get; }
    public BadgerObjectType Type { get; }

    public BadgerPropertyAttribute(string name, BadgerObjectType type)
    {
        Name = name;
        Type = type;
    }
}