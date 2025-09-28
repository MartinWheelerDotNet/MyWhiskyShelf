# MyWhiskyShelf
A Whisky Shelf app to allow you to add bottles / drams you've had, and store information about bottles you own.

### Mutation Status

| Branch | Project        | Score                                                                                                                                                                                                                                                                                                                            |
|--------|----------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Main   | Total          | [![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FMartinWheelerDotNet%2FMyWhiskyShelf%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/MartinWheelerDotNet/MyWhiskyShelf/main)                                                 |
|        | AppHost        | [![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FMartinWheelerDotNet%2FMyWhiskyShelf%2Fmain%3Fmodule%3DAppHost)](https://dashboard.stryker-mutator.io/reports/github.com/MartinWheelerDotNet/MyWhiskyShelf/main?module=AppHost)               |
|        | Application    | [![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FMartinWheelerDotNet%2FMyWhiskyShelf%2Fmain%3Fmodule%3DApplication)](https://dashboard.stryker-mutator.io/reports/github.com/MartinWheelerDotNet/MyWhiskyShelf/main?module=Application)       |
|        | Infrastructure | [![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FMartinWheelerDotNet%2FMyWhiskyShelf%2Fmain%3Fmodule%3DInfrastructure)](https://dashboard.stryker-mutator.io/reports/github.com/MartinWheelerDotNet/MyWhiskyShelf/main?module=Infrastructure) |
|        | WebApi         | [![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FMartinWheelerDotNet%2FMyWhiskyShelf%2Fmain%3Fmodule%3DWebApi)](https://dashboard.stryker-mutator.io/reports/github.com/MartinWheelerDotNet/MyWhiskyShelf/main?module=WebApi)                 |        

### Quality Gate

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=MartinWheelerDotNet_MyWhiskyShelf)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=MartinWheelerDotNet_MyWhiskyShelf&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=MartinWheelerDotNet_MyWhiskyShelf&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=MartinWheelerDotNet_MyWhiskyShelf&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=MartinWheelerDotNet_MyWhiskyShelf&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=MartinWheelerDotNet_MyWhiskyShelf&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=MartinWheelerDotNet_MyWhiskyShelf)<br>

### Setup

* Ensure .NET 9 is installed: `https://learn.microsoft.com/en-us/dotnet/core/install/`
* Ensure EF Tools is installed: `dotnet tool install --global dotnet-ef`
* Ensure Aspire CLI is installed: `dotnet tool install --global Aspire.Cli`
* Ensure docker is installed: `https://docs.docker.com/desktop/` 

### Project Dependency Diagram

```mermaid
flowchart LR
  %% High-level architecture & project relationships

  subgraph AppHost["AppHost (Aspire orchestrator)"]
    AH["MyWhiskyShelf.AppHost"];
  end

  subgraph Web["Web API (composition root)"]
    WAPI["MyWhiskyShelf.WebApi"];
    WDI["DI wiring & endpoint filters"];
  end

  subgraph Application["Application"]
    A1["Use cases / services"];
    A2["Repository interfaces"];
    A3["DTOs / contracts"];
  end

  subgraph Core["Core (Domain)"]
    C1["Domain models"];
    C2["Value objects"];
  end

  subgraph Infra["Infrastructure"]
    I1["Repository implementations · EF Core"];
    I2["DbContext, EF entities, mapping"];
    I3["Adapters · Redis · File loader"];
    I4["DataSeederHostedService"];
  end

  subgraph Migr["Migrations & runner"]
    M1["MyWhiskyShelf.Migrations"];
    MS["MyWhiskyShelf.MigrationService"];
  end

  subgraph SvcDefaults["ServiceDefaults"]
    SD["MyWhiskyShelf.ServiceDefaults"];
  end

  subgraph Tests["Tests"]
    T1["WebApi.Tests"];
    T2["Application.Tests"];
    T3["Infrastructure.Tests"];
    T4["IntegrationTests"];
    T5["Core.Tests"];
  end

  %% Runtime flow
  Client((Client)) -->|HTTP| WAPI
  AH --> WAPI
  AH --> MS

  %% Compile-time dependencies (solid) and wiring-only (dashed)
  WAPI --> Application
  WAPI -. "wiring only" .-> Infra

  Application --> Core
  Infra --> Application
  Infra --> Core

  %% EF migrations usage
  Infra --> I2
  M1 --> I2
  M1 -. "uses as startup" .-> WAPI

  %% Seed & adapters
  I4 --> I2
  I3 --> Cache[("Redis")]
  I3 --> Files[("Filesystem / JSON")]
  I1 --> DB[("PostgreSQL")]

  %% Service defaults (common hosting, health, etc.)
  SD --> WAPI
```

### Environment Variables

#### Database

`MYWHISKYSHELF_DATA_SEEDING_ENABLED`: Enables seeding of test data (default: true)

`MYWHISKYSHELF_RUN_MIGRATIONS`: Runs the migrations against the database in Aspire (default: true)

#### Tools

`MYWHISKYSHELF_REDIS_INSIGHT_ENABLED`: Enables `Redis Insight` container in Asipre (default: true)

`MYWHISKYSHELF_PG_WEB_ENABLED`: Enables PgWeb container in Aspire (default: true)


### Useful commands

### Migrations

Create the migrations:

```
$env:ConnectionStrings__myWhiskyShelfDb = 'Host=localhost;Port=;Database=;Username=;Password='
dotnet ef migrations add InitialMigration -p MyWhiskyShelf.Migrations/MyWhiskyShelf.Migrations.csproj -s MyWhiskyShelf.WebApi/MyWhiskyShelf.WebApi.csproj -c MyWhiskyShelf.Infrastructure.Persistence.Contexts.MyWhiskyShelfDbContext -o Migrations
```

Update all aspire packages accross all projects in the solution: `aspire update`
