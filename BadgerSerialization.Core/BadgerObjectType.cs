namespace BadgerSerialization.Core;

public enum BadgerObjectType
{
    Boolean = 0x1,
    Short = 0x3,
    Integer = 0x4,
    Long = 0x5,
    VarUInt = 0x6,
    VarULong = 0x7,
    UShort = 0x8,
    Float = 0x9,
    String = 0xb,
    Compound = 0xc,
    HashedEnum = 0xf,
    UnsignedIntegerList = 0x10,
    OptionalVarUInt = 0x11,
    DynamicList = 0x12,
    Vector2F = 0x13,
    Vector3F = 0x14,
    AxisAlignedBoundingBox = 0x15,
    Rect = 0x16,
    DictionaryKey = 0x17,
    Vector3I = 0x18,
    Dictionary = 0x28,
    PositionList = 0x2a,
    List = 0x2b,
    End = 0x50,
    OptionalString = 0x51,
    OptionalStringList = 0x52,
    OptionalString2 = 0x53, // Gets hashed by game
    EntityUniqueId = 0x55,
    OptionalHashedEnum = 0x56,
    OptionalString3 = 0x57,
    SerializedBlock = 0x58,
}

public static class BadgerObjectTypeExtensions
{
    public static bool IsSimpleType(this BadgerObjectType type)
        => type != BadgerObjectType.Compound
           && type != BadgerObjectType.Dictionary
           && !type.IsList();

    public static bool IsList(this BadgerObjectType type)
        => type
            is BadgerObjectType.List
            or BadgerObjectType.DynamicList
            or BadgerObjectType.PositionList; // OptionalStringList and UnsignedIntegerList are both considered simple types
}