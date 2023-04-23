using System.Globalization;
using System.Numerics;

namespace BadgerSerialization.Models;

public struct AxisAlignedBoundingBox
{
    public Vector3 Min;
    public Vector3 Max;

    public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
    {
        Min = min; 
        Max = max;
    }

    public override string ToString()
        => $"{{\"min\":{{\"x\": {Min.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Min.Y.ToString(CultureInfo.InvariantCulture)}, \"z\": {Min.Z.ToString(CultureInfo.InvariantCulture)}}}, \"max\":{{\"x\": {Max.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Max.Y.ToString(CultureInfo.InvariantCulture)}, \"z\": {Max.Z.ToString(CultureInfo.InvariantCulture)}}}}}";
}