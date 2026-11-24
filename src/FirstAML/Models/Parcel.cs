namespace FirstAML.Models;

/// <summary>A parcel with nonnegative whole-centimetre dimensions and whole-kilogram weight.</summary>
public sealed record Parcel(int Width, int Height, int Depth, int WeightKg)
{
    private int _width = ValidateDimension(Width, nameof(Width));
    private int _height = ValidateDimension(Height, nameof(Height));
    private int _depth = ValidateDimension(Depth, nameof(Depth));
    private int _weightKg = ValidateWeight(WeightKg);

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

    public int WeightKg
    {
        get => _weightKg;
        init => _weightKg = ValidateWeight(value);
    }

    private static int ValidateWeight(int weightKg)
    {
        if (weightKg < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(WeightKg), weightKg, "Parcel weight must not be negative.");
        }

        return weightKg;
    }

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
