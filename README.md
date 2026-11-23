# FirstAML

A C# library for calculating the cost of sending an order of parcels, targeting
.NET 10. The library selects the cheapest eligible shipping option for each
parcel and returns the individual prices and shipment total.

## Calculate a shipment quote

```csharp
using FirstAML.Models;
using FirstAML.Services;

var service = new ShippingService();
var request = new ShipmentRequest([
    new Parcel(5, 6, 7),
    new Parcel(10, 20, 30)
], ShippingSpeed.Normal);
var quote = service.CalculateQuote(request);

// quote.Items contains a Small parcel costing $3 and a Medium parcel costing $8.
// quote.TotalCost is 11m.
```

`ShipmentRequest` captures the customer's parcels and required shipping speed
(`ShippingSpeed.Normal` or `ShippingSpeed.Speedy`) before pricing. The service
returns a `ShipmentQuote` containing the priced parcels and total.
Both models reject undefined shipping speed values with `ArgumentOutOfRangeException`.
The selected speed is available as `quote.ShippingSpeed`. Speedy shipping adds a
separate entry in `quote.AdditionalCharges` with description `"Speedy shipping"` and a cost
equal to the parcel subtotal, doubling `quote.TotalCost`. Individual parcel
prices in `quote.Items` stay unchanged. For the example above, selecting Speedy
keeps parcel prices at $3 and $8, adds an $11 charge, and gives a $22 total.
Normal shipping has an empty `quote.AdditionalCharges` collection.
An empty speedy shipment has a separate $0 charge and a $0 total.
`quote.TotalCost` is calculated from the parcel subtotal plus all additional charges.

Dimensions are whole centimetres. The default options are Small (all dimensions
below 10 cm, $3), Medium (below 50 cm, $8), Large (below 100 cm, $15), and XL
(no upper limit, $25). Limits are exclusive: exactly 10, 50, or 100 cm moves a
parcel into the next category.

Custom shipping options can be passed to the `ShippingService` constructor.
The service selects by cost among all eligible options. An empty shipment has a
zero total; if no configured option fits a parcel, it throws
`InvalidOperationException`.

## Run with Docker

Install and start [Docker Desktop](https://www.docker.com/products/docker-desktop/),
or use Docker Engine with the Compose plugin. From the repository root, run:

```sh
docker compose run --build --rm tests
```

This restores dependencies, builds the library, runs the tests, and removes the
test container. A failing test returns a nonzero exit code. The first run requires
internet access to download the .NET SDK image and NuGet packages; subsequent
builds reuse Docker's cache. Run the same command after changing code.

The container uses Microsoft's [.NET 10 SDK image](https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/10.0/default-images-use-ubuntu).
No local .NET installation is required. This project is a library consumed by its
tests, so there is no web server or CLI to launch.

## Run locally (optional)

Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0),
then run:

```sh
dotnet test FirstAML.sln --configuration Release
```

If the SDK was installed with Microsoft's install script into `~/.dotnet`, make
it available in your terminal first:

```sh
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
```

## Project structure

- `src/FirstAML/` — class library for parcel pricing.
- `tests/FirstAML.Tests/` — xUnit tests referencing the library.
- `FirstAML.sln` — solution containing both projects.
- `Dockerfile` and `compose.yaml` — containerized build and test setup.

The tests cover loading the library assembly, model calculations, and shipment
pricing. Shipping service tests use a shared table of empty, single-category, and
mixed-category shipments for both speeds, with separate assertions for quoted
items, shipping charges, and totals. These include the 50-parcel shipment and
custom rates where a larger category is cheaper.
Normal and speedy shipping tests are in `ShippingServiceTests.cs` and
`SpeedyShippingServiceTests.cs`, with shared cases in `ShipmentTestData.cs`.
