namespace FirstAML.Models;

/// <summary>A shipment-level charge, separate from individual parcel prices.</summary>
public sealed record ShippingCharge(AdditionalChargeType Type, decimal Cost)
{
    private AdditionalChargeType _type = ValidateType(Type);

    public AdditionalChargeType Type
    {
        get => _type;
        init => _type = ValidateType(value);
    }

    private static AdditionalChargeType ValidateType(AdditionalChargeType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(
                nameof(Type), type, "Unsupported additional charge type.");
        }

        return type;
    }
}
