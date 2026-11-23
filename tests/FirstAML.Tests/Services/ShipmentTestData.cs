using FirstAML.Models;

namespace FirstAML.Tests.Services;

internal static class ShipmentTestData
{
    internal static readonly ShipmentCase[] Cases =
    [
        // Empty shipment: no parcel items; both normal and speedy totals are zero.
        new([], [], [], 0m, 0m),

        // One small parcel: all dimensions below 10 cm.
        new([new(5, 6, 7)], [ParcelCategory.Small], [3m], 3m, 6m),

        // One medium parcel: fits below 50 cm, but not below 10 cm.
        new([new(20, 30, 40)], [ParcelCategory.Medium], [8m], 8m, 16m),

        // One large parcel: fits below 100 cm, but not below 50 cm.
        new([new(60, 70, 80)], [ParcelCategory.Large], [15m], 15m, 30m),

        // One XL parcel: at least one dimension exceeds 100 cm.
        new([new(120, 20, 30)], [ParcelCategory.XL], [25m], 25m, 50m),

        // Small and medium: each retains its own rate; $3 + $8 = $11.
        new([new(5, 6, 7), new(20, 30, 40)],
            [ParcelCategory.Small, ParcelCategory.Medium], [3m, 8m], 11m, 22m),

        // Small and large: $3 + $15 = $18.
        new([new(5, 6, 7), new(60, 70, 80)],
            [ParcelCategory.Small, ParcelCategory.Large], [3m, 15m], 18m, 36m),

        // Small and XL: $3 + $25 = $28.
        new([new(5, 6, 7), new(120, 20, 30)],
            [ParcelCategory.Small, ParcelCategory.XL], [3m, 25m], 28m, 56m),

        // Medium and large: $8 + $15 = $23.
        new([new(20, 30, 40), new(60, 70, 80)],
            [ParcelCategory.Medium, ParcelCategory.Large], [8m, 15m], 23m, 46m),

        // Medium and XL: $8 + $25 = $33.
        new([new(20, 30, 40), new(120, 20, 30)],
            [ParcelCategory.Medium, ParcelCategory.XL], [8m, 25m], 33m, 66m),

        // Large and XL: $15 + $25 = $40.
        new([new(60, 70, 80), new(120, 20, 30)],
            [ParcelCategory.Large, ParcelCategory.XL], [15m, 25m], 40m, 80m),

        // All four categories together: $3 + $8 + $15 + $25 = $51.
        new([new(5, 6, 7), new(20, 30, 40), new(60, 70, 80), new(120, 20, 30)],
            [ParcelCategory.Small, ParcelCategory.Medium, ParcelCategory.Large, ParcelCategory.XL],
            [3m, 8m, 15m, 25m], 51m, 102m),

        // 50 parcels: 13 small, 13 medium, 12 large, and 12 XL, interleaved.
        // (13 x $3) + (13 x $8) + (12 x $15) + (12 x $25) = $623; speedy = $1,246.
        CreateFiftyParcelCase()
    ];

    private static ShipmentCase CreateFiftyParcelCase()
    {
        (Parcel Parcel, ParcelCategory Category, decimal Cost)[] pattern =
        [
            (new(5, 6, 7), ParcelCategory.Small, 3m),
            (new(20, 30, 40), ParcelCategory.Medium, 8m),
            (new(60, 70, 80), ParcelCategory.Large, 15m),
            (new(120, 20, 30), ParcelCategory.XL, 25m)
        ];

        var items = Enumerable.Range(0, 50)
            .Select(index => pattern[index % pattern.Length])
            .ToArray();

        return new ShipmentCase(
            items.Select(item => item.Parcel).ToArray(),
            items.Select(item => item.Category).ToArray(),
            items.Select(item => item.Cost).ToArray(),
            623m,
            1246m);
    }

    internal sealed record ShipmentCase(
        Parcel[] Parcels,
        ParcelCategory[] Categories,
        decimal[] Costs,
        decimal TotalCost,
        decimal SpeedyTotalCost);
}
