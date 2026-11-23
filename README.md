# FirstAML

A C# library for calculating the cost of sending an order of parcels, targeting
.NET 10. The library selects the cheapest eligible shipping option for each
parcel and returns the individual prices and shipment total.

## Calculate a shipment quote

```csharp
using FirstAML.Models;
using FirstAML.Services;

var service = new ShippingService();
var quote = service.CalculateQuote([
    new Parcel(5, 6, 7),
    new Parcel(10, 20, 30)
]);

// quote.Items contains a Small parcel costing $3 and a Medium parcel costing $8.
// quote.TotalCost is 11m.
```

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
mixed-category shipments, with separate assertions for quoted items and totals.
