# ShoppingApp — Microservices Architecture on ASP.NET Core (.NET 8)

Requirements
Visual Studio 2022
.NET 8 SDK
Docker Desktop

This system is a modular microservices-based application built on .NET 8 and C# 12, following clean architecture principles, modern communication patterns, and full Docker containerization.

## Catalog Microservice

- ASP.NET Core Minimal APIs using Carter for endpoint routing
- Vertical Slice Architecture with feature folders (one `.cs` file per slice including request, handler, model, etc.)
- CQRS pattern implemented via MediatR
- Validation pipeline with MediatR behaviors and FluentValidation
- Marten for transactional document storage on PostgreSQL
- Cross-cutting concerns: logging, global exception handling, health checks

## Basket Microservice

- ASP.NET 8 Web API following RESTful principles and CRUD operations
- Redis used as distributed cache (`basketdb`) with:
  - Cache-aside pattern
  - Proxy and Decorator patterns
- gRPC client to Discount service for dynamic price resolution
- Publishes `BasketCheckout` events using MassTransit and RabbitMQ

## Discount Microservice

- ASP.NET Core gRPC Server exposing Protobuf-defined services
- Inter-service communication via high-performance gRPC
- EF Core with SQLite provider, using code-first migrations
- Containerized SQLite storage

## Ordering Microservice

- Domain-Driven Design, CQRS, and Clean Architecture
- Uses MediatR, FluentValidation, and Mapster for separation of concerns
- Consumes `BasketCheckout` events via MassTransit/RabbitMQ
- EF Core with SQL Server and automatic migrations on startup
- Fully containerized SQL Server instance

## API Gateway (YARP)

- YARP (Yet Another Reverse Proxy) for API Gateway layer
- Implements Gateway Routing Pattern with route, cluster, destination, transform configuration
- FixedWindowLimiter enabled for rate limiting and throttling

## Web UI (ShoppingApp)

- ASP.NET Core Web Application using Razor Pages and Bootstrap 4
- Uses Refit with HttpClientFactory to call API Gateway (YARP)
- Acts as frontend entry point for the entire system

## Microservice Communication

- Synchronous: gRPC between Basket and Discount
- Asynchronous: RabbitMQ used with MassTransit for message-based communication
  - Basket publishes `BasketCheckout` event
  - Ordering service subscribes and processes event
  - Shared message contracts in `EventBus.Messages` library

## Docker & Deployment

- Full Docker Compose setup:
  - Containerization of all services and databases
  - Environment variable overrides supported
- Launch the stack:

```bash
docker-compose -f compose.yaml  up -d
