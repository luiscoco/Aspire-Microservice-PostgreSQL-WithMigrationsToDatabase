# Building a Blazor Web App with a PostgreSQL CRUD Microservice Using Aspire .NET 9

## 1. Create .NET Aspire 9 Starter Application

We run Visual Studio 2022 Community Edition and Create a New Project

We select the **.NET Aspire 9 Starter App** project template

![image](https://github.com/user-attachments/assets/5e31cf38-1f0a-4398-9591-7f8fb1f81e20)

We input the project name and location and press the Next button

![image](https://github.com/user-attachments/assets/d2c4ffec-a540-45fe-a871-13de0ef03e46)

We select the .NET 9 framework and press the Create button

![image](https://github.com/user-attachments/assets/59ddc3c2-0fe0-4f4d-8295-9c6ff617d14e)

We can verify the project folders and files structure

![image](https://github.com/user-attachments/assets/b1ec8fdd-362f-42af-a64c-4d4ce49e45d1)

## 2. Load the PostgreSQL Nuget package (AspirePostgreSQL.AppHost project)

![image](https://github.com/user-attachments/assets/9111f764-0ba2-4ffe-a8dd-ac2f22b148a3)

## 3. We modify the middleware(Program.cs) (AspirePostgreSQL.AppHost project)

We define the database username and password:

```csharp
var contentplatformdbpassword = builder.AddParameter("contentplatform-db-password");
var contentplatformdbusername = builder.AddParameter("contentplatform-db-username");
```

We also register the database server and assign the username, password and port for accesing:

```csharp
var postgreServer = builder.AddPostgres("contentplatform-db", contentplatformdbusername, contentplatformdbpassword, port: 5432)
    .WithDataVolume()
    .WithPgAdmin();
```

We create the database:

```csharp
var postgredatabase = postgreServer.AddDatabase("contentplatform");
```

This code sets up an **API Service project** with a **PostgreSQL database** reference, registering it in the application's builder so that it can be managed and injected into other parts of the application as needed

```csharp
var apiService = builder.AddProject<Projects.AspirePostgreSQL_ApiService>("apiservice").WithReference(postgredatabase);
```

This code sets up a **Web Frontend project** that connects to a **PostgreSQL database**, can interact with **external HTTP endpoints**, and relies on an **API Service** (apiService) for additional backend functionality

```csharp
builder.AddProject<Projects.AspirePostgreSQL_Web>("webfrontend")
    .WithReference(postgredatabase)
    .WithExternalHttpEndpoints()
    .WithReference(apiService);
```

We review the middleware whole code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var contentplatformdbpassword = builder.AddParameter("contentplatform-db-password");
var contentplatformdbusername = builder.AddParameter("contentplatform-db-username");

var postgreServer = builder.AddPostgres("contentplatform-db", contentplatformdbusername, contentplatformdbpassword, port: 5432)
    .WithDataVolume()
    .WithPgAdmin();

var postgredatabase = postgreServer.AddDatabase("contentplatform");

var apiService = builder.AddProject<Projects.AspirePostgreSQL_ApiService>("apiservice").WithReference(postgredatabase);

builder.AddProject<Projects.AspirePostgreSQL_Web>("webfrontend")
    .WithReference(postgredatabase)
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```

## 4. We manage the PostgreSQL database secrets (AspirePostgreSQL.AppHost project)

We right click on the project name and select the **Manage User Secrets...** menu option

![image](https://github.com/user-attachments/assets/83e61803-5d35-437c-bc49-53367975abfe)

We add the secrets in the **secrets.json** file

```json
{
  "Parameters:contentplatform-db-username": "postgres",
  "Parameters:contentplatform-db-password": "3AfRV)vhP23aj1!!wHU{Hc"
}
```

## 5. Load the Nuget packages (AspirePostgreSQL.ApiService)

These packages together allow the project to use Entity Framework Core for database access (specifically PostgreSQL) and provide Swagger-based API documentation

![image](https://github.com/user-attachments/assets/eb91cd58-69e0-42b0-9772-a9f8119e3b9f)

**Microsoft.EntityFrameworkCore (9.0.0-rc.2.24474.1)**: This is the core package for Entity Framework Core, which is an Object-Relational Mapping (ORM) framework for .NET

It allows developers to work with databases using .NET objects and LINQ queries instead of raw SQL

The version here is a release candidate (rc), meaning it's close to stable but not fully released

**Microsoft.EntityFrameworkCore.Tools (9.0.0-rc.2.24474.1)**: This package provides tools for Entity Framework Core, enabling features like migrations, model scaffolding, and database updates via the command line

It’s essential for managing schema changes in a database through code

**Npgsql.EntityFrameworkCore.PostgreSQL (9.0.0-rc.2)**: This package is the PostgreSQL provider for Entity Framework Core

It allows EF Core to connect to PostgreSQL databases specifically, translating LINQ queries into PostgreSQL-compatible SQL

This package is needed if the project uses PostgreSQL as its database

**Swashbuckle.AspNetCore (6.9.0)**: Swashbuckle is a library that integrates Swagger with ASP.NET Core applications

It generates Swagger documentation for APIs, allowing you to create a user-friendly API documentation and test interface

This package makes it easier to visualize and test APIs directly from a web UI

## 6. 

## 7.

## 8.

## 9.

## 10.

