namespace FirstAML.Models;

/// <summary>A parcel with dimensions measured in whole centimetres.</summary>
public sealed record Parcel(int Width, int Height, int Depth)
{
    public int MaxDimension => Math.Max(Width, Math.Max(Height, Depth));
}
