namespace BadgerSerialization.Models;

public struct Vector3I
{
    public int X; 
    public int Y; 
    public int Z;

    public Vector3I(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
        => $"{{\"x\": {X}, \"y\": {Y}, \"z\": {Z}}}";
}