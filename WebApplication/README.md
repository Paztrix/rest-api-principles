# TODO API — C# Minimal API Workshop Starter

A .NET 10 translation of the REST API principles course starter. It uses ASP.NET Core Minimal APIs, EF Core, PostgreSQL, Data Annotations, and MSTest/Moq.

The exercise TODOs are deliberately unimplemented:

- `TodoService.cs`: TODOs 1–6
- `TodoEndpoint.cs`: TODOs A–F
- `GlobalExceptionHandling.cs`: TODOs G–I

## Run with Docker

```bash
docker compose up --build
```

The API is available at `http://localhost:8080`. PostgreSQL is initialized with database, user, and password all set to `todos`.

## Run locally

Start PostgreSQL, then run:

```bash
dotnet run --project WebApplication/WebApplication.csproj
```

The local launch profile serves HTTP on `http://localhost:5239`. Database settings use `ConnectionStrings__Todos` or the `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, and `DB_PASSWORD` environment variables.

## Test project

```bash
dotnet test WebApplication.slnx
```

The translated starter tests are expected to fail until the service-layer TODOs are completed.
