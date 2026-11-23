# FirstAML

A C# library for calculating the cost of sending an order of parcels, targeting
.NET 10.

## Assumptions

Parcel dimensions cannot be negative. Zero is treated as a very small dimension
and is allowed for now. A parcel with all dimensions zero is small and costs $3;
when other dimensions are larger, the largest dimension still determines pricing.

Each parcel is priced individually, and those costs are added together to form
the shipment subtotal. The shipment is not priced solely using its largest or
most expensive parcel. For example, a small parcel ($3) and a medium parcel ($8)
have a subtotal of $11, before any additional shipping charges.

## Run tests with Docker

Install and start [Docker Desktop](https://www.docker.com/products/docker-desktop/),
or use Docker Engine with the Compose plugin. From the repository root, run:

```sh
docker compose run --build --rm tests
```
