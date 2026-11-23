namespace FirstAML.Models;

/// <summary>A parcel with nonnegative dimensions measured in whole centimetres.</summary>
public sealed record Parcel(int Width, int Height, int Depth)
{
    private int _width = ValidateDimension(Width, nameof(Width));
    private int _height = ValidateDimension(Height, nameof(Height));
    private int _depth = ValidateDimension(Depth, nameof(Depth));

    public int Width
    {
        get => _width;
        init => _width = ValidateDimension(value, nameof(Width));
    }

    public int Height
    {
        get => _height;
        init => _height = ValidateDimension(value, nameof(Height));
    }

    public int Depth
    {
        get => _depth;
        init => _depth = ValidateDimension(value, nameof(Depth));
    }

    public int MaxDimension => Math.Max(Width, Math.Max(Height, Depth));

    private static int ValidateDimension(int dimension, string parameterName)
    {
        if (dimension < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName, dimension, "Parcel dimensions must not be negative.");
        }

        return dimension;
    }
}
