# FirstAML

A C# library for calculating the cost of sending an order of parcels, targeting
.NET 10.

## Run tests with Docker

Install and start [Docker Desktop](https://www.docker.com/products/docker-desktop/),
or use Docker Engine with the Compose plugin. From the repository root, run:

```sh
docker compose run --build --rm tests
```

## Transcripts
Please see ./transcripts 

## Assumptions

We support free shipping if configured in that way. Each shipping option has its
own positive integer weight limit and nonnegative excess-weight rate; a zero rate
means there is no excess-weight charge.

Parcel dimensions cannot be negative. Zero is treated as a very small dimension
and is allowed for now. A parcel with all dimensions zero fits the small category,
with a $3 base cost under the default rates. The largest dimension determines
which options are eligible; the cheapest total price determines the chosen option.

Parcel weight is required and measured in whole kilograms. Zero is treated as
very light and is allowed; negative weights are rejected.

Speedy shipping is assumed to double the entire parcel subtotal, including
excess-weight charges. It remains a separate shipment-level charge and does not
change the individual parcel prices.

Each parcel is priced individually, and those costs are added together to form
the shipment subtotal. The shipment is not priced solely using its largest or
most expensive parcel. For example, a small parcel ($3) and a medium parcel ($8)
have a subtotal of $11 when within their weight allowances, before any additional
shipping charges.

## Weight-based pricing

| Category | Dimension limit (exclusive, cm) | Base cost | Weight allowance (kg) | Excess per kg |
| --- | --- | --- | --- | --- |
| Small | 10 | $3 | 1 | $2 |
| Medium | 50 | $8 | 3 | $2 |
| Large | 100 | $15 | 6 | $2 |
| XL | Unlimited | $25 | 10 | $2 |

For each dimension-eligible option, the price is its base cost plus
`max(0, WeightKg - WeightLimitKg) * OveragePerKg`. The cheapest total is selected.
Each priced parcel exposes `BaseCost`, `ExcessWeightCharge`, and `Cost` (their sum).

```csharp
var parcel = new Parcel(5, 6, 7, WeightKg: 4);
var request = new ShipmentRequest([parcel], ShippingSpeed.Normal);
var quote = new ShippingService().CalculateQuote(request);
// Small: $3 base + $6 excess weight = $9 total.
```

Custom options require both weight settings, for example:
`new ShippingOption(ParcelCategory.Small, 10, 3m, WeightLimitKg: 1, OveragePerKg: 2m)`.


