using System.Globalization;
using System.Numerics;

namespace BadgerSerialization.Models;

public struct Rect
{
    public Vector2 Min;
    public Vector2 Max;

    public Rect(Vector2 min, Vector2 max)
    {
        Min = min; 
        Max = max;
    }

    public override string ToString()
        =>
            $"{{\"min\":{{\"x\": {Min.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Min.Y.ToString(CultureInfo.InvariantCulture)}}}, \"max\":{{\"x\": {Max.X.ToString(CultureInfo.InvariantCulture)}, \"y\": {Max.Y.ToString(CultureInfo.InvariantCulture)}}}}}";
}