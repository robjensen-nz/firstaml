namespace FirstAML.Models;

/// <summary>A shipment-level charge, separate from individual parcel prices.</summary>
public sealed record ShippingCharge(string Description, decimal Cost);
